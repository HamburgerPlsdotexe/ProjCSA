using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Konscious.Security.Cryptography;
using DataLibrary;

namespace ProjectCSA
{
    public class Pwenc
    {
        private byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            rng.Dispose();
            return buffer;
        }
        private byte[] HashPassword(string password, byte[] salt)
        {
            Argon2id argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8, 
                Iterations = 24,
                MemorySize = 128 * 128 
            };
            argon2.Dispose();
            return argon2.GetBytes(16);
        }
        public bool Verifyhash(string password, byte[] salt, byte[] hash)
        {
            var newhash = HashPassword(password, salt);
            return hash.SequenceEqual(newhash);
        }
        public string[] Run(string password)
        {
            var salt = CreateSalt();
            var hash = HashPassword(password, salt);
            byte[][] fused = new byte[][] { hash, salt };

            string[] intermediate = new string[] { Convert.ToBase64String(fused[0]), Convert.ToBase64String(fused[1]) };

            return intermediate;
            //if (VerifyHash(password, salt, hash))
            //{
            //    return fused;
            //}

            // else
            // {
            //     return empty;
            // }
        }
        
        public string GetHashPw(string password, byte[] salt)
        {
            return Convert.ToBase64String(HashPassword(password, salt));
        }
    }
}