using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsnTester.Impl;

namespace OsnTester
{
    /// <summary>
    /// Holds references to OsnClient instances - ensuring uniqueness, 
    /// and allowing 'global' lookups.
    /// </summary>
    public class ClientCache : BaseCache<OsnClient>
    {
        // Fields
        private string defaultName;
        private static ClientCache instance = new ClientCache();

        /// <summary>
        /// Gets or sets the default Client
        /// </summary>
        public OsnClient Default
        {
            set 
            { 
                defaultName = value.Name; 
            }
            get
            {
                if (String.IsNullOrEmpty(defaultName))
                    return null;

                return Lookup(defaultName);
            }
        }

        private ClientCache() : base() { }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ClientCache Instance
        {
            get { return instance; }
        }
    }
}
