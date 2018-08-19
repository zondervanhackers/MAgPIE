using System;

namespace ZondervanLibrary.Harvester.Service.Configuration
{
    public class EmailAccount
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public Tuple<string, string> EmailTuple => new Tuple<string, string>(UserName, Password);
    }
}
