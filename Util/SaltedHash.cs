using System;
using System.Security.Cryptography;

namespace TodoApi.Util
{
    public class SaltedHash
    {
        public const int SALT_BYTE_SIZE = 24;
        public const int HASH_BYTE_SIZE = 24;
        public const int PBKDF2_ITERATIONS = 1000;

        public string Hash { get; private set; }
        public string Salt { get; private set; }

        public SaltedHash(string password)
        {
            Salt = GenerateSalt();
            Hash = ComputeHash(Salt, password);
        }

        public static string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buff = new byte[SALT_BYTE_SIZE];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string ComputeHash(string salt, string password)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, PBKDF2_ITERATIONS))
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(HASH_BYTE_SIZE));
        }

        public static bool Verify(string salt, string hash, string password)
        {
            return hash == ComputeHash(salt, password);
        }
    }
}	