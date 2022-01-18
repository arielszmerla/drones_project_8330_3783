using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// implement Ilocatable so used in inheritance by other classse BO
    /// </summary>
    public interface ILocatable
    {
        Location Location { get; set; }
    }
}
