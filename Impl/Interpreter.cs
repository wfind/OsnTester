using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsnTester.Impl
{
    public abstract class Interpreter
    {
        /// <summary>
        /// Translates the error code into string.
        /// </summary>
        public virtual string TranslateCommon(int errorCode)
        {
            string readable = String.Empty;
            switch (errorCode)
            {
                case -1:
                    readable = "with exception";
                    break;
                case 0:
                    readable = "successfully";
                    break;
                case 1:
                    readable = "with fetal error : can not connect database";
                    break;
                case 2:
                    readable = "with fetal error : fail to communicate with stub";
                    break;

                default:
                    readable = Translate(errorCode);
                    break;
            }

            return readable;
        }

        /// <summary>
        /// The derived class overrides this method to translate their 
        /// exclusive error code.
        /// </summary>
        public abstract string Translate(int errorCode);
    }
}
