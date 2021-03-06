﻿/*
    [Reloaded] Mod Loader Common Library (libReloaded)
    The main library acting as common, shared code between the Reloaded Mod 
    Loader Launcher, Mods as well as plugins.
    Copyright (C) 2018  Sewer. Sz (Sewer56)

    [Reloaded] is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    [Reloaded] is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
*/

namespace Reloaded.Overlay.External.WPF.Structures
{
    /// <summary>
    /// Defines the following modes of the current window.
    /// </summary>
    public enum WindowFollowMode
    {
        /// <summary>
        /// Window will behave as normal.
        /// </summary>
        None,

        /// <summary>
        /// Window will follow movements of game window.
        /// </summary>
        FollowWindow,

        /// <summary>
        /// Window will overlay parent window's client area.
        /// </summary>
        Overlay
    }
}
