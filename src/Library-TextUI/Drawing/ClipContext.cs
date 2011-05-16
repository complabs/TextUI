/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Drawing
 *  File:       ClipContext.cs
 *  Created:    2011-03-16
 *  Modified:   2011-03-28
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
    /// Represents clipping context with offset and clipping region used 
    /// while drawing on the screen.
    /// </summary>
    /// 
    internal class ClipContext
    {
        #region [ Fields ]

        private Screen    screen;
        private Point     savedOffset;
        private Rectangle savedClipRegion;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ClipContext class for the specified
        /// screen relative to specified offset. 
        /// </summary>
        /// 
        public ClipContext( Screen screen, int left, int top )
        {
            this.screen = screen;

            if ( this.screen != null )
            {
                this.savedOffset = screen.Offset;
                this.savedClipRegion = screen.ClipRegion;

                this.screen.AddOffset( left, top );
            }
        }

        #endregion 

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Adds clipping region to the clipping context.
        /// </summary>
        /// 
        public void Clip( int width, int height )
        {
            if ( this.screen != null )
            {
                this.screen.AddClipRegion( width, height );
            }
        }

        /// <summary>
        /// Restores original clipping region (saved at the time when object was
        /// constructed).
        /// </summary>
        /// 
        public void RestoreClipRegion ()
        {
            if ( this.screen != null )
            {
                this.screen.ClipRegion = this.savedClipRegion;
            }
        }

        /// <summary>
        /// Restores both offset and clipping region saved when object was
        /// instantiated.
        /// </summary>
        /// 
        public void Restore ()
        {
            if ( this.screen != null )
            {
                this.screen.Offset = this.savedOffset;
                this.screen.ClipRegion = this.savedClipRegion;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}