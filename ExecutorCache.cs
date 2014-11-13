using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsnTester
{
    /// <summary>
    /// Holds references to Executor instances - ensuring uniqueness, 
    /// and allowing 'global' lookups.
    /// </summary>
    public class ExecutorCache : BaseCache<IExecutor>
    {
        // Fields
        private static readonly ExecutorCache instance = new ExecutorCache();

        private ExecutorCache() : base() { }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ExecutorCache Instance
        {
            get { return instance; }
        }
    }
}
