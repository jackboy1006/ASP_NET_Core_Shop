using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MyDLL
{
	public class Account
	{
        /// <summary>
        /// 雜湊SHA256
        /// </summary>
        /// <param name="uInput">輸入值</param>
        /// <returns></returns>
        public static string GetSHA1Hash(string uInput)
        {
			SHA1 SHA1Hasher = SHA1.Create();

			Byte[] data = SHA1Hasher.ComputeHash(Encoding.Default.GetBytes(uInput));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
