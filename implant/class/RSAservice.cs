using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace implant
{
    public static class RSAservice
    {
        public static string generateKeyPair(string length)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(Int32.Parse(length));
            rsa.PersistKeyInCsp = false;

            string publicPrivateKeyXML = rsa.ToXmlString(true);
            return publicPrivateKeyXML;
        }

        public static string sign(string key, string input)
        {
            string hashAlgo = "SHA512";
            string signature = string.Empty;

            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                //key = decryptRSAKey(key);
                rsa.FromXmlString(key);

                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] signBytes = rsa.SignData(inputBytes, hashAlgo);
                signature = Convert.ToBase64String(signBytes);
            }
            catch (Exception)
            { }
            return signature;
        }

        public static bool verify(string key, string input, string signature)
        {
            string hashAlgo = "SHA512";
            bool valid = false;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                //key = decryptRSAKey(key);
                rsa.FromXmlString(key);

                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] signBytes = Convert.FromBase64String(signature);
                valid = rsa.VerifyData(inputBytes, hashAlgo, signBytes);
            }
            catch (Exception)
            { }
            return valid;
        }

        public static List<string> getPossibleKeyLengths()
        {
            List<string> lengthList = new List<string>()
            {
                "1024",
                "2048",
                "4096",
                "8192"
            };

            return lengthList;
        }

        public static string getPubKey(string name)
        {
            string pubKey = null;

            try
            {
                MyKey.get(name);
                string privPubKey = MyKey.privPubKey;
                privPubKey = decryptRSAKey(privPubKey);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privPubKey);
                pubKey = rsa.ToXmlString(false);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return pubKey;
        }

        public static string getPrivKey(string name)
        {
            string privPubKey = null;

            try
            {
                MyKey.get(name);
                privPubKey = MyKey.privPubKey;
                privPubKey = decryptRSAKey(privPubKey);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
            }

            return privPubKey;
        }

        static string encryptRSAKey(string rsaKey)
        {
            string encryptedKey = null;
            //string salt = SQLservice.getSalt();
            //string initializationVector = SQLservice.getInitializationVector();
            //string key = SQLservice.getPassword();

            //AESservice aes = new AESservice(salt, initializationVector);
            //CryptObject cryptObject = aes.encrypt(rsaKey,key);
            //encryptedKey = cryptObject.value;

            return encryptedKey;
        }

        static string decryptRSAKey(string encryptedRsaKey)
        {
            string decryptedKey = null;
            //string salt = SQLservice.getSalt();
            //string initializationVector = SQLservice.getInitializationVector();
            //string key = SQLservice.getPassword();

            //AESservice aes = new AESservice(salt, initializationVector);
            //CryptObject cryptObject = aes.decrypt(encryptedRsaKey, key);
            //decryptedKey = cryptObject.value;

            return decryptedKey;
        }
    }
}

