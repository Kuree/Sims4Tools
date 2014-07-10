using System;
using s4pi.Interfaces;
using System.IO;
using s4pi.GenericRCOLResource;
using System.Collections.Generic;

namespace meshExpImp.ModelBlocks
{
    public class GeometryResource : GenericRCOLResource
    {
        public GeometryResource(int APIversion, Stream s)
            : base(APIversion, s)
        {
            if (ChunkEntries.Count != 1)
                throw new InvalidDataException(String.Format("Expected one chunk, found {0}.", ChunkEntries.Count));

            var chunk = ChunkEntries[0];
            ChunkEntries.Remove(chunk);
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryWriter(ms).Write(chunk.RCOLBlock.AsBytes);
                ms.Flush();
                ms.Position = 0;
                GEOM geom = new GEOM(requestedApiVersion, OnResourceChanged, ms);
                ChunkEntries.Add(new ChunkEntry(0, OnResourceChanged, chunk.TGIBlock, geom));
            }
        }

        protected override Stream UnParse()
        {
            if (version == 0 && publicChunks == 0 && unused == 0 && blockList == null && resources == null)
            {
                // In that case, assume we're a newly created "stream == null" situation GenericRCOLResource that needs
                // some help to become a real life GeometryResource.

                resources = new CountedTGIBlockList(OnResourceChanged, "ITG");
                // Unfortunately, a Resource does not know its own ResourceKey.  This is the best we can manage.
                TGIBlock rk = new TGIBlock(0, null, 0x015A1849, 0, 0);
                GEOM geom = new GEOM(requestedApiVersion, OnResourceChanged);
                blockList = new ChunkEntryList(OnResourceChanged, new ChunkEntry[] { new ChunkEntry(0, OnResourceChanged, rk, geom), }) { ParentTGIBlocks = Resources, };
            }
            return base.UnParse();
        }
    }

    /// <summary>
    /// ResourceHandler for GeometryResource wrapper
    /// </summary>
    public class GeometryResourceHandler : AResourceHandler
    {
        public GeometryResourceHandler()
        {
            this.Add(typeof(GeometryResource), new List<string>(new string[] { "0x015A1849", }));
        }
    }
}
