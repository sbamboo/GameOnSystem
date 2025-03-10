using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOnSystem {
    internal static class SecretConfig {
        private static string externalDbAdress;
        private static string externalDbUser;
        private static string externalDbPassword;

        public static string ExternalDbAdress { get { return externalDbAdress; } }
        public static string ExternalDbUser { get { return externalDbUser; } }
        public static string ExternalDbPassword { get { return externalDbPassword; } }

        static SecretConfig() {
            externalDbAdress = "localhost";
            externalDbUser = "root";
            externalDbPassword = "";
        }
    }
}
