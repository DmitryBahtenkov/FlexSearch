using System;

namespace Core.Storage.BinaryStorage
{
	public class TreeKeyExistsException : Exception
	{
		public TreeKeyExistsException (object key) : base ("Duplicate key: " + key.ToString())
		{
			
		}
	}
}