﻿using System.Security.Cryptography;
using System.Text;

namespace KormoranAdminSystemRevamped.Helpers
{
	public static class Extensions
	{
		public static string Sha256(this string s)
		{
			StringBuilder sb = new();

			using (var hash = SHA256.Create())
			{
				Encoding enc = Encoding.UTF8;
				byte[] result = hash.ComputeHash(enc.GetBytes(s));

				foreach (byte b in result)
					sb.Append(b.ToString("x2"));
			}

			return sb.ToString();
		}
	}
}