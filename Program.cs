using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Specialized;

using OsnTester.Impl;
using Common.Logging;

namespace OsnTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog log = LogManager.GetLogger(typeof(Program));
            string arg = String.Empty;
            OsnExecutorFactory oef = new OsnExecutorFactory();

            while (arg.Equals("exit") == false)
            {
                Console.Write(@"OsnTester:\>");
                arg = Console.ReadLine();

                args = arg.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // Check args
                if (args.Length == 0)
                    continue;

                NameValueCollection properties = new NameValueCollection();

                properties["osnTester.executor.type"] = args[0];
                properties["osnTester.executor.com"] = args.Length > 1 ? args[1] : null;
                for (int i = 2; i < args.Length; i++)
                    properties.Add("osnTester.executor.paras", args[i]);

                try
                {
                    oef.Initialize(properties);

                    IExecutor executor = oef.CreateExecutor();
                    if (executor == null)
                    {
                        Console.WriteLine(String.Format("{0} : command not found"), args[0]);
                        continue;
                    }

                    executor.Run();
                }
                catch (Exception ex)
                {
                    log.Error("exception occurs.", ex);
                    Console.WriteLine("fetal error : exception occur");
                }
            }
        }
    }
}
