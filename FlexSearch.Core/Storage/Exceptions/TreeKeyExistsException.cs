using System;

namespace Core.Storage.Exceptions
{
	public class TreeKeyExistsException : Exception
	{
		public TreeKeyExistsException(object key) : base ("Duplicate key: " + key)
		{
			
		}
	}
}