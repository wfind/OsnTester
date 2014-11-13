using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsnTester.Impl.Group
{
    /// <summary>
    /// A implementation of <see cref="Interpreter"/> that 
    /// translate its exclusive error code into readable string.
    /// </summary>
    public class GroupInterpreter : Interpreter
    {
        public override string Translate(int errorCode)
        {
            string readable = string.Empty;

            switch (errorCode)
            {
                case 3:
                    readable = "with fetal error : invalid name";
                    break;
                case 4:
                    readable = "with fetal error : name conflict";
                    break;
                case 5:
                    readable = "with error : fail to communicate with other group";
                    break;

                default:
                    readable = "failed";
                    break;
            }

            return readable;
        }
    }
}
