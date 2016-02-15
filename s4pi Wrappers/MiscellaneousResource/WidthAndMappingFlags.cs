/***************************************************************************
 *  Copyright (C) 2016 by the Sims 4 Tools development team                *
 *                                                                         *
 *  Contributors:                                                          *
 *  pbox                                                                   *
 *  Buzzler                                                                * 
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using System;

namespace s4pi.Miscellaneous
{ 
    [Flags]
    public enum WidthAndMappingFlags : byte
    {
        NONE = 0,
        WIDTH_CUTOUT_1 = (1 << 0),
        WIDTH_CUTOUT_2 = (1 << 1),
        WIDTH_CUTOUT_3 = (1 << 2),
        NO_OPAQUE = (1 << 3),
        IS_ARCHWAY = (1 << 4),
        SINGLE_TEXTURE_CUTOUT = (1 << 5),
        DIAGONAL_CUTOUT_MAPPING_IN_USE = (1 << 6),
    }
}