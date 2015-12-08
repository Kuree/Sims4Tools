using System;
using System.IO;

namespace CatalogResource.Common
{
	/// <summary>
	/// Extensions for the <see cref="BinaryReader"/> class.
	/// </summary>
	public static class BinaryReaderExtensions
	{
		/// <summary>
		/// Reads the specified number of bytes using the <see cref="BinaryReader"/>.
		/// </summary>
		public static void ReadAndDiscardBytes(this BinaryReader reader, int count)
		{
			for (int i = 0; i < count; i++)
			{
				reader.ReadByte();
			}
		}
	}
}
