using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.data;
using SDC.data.Entity.Books;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace SDC.Tests
{
    public class Chrono : IDisposable
    {
        DateTime _start;
        string _task;
        public Chrono(string task)
        {
            _task = task;
            _start = DateTime.Now;
        }

        public void Time()
        {
            TimeSpan ts = DateTime.Now.Subtract(_start);
            Debug.WriteLine(String.Format("Chrono: END {0}: {1}ms", _task, ts.TotalMilliseconds));
        }

        public void Dispose()
        {
            Time();
        }
    }

    public class U
    {
        public static long ObjectSize(object o)
        {
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                return s.Length;
            }
        }

        public static SomeObject[] GetData(int size)
        {
            int[] a = new int[size];
            SomeObject[] o;
            using (var c = new Chrono("data init"))
            {
                long l = 0;
                o = a.Select(p =>
                {
                    return new SomeObject
                    {
                        Data = ++l,
                        Key = Guid.NewGuid().ToString()
                    };
                }).ToArray();


                return o;
            }
        }
    }



    [TestClass]
    public class CollectionsWebinar_Tests
    {
        //arrays: IMMUTABLE

        [TestMethod]
        public void SortedListVsSortedDictionary()
        {
            int SIZE = 100 * 1000;
            var o = U.GetData(SIZE);

            Debug.WriteLine("------------------------");

            #region Sorted list
            SortedList<string, SomeObject> list = new SortedList<string, SomeObject>();
            using (var c = new Chrono("SortedList insertion"))
            {
                for (int i = 0; i < o.Length; i++)
                    list.Add(o[i].Key, o[i]);
            }

            long size1 = U.ObjectSize(list);
            Debug.WriteLine("SortedList full size: " + String.Format("{0:n0}", size1));

            using (var c = new Chrono("SortedList access"))
            {
                for (int i = 0; i < o.Length; i++)
                {
                    var data = list[o[i].Key];
                }
            }

            using (var c = new Chrono("SortedList removal"))
            {
                for (int i = 0; i < o.Length; i++)
                    list.Remove(o[i].Key);
            }

            list = null;
            GC.Collect();
            #endregion


            Debug.WriteLine("--------------------------");


            #region Sorted dictionary
            SortedDictionary<string, SomeObject> dict = new SortedDictionary<string, SomeObject>();
            using (var c = new Chrono("SortedDictionary insertion"))
            {
                for (int i = 0; i < o.Length; i++)
                    dict.Add(o[i].Key, o[i]);
            }

            long size2 = U.ObjectSize(dict);
            Debug.WriteLine("SortedDictionary full size: " + String.Format("{0:n0}", size2));

            using (var c = new Chrono("SortedDictionary access"))
            {
                for (int i = 0; i < o.Length; i++)
                {
                    var data = dict[o[i].Key];
                }
            }

            using (var c = new Chrono("SortedDictionary removal"))
            {
                for (int i = 0; i < o.Length; i++)
                    dict.Remove(o[i].Key);
            }

            dict = null;
            GC.Collect();
            #endregion

            Debug.WriteLine("------------------------");

            #region Dictionary
            Dictionary<string, SomeObject> dictx = new Dictionary<string, SomeObject>();
            using (var c = new Chrono("Dictionary insertion"))
            {
                for (int i = 0; i < o.Length; i++)
                {
                    dictx.Add(o[i].Key, o[i]);
                }
            }

            using (var c = new Chrono("Dictionary retrieval"))
            {
                for (int i = 0; i < o.Length; i++)
                {
                    var data = dictx[o[i].Key];
                }
            }

            long size3 = U.ObjectSize(dictx);
            Debug.WriteLine("Dictionary full size: " + String.Format("{0:n0}", size3));

            using (var c = new Chrono("Dictionary removal"))
            {
                for (int i = 0; i < o.Length; i++)
                {
                    dictx.Remove(o[i].Key);
                }
            }

            dictx = null;
            GC.Collect();
            #endregion

            Debug.WriteLine("------------------------");

            SortedDictionary<long, SomeObject> dict2 = new SortedDictionary<long, SomeObject>();
            var o_random = o.OrderBy(a => Guid.NewGuid().ToString()).ToArray();
            using (var c = new Chrono("SortedDictionary<long> insertion"))
            {
                for (int i = 0; i < o_random.Length; i++)
                {
                    dict2.Add(o_random[i].Data, o_random[i]);
                }
            }

            long size4 = U.ObjectSize(dict2);
            Debug.WriteLine("SortedDictionary<long> full size: " + String.Format("{0:n0}", size4));

            using (var c = new Chrono("SortedDictionary<long> removal"))
            {
                for (int i = 0; i < o_random.Length; i++)
                    dict2.Remove(o_random[i].Data);
            }


            dict2 = null;
            GC.Collect();





        }

        [TestMethod]
        public void ArrayVsList_IteratorPerformance()
        {
            var array = U.GetData(10 * 1000 * 1000);
            var list = array.ToList();

            using (var c = new Chrono("iterating array"))
            {
                foreach (var o in array)
                {
                    var o1 = o;
                }
            }
            var size1 = U.ObjectSize(array);
            Debug.WriteLine("array size: " + String.Format("{0:n0}", size1));

            using (var c = new Chrono("iterating a list"))
            {
                foreach (var o in list)
                {
                    var o1 = o;
                }
            }

            var size2 = U.ObjectSize(list);
            Debug.WriteLine("list size: " + String.Format("{0:n0}", size2));
        }

        public void os()
        {
            List<int> o = new List<int>();
            IList<string> q;

            var empty = Enumerable.Empty<int>();

        }

        [TestMethod]
        public void Custom_Enumerable()
        {
            MySomeObjectCollection c = new MySomeObjectCollection();
            var filtered = c.Where(p => p.Data % 2 == 0);

            var count = filtered.Count();
            var count2 = filtered.Count();

            filtered = null;
        }




    }



    public class ExternalService
    {
        int _count = 0;
        public SomeObject GetNextObject()
        {
            if (_count < 10)
            {
                _count++;
                return new SomeObject() { Data = _count, Key = _count.ToString() };
            }
            else return null;
        }

        public void Reset()
        {
            _count = 0;
        }
    }

    public class MySomeObjectCollection : IEnumerable<SomeObject>
    {
        ExternalService _service;
        ExternalServiceEnumerator _enumerator;
        public MySomeObjectCollection()
        {
            _service = new ExternalService();
            _enumerator = new ExternalServiceEnumerator(_service);
        }

        public IEnumerator<SomeObject> GetEnumerator()
        {
            return new ExternalServiceEnumerator(_service);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ExternalServiceEnumerator : IEnumerator<SomeObject>
    {
        ExternalService _service;
        SomeObject _current;
        public ExternalServiceEnumerator(ExternalService service)
        {
            _service = service;
        }

        public SomeObject Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            _service = null;
            _current = null;
        }

        public bool MoveNext()
        {
            _current = _service.GetNextObject();
            return _current != null;
        }

        public void Reset()
        {
            _service.Reset();
        }
    }



    [Serializable]
    public class SomeObject
    {
        public string Key { get; set; }
        public long Data { get; set; }

        public SomeObject()
        {

        }
    }
    
}
