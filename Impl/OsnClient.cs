using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsnTester.OsnProxy;

namespace OsnTester.Impl
{
    public class OsnClient : IAttribute
    {
        // Fields
        private string name;
        private IOSNPlatformService platformClient;

        public string Name
        {
            get { return name; }
        }

        public IOSNPlatformService Client
        {
            get { return platformClient; }
        }

        public OsnClient()
        {
            // Using default constructor
            platformClient = new OSNPlatformServiceClient();
            name = (platformClient as OSNPlatformServiceClient).Endpoint.Address.ToString();
        }

        public OsnClient(string remoteAddress)
        {
            name = remoteAddress;
            platformClient = new OSNPlatformServiceClient(ENDPOINT_CONFIGNAME, remoteAddress);
        }

        // Constants
        private const string ENDPOINT_CONFIGNAME = "BasicHttpBinding_IOSNPlatformService";
    }
}
