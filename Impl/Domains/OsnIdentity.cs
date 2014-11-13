using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsnTester.Impl.Domains
{
    public class OsnIdentity
    {
        public string UserName { get; set; }
        public string Passwd { get; set; }

        /// <summary>
        /// The URL for the <see cref="IOSNPlatformService"/> instance.
        /// </summary>
        public string URL { get; set; }
    }
}
