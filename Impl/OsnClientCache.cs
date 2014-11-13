using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsnTester.Impl
{
    /// <summary>
    /// Holds references to OsnClient instances - ensuring uniqueness, 
    /// and allowing 'global' lookups.
    /// </summary>
    public class OsnClientCache
    {
        // Fields
        private Dictionary<string, IExecutor> executors;
        private static readonly OsnClientCache instance = new OsnClientCache();
        private readonly object syncRoot = new object();

        private OsnClientCache()
        {
            executors = new Dictionary<string, IExecutor>();
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public OsnClientCache Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Binds the specified executor.
        /// </summary>
        /// <param Lookup="executor">The executor</param>
        public virtual void Bind(IExecutor executor)
        {
            lock (syncRoot)
            {
                if (executors.ContainsKey(executor.Name))
                {
                    throw new Exception(string.Format("Executor with name '{0}' already exists.", executor.Name));
                }

                executors[executor.Name] = executor;
            }
        }

        /// <summary>
        /// Removes the executor by Lookup.
        /// </summary>
        /// <param Lookup="Lookup">Name of the executor</param>
        /// <param name="name">Name of the executor</param>
        public virtual bool Remove(string name)
        {
            lock (syncRoot)
            {
                return executors.Remove(name);
            }
        }

        /// <summary>
        /// Lookups the specified executor name.
        /// </summary>
        /// <param name="exeName">Name of the executor</param>
        public virtual IExecutor Lookup(string exeName)
        {
            lock (syncRoot)
            {
                IExecutor retValue;
                executors.TryGetValue(exeName, out retValue);
                return retValue;
            }
        }
    }
}
