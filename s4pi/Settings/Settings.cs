using System;
using System.Collections.Generic;

namespace s4pi.Settings
{
    /// <summary>
    /// Holds global settings, currently statically defined
    /// </summary>
    public static class Settings
    {
        static Settings()
        {
            // initialisation code, like read from settings file...
        }

        static bool checking = true;
        /// <summary>
        /// When true, run extra checks as part of normal operation.
        /// </summary>
        public static bool Checking { get { return checking; } }

        static bool asBytesWorkaround = true;
        /// <summary>
        /// When true, assume data is dirty regardless of tracking.
        /// </summary>
        public static bool AsBytesWorkaround { get { return asBytesWorkaround; } }

        static bool isTS4 = true;
        /// <summary>
        /// Indicate whether the wrapper should use TS4 format to decode files.
        /// </summary>
        public static bool IsTS4 { get { return isTS4; } }
    }
}
