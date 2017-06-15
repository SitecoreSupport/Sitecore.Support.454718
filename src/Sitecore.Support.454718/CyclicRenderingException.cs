using Sitecore.Exceptions;
using System;

namespace Sitecore.Mvc.Exceptions
{
	[Serializable]
	public class CyclicRenderingException : SitecoreException
	{
		public CyclicRenderingException(string message) : base(message)
		{
		}
	}
}
