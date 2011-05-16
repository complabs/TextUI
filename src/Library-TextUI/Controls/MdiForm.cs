/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       MdiForm.cs
 *  Created:    2011-03-26
 *  Modified:   2011-05-11
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace TextUI.Controls
{
    using TextUI.Drawing;

    /// <summary>
    /// Represents a TextUI multiple-document interface (MDI) client control, 
    /// which holds cached window contents and is ment to be either as a 
    /// container holding a MDI form or a MDI form itself.
    /// </summary>
    /// 
    public class MdiForm : Control
    {
        #region [ Private Properties ]

        /// <summary>
        /// Gets or sets the screen buffer holding the contents of the window.
        /// </summary>
        /// 
        private Screen Buffer { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the current multiple-document interface (MDI) parent form 
        /// of the window.
        /// </summary>
        /// 
        public Form MdiParent
        {
            get 
            { 
                MdiForm mdiClient = this.Parent as MdiForm;
                return mdiClient != null ? mdiClient.Parent as Form : null;
            }
            set
            {
                Parent = value.MdiClient;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// 
        public override int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                SetSize( value, Height );
            }
        }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// 
        public override int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                SetSize( Width, value );
            }
        }

        /// <summary>
        /// Gets or sets the distance between the left edge of the window and 
        /// the cursor.
        /// </summary>
        /// 
        public override int CursorLeft
        {
            get
            {
                return base.CursorLeft;
            }
            set
            {
                base.CursorLeft = value;
                if ( Buffer != null ) 
                {
                    Buffer.CursorLeft = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the distance between the top edge of the window and 
        /// the cursor.
        /// </summary>
        /// 
        public override int CursorTop
        {
            get
            {
                return base.CursorTop;
            }
            set
            {
               base.CursorTop = value;
               if ( Buffer != null ) 
               {
                   Buffer.CursorTop = value;
               }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the window, used when the window
        /// is in the focus.
        /// </summary>
        /// 
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                if ( Buffer != null ) 
                {
                    Buffer.ForeColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the window, used when the window
        /// is in the focus.
        /// </summary>
        /// 
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                if ( Buffer != null )
                {
                    Buffer.BackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window handles itself 
        /// erasing of its background. Returns always true.
        /// </summary>
        /// 
        public sealed override bool OwnErase
        {
            get { return true; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the MdiForm class. 
        /// </summary>
        /// 
        public MdiForm ()
            : base ()
        {
            Buffer = null;
        }

        /// <summary>
        /// Initializes a new instance of the MdiForm class with given width and height. 
        /// </summary>
        /// 
        public MdiForm( int width, int height )
            : this ()
        {
            SetSize( width, height );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Sets the position of the cursor.
        /// </summary>
        /// 
        public override void SetCursorPosition( int left, int top )
        {
            CursorLeft = left;
            CursorTop  = top;

            if ( Buffer != null )
            {
                Buffer.CursorLeft = CursorLeft = left;
                Buffer.CursorTop  = CursorTop  = top;
            }
        }

        /// <summary>
        /// Raizes the Resize event.
        /// </summary>
        /// <remarks>
        /// Also adjusts internal buffer size.
        /// </remarks>
        /// 
        protected override void OnResize ()
        {
            if ( Width <= 0 || Height <= 0 )
            {
                Buffer = null;
            }
            else
            {
                // Create a buffer with new size with contents copied from
                // the old buffer

                Screen oldBuffer = Buffer;

                Buffer = new Screen( this );

                if ( oldBuffer != null )
                {
                    Buffer.CopyFrom( oldBuffer );
                }
            }

            base.OnResize ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Raises the EraseBackground event. Does nothing as
        /// there is no need to erase background because OnDrawContents() will fill 
        /// entire screen with Buffer contents.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnEraseBackground( Screen screen )
        {
        }

        /// <summary>
        /// Raises the DrawContents event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnDrawContents( Screen screen, bool hasFocus )
        {
            if ( screen == null || Buffer == null )
            {
                return;
            }

            screen.CopyFrom( Buffer );

            base.OnDrawContents( screen, hasFocus );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Moves cursor position in the buffer.
        /// </summary>
        /// 
        public MdiForm At( int x, int y )
        {
            if ( Buffer != null )
            {
                Buffer.CursorLeft = x;
                Buffer.CursorTop  = y;
            }

            return this;
        }

        /// <summary>
        /// Writes a string to the buffer at current cursor position.
        /// </summary>
        /// 
        public MdiForm Write( string str )
        {
            if ( Buffer != null )
            {
                Buffer.Write( str );
            }

            return this;
        }

        /// <summary>
        /// Writes a string to the buffer at current cursor position using specified 
        /// format.
        /// </summary>
        /// 
        public MdiForm Write( string format, params Object[] args )
        {
            if ( Buffer != null )
            {
                Buffer.Write( format, args );
            }

            return this;
        }

        /// <summary>
        /// Writes new line to the buffer at current cursor position.
        /// </summary>
        /// 
        public MdiForm WriteLine ()
        {
            if ( Buffer != null )
            {
                Buffer.WriteLine ();
                Invalidate ();
            }

            return this;
        }

        /// <summary>
        /// Writes a string terminated with new line to the buffer at current 
        /// cursor position using specified format.
        /// </summary>
        /// 
        public MdiForm WriteLine( string format, params Object[] args )
        {
            if ( Buffer != null )
            {
                Buffer.WriteLine( format, args );
                Invalidate ();
            }

            return this;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Draws frame (rectangle) in the buffer.
        /// </summary>
        /// 
        public void DrawFrame(
            int left, int top, int width, int height, 
            BoxLines connect = BoxLines.NotJoined
            )
        {
            if ( Buffer != null )
            {
                Buffer.DrawRectangle( left, top, width, height, connect );
                Invalidate ();
            }
        }

        /// <summary>
        /// Fills rectangle area in the buffer.
        /// </summary>
        /// 
        public void FillRectangle( int x, int y, int width, int height, char character )
        {
            if ( Buffer != null )
            {
                Buffer.FillRectangle( x, y, width, height, character );
                Invalidate ();
            }
        }

        /// <summary>
        /// Changes colors of the specified area in the buffer.
        /// </summary>
        /// 
        public void SetColors( int x, int y, int width, int height, 
            Color backColor, Color foreColor )
        {
            if ( Buffer != null )
            {
                Buffer.SetColors( x, y, width, height, backColor, foreColor );
                Invalidate ();
            }
        }

        #endregion
    }
}