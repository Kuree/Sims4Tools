// Portions of this code
// Copyright (c) 2010, LizardTech, a Celartem company
// All rights reserved.
//                                                              
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of LizardTech nor the names of its contributors may be
//       used to endorse or promote products derived from this software without
//       specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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
using System.Windows;
using System.Windows.Resources;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Wpf;
using System.IO;
using System.Drawing;

namespace S3PIDemoFE.DDSWidget
{
    class DDSRenderEngine : IRenderEngine
    {
        private Sprite sprite;
        Texture texture;
        Stream dds = null;
        DDSSurface surface;

        public DDSRenderEngine(DDSSurface surface) { this.surface = surface; }

        public void OnDeviceCreated(object sender, EventArgs e)
        {
            return;
        }

        private void free()
        {
            if (sprite != null && !sprite.Disposed)
            {
                sprite.Dispose();
                //? sprite = null;
            }
            if (texture != null && !texture.Disposed)
            {
                texture.Dispose();
                texture = null;
            }
        }

        public void OnDeviceDestroyed(object sender, EventArgs e) { free(); }

        public void OnDeviceLost(object sender, EventArgs e) { free(); }

        public void OnDeviceReset(object sender, EventArgs e)
        {
            if (dds == null) return;
            free();

            SlimDXControl control = surface.m_slimDXControl;
            sprite = new Sprite(control.Device);
            dds.Position = 0;
            texture = Texture.FromStream(control.Device, dds, Usage.None, Pool.Default);
        }

        public void OnMainLoop(object sender, EventArgs e)
        {
            if (dds == null) return;

            sprite.Begin(SpriteFlags.AlphaBlend);
            sprite.Draw(texture, Vector3.Zero, Vector3.Zero, new Color4(System.Drawing.SystemColors.AppWorkspace));
            sprite.End();
        }

        public Stream DDS
        {
            set
            {
                if (dds != null) { dds = null; free(); }
                dds = value;
                surface.m_slimDXControl.ForceRendering();
                OnDeviceReset(this, EventArgs.Empty);
            }
        }
    }
}
