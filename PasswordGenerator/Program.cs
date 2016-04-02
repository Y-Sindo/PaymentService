using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; //连接SQLServer 数据库专用
using System.Security.Cryptography;

namespace PasswordGenerator
{

    abstract class MyRAS
    {
        /// <summary>
        /// RAS加密
        /// </summary>
        /// <param name="PrivateKey">私钥</param>
        /// <param name="EncryptString">明文</param>
        /// <returns>密文</returns>
        public static string RSAEncrypt(string PrivateKey, string EncryptString)
        {
            byte[] PlainTextBArray;
            byte[] CypherTextBArray;
            string Result = String.Empty;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PrivateKey);
            int t = (int)(Math.Ceiling((double)EncryptString.Length / (double)50));
            //分割明文
            for (int i = 0; i <= t - 1; i++)
            {
                PlainTextBArray = (new UnicodeEncoding()).GetBytes(EncryptString.Substring(i * 50, EncryptString.Length - (i * 50) > 50 ? 50 : EncryptString.Length - (i * 50)));
                CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
                Result += Convert.ToBase64String(CypherTextBArray) + "ThisIsSplit";
            }
            return Result;
        }
        /// <summary>
        /// RAS解密
        /// </summary>
        /// <param name="PublicKey">公钥</param>
        /// <param name="DecryptString">密文</param>
        /// <returns>明文</returns>
        public static string RSADecrypt(string PublicKey, string DecryptString)
        {
            byte[] PlainTextBArray;
            byte[] DypherTextBArray;
            string Result = String.Empty;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(PublicKey);
            string[] Split = new string[1];
            Split[0] = "ThisIsSplit";
            //分割密文
            string[] mis = DecryptString.Split(Split, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < mis.Length; i++)
            {
                PlainTextBArray = Convert.FromBase64String(mis[i]);
                DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
                Result += (new UnicodeEncoding()).GetString(DypherTextBArray);
            }
            return Result;
        }
        /// <summary>
        /// 产生公钥和私钥对
        /// </summary>
        /// <returns>string[] 0:私钥;1:公钥</returns>
        public static string[] RSAKey()
        {
            string[] keys = new string[2];
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            keys[0] = rsa.ToXmlString(true);
            keys[1] = rsa.ToXmlString(false);
            return keys;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string[] key = MyRAS.RSAKey();
            Console.WriteLine("私钥：" + key[0]);
            Console.WriteLine("公钥：" + key[1]);
            string chphertxet = MyRAS.RSAEncrypt("<RSAKeyValue><Modulus>3ICZEvo7Vod0g8dWpykEnTo9tNdQT72s+XfcPjPEiCnd5Sg5pklT6kety9DtofPTD8fLYf7zhcw0L7ND3MUiBC+5qrqms9I5TVMd4SLV0BgCr8FGKicxjMTM0ptWmqD/LT2+n9Dg0MVzWSdHFDhQUdorNPmTdG8he9IpsiAlxYs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", "443");
            Console.WriteLine(chphertxet);
            string plaintxet = MyRAS.RSADecrypt("<RSAKeyValue><Modulus>3ICZEvo7Vod0g8dWpykEnTo9tNdQT72s+XfcPjPEiCnd5Sg5pklT6kety9DtofPTD8fLYf7zhcw0L7ND3MUiBC+5qrqms9I5TVMd4SLV0BgCr8FGKicxjMTM0ptWmqD/LT2+n9Dg0MVzWSdHFDhQUdorNPmTdG8he9IpsiAlxYs=</Modulus><Exponent>AQAB</Exponent><P>8Z69Gevu2jHM/efz8icMzH4E+4+GawIwsoKoKhM1mE6VDrqpbQE0g9WMZhb5TPL9OdPP9X6RINKZFCy840q5jw==</P><Q>6aAdZwABjdjICvu6HYsKD5Cz6BcoXzVBYO43FRz42IiOdub3pSxW25vo2jAfNt6FyfezqS3zw/JCFrF84Q0eRQ==</Q><DP>hckfa/r3hlmM6ApHSQ3WSGR+3cva4eWUkUNHWgTI6EyavE0fAvxn15em5eBSqgjhreNagtRSB5qUqFcdP71ggQ==</DP><DQ>MbCHcGfU7MmFyqg9rpjq++KIET4TrSRTkn04I/p0hwGMY4e+dlgW5UCk5vtDOFVd2VYg1UPby/pTyiXX7LnwjQ==</DQ><InverseQ>488VnomjdBj3kieQdre4/kmnet6B5wpuhRfodrceaCdsNnuzLBVGbms8eq7+4CCPbxBl+8C+zyohNJZaCO73tg==</InverseQ><D>YT5rg7WMjrwdM/Ko8f6CdO6XARaF6izdyIVGQb5l2aERJXtIdV/YXLVw3baN3kMgHVEBDn44GpU0nKbD8myT85AbO5KCHaVJ5meB5D14Hzxj+sbBi0+1UylNwnUs0sCoH6UtUGF8wUnb4ozg7roqxIBBYn+S4ihUcltlSra+Ci0=</D></RSAKeyValue>", chphertxet);
            Console.WriteLine(plaintxet);
            Console.Read();
        }
    }
}
/* 
          string[] key  = MyRAS.RSAKey();
          Console.WriteLine(key[0].Length);
          Console.WriteLine(key[1].Length);
          string chphertxet=MyRAS.RSAEncrypt(key[1], "你知道什么呀!");
          Console.WriteLine(chphertxet);
          string plaintxet = MyRAS.RSADecrypt(key[0], chphertxet);
          Console.WriteLine(plaintxet);
          Console.Read(); 
*/
