using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Hashing
{
    public class Password_Hash
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        public string HashPassword(string userPass)
        {
            try
            {
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);
                var pbkdf2 = new Rfc2898DeriveBytes(userPass, salt, Iterations);

                byte[] hash = pbkdf2.GetBytes(HashSize);
                byte[] hashByte = new byte[SaltSize + HashSize];

                Array.Copy(salt, 0, hashByte, 0, SaltSize);
                Array.Copy(hash, 0, hashByte, SaltSize, HashSize);

                string hashedPassword = Convert.ToBase64String(hashByte);
                return hashedPassword;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public bool VerifyPassword(string userPass, string storedHashPass)
        {
            try
            {
                byte[] hashByte = Convert.FromBase64String(storedHashPass);
                byte[] salt = new byte[SaltSize];

                Array.Copy(hashByte, 0, salt, 0, SaltSize);

                var pbkdf2 = new Rfc2898DeriveBytes(userPass, salt, Iterations);
                byte[] hash = pbkdf2.GetBytes(HashSize);

                bool result = ComparePassword(hash, hashByte);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private bool ComparePassword(byte[] hash, byte[] hashByte)
        {
            for (int i = 0; i < HashSize; i++)
            {
                if (hash[i] != hashByte[SaltSize + i]) 
                {
                    return false;
                }
            }
            return true;
        }
    }
}
