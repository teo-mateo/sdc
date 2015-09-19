using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.data.ViewModels
{
    public class Breadcrumb
    {
        public string Url { get; set; }
        public string Name { get; set; }

        public static Breadcrumb[] Generate(params string[] data)
        {
            int length = data.Length;
            if (data.Length % 2 == 1)
                length--;

            List<Breadcrumb> bc = new List<Breadcrumb>();
            for (int i = 0; i < length; i += 2)
            {
                bc.Add(new Breadcrumb() { Name = data[i], Url = data[i + 1] });
            }
            return bc.ToArray();
        }
    }
}
