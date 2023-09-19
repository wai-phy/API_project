using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TodoApi.Util
{
    public class Encryption
    {
        private static readonly IConfiguration _configuration = Startup.StaticConfiguration!;
         private static readonly string _EncryptionKey = _configuration.GetSection("Encryption:EncryptionKey").Value ?? throw new Exception("Invalid EncryptionKey");
        private static readonly string _SaltKey = _configuration.GetSection("Encryption:EncryptionSalt").Value ?? throw new Exception("Invalid EncryptionSalt");
        private static readonly string _ClientEncryptionKey = _configuration.GetSection("Encryption:ClientEncryptionKey").Value ?? throw new Exception("Invalid ClientEncryptionKey");
        private static readonly string _ClientEncryptionSalt =_configuration.GetSection("Encryption:ClientEncryptionSalt").Value ?? throw new Exception("Invalid ClientEncryptionSalt");

        public static string EncryptFileName(string FileName)
        {
            return Encrypt_CBC_256(FileName);
        }
        public static string DecryptFileName(string EncFileName)
        {
            return Decrypt_CBC_256(EncFileName);
        }
        public static string DecryptID(string cipherText, string EncryptKey)
        {
            return MySQL_AES_Decrypt_ECB_128(cipherText, EncryptKey);
        }

        public static string EncryptID(string plainText, string EncryptKey)
        {
            return MySQL_AES_Encrypt_ECB_128(plainText, EncryptKey);
        }

        public static string Decrypt_URLParam_ID(string cipherText, string EncryptionKey = "") {
            byte[] data = Convert.FromBase64String(cipherText);  //it is B64 encoded from url
            cipherText = Encoding.UTF8.GetString(data);
            return MySQL_AES_Decrypt_ECB_128(cipherText, EncryptionKey);
        }

        public static string Encrypt_String(string PlainText, string EncryptionKey = "") {
            return Encrypt_CBC_256(PlainText, EncryptionKey);
        }

        public static string Decrypt_String(string cipherText, string EncryptionKey = "") {
            return Decrypt_CBC_256(cipherText, EncryptionKey);
        }

        public static string DecryptClient_String(string cipherText) {
            return DecryptClient_CBC_256(cipherText);
        }

        public static string MySQL_AES_Decrypt_ECB_TokenData(string cipherText, string EncryptKey)
        {
            if(cipherText=="")
                return "";

            string plainText = "";
            try
            {
                using (AesCryptoServiceProvider aesAlg = new())
                {
                    aesAlg.KeySize = 128;
                    aesAlg.BlockSize = 128;
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.Key = MakeFullLengthKey(EncryptKey); 
                    aesAlg.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plainText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plainText;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MySQL_AES_Decrypt_ECB_TokenData Fail");
            }
            return plainText;
        }      

        public static string EncryptToken(string PlainText, string EncryptionKey ,string SaltKey = "")//Encrypt_CBC_256_Token
        {
            return Encrypt_CBC_256(PlainText, EncryptionKey,SaltKey);
        }
        public static string DecryptToken(string cipherText, string EncryptionKey = "",string SaltKey = "")
        {
           return Decrypt_CBC_256(cipherText, EncryptionKey,SaltKey);
        }

        public static string Encrypt_CBC_256_URL(string PlainText, string EncryptionKey ,string _SaltKey = "")
        {
             return Encrypt_CBC_256(PlainText, EncryptionKey,_SaltKey);
        }
        public static string Decrypt_CBC_256_URL(string cipherText, string EncryptionKey = "",string _SaltKey = "")
        {
           return Decrypt_CBC_256(cipherText, EncryptionKey,_SaltKey);
        }

        private static string Encrypt_CBC_256(string PlainText, string EncryptionKey = "",string SaltKey = "") //,string _SaltKey = ""
        {
            
            if(PlainText == "") 
                return "";

            string encryptString = "";
            try
            {
                if (EncryptionKey.Trim() == "") EncryptionKey = _EncryptionKey;  // You can overwrite default enc key
                
                if (SaltKey.Trim()== "") SaltKey=_SaltKey;

                var bsaltkey = Encoding.UTF8.GetBytes(SaltKey);
                byte[] clearBytes = Encoding.UTF8.GetBytes(PlainText);
                
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new(EncryptionKey, bsaltkey, 1000, HashAlgorithmName.SHA256);
                    encryptor.Key = pdb.GetBytes(32);  //256 bit Key
                    encryptor.IV = GenerateRandomBytes(16);
                    encryptor.Mode = CipherMode.CBC;
                    encryptor.Padding = PaddingMode.PKCS7;

                    using (MemoryStream ms = new())
                    {
                        using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                        }
                        byte[] result = MergeArrays(encryptor.IV, ms.ToArray());  //append IV to cipher, so cipher length will longer
                        encryptString = Convert.ToBase64String(result);

                    }
                }
                return encryptString;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Encrypt_CBC_256 Fail");
            }
            return encryptString;
        }
		
		private static string Decrypt_CBC_256(string cipherText, string EncryptionKey = "", string SaltKey = "")
        {
            if(cipherText == "") 
                return "";
           
            string plainText = "";
            try
            {
                if (EncryptionKey.Trim() == "") EncryptionKey = _EncryptionKey;
                if (SaltKey.Trim()== "") SaltKey = _SaltKey;

                var bsaltkey = Encoding.UTF8.GetBytes(SaltKey);
                
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {

                    Rfc2898DeriveBytes pdb = new(EncryptionKey, bsaltkey, 1000, HashAlgorithmName.SHA256);
                    encryptor.Mode = CipherMode.CBC;
                    encryptor.Padding = PaddingMode.PKCS7;
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = cipherBytes.Take(16).ToArray();
                    cipherBytes = cipherBytes.Skip(16).ToArray();

                    using (MemoryStream ms = new())
                    {
                        using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                        }
                        plainText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                return plainText;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Decrypt_CBC_256 Fail, Input: {@cipherText}", cipherText);
            }
            return plainText;
        }

        private static string MySQL_AES_Decrypt_ECB_128(string cipherText, string EncryptKey)
        {
            if(cipherText=="")
                return "";

            string plainText = "";
            try
            {
                using (AesCryptoServiceProvider aesAlg = new())
                {
                    aesAlg.KeySize = 128;
                    aesAlg.BlockSize = 128;
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.Key = MakeFullLengthKey(EncryptKey); 
                    aesAlg.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plainText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plainText;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MySQL_AES_Decrypt Fail");
            }
            return plainText;
        }
        private static string MySQL_AES_Encrypt_ECB_128(string plainText, string EncryptKey)
        {
            if(plainText=="")
                return "";
            string cipherText = "";
            try
            {
                using (AesCryptoServiceProvider aesAlg = new())
                {
                    aesAlg.KeySize = 128;
                    aesAlg.BlockSize = 128;
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.Key = MakeFullLengthKey(EncryptKey); //Encoding.UTF8.GetBytes(Iat);
                    aesAlg.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    byte[] clearBytes = Encoding.UTF8.GetBytes(plainText);

                    // Create a encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(clearBytes, 0, clearBytes.Length);
                        }
                        cipherText = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MySQL_AES_Encrypt Fail");
            }
            return cipherText;
        }

        public static string DecryptClientString(string cipherText, string EncryptionKey = "",string SaltKey = "")
        {
           return DecryptClient_CBC_256(cipherText, EncryptionKey, SaltKey);
        } 
        private static string DecryptClient_CBC_256(string CipherText, string ClientEncryptionKey = "", string ClientEncryptionSalt = "")
        {
            if(CipherText == "")
                return "";

            string plainText = "";
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(CipherText);
                using (Aes encryptor = Aes.Create())
                {
                    if (ClientEncryptionSalt == "") 
                        ClientEncryptionSalt = _ClientEncryptionSalt;

                     if (ClientEncryptionKey == "") 
                        ClientEncryptionKey = _ClientEncryptionKey;

                    byte[] ClientSalt = Encoding.UTF8.GetBytes(ClientEncryptionSalt);

                    Rfc2898DeriveBytes pdb = new(ClientEncryptionKey, ClientSalt, 1000, HashAlgorithmName.SHA256);
                    encryptor.Mode = CipherMode.CBC;
                    encryptor.Padding = PaddingMode.PKCS7;
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = cipherBytes.Take(16).ToArray();
                    cipherBytes = cipherBytes.Skip(16).ToArray();

                    using (MemoryStream ms = new())
                    {
                        using (CryptoStream cs = new(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                        }
                        plainText = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                return plainText;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DecryptClient_CBC_256 Fail");
            }
            return plainText;
        }
        
        // private static byte[] GenerateRandomBytes(int numberOfBytes)
        // {
        //     RNGCryptoServiceProvider rng = new();
        //     var randomBytes = new byte[numberOfBytes];
        //     rng.GetBytes(randomBytes);
        //     return randomBytes;
        // }
        private static byte[] GenerateRandomBytes(int NumberOfBytes)
        {
            byte[] randomBytes = new byte[NumberOfBytes];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

       private static byte[] MergeArrays(params byte[][] arrays)
        {
            var merged = new byte[arrays.Sum(a => a.Length)];
            var mergeIndex = 0;
            for (int i = 0; i < arrays.GetLength(0); i++)
            {
                arrays[i].CopyTo(merged, mergeIndex);
                mergeIndex += arrays[i].Length;
            }

            return merged;
        }
       
        private static byte[] MakeFullLengthKey(string skey)
        {

            byte[] key = Encoding.UTF8.GetBytes(skey);
            byte[] k = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < key.Length; i++)
            {
                k[i % 16] = (byte)(k[i % 16] ^ key[i]);
            }

            return k;
        }
    }
}	