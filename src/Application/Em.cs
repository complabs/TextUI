/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       Em.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents "Em" units used to layout forms scaled to form size.
/// </summary>
/// <remarks>
/// In TextUI mode, Em units are expressed in characters and are read-only constants.
/// In GUI mode, Em units are expressed pixels and adjusted when the default font 
/// is changed.
/// </remarks>
/// 
internal static class Em
{
    #if TEXTUI

        public static readonly bool IsTextUI = true;
        public static readonly bool IsGUI    = false;
        public static readonly int  Width    = 1;
        public static readonly int  Height   = 1;

    #else

        public static readonly bool IsTextUI = false;
        public static readonly bool IsGUI = true;

        // Following fields depends on selected default font:
        //
        public static int Width  =  8;
        public static int Height = 16;

    #endif
}