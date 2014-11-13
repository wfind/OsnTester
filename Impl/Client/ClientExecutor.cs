using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using Common.Logging;

namespace OsnTester.Impl.Client
{
    /// <summary>
    /// The Client
    /// </summary>
    /// <remarks>
    /// <para>
    /// The first character of methods' name must be upper case 
    /// and others must be lower case
    /// </para>
    /// <para>
    /// Methods that are used to be invoked must be public
    /// </para>
    /// </remarks>
    public class ClientExecutor : IExecutor
    {
        // Fields
        private string com;
        private List<string> paras;

        private readonly ILog log;

        public string Name
        {
            get { return declaringType.Name; }
        }

        // Properties
        public string Com
        {
            set { com = value; }
        }

        public List<string> Paras
        {
            set { paras = value; }
        }

        public ClientExecutor()
        {
            log = LogManager.GetLogger(declaringType);
        }

        /// <summary>
        /// Shows the message to users.
        /// </summary>
        /// <param name="msg"></param>
        public virtual void Output(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Prompt()
        {
            Output(@"Usage: client command 
       client connect remoteAddress
       client current

client is a client.

Commands:
    connect - Connect to osn proxy
    current - Show the info of the current proxy");
        }

        public void Run()
        {
            try
            {
                com = com.Substring(0, 1).ToUpper() + com.Substring(1).ToLower();

                MethodInfo method = declaringType.GetMethod(com);
                if (method == null)
                {
                    Prompt();
                    return;
                }

                string msg = method.Invoke(this, new object[0]) as string;
                if (String.IsNullOrEmpty(msg) == false)
                    Output(msg);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Run {0} threw an unhandled exception.", ex, com);
                Output(String.Format("fetal error : exception occurred. See log for detail."));

                if (ex is AmbiguousMatchException)
                    Prompt();
            }
        }

        /// <summary>
        /// Connect to the specified endpoint.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// If the client has been created, then the client will be gotten from
        /// cache.
        /// </remarks>
        public string Connect()
        {
            if (paras.Count < 1)
            {
                return "fatal error: missing parameters";
            }

            string msg = String.Empty;
            string remoteAddress = paras[0];

            try
            {
                // Check the cache if the client has already been created
                ClientCache cliCache = ClientCache.Instance;
                OsnClient client = cliCache.Lookup(remoteAddress);
                if (client == null)
                {
                    client = new OsnClient(remoteAddress);
                    cliCache.Bind(client);
                }
                cliCache.Default = client;

                msg = String.Format("connect {0} successfully", remoteAddress);

                return msg;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException)
                    msg = "fetal error: invalid parameters";
                else if (ex is InvalidOperationException)
                    msg = "fetal error: invalid endpoint";
                else
                    msg = "fetal error: exception occur";

                return msg;
            }
        }

        public string Current()
        {
            ClientCache cliCache = ClientCache.Instance;
            OsnClient client = cliCache.Default;
            if (client == null)
                return "did not connect to a proxy yet";
            else
                return String.Format("current proxy: {0}", client.Name);
        }

        private readonly static Type declaringType = typeof(ClientExecutor);
    }
}
