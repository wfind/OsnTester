using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsnTester
{
    public enum OPStatus
    {
        OP_SUCCESS,

        OP_UNKNOWN
    }

    public enum Command
    {
        CREATE,
        DELETE,

        ADD,
        REMOVE,

        LIST,
        GET,

        UNKNOWN
    }

    /// <summary>
    /// Interface for Executors.
    /// </summary>
    public interface IExecutor : IAttribute
    {    
        void Run();

        /// <summary>
        /// Shows the tips
        /// </summary>
        void Prompt();
    }
}
