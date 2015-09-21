using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Tests.Utility
{
    /// <summary>
    /// utility class
    /// </summary>
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
    }
}
