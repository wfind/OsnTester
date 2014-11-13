using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

using Common.Logging;

using OsnTester.OsnProxy;
using OsnTester.Impl.Domains;

namespace OsnTester.Impl
{
    /// <summary>
    /// Osn modules base class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Osn module classes must inherit this class.
    /// </para>
    /// <para>
    /// The first character of derived classes' methods' name 
    /// must be upper case and others must be lower case
    /// </para>
    /// <para>
    /// Methods that are used to be invoked must be private
    /// </para>
    /// </remarks>
    public abstract class OsnExecutor : IExecutor
    {
        // Fields
        protected Interpreter interpreter;
        protected IOSNPlatformService platformClient;

        protected readonly ILog log;

        protected string com;
        protected List<string> paras;

        // Public instance properties
        public string Name
        {
            get { return GetType().Name; }
        }

        public string Com
        {
            set { com = value; }
        }
        public List<string> Paras
        {
            set { paras = value; }
        }

        // Constructors
        public OsnExecutor(Interpreter interpreter)
        {
            this.interpreter = interpreter;
            log = LogManager.GetLogger(GetType());
        }

        // Public instance methods

        /// <summary>
        /// Checks the input data roughly.
        /// </summary>
        /// <returns>True pass, false fail</returns>
        public abstract bool CheckInput();

        /// <summary>
        /// Shows the message to users.
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Output(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Using reflection to run the specific method.
        /// </summary>
        /// <remarks>
        /// The value of <para>Com</para> must be same with the name of the method.
        /// </remarks>
        public virtual void Run()
        {
            try
            {
                // Gets the platform service
                ClientCache cliCache = ClientCache.Instance;
                OsnClient client = cliCache.Default;
                if (client == null)
                {
                    Output("Please connect a server first.");
                    return;
                }
                platformClient = client.Client;

                com = com.Substring(0, 1).ToUpper() + com.Substring(1).ToLower();
                MethodInfo method = GetType().GetMethod(com, BindingFlags.Instance | BindingFlags.NonPublic);
                if (method == null)
                {
                    Prompt();
                    return;
                }

                string msg = method.Invoke(this, new object[0]) as string;
                if (String.IsNullOrEmpty(msg) == false)
                    Output(msg);
            }
            catch (System.Exception ex)
            {
                log.ErrorFormat("Run {0} threw an unhandled exception.", ex, com);
                Output(String.Format("fetal error : exception occurred. See log for detail."));

                if (ex is AmbiguousMatchException)
                    Prompt();
            }
        }

        public virtual void Prompt()
        {
            Output(String.Format("No help entry for {0}.", com));
        }
    }
}
