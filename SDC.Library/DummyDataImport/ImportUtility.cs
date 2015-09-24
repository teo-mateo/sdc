using SDC.data;
using SDC.data.Entity;
using SDC.data.Entity.Books;
using SDC.data.Entity.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WebMatrix.WebData;
using SDC.Library.Extensions;
using System.Net;
using SDC.Library.S3;
using SDC.data.Entity.Location;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

namespace SDC.Library.DummyDataImport
{
    public class BookInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Publisher { get; set; }
    }

    public class ImportUtility
    {
        Random _rnd = new Random(Guid.NewGuid().GetHashCode());

        const string _tmp = @"D:\HEAPZILLA\sdc\SDC.Library";
        const string FirstNamesCsv = "DummyDataImport\\firstNames.csv";
        const string LastNamesCsv = "DummyDataImport\\firstNames.csv";
        const string BooksCsv = "DummyDataImport\\books.csv";

        private string _firstNamesCsv;
        private string _lastNamesCsv;
        private string _booksCsv;

        private string[] _firstNames;
        private string[] _lastNames;
        private string[] _books;
        private string[,] _booksSplit;
        private bool _csvDataLoaded = false;

        public int TotalBookCount { get { return _books == null ? 0 : _books.Length; } }
        private int _max;

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static int Progress { get; set; }
        public static bool Running { get; set; }
        public static DateTime ImportStart { get; set; }
        public static bool Cancel { get; set; }
        public static double AvgBookImportTime { get; set; }
        public static int TargetCount { get; set; }

        private SortedDictionary<String, Publisher> _publishersSet = new SortedDictionary<string, Publisher>();
        private SortedDictionary<String, Author> _authorsSet = new SortedDictionary<string, Author>();

        Genre[] _allGenres;
        Language _lang;
        Country _country;
        City _city;

        /// <summary>
        /// Initializes the paths and verifies that the csv files are present.
        /// </summary>
        public ImportUtility()
        {
            var importDirectory = ConfigurationManager.AppSettings["CSVImportSource"];
            if (Directory.Exists(importDirectory))
            {
                _firstNamesCsv = Path.Combine(importDirectory, FirstNamesCsv);
                _lastNamesCsv = Path.Combine(importDirectory, LastNamesCsv);
                _booksCsv = Path.Combine(importDirectory, BooksCsv);

                if (File.Exists(_firstNamesCsv) && File.Exists(_lastNamesCsv) && File.Exists(_booksCsv))
                {
                    //all good
                    if (!WebSecurity.Initialized)
                        WebSecurity.InitializeDatabaseConnection("SDCConnectionString", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    return;
                }
            }

            throw new Exception("One or more CSV files are missing.");
        }

        /// <summary>
        /// Loads the csv contents into string arrays.
        /// To be called before calling Import
        /// </summary>
        public void LoadData()
        {
            try
            {
                //load data;
                _firstNames = File.ReadAllLines(_firstNamesCsv);
                _lastNames = File.ReadAllLines(_lastNamesCsv);
                _books = File.ReadAllLines(_booksCsv);

                _booksSplit = new string[_books.Length,7];

                for (int i = 0; i < _books.Length; i++)
                {
                    string bookraw = _books[i];
                    bookraw = bookraw.Replace("&amp;", "&");
                    bookraw = bookraw.TrimDoubleQuotes();
                    string[] split = bookraw.Split(new string[] { "\";" }, StringSplitOptions.RemoveEmptyEntries);
                    _booksSplit[i, 0] = split[0].TrimDoubleQuotes();
                    _booksSplit[i, 1] = split[1].TrimDoubleQuotes();
                    _booksSplit[i, 2] = split[2].TrimDoubleQuotes();
                    _booksSplit[i, 3] = split[3].TrimDoubleQuotes();
                    _booksSplit[i, 4] = split[4].TrimDoubleQuotes();
                    _booksSplit[i, 5] = split[split.Length - 1].TrimDoubleQuotes();
                    _booksSplit[i, 6] = _books[i];
                }

                _csvDataLoaded = true;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private UserProfile CreateUser(SDCContext db, string firstName, string lastName, out Shelf shelf)
        {
            string username = firstName + "." + lastName;
            string email = username + "@gmail.com";
            string password = "123456";
            WebSecurity.CreateUserAndAccount(username, password, 
                new
                {
                    Email = email,
                    Avatar_Id = 1, // by default, use the first avatar that is available.
                    LastSeen = DateTime.Now,
                    IsLocked = false,
                    ShowEmail = false,
                    City_Id = 1
                });

            Roles.AddUsersToRole(new string[] { username }, RolesCustom.USER);

            //get profile
            var profile = db.UserProfiles.FirstOrDefault(p => p.UserName == username);
            profile.Country = _country;
            profile.City = _city;
            profile.PageSize = 10;

            //create default shelf
            //create default shelf
            Shelf defaultShelf = new Shelf()
            {
                CreationDate = DateTime.Now,
                Name = String.Format("{0}'s shelf", username),
                IsVisible = true,
                Owner = profile
            };
            db.Shelves.Add(defaultShelf);
            shelf = defaultShelf;

            return profile;
        }

        private void LoadBooks(SDCContext db, UserProfile profile, Shelf shelf)
        {
            int perShelf = _rnd.Next(10, 50);
            int startindex = Progress;
            for(int i = startindex; i < startindex + perShelf && Progress < _max; i++)
            {
                string isbn, title, author, publisher, imgurl;
                int year;
                try {
                    isbn = _booksSplit[i, 0];
                    title = _booksSplit[i, 1];
                    author = _booksSplit[i, 2];
                    year = Int32.Parse(_booksSplit[i, 3]);
                    publisher = _booksSplit[i, 4];
                    imgurl = _booksSplit[i, 5];
                }catch(Exception)
                {
                    continue;
                }

                DateTime d1 = DateTime.Now;
                Author a = null;
                if (!_authorsSet.ContainsKey(author))
                {
                    a = new Author()
                    {
                        Name = author,
                        AddedDate = DateTime.Now,
                        AddedBy = profile,
                        LastModifiedBy = profile,
                        IsVerified = true
                    };
                    db.Authors.Add(a);
                    _authorsSet.Add(author, a);
                }
                else
                {
                    a = _authorsSet[author];
                }
                Debug.WriteLine("Author lookup in " + DateTime.Now.Subtract(d1).TotalMilliseconds);

                DateTime d2 = DateTime.Now;
                Publisher p = null;
                if (!_publishersSet.ContainsKey(publisher))
                {
                    p = new Publisher()
                    {
                        Name = publisher,
                        AddedBy = profile,
                        IsVerified = true
                    };
                    db.Publishers.Add(p);
                    _publishersSet.Add(publisher, p);
                }
                else
                {
                    p = _publishersSet[publisher];
                }
                Debug.WriteLine("Author lookup in " + DateTime.Now.Subtract(d2).TotalMilliseconds);

                Book book = new Book()
                {
                    Authors = new List<Author>(new Author[] { a }),
                    Title = title,
                    AddedDate = DateTime.Now,
                    Language = _lang,
                    Shelf = shelf,
                    ISBN = isbn,
                    Publisher = p,
                    Year = year,
                    Description = _booksSplit[i, 6],
                    Pages = _rnd.Next(1, 500),
                    Price = _rnd.Next(10, 100)
                };

                //3 random genres bongo bong!
                var bookGenres = _allGenres.OrderBy(x => Guid.NewGuid().ToString()).Take(3).ToArray();
                book.Genres.Add(bookGenres[0]);

                db.Books.Add(book);

                //book pictures
                BookPicture bp = new BookPicture()
                {
                    Book = book,
                    Key = null,
                    Url = imgurl,
                    IsMain = true,
                    Title = "img title"
                };

                //book.Pictures = new List<BookPicture>();
                book.Pictures.Add(bp);

                Progress++;
                if (Cancel)
                    break;
            }
        }

        public Task<int> Import(int max = 0)
        {
            if (!_csvDataLoaded)
                throw new Exception("CSV data not loaded.");

            if (max == 0)
                _max = TotalBookCount;
            else
                _max = max;

            Task<int> importTask = new Task<int>(() =>
            {
                try {
                    Progress = 0;
                    Running = true;
                    Cancel = false;
                    ImportStart = DateTime.Now;
                    TargetCount = _max;
                    using (var db = new SDCContext())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        _allGenres = db.Genres.ToArray();
                        _lang = db.Languages.Find("FR");
                        _country = db.Countries.Find("CA");
                        _city = db.Cities.Find(4);

                        var authors = db.Authors.ToList();
                        authors.ForEach(a =>
                        {
                            if (!_authorsSet.ContainsKey(a.Name))
                                _authorsSet.Add(a.Name, a);
                        });
                        var publishers = db.Publishers.ToList();
                        publishers.ForEach(p =>
                        {
                            if (!_publishersSet.ContainsKey(p.Name))
                                _publishersSet.Add(p.Name, p);
                        });

                        do
                        {
                            string firstName = _firstNames[_rnd.Next(0, _firstNames.Length - 1)];
                            string lastName = _lastNames[_rnd.Next(0, _lastNames.Length - 1)];

                            Shelf shelf;
                            var profile = CreateUser(db, firstName, lastName, out shelf);
                            LoadBooks(db, profile, shelf);

                            db.ChangeTracker.DetectChanges();
                            db.SaveChanges();

                            var localBooks = db.Books.Local.ToArray();
                            foreach(var le in localBooks)
                            {
                                db.Entry(le).State = System.Data.Entity.EntityState.Detached;
                            }
                            var localPictures = db.BookPictures.Local.ToArray();
                            foreach(var le in localPictures)
                            {
                                db.Entry(le).State = System.Data.Entity.EntityState.Detached;
                            }

                        } while (Progress < _max && !Cancel);
                    }

                    return Progress;
                }
                finally
                {
                    Running = false;
                }
            });


            importTask.Start();
            return importTask;
        }

    }
}
