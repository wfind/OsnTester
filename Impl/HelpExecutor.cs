using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Collections.Specialized;

using Common.Logging;

namespace OsnTester.Impl
{
    /// <summary>
    /// Help
    /// </summary>
    public class HelpExecutor : IExecutor
    {
        // Fields
        private string com;
        private readonly ILog log;
        private OsnExecutorFactory oef;

        // Properties
        public string Name
        {
            get { return GetType().Name; }
        }
        /// <summary>
        /// Sets the command.
        /// </summary>
        public string Com
        {
            set { com = value; }
        }

        public HelpExecutor()
        {
            log = LogManager.GetLogger(GetType());
            oef = new OsnExecutorFactory();
        }

        // Public instance methods

        /// <summary>
        /// Shows the message to users.
        /// </summary>
        /// <param name="msg"></param>
        public void Output(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Displays the help information.
        /// </summary>
        public void Help()
        {
            Output(@"Modules: 
client - The base.
group - The set of the server.
help - Displays this help. '?' does the same.

For more information, type 'help module' for each command.
ex) help group");
        }

        public void Prompt()
        {
            Output(@"NAME
	help - displays information of OsnTester prompt's modules.

SYNOPSYS
	help [module]

OPTIONS
	module   Indicates a module to know about. If the module is
	          not specified, it will displays. general help of
	          OsnTester prompt. ? does the same.");
        }

        public void Run()
        {
            if (string.IsNullOrEmpty(com))
            {
                Help();
                return;
            }

            try
            {
                NameValueCollection properties = new NameValueCollection();
                properties["osnTester.executor.type"] = com;
                oef.Initialize(properties);
                IExecutor module = oef.CreateExecutor();

                module.Prompt();
            }
            catch (System.Exception ex)
            {
                log.ErrorFormat("Run {0} threw an unhandled exception.", ex, com);
                Output(String.Format("fetal error : exception occurred. See log for detail.\n"));

                Help();
            }
        }
    }
}
