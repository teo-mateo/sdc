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

        const string _tmp = @"C:\temp";
        const string FirstNamesCsv = "DummyDataImport\\firstNames.csv";
        const string LastNamesCsv = "DummyDataImport\\firstNames.csv";
        const string BooksCsv = "DummyDataImport\\books.csv";

        private string _firstNamesCsv;
        private string _lastNamesCsv;
        private string _booksCsv;

        private string[] _firstNames;
        private string[] _lastNames;
        private string[] _books;
        private bool _csvDataLoaded = false;

        public int TotalBookCount { get { return _books == null ? 0 : _books.Length; } }
        private int _loadedBooksCount;
        public int LoadedBooksCount
        {
            get
            {
                return _loadedBooksCount;
            }
        }

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

        /// <summary>
        /// Initializes the paths and verifies that the csv files are present.
        /// </summary>
        public ImportUtility()
        {
            var basePath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            _firstNamesCsv = Path.Combine(basePath, FirstNamesCsv);
            _lastNamesCsv = Path.Combine(basePath, LastNamesCsv);
            _booksCsv = Path.Combine(basePath, BooksCsv);

            if (!File.Exists(_firstNamesCsv) || !File.Exists(_lastNamesCsv) || !File.Exists(_booksCsv))
            {
                throw new FileNotFoundException("One of the required csv files is missing.");
            }

            
            WebSecurity.InitializeDatabaseConnection("SDCConnectionString", "UserProfile", "UserId", "UserName", autoCreateTables: true);

            _csvDataLoaded = false;
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
                _csvDataLoaded = true;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public Task Import()
        {
            if (!_csvDataLoaded)
                throw new Exception("CSV data not loaded.");

            return new Task(() =>
            {
                using (var db = new SDCContext())
                {
                    do
                    {
                        string firstName = _firstNames[_rnd.Next(0, _firstNames.Length - 1)];
                        string lastName = _lastNames[_rnd.Next(0, _lastNames.Length - 1)];

                        Shelf shelf;
                        var profile = CreateUser(db, firstName, lastName, out shelf);
                        LoadBooks(db, profile, shelf, 10);

                    } while (_loadedBooksCount < TotalBookCount);
                }
            });
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
            db.SaveChanges();

            shelf = defaultShelf;

            return profile;
        }

        private void LoadBooks(SDCContext db, UserProfile profile, Shelf shelf, int howMany)
        {
            for(int i = 0; i < howMany; i++)
            {
                string bookraw = _books[_loadedBooksCount++];
                string[] split = bookraw.Split(new char[] { ';' });
                string isbn = split[0].TrimDoubleQuotes();
                string title = split[1].TrimDoubleQuotes();
                string author = split[2].TrimDoubleQuotes();
                int year = Int32.Parse(split[3].TrimDoubleQuotes());
                string publisher = split[4].TrimDoubleQuotes();
                string imgurl = split[split.Length - 1].TrimDoubleQuotes();

                Author a = db.Authors.FirstOrDefault(x => x.Name.Equals(author));
                if(a == null)
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
                }

                Publisher p = db.Publishers.FirstOrDefault(x => x.Name.Equals(publisher));
                if(p == null)
                {
                    p = new Publisher()
                    {
                        Name = publisher,
                        AddedBy = profile,
                        IsVerified = true
                    };
                    db.Publishers.Add(p);
                }

                Book book = new Book()
                {
                    Authors = new List<Author>(new Author[] { a }),
                    Title = title,
                    AddedDate = DateTime.Now,
                    Language = db.Languages.First(),
                    Shelf = shelf,
                    ISBN = isbn,
                    Publisher = p,
                    Year = year,
                    Description = bookraw,
                    Pages = _rnd.Next(1, 500),
                    Price = _rnd.Next(10, 100)
                };

                db.Books.Add(book);
                db.SaveChanges();

                S3File s3file = null;
                using(WebClient wc = new WebClient())
                {
                    string localtmp = Path.Combine(_tmp, Guid.NewGuid().ToString() + ".jpg");
                    wc.DownloadFile(imgurl, localtmp);
                    using (var fs = new FileStream(localtmp, FileMode.Open))
                    {
                        s3file = S3.S3.UploadBookImage(book.Id.ToString(), Guid.NewGuid().ToString(), fs);
                    }
                    File.Delete(localtmp);
                }

                BookPicture bp = new BookPicture()
                {
                    Book = book,
                    Key = (s3file != null) ? s3file.Key : null,
                    Url = (s3file != null) ? s3file.Url : null,
                    IsMain = true,
                    Title = "img title"
                };

                //book.Pictures = new List<BookPicture>();
                book.Pictures.Add(bp);
                //db.BookPictures.Add(bp);
                db.SaveChanges();
            }
        }
    }
}
