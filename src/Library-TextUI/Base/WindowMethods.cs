/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       WindowMethods.cs
 *  Created:    2011-03-24
 *  Modified:   2011-04-07
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace TextUI
{
    using TextUI.Drawing;

    /// <summary>
    /// WindowMethod.cs implements methods dealing with the position and size of 
    /// the window.
    /// </summary>
    /// 
    public partial class Window
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Cursor Position ]

        /// <summary>
        /// Sets the position of the cursor.
        /// </summary>
        /// 
        public virtual void SetCursorPosition( int left, int top )
        {
            CursorLeft = left;
            CursorTop  = top;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Window Position and Size ]

        /// <summary>
        /// Sets the height and width of the window to the specified values. 
        /// </summary>
        /// 
        public void SetSize( Size size )
        {
            this.ClientSize = size;
        }

        /// <summary>
        /// Sets the height and width of the window to the specified values. 
        /// </summary>
        /// 
        public void SetSize( int width, int height )
        {
            this.ClientSize = new Size( width, height );
        }

        /// <summary>
        /// Sets the left-top position of the window (i.e. window's client area).
        /// </summary>
        /// 
        public void Move( Point position )
        {
            this.position = position;
        }

        /// <summary>
        /// Sets the left-top position of the window (i.e. window's client area).
        /// </summary>
        /// 
        public void Move( int left, int top )
        {
            this.position = new Point( left, top );
        }

        /// <summary>
        /// Moves the left-top position of the window (relative to window's border).
        /// </summary>
        /// 
        public void MoveBorderPosition( int left, int top )
        {
            // Consider also border and caption width when moving
            //
            Left = left + ExtraLeft;
            Top  = top  + ExtraTop;
        }

        /// <summary>
        /// Centers window reltaive to some other window, or parent if other window
        /// is not specified.
        /// </summary>
        ///
        public void Center( Window window = null )
        {
            if ( window != null )
            {
                Left = ExtraLeft + ( window.Width  - TotalWidth  ) / 2;
                Top  = ExtraTop  + ( window.Height - TotalHeight ) / 2;
            }
            else if ( Parent != null )
            {
                Left = ExtraLeft + ( Parent.Width  - TotalWidth  ) / 2;
                Top  = ExtraTop  + ( Parent.Height - TotalHeight ) / 2;
            }
        }

        /// <summary>
        /// Maximizes window dimensions and position relative to parent.
        /// </summary>
        /// 
        public void Maximize ()
        {
            if ( Parent != null )
            {
                Left   = ExtraLeft;
                Top    = ExtraTop;
                Width  = Parent.Width  - ExtraLeft - ExtraRight;
                Height = Parent.Height - ExtraTop  - ExtraBottom;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}