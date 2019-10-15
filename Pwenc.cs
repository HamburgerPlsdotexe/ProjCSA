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
            return buffer;
        }
        private byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8, // four cores
                Iterations = 24,
                MemorySize = 128 * 128 // 1 GB
            };

            return argon2.GetBytes(16);
        }
        private bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
        public string Run(string password)
        {
            var salt = CreateSalt();
            var hash = HashPassword(password, salt);
            if (VerifyHash(password, salt, hash))
            {
                return Convert.ToBase64String(hash);
            }
            else
            {
                return "dosnt work blyat";
            }
        }
    }
}