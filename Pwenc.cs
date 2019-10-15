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
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8, 
                Iterations = 24,
                MemorySize = 128 * 128 
            };
            argon2.Dispose();
            return argon2.GetBytes(16);
        }   
        private bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
        public string Run(string password, bool state)
        {
            var salt = CreateSalt();
            var hash = HashPassword(password, salt);
            byte[][] fused = new byte[][] { hash, salt };

            if (state)
            {

                if (VerifyHash(password, salt, hash))
                {
                    return Convert.ToBase64String(fused[0]);
                }

                else
                {
                    return "";
                }
            }
            else
            {
                return Convert.ToBase64String(fused[1]);
            }
        }
    }
}