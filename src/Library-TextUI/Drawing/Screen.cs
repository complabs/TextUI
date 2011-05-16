/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Drawing
 *  File:       Screen.cs
 *  Created:    2011-03-15
 *  Modified:   2011-04-02
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
    /// Represents a screen with off-line character buffer where it can be written to
    /// and which is synchronized with character buffer on some media (like .NET
    /// Console).
    /// </summary>
    /// 
    public class Screen
    {
        #region [ Properties ]

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the character and color buffer area of the screen. 
        /// </summary>
        /// 
        internal ScreenBuffer Buffer { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets current the screen updater used to synchronize screen to media.
        /// </summary>
        /// 
        public IScreenSynchronizer Updater { get; internal set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the root window associated with the screen. Root window is a window
        /// that is a top-level container for all windows in TextUI application.
        /// </summary>
        /// 
        public Window RootWindow    { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the height of the screen buffer area.
        /// </summary>
        /// 
        public int Height { get; private set; }

        /// <summary>
        /// Gets the width of the screen buffer area.
        /// </summary>
        /// 
        public int Width { get; private set; }

        /// <summary>
        /// Gets or sets the current foreground color of the screen.
        /// </summary>
        /// 
        public Color ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the current background color of the screen.
        /// </summary>
        /// 
        public Color BackColor { get; set; }

        /// <summary>
        /// Gets or sets the column (x) position of the cursor within the buffer area. 
        /// </summary>
        /// 
        public int CursorLeft { get; set; }

        /// <summary>
        /// Gets or sets the row (y) position of the cursor within the buffer area. 
        /// </summary>
        /// 
        public int CursorTop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cursor is visible. 
        /// </summary>
        /// 
        public bool CursorVisible { get; set; }

        /// <summary>
        /// Plays the sound of a beep.
        /// </summary>
        /// 
        public bool Beep { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the curernt position of the line 0 column 0.
        /// The Offset is by default added to all x/y-coordinate parameters to methods.
        /// </summary>
        /// 
        public Point Offset
        {   
            get          { return Buffer.Offset; }
            internal set { Buffer.Offset = value; }
        }

        /// <summary>
        /// Gets or sets the current clipping region. Writing to outside of this region
        /// does nothing.
        /// </summary>
        /// 
        public Rectangle ClipRegion
        {   
            get          { return Buffer.ClipRegion; }
            internal set { Buffer.ClipRegion = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the Screen class associated with an existing
        /// instance of the Window class that will be used as a <see cref="RootWindow"/>.
        /// </summary>
        ///
        public Screen( Window rootWindow )
        {
            Updater       = null;
            RootWindow    = rootWindow;

            Width         = Math.Max( 0, RootWindow.Width );
            Height        = Math.Max( 0, RootWindow.Height );
            BackColor     = RootWindow.BackColor;
            ForeColor     = RootWindow.ForeColor;
            CursorLeft    = RootWindow.CursorLeft;
            CursorTop     = RootWindow.CursorTop;
            CursorVisible = RootWindow.CursorVisible;
            Beep          = false;

            // Allocate screen buffer for this Screen
            //
            Buffer = new ScreenBuffer( this );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Resizes screen buffer area (and underlying root window).
        /// </summary>
        /// 
        public void Resize( int width, int height )
        {
            RootWindow.SetSize( width, height );

            Width  = RootWindow.Width;
            Height = RootWindow.Height;
            Buffer = new ScreenBuffer( this );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Synchronizes current screen buffer area contents with the media.
        /// </summary>
        /// 
        public void UpdateScreen ()
        {
            if ( RootWindow == null )
            {
                return;
            }

            if ( Updater != null )
            {
                Updater.BeginUpdate ();
            }

            // Repaint screen

            BackColor = RootWindow.BackColor;
            ForeColor = RootWindow.ForeColor;

            ResetClipRegion ();

            RootWindow.Repaint( this );

            if ( Updater != null )
            {
                Updater.Update ();
            }

            Buffer.ResetDirtyRows ();

            // Repaint cursor

            ResetClipRegion ();

            RootWindow.RepaintCursor( this );

            if ( Updater != null )
            {
                Updater.EndUpdate ();
            }
        }

        /// <summary>
        /// Erases the media and forces full synchronization of the current screen buffer
        /// with the media.
        /// </summary>
        /// 
        public void FullRepaint ()
        {
            if ( RootWindow == null )
            {
                return;
            }

            ResetClipRegion ();

            BackColor = RootWindow.BackColor;
            ForeColor = RootWindow.ForeColor;

            Buffer.Clear ();
            Buffer.ResetDirtyRows ();

            if ( Updater != null )
            {
                Updater.Clear ();
            }

            RootWindow.Invalidate ();
            RootWindow.Repaint( this );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets offset to (0,0) and clipping region to whole screen buffer area.
        /// </summary>
        /// 
        internal void ResetClipRegion ()
        {
            Buffer.ResetOffset ();
            Buffer.ResetClip ();
        }

        /// <summary>
        /// Adds coordinates to the <see cref="Offset"/>.
        /// </summary>
        /// 
        internal void AddOffset( int left, int top )
        {
            Buffer.AddOffset( left, top );
        }

        /// <summary>
        /// Adds clip region to the <see cref="ClipRegion"/> relative to 
        /// current <see cref="Offset"/>.
        /// </summary>
        /// 
        internal void AddClipRegion( int width, int height )
        {
            Buffer.AddClipRegion( 0, 0, width, height );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Copies the contents from the other screen buffer area
        /// to this screen buffer area.
        /// </summary>
        ///
        public void CopyFrom( Screen other )
        {
            Buffer.CopyFrom( other.Buffer );
        }

        /// <summary>
        /// Clears the sreen buffer. 
        /// </summary>
        /// 
        public void Clear ()
        {
            Buffer.Clear ();
        }

        /// <summary>
        /// Puts character using current colors at some position.
        /// </summary>
        /// 
        public void Put( int x, int y, char character )
        {
            Buffer.Put( x, y, character );
        }

        /// <summary>
        /// Gets a value indicating whether character at some position is visible
        /// (not outside the screen buffer area nor clipped).
        /// </summary>
        /// 
        public bool IsVisible( int x, int y )
        {
            return Buffer.IsVisible( x, y );
        }

        /// <summary>
        /// Fills rectangle with a character using the current screen colors.
        /// </summary>
        /// 
        public void FillRectangle( int x, int y, int width, int height, char character )
        {
            for ( int i = 0; i < width; ++i )
            {
                for ( int j = 0; j < height; ++j )
                {
                    Buffer.Put( x + i, y + j, character );
                }
            }
        }

        /// <summary>
        /// Sets color attributes for existing characters in screen buffer area.
        /// </summary>
        /// 
        public void SetColors( int x, int y, int width, int height, 
            Color backColor, Color foreColor )
        {
            for ( int i = 0; i < width; ++i )
            {
                for ( int j = 0; j < height; ++j )
                {
                    Buffer.SetColors( x + i, y + j, backColor, foreColor );
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets the position of the cursor.  
        /// </summary>
        /// 
        public void SetCursorPosition( int left, int top )
        {
            CursorLeft = left;
            CursorTop  = top;
        }

        /// <summary>
        /// Writes the specified string value to the screen buffer area at current
        /// position. 
        /// </summary>
        ///
        public void Write( string str )
        {
            if ( str == null )
            {
                return;
            }

            int startColumn = CursorLeft;

            for( int i = 0; i < str.Length; ++i )
            {
                if ( str[ i ] == '\b' )
                {
                    if ( --CursorLeft < startColumn )
                    {
                        CursorLeft = startColumn;
                    }
                }
                else if ( str[ i ] == '\r' )
                {
                    CursorLeft = startColumn;
                }
                else if ( str[ i ] == '\n' )
                {
                    CursorTop++;
                }
                else if ( ! Char.IsControl( str[ i ] ) )
                {
                    Buffer.Put( CursorLeft++, CursorTop, str[ i ] );
                }
            }
        }

        /// <summary>
        /// Writes the specified string value, followed by the line terminator,
        /// to the screen buffer area at current position. 
        /// </summary>
        ///
        public void WriteLine( string str = null )
        {
            int startColumn = CursorLeft;

            if ( str != null )
            {
                Write( str );
            }

            CursorLeft = startColumn;
            CursorTop++;
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects to the
        /// screen buffer area using the specified format information. 
        /// </summary>
        /// 
        public void Write( string format, params Object[] args )
        {
            if ( format != null )
            {
                Write( string.Format( format, args ) );
            }
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects,
        /// followed by the current line terminator, to the screen buffer area 
        /// using the specified format information. 
        /// </summary>
        /// 
        public void WriteLine( string format, params Object[] args )
        {
            if ( format == null )
            {
                WriteLine ();
            }
            else
            {
                WriteLine( string.Format( format, args ) );
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods used by DrawRectangle ]

        /// <summary>
        /// Draws a horizontal line. Private method.
        /// </summary>
        /// 
        private void DrawHorizontal( int left, int top, int width, 
            BoxLines connect = BoxLines.NotJoined )
        {
            for( int i = 0; i < width;  ++i )
            {
                char boxElement = i == 0         ? Box._sssR 
                                : i == width - 1 ? Box._ssLs 
                                                 : Box._ssLR;

                Buffer.PutBox( left + i, top, boxElement, connect );
            }
        }

        /// <summary>
        /// Draws a vertical line. Private method.
        /// </summary>
        /// 
        private void DrawVertical( int left, int top, int height, 
            BoxLines connect = BoxLines.NotJoined )
        {
            for( int i = 0; i < height;  ++i )
            {
                char boxElement = i == 0          ? Box._sDss 
                                : i == height - 1 ? Box._Usss 
                                                  : Box._UDss;

                Buffer.PutBox( left, top + i, boxElement, connect );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Draws a rectangle, which edges may be optionally joined (connected
        /// when crossing) with existing lines in the screen buffer.
        /// </summary>
        /// 
        public void DrawRectangle(
            int left, int top, int width, int height, 
            BoxLines connect = BoxLines.NotJoined
            )
        {
            if ( height == 1 && width == 1 )
            {
                Buffer.Put( left, top, Box.Rectangle );
            }
            else if ( height == 1 )
            {
                DrawHorizontal( left, top, width, connect );
            }
            else if ( width == 1 )
            {
                DrawVertical( left, top, height, connect );
            }
            else if ( connect == BoxLines.Joined )
            {
                // Draw top & bottom horizontal edge
                //
                DrawHorizontal( left, top,              width, connect );
                DrawHorizontal( left, top + height - 1, width, connect );

                // Draw left & right vertical edge
                //
                DrawVertical( left,             top, height, connect );
                DrawVertical( left + width - 1, top, height, connect );
            }
            else
            {
                // Draw top & left edges
                //
                Put( left,top,              Box._sDsR );
                Put( left,top + height - 1, Box._UssR );

                DrawHorizontal( left + 1, top, width - 2 );
                DrawVertical( left, top + 1, height - 2 );

                // Draw bottom & right edges
                //
                Put( left + width - 1, top + height - 1, Box._UsLs );
                Put( left + width - 1, top,              Box._sDLs );
                DrawHorizontal( left + 1, top + height - 1, width - 2 );
                DrawVertical( left + width - 1, top + 1, height - 2 );
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Draws a vertical scroll bar. Utility method.
        /// </summary>
        /// 
        public void DrawVerticalScrollBar( int xpos, int ypos, int height, 
            int firstItem, int lastItem, int itemCount )
        {
            if ( height < 2 )
            {
                return;
            }

            double count = itemCount == 0 ? 1 : itemCount;

            int yMax = height - 2;
            int yBeg = (int)( yMax * firstItem / count );
            int yEnd = yBeg + (int)( yMax * ( lastItem - firstItem ) / count );

            // Draw vertical line with highlighted area
            //
            for ( int y = 0; y < height - 2; ++y )
            {
                Put( xpos, ypos + y + 1, 
                     y >= yBeg && y <= yEnd ? Box.Rectangle : Box._UDss );
            }

            // Draw end-arrows
            //
            Put( xpos, ypos, Box.Up );
            Put( xpos, ypos + height - 1, Box.Down );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}