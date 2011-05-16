/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       MainMenu.cs
 *  Created:    2011-04-15
 *  Modified:   2011-04-26
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace TextUI
{
    /// <summary>
    /// Represents the menu structure of a form. 
    /// </summary>
    /// 
    public class MainMenu : Menu
    {
        /// <summary>
        /// Initializes a new instance of the MainMenu class without any specified 
        /// menu items. 
        /// </summary>
        /// 
        public MainMenu ()
            : base ()
        {
            IsMainMenu        = true;
            Border            = false;
            LeftPadding       = 1;
            RightPadding      = 1;
            HorizontalSpacing = 1;
        }
    }
}