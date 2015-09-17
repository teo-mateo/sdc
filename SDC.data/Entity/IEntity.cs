using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.data.Entity
{

    public interface IEntity
    {
        int Id { get; set; }
    }

    public interface ICodeEntity
    {
        string Code { get; set; }
    }
}
