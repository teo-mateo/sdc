using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.Extensions
{
    public static class StringExtensions
    {
        public static string TrimDoubleQuotes(this string s)
        {
            return s.Trim(new char[] { '"' });
        }
    }
}
