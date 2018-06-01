using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace implant
{
    public class CryptObject
    {
        public bool operationSuccess;
        public string value;

        public CryptObject(bool encryptionSuccesful, string value)
        {
            this.operationSuccess = encryptionSuccesful;
            this.value = value;
        }
    }
}
