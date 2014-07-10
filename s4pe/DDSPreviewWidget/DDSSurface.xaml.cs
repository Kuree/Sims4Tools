// Portions of this code are
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
using System.Windows.Controls;
using SlimDX.Wpf;

namespace S3PIDemoFE.DDSWidget
{
    /// <summary>
    /// Interaction logic for DDSSurface.xaml
    /// </summary>
    public partial class DDSSurface : UserControl
    {
        DDSRenderEngine m_renderEngine;
        public DDSSurface()
        {
            InitializeComponent();

            if (m_slimDXControl.DirectXStatus != DirectXStatus.Available)
            {
                if (m_slimDXControl.DirectXStatus == DirectXStatus.Unavailable_RemoteSession)
                    throw new DDSException("DirectX not supported when using Remote Desktop");
                if (m_slimDXControl.DirectXStatus == DirectXStatus.Unavailable_LowTier)
                    throw new DDSException("Insufficient graphics acceleration on this machine");
                else if (m_slimDXControl.DirectXStatus == DirectXStatus.Unavailable_MissingDirectX)
                    throw new DDSException("DirectX libraries are missing or need to be updated");
                else
                    throw new DDSException(String.Format("Unable to start DirectX (reason unknown: {0})", (int)m_slimDXControl.DirectXStatus));
            }
            m_renderEngine = new DDSRenderEngine(this);

            this.Loaded += new RoutedEventHandler(Window_Loaded);
            this.Unloaded += new RoutedEventHandler(Window_Closed);
        }

        bool running = false;
        public System.IO.Stream DDS
        {
            set
            {
                if (running && value == null) { m_slimDXControl.Shutdown(); }
                running = value != null;
                m_renderEngine.DDS = value;
            }
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_slimDXControl.SetRenderEngine(m_renderEngine);
        }

        void Window_Closed(object sender, RoutedEventArgs e)
        {
            m_slimDXControl.Shutdown();
        }
    }

    public class DDSException : Exception
    {
        public DDSException() : base() { }
        public DDSException(string message) : base(message) { }
        public DDSException(string message, Exception innerException) : base(message, innerException) { }
    }
}
