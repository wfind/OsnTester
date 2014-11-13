using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsnTester
{
    /// <summary>
    /// Provides a mechanism for obtaining client-usable handles to 
    /// <see cref="IExecutor"/> instances
    /// </summary>
    public interface IExecutorFactory
    {
        /// <summary>
        /// Returns a instance of <see cref="IExecutor"/>.
        /// </summary>
        IExecutor CreateExecutor();

        /// <summary>
        /// Returns a handle to the Executor with the given name, if it exists.
        /// </summary>
        /// <param name="exeName">Name of the executor</param>
        IExecutor GetExecutor(string exeName);
    }
}
