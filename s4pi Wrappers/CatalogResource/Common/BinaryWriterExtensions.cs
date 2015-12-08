using System.IO;

namespace CatalogResource.Common
{
	/// <summary>
	/// Extensions for the <see cref="BinaryWriter"/> class.
	/// </summary>
	public static class BinaryWriterExtensions
	{
		/// <summary>
		/// Writes the specified number of empty bytes using the <see cref="BinaryWriter"/>.
		/// </summary>
		public static void WriteEmptyBytes(this BinaryWriter writer, int count)
		{
			for (int i = 0; i < count; i++)
			{
				writer.Write((byte)0);
			}
		}
	}
}