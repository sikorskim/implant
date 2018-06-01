using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace implant
{
    public  class AESservice
    {
        #region settings

        public AESservice(string salt, string initializationVector)
        {
            this.salt = salt;
            this.initializationVector = initializationVector;
        }

        private int iterations = 16999;
        private int keySize = 256;

       // private string hash = "SHA512";
        private string salt;
        private string initializationVector;

        #endregion

        public  CryptObject encrypt(string plainText, string password)
        {
            string ciphertext = null;
            CryptObject cryptObject = null;

            try
            {
                byte[] vectorBytes = Encoding.ASCII.GetBytes(initializationVector);
                byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
                byte[] valueBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] ciphertextBytes;

                using (AesManaged cipher = new AesManaged())
                {
                    cipher.Mode = CipherMode.CBC;
                    cipher.KeySize = keySize;

                    // using PBKDF2 algorithm
                    Rfc2898DeriveBytes passBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations);
                    byte[] keyBytes = passBytes.GetBytes(keySize / 8);
                    
                    using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(valueBytes, 0, valueBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                ciphertextBytes = memoryStream.ToArray();
                            }
                        }
                    }
                    cipher.Clear();
                }

                ciphertext = Convert.ToBase64String(ciphertextBytes);
                cryptObject = new CryptObject(true, ciphertext);
            }
            catch (Exception ex)
            {
                cryptObject = new CryptObject(false, "Nieprawidłowe parametry szyfrowania!"+Environment.NewLine+"iv="+initializationVector+"salt="+salt);
                LogService.add(ex.ToString());
            }

            return cryptObject;
        }

        public  CryptObject decrypt(string cipherText, string password)
        {
            string plainText = null;
            CryptObject cryptObject = null;

            try
            {
                byte[] vectorBytes = Encoding.ASCII.GetBytes(initializationVector);
                byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
                byte[] valueBytes = Convert.FromBase64String(cipherText);

                byte[] decrypted;
                int decryptedByteCount = 0;

                using (AesManaged cipher = new AesManaged())
                {
                    cipher.KeySize = keySize;
                    Rfc2898DeriveBytes passBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations);
                    byte[] keyBytes = passBytes.GetBytes(keySize / 8);

                    cipher.Mode = CipherMode.CBC;

                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(valueBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = cryptoStream.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }

                    cipher.Clear();
                    plainText = Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
                    cryptObject = new CryptObject(true, plainText);
                }
            }
            catch (Exception ex)
            {
                cryptObject = new CryptObject(false, "Nieprawidłowe parametry deszyfrowania!");
                LogService.add(ex.ToString());
            }

            return cryptObject;
        }
    }
}
