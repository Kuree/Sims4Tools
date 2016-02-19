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
    public enum ModelFlags : uint
    {
        NONE = 0,
        USES_INSTANCED_SHADER = (1 << 0),
        VERTICAL_SCALING = (1 << 1),
        REQUIRES_PROCEDURAL_LIGHTMAP = (1 << 2),
        USES_TREE_INSTANCE_SHADER = (1 << 3),
        HORIZONTAL_SCALING = (1 << 4),
        IS_PORTAL = (1 << 5),
        USES_COUNTER_CUTOUT = (1 << 6),
        SHARE_TERRAIN_LIGHTMAP = (1 << 7),
        USES_WALL_LIGHTMAP = (1 << 8),
        USES_CUTOUT = (1 << 9),
        INSTANCE_WITH_FULL_TRANSFORM = (1 << 10),
    }
}