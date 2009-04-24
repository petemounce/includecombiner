using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nrws
{
	public class KeyGenerator : IKeyGenerator
	{
		public string Generate(IEnumerable<string> generateFrom)
		{
			if (generateFrom == null)
			{
				throw new ArgumentNullException("generateFrom");
			}
			var longKey = new StringBuilder();
			foreach(var s in generateFrom)
			{
				longKey.Append("|").Append(s);
			}
			longKey.Remove(0, 1);
			var toHashBytes = Encoding.UTF8.GetBytes(longKey.ToString());
			var hashAlgorithm = SHA1.Create();
			var hashed = hashAlgorithm.ComputeHash(toHashBytes);
			return Convert.ToBase64String(hashed);
		}
	}
}