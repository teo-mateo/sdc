using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// this thing doesn't work.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static unsafe long GetObjectSize(this object obj)
        {
            unsafe
            {
                RuntimeTypeHandle th = obj.GetType().TypeHandle;
                int size = *(*(int**)&th + 1);
                return (long)size;
            }
        }
    }
}
