/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Drawing
 *  File:       ColorContext.cs
 *  Created:    2011-05-08
 *  Modified:   2011-05-08
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace TextUI.Drawing
{
    /// <summary>
    /// Represents context used to save current screen colors and cursor position while
    /// drawing to the screen.
    /// </summary>
    /// 
    internal class ColorContext
    {
        #region [ Fields ]

        private Screen screen;
        private Color  savedForeColor;
        private Color  savedBackColor;
        private int    savedCursorLeft;
        private int    savedCursorTop;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ColorContext class and saves
        /// current screen colors and cursor position.
        /// </summary>
        ///
        public ColorContext( Screen screen )
        {
            this.screen = screen;

            if ( this.screen != null )
            {
                this.savedForeColor  = screen.ForeColor;
                this.savedBackColor  = screen.BackColor;
                this.savedCursorLeft = screen.CursorLeft;
                this.savedCursorTop  = screen.CursorTop;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Restores original screen colors and cursor position.
        /// </summary>
        /// 
        public void Restore ()
        {
            if ( this.screen != null )
            {
                screen.ForeColor  = this.savedForeColor;
                screen.BackColor  = this.savedBackColor;
                screen.CursorLeft = this.savedCursorLeft;
                screen.CursorTop  = this.savedCursorTop;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}