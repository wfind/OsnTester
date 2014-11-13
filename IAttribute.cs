using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsnTester
{
    /// <summary>
    /// Can be the attribute of anything.
    /// </summary>
    public interface IAttribute
    {
        /// <summary>
        /// Returns the name.
        /// </summary>
        /// <remarks>Unique</remarks>
        string Name
        { 
            get; 
        }
    }
}
