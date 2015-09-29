using SDC.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.ServiceLayer
{
    public interface ISDCService
    {
        SearchResultDTO Search(string term, int? userid = null);
    }
}
