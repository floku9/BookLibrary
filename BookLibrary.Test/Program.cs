using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace BookLibrary.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Regex.IsMatch("aAcasdd", @"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{6,}"));
            Console.ReadKey();
            //var pass = "12345adafasfda";
            //var guid = Guid.NewGuid();
            //var hMACMD5 = new HMACMD5(System.Text.Encoding.UTF8.GetBytes("aaaa"));
            //var saltedHash = hMACMD5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pass));
            //var stringHash = Convert.ToBase64String(saltedHash);
            //var a = "b";
        }
        //public string GenerateSalt(int length)
        //{
        //    const string chars = "abcdefghijklnmopqstufwxyz0*@#$%%^&*())_+ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //}
    }
}
