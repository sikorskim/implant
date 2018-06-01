using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace implant
{
    public class HashService
{
        public static string compute(string input, string hashAlgo)

        {
            HashAlgorithm algo = HashAlgorithm.Create(hashAlgo);            
            byte[] hashBytes = algo.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            
            foreach (byte b in hashBytes)
            {

                sb.Append(b.ToString("X2"));
            }
            string computedHash = sb.ToString();

            return computedHash;
        }

        public static List<string> getAlgorithms()
        {
            List<string> algoList = new List<string>()
            {
                "MD5",
                "SHA1",
                "SHA256",
                "SHA384",
                "SHA512",
                "RIPEMD160"
            };

            return algoList;
        }
    }
}
