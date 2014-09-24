/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	internal sealed class DdsSquish
    {
        #region SquishFlags
        [Flags]
        public enum SquishFlags
        {
            /// <summary>
            /// Use DXT1 compression.
            /// </summary>
            kDxt1 = (1 << 0),
            /// <summary>
            /// Use DXT3 compression.
            /// </summary>
            kDxt3 = (1 << 1),
            /// <summary>
            /// Use DXT5 compression.
            /// </summary>
            kDxt5 = (1 << 2),

            /// <summary>
            /// Use a slow but high quality colour compressor (the default).
            /// </summary>
            kColourClusterFit = (1 << 3),
            /// <summary>
            /// Use a fast but low quality colour compressor.
            /// </summary>
            kColourRangeFit = (1 << 4),

            /// <summary>
            /// Use a perceptual metric for colour error (the default).
            /// </summary>
            kColourMetricPerceptual = (1 << 5),
            /// <summary>
            /// Use a uniform metric for colour error.
            /// </summary>
            kColourMetricUniform = (1 << 6),

            /// <summary>
            /// Weight the colour by alpha during cluster fit (disabled by default).
            /// </summary>
            kWeightColourByAlpha = (1 << 7),

            /// <summary>
            /// Use a very slow but very high quality colour compressor.
            /// </summary>
            kColourIterativeClusterFit = (1 << 8),
        }
        #endregion

#if NOTIMPLEMENTED
        /// <summary>
        /// Compresses a 4x4 block of pixels.
        /// </summary>
        /// <param name="pixelInput">The rgba values of the 16 source pixels.</param>
        /// <param name="mask">The valid pixel mask.</param>
        /// <param name="flags">Compression flags.</param>
        /// <param name="metric">An optional perceptual metric.</param>
        /// <returns>Byte array containing the compressed DXT block.</returns>
        /// <remarks>
        /// <para>The source pixels should be presented as a contiguous array of 16 rgba
        /// values, with each component as 1 byte each. In memory this should be:
        /// rgba values, with each component as 1 byte each. In memory this should be:
        /// <c>{ r1, g1, b1, a1, .... , rn, gn, bn, an }</c> for n = width*height
        /// </para>
        /// <para>
        /// The <paramref name="flags"/> parameter should specify either <c>kDxt1</c>, <c>kDxt3</c> or <c>kDxt5</c> compression, 
        /// however, DXT1 will be used by default if none is specified. When using DXT1 
        /// compression, 8 bytes of storage are required for each compressed DXT block. 
        /// DXT3 and DXT5 compression require 16 bytes of storage per block.
        /// </para>
        /// <para>
        /// The <paramref name="flags"/> parameter can also specify a preferred colour compressor to use 
        /// when fitting the RGB components of the data. Possible colour compressors 
        /// are: <c>kColourClusterFit</c> (the default), <c>kColourRangeFit</c> (very fast, low 
        /// quality) or <c>kColourIterativeClusterFit</c> (slowest, best quality).
        /// </para>
        /// <para>
        /// When using <c>kColourClusterFit</c> or <c>kColourIterativeClusterFit</c>, an additional 
        /// flag can be specified to weight the importance of each pixel by its alpha 
        /// value. For images that are rendered using alpha blending, this can 
        /// significantly increase the perceived quality.</para>
        /// <para>
        /// The <paramref name="metric"/> parameter can be used to weight the relative importance of each
        /// colour channel, or pass NULL to use the default uniform weight of 
        /// <c>{ 1.0f, 1.0f, 1.0f }</c>. This replaces the previous flag-based control that 
        /// allowed either uniform or "perceptual" weights with the fixed values
        /// <c>{ 0.2126f, 0.7152f, 0.0722f }</c>. If non-NULL, the metric should point to a 
        /// contiguous array of 3 floats.
        /// </para>
        /// </remarks>
        public static byte[] CompressMasked(byte[] pixelInput, int mask, SquishFlags flags, float[] metric = null)
        {
            if (metric != null && metric.Length != 3)
                throw new ArgumentException("Non-null metric must reference an array of three floats.");

            byte[] block = new byte[(flags & SquishFlags.kDxt1) != 0 ? 8 : 16];

            SquishCompressMasked(pixelInput, mask, block, flags, metric);

            return block;
        }
#endif

#if NOTIMPLEMENTED
        /// <summary>
        /// Compresses a 4x4 block of pixels.
        /// </summary>
        /// <param name="pixelInput">The rgba values of the 16 source pixels.</param>
        /// <param name="flags">Compression flags.</param>
        /// <param name="metric">An optional perceptual metric.</param>
        /// <returns>Byte array containing the compressed DXT block.</returns>
        /// <remarks>
        /// <para>The source pixels should be presented as a contiguous array of 16 rgba
        /// values, with each component as 1 byte each. In memory this should be:
        /// rgba values, with each component as 1 byte each. In memory this should be:
        /// <c>{ r1, g1, b1, a1, .... , rn, gn, bn, an }</c> for n = width*height
        /// </para>
        /// <para>
        /// The <paramref name="flags"/> parameter should specify either <c>kDxt1</c>, <c>kDxt3</c> or <c>kDxt5</c> compression, 
        /// however, DXT1 will be used by default if none is specified. When using DXT1 
        /// compression, 8 bytes of storage are required for each compressed DXT block. 
        /// DXT3 and DXT5 compression require 16 bytes of storage per block.
        /// </para>
        /// <para>
        /// The <paramref name="flags"/> parameter can also specify a preferred colour compressor to use 
        /// when fitting the RGB components of the data. Possible colour compressors 
        /// are: <c>kColourClusterFit</c> (the default), <c>kColourRangeFit</c> (very fast, low 
        /// quality) or <c>kColourIterativeClusterFit</c> (slowest, best quality).
        /// </para>
        /// <para>
        /// When using <c>kColourClusterFit</c> or <c>kColourIterativeClusterFit</c>, an additional 
        /// flag can be specified to weight the importance of each pixel by its alpha 
        /// value. For images that are rendered using alpha blending, this can 
        /// significantly increase the perceived quality.</para>
        /// <para>
        /// The <paramref name="metric"/> parameter can be used to weight the relative importance of each
        /// colour channel, or pass NULL to use the default uniform weight of 
        /// <c>{ 1.0f, 1.0f, 1.0f }</c>. This replaces the previous flag-based control that 
        /// allowed either uniform or "perceptual" weights with the fixed values
        /// <c>{ 0.2126f, 0.7152f, 0.0722f }</c>. If non-NULL, the metric should point to a 
        /// contiguous array of 3 floats.
        /// </para>
        /// </remarks>
        public static byte[] Compress(byte[] pixelInput, SquishFlags flags, float[] metric = null)
        {
            if (metric != null && metric.Length != 3)
                throw new ArgumentException("Non-null metric must reference an array of three floats.");

            byte[] block = new byte[(flags & SquishFlags.kDxt1) != 0 ? 8 : 16];

            SquishCompress(pixelInput, block, flags, metric);

            return block;
        }
#endif

#if NOTIMPLEMENTED
        /// <summary>
        /// Computes the amount of compressed storage required.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="flags">Compression flags.</param>
        /// <returns>The amount of compressed storage required.</returns>
        /// <remarks>
        /// <para>
        /// The <paramref name="flags"/> parameter should specify either <c>kDxt1</c>, <c>kDxt3</c> or <c>kDxt5</c> compression, 
        /// however, DXT1 will be used by default if none is specified. All other flags 
        /// are ignored.
        /// </para>
        /// <para>
        /// Most DXT images will be a multiple of 4 in each dimension, but this 
        /// function supports arbitrary size images by allowing the outer blocks to
        /// be only partially used.
        /// </para>
        /// </remarks>
        public static int GetStorageRequirements(int width, int height, SquishFlags flags)
        {
            return SquishGetStorageRequirements(width, height, flags);
        }
#endif
        /// <summary>
        /// Compresses an image in memory.
        /// </summary>
        /// <param name="pixelInput">The pixels of the source.</param>
        /// <param name="width">The width of the source image.</param>
        /// <param name="height">The height of the source image.</param>
        /// <param name="flags">Compression flags.</param>
        /// <param name="metric">An optional perceptual metric.</param>
        /// <returns>Byte array containing the compressed output.</returns>
        /// <remarks>
        /// <para>
        /// The source pixels should be presented as a contiguous array of width*height
        /// rgba values, with each component as 1 byte each. In memory this should be:
        /// <c>{ r1, g1, b1, a1, .... , rn, gn, bn, an }</c> for n = width*height
        /// </para>
        /// <para>
        /// The <paramref name="flags"/> parameter should specify either <c>kDxt1</c>, <c>kDxt3</c> or <c>kDxt5</c> compression, 
        /// however, DXT1 will be used by default if none is specified. When using DXT1 
        /// compression, 8 bytes of storage are required for each compressed DXT block. 
        /// DXT3 and DXT5 compression require 16 bytes of storage per block.
        /// </para>
        /// <para>
        /// The <paramref name="flags"/> parameter can also specify a preferred colour compressor to use 
        /// when fitting the RGB components of the data. Possible colour compressors 
        /// are: <c>kColourClusterFit</c> (the default), <c>kColourRangeFit</c> (very fast, low 
        /// quality) or <c>kColourIterativeClusterFit</c> (slowest, best quality).
        /// </para>
        /// <para>
        /// When using <c>kColourClusterFit</c> or <c>kColourIterativeClusterFit</c>, an additional 
        /// flag can be specified to weight the importance of each pixel by its alpha 
        /// value. For images that are rendered using alpha blending, this can 
        /// significantly increase the perceived quality.</para>
        /// <para>
        /// The <paramref name="metric"/> parameter can be used to weight the relative importance of each
        /// colour channel, or pass NULL to use the default uniform weight of 
        /// <c>{ 1.0f, 1.0f, 1.0f }</c>. This replaces the previous flag-based control that 
        /// allowed either uniform or "perceptual" weights with the fixed values
        /// <c>{ 0.2126f, 0.7152f, 0.0722f }</c>. If non-NULL, the metric should point to a 
        /// contiguous array of 3 floats.
        /// </para>
        /// </remarks>
        public static byte[] CompressImage(byte[] pixelInput, int width, int height, SquishFlags flags, float[] metric = null)
        {
            if (metric != null && metric.Length != 3)
                throw new ArgumentException("Non-null metric must reference an array of three floats.");

            //int storageRequirements = GetStorageRequirements(width, height, flags);
            byte[] blocks = new byte[((width + 3) >> ((flags & DdsSquish.SquishFlags.kDxt1) == 0 ? 2 : 3)) * 4 * height];

            // Invoke squish::CompressImage() with the required parameters
            SquishCompressImage(pixelInput, width, height, blocks, flags, metric);

            return blocks;
        }

#if NOTIMPLEMENTED
        /// <summary>
        /// Decompresses a 4x4 block of pixels.
        /// </summary>
        /// <param name="block">The compressed DXT block.</param>
        /// <param name="width">The width of the source image.</param>
        /// <param name="height">The height of the source image.</param>
        /// <param name="flags">Compression flags.</param>
        /// <returns>Byte array containing the 16 decompressed pixels.</returns>
        /// <remarks>The decompressed pixels will be written as a contiguous array of 16 rgba
        /// values, with each component as 1 byte each. In memory this is:
        /// <code>{ r1, g1, b1, a1, .... , r16, g16, b16, a16 }</code>
        /// The <paramref name="flags"/> parameter should specify either <c>kDxt1</c>, <c>kDxt3</c> or <c>kDxt5</c> compression, 
        /// however, DXT1 will be used by default if none is specified. All other flags are ignored.</remarks>
        public static byte[] Decompress(byte[] block, SquishFlags flags)
        {
            // Allocate room for decompressed output
            byte[] pixelOutput = new byte[16];

            // Invoke squish::Decompress() with the required parameters
            SquishDecompress(pixelOutput, block, flags);

            // Return our pixel data to caller..
            return pixelOutput;
        }
#endif

        /// <summary>
        /// Decompresses an image in memory.
        /// </summary>
        /// <param name="blocks">The compressed DXT blocks.</param>
        /// <param name="width">The width of the source image.</param>
        /// <param name="height">The height of the source image.</param>
        /// <param name="flags">Compression flags.</param>
        /// <returns>Byte array containing the decompressed pixels.</returns>
        /// <remarks>The decompressed pixels will be written as a contiguous array of width*height
        /// 16 rgba values, with each component as 1 byte each. In memory this is:
        /// <code>{ r1, g1, b1, a1, .... , rn, gn, bn, an } for n = width*height</code>
        /// The <paramref name="flags"/> parameter should specify either <c>kDxt1</c>, <c>kDxt3</c> or <c>kDxt5</c> compression, 
        /// however, DXT1 will be used by default if none is specified. All other flags are ignored.</remarks>
        public static byte[] DecompressImage(byte[] blocks, int width, int height, SquishFlags flags)
		{
			// Allocate room for decompressed output
			byte[]	pixelOutput	= new byte[ width * height * 4 ];

			// Invoke squish::DecompressImage() with the required parameters
            SquishDecompressImage(pixelOutput, width, height, blocks, flags);

			// Return our pixel data to caller..
			return pixelOutput;
		}

        private static bool Is64Bit() { return (Marshal.SizeOf(IntPtr.Zero) == 8); }

#if NOTIMPLEMENTED
		[DllImport("kernel32.dll")]
		static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);
		private const int PF_MMX_INSTRUCTIONS_AVAILABLE = 3;
		private static unsafe bool IsSSE2Present() { return IsProcessorFeaturePresent(PF_MMX_INSTRUCTIONS_AVAILABLE); }
#endif

        private sealed class SquishInterface_32
        {
#if NOTIMPLEMENTED
            [DllImport("squishinterface_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishCompressMasked(byte* rgba, int mask, byte* block, int flags, float* metric);
#endif

#if NOTIMPLEMENTED
            [DllImport("squishinterface_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishCompress(byte* rgba, byte* block, int flags, float* metric);
#endif

            [DllImport("squishinterface_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishCompressImage(byte* rgba, int width, int height, byte* blocks, int flags, float* metric);

#if NOTIMPLEMENTED
            [DllImport("squishinterface_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe int SquishGetStorageRequirements(int width, int height, int flags);
#endif

#if NOTIMPLEMENTED
            [DllImport("squishinterface_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishDecompress(byte* rgba, byte* block, int flags);
#endif

            [DllImport("squishinterface_Win32.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishDecompressImage(byte* rgba, int width, int height, byte* blocks, int flags);
        }

        private sealed class SquishInterface_64
        {
#if NOTIMPLEMENTED
            [DllImport("squishinterface_x64.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishCompressMasked(byte* rgba, int mask, byte* block, int flags, float* metric);
#endif

#if NOTIMPLEMENTED
            [DllImport("squishinterface_x64.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishCompress(byte* rgba, byte* block, int flags, float* metric);
#endif

            [DllImport("squishinterface_x64.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishCompressImage(byte* rgba, int width, int height, byte* blocks, int flags, float* metric);

#if NOTIMPLEMENTED
            [DllImport("squishinterface_x64.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe int SquishGetStorageRequirements(int width, int height, int flags);
#endif

#if NOTIMPLEMENTED
            [DllImport("squishinterface_x64.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishDecompress(byte* rgba, byte* block, int flags);
#endif

            [DllImport("squishinterface_x64.dll", CallingConvention = CallingConvention.Cdecl)]
            internal static extern unsafe void SquishDecompressImage(byte* rgba, int width, int height, byte* blocks, int flags);
        }

#if NOTIMPLEMENTED
        private static unsafe void SquishCompressMasked(byte[] rgba, int mask, byte[] block, SquishFlags flags, float[] metric)
        {
            fixed (byte* _rgba = rgba)
            fixed (byte* _block = block)
            fixed (float* _metric = metric)
            {
                if (Is64Bit())
                {
                    SquishInterface_64.SquishCompressMasked(_rgba, mask, _block, (int)flags, _metric);
                }
                else
                {
                    SquishInterface_32.SquishCompressMasked(_rgba, mask, _block, (int)flags, _metric);
                }
            }
        }
#endif
#if NOTIMPLEMENTED
        private static unsafe void SquishCompress(byte[] rgba, byte[] block, SquishFlags flags, float[] metric)
        {
            fixed (byte* _rgba = rgba)
            fixed (byte* _block = block)
            fixed (float* _metric = metric)
            {
                if (Is64Bit())
                {
                    SquishInterface_64.SquishCompress(_rgba, _block, (int)flags, _metric);
                }
                else
                {
                    SquishInterface_32.SquishCompress(_rgba, _block, (int)flags, _metric);
                }
            }
        }
#endif
        private static unsafe void SquishCompressImage(byte[] rgba, int width, int height, byte[] blocks, SquishFlags flags, float[] metric)
        {
            fixed (byte* _rgba = rgba)
            fixed (byte* _blocks = blocks)
            fixed (float* _metric = metric)
            {
                if (Is64Bit())
                {
                    SquishInterface_64.SquishCompressImage(_rgba, width, height, _blocks, (int)flags, _metric);
                }
                else
                {
                    SquishInterface_32.SquishCompressImage(_rgba, width, height, _blocks, (int)flags, _metric);
                }
            }
        }
#if NOTIMPLEMENTED
        private static unsafe int SquishGetStorageRequirements(int width, int height, SquishFlags flags)
        {
            if (Is64Bit())
                return SquishInterface_64.SquishGetStorageRequirements(width, height, (int)flags);
            else
                return SquishInterface_32.SquishGetStorageRequirements(width, height, (int)flags);
        }
#endif
#if NOTIMPLEMENTED
        private static unsafe void SquishDecompress(byte[] rgba, byte[] block, SquishFlags flags)
        {
            fixed (byte* pRGBA = rgba)
            fixed (byte* pBlock = block)
            {
                if (Is64Bit())
                    SquishInterface_64.SquishDecompress(pRGBA, pBlock, (int)flags);
                else
                    SquishInterface_32.SquishDecompress(pRGBA, pBlock, (int)flags);
            }
        }
#endif
        private static unsafe void SquishDecompressImage(byte[] rgba, int width, int height, byte[] blocks, SquishFlags flags)
        {
            fixed (byte* pRGBA = rgba)
            fixed (byte* pBlocks = blocks)
            {
                if (Is64Bit())
                    SquishInterface_64.SquishDecompressImage(pRGBA, width, height, pBlocks, (int)flags);
                else
                    SquishInterface_32.SquishDecompressImage(pRGBA, width, height, pBlocks, (int)flags);
            }
        }
    }
}
