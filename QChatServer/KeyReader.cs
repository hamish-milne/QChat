using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace QChatServer
{
	public enum CertificateFormat
	{
		Auto,
		PEM,
		DER,
		P7B,
		PFX,
	}

	public static class CertificateBuilder
	{
		public static X509Certificate Get()
		{
			var cert = new X509Certificate2();
			
		}
	}
}
