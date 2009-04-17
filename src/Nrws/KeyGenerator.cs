using System;
using System.Security.Cryptography;
using System.Text;

namespace Nrws
{
	public class KeyGenerator : IKeyGenerator
	{
		public string Generate(string generateFrom)
		{
			var toHashBytes = Encoding.UTF8.GetBytes(generateFrom);
			var hashAlgorithm = SHA1.Create();
			var hashed = hashAlgorithm.ComputeHash(toHashBytes);
			return Convert.ToBase64String(hashed);
		}
	}
}