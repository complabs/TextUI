/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Drawing
 *  File:       ScreenBuffer.cs
 *  Created:    2011-03-15
 *  Modified:   2011-04-25
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
    /// Represents a TextUI screen buffer area where it can be written to.
    /// Each element of the screen buffer holds a character with its attributes (
    /// background and foreground colors).
    /// </summary>
    /// 
    internal class ScreenBuffer
    {
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Represents character element of the ScreenBuffer with background and 
        /// foreground color attributes.
        /// </summary>
        /// <remarks>
        /// Note that Equals() method is not provided to compare two instances
        /// as, accordingto performance profiling, it is too slow (as well default
        /// System.Value.Equals()). So, throughout ScreenBuffer, all comparison
        /// between ScreenBuffer Elements is done manually between structure fields
        /// fields (Character field first, ForeColor next and BackColor last).
        /// </remarks>
        /// 
        public struct Element
        {
            public Color ForeColor;
            public Color BackColor;
            public char  Character;
        }

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Fields and Properties ]

        private Point offset;      // current x/y-offset added to all coordinates
        private Point clipStart;   // top-left point of the clipping region
        private Point clipEnd;     // bottom-right point of the clipping region

        private Screen Screen { get; set; } // parent screen owning this buffer

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets the height of the screen buffer area.
        /// </summary>
        /// 
        public int Height { get; private set; }

        /// <summary>
        /// Gets the width of the screen buffer area.
        /// </summary>
        /// 
        public int Width  { get; private set; }

        /// <summary>
        /// Gets the screen buffer area contents (a two dimensional array of elements).
        /// </summary>
        /// 
        public Element[,] Area { get; private set; }

        /// <summary>
        /// Gets array of values indicating whether buffer area rows are changed.
        /// </summary>
        /// 
        public bool[] IsRowDirty { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current clipping region. Writing to outside of this region
        /// does nothing.
        /// </summary>
        /// 
        public Rectangle ClipRegion 
        { 
            get
            { 
                return new Rectangle( clipStart, clipEnd );
            } 
            set
            {
                this.clipStart = new Point( value.Left, value.Top );
                this.clipEnd = this.clipStart.Translate( value.Width, value.Height );
            }
        }

        /// <summary>
        /// Gets or sets the curernt position of the line 0 column 0.
        /// The Offset is by default added to all x/y-coordinate parameters to methods.
        /// </summary>
        /// 
        public Point Offset
        { 
            get { return this.offset; } 
            set { this.offset = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ScreenBuffer class belonging 
        /// to some screen.
        /// </summary>
        /// 
        public ScreenBuffer( Screen parent )
        {
            Screen     = parent;
            Width      = Screen.Width;
            Height     = Screen.Height;

            Area       = new Element[ Height, Width ];
            IsRowDirty = new bool[ Height ];

            ResetClip ();
            Clear ();
            ResetDirtyRows ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Sets <see cref="Offset"/> to (0,0).
        /// </summary>
        /// 
        public void ResetOffset ()
        {
            this.offset.X = 0;
            this.offset.Y = 0;
        }

        /// <summary>
        /// Sets <see cref="ClipRegion"/> to whole screen buffer area.
        /// </summary>
        /// 
        public void ResetClip ()
        {
            this.clipStart.X = 0;
            this.clipStart.Y = 0;
            this.clipEnd.X = Width;
            this.clipEnd.Y = Height;
        }

        /// <summary>
        /// Adds coordinates to the <see cref="Offset"/>.
        /// </summary>
        /// 
        public void AddOffset( int left, int top )
        {
            this.offset.X += left;
            this.offset.Y += top;
        }

        /// <summary>
        /// Adds (intersects existing clipping region with) new clipping region.
        /// </summary>
        /// 
        public void AddClipRegion( int left, int top, int width, int height )
        {
            left += this.offset.X;
            top  += this.offset.Y;

            this.clipStart.X = Math.Max( this.clipStart.X, left         );
            this.clipStart.Y = Math.Max( this.clipStart.Y, top          );
            this.clipEnd.X   = Math.Min( this.clipEnd.X,   left + width );
            this.clipEnd.Y   = Math.Min( this.clipEnd.Y,   top + height );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a value indicating whether character at some position is visible
        /// (not outside the screen buffer area nor clipped).
        /// </summary>
        /// 
        public bool IsVisible( int x, int y )
        {
            x += this.offset.X;
            y += this.offset.Y;

            if ( x < this.clipStart.X || x >= this.clipEnd.X ) return false;
            if ( y < this.clipStart.Y || y >= this.clipEnd.Y ) return false;

            return true;
        }

        /// <summary>
        /// Puts character using current colors at some position.
        /// </summary>
        /// 
        public void Put( int x, int y, char character )
        {
            x += this.offset.X;
            y += this.offset.Y;

            if ( x < this.clipStart.X || x >= this.clipEnd.X ) return;
            if ( y < this.clipStart.Y || y >= this.clipEnd.Y ) return;

            if ( character == '\0' )
            {
                character = ' ';
            }

            IsRowDirty[ y ] = true;

            Area[ y, x ].ForeColor = Screen.ForeColor;
            Area[ y, x ].BackColor = Screen.BackColor;
            Area[ y, x ].Character = character;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Puts character at some position, optionally joining it with underlying
        /// box characters if it itself is a box character.
        /// </summary>
        /// 
        public void PutBox( int x, int y, char character, BoxLines join )
        {
            x += this.offset.X;
            y += this.offset.Y;

            if ( x < this.clipStart.X || x >= this.clipEnd.X ) return;
            if ( y < this.clipStart.Y || y >= this.clipEnd.Y ) return;

            IsRowDirty[ y ] = true;

            if ( join == BoxLines.Joined 
                && Box.IsPrivateUnicode( Area[ y, x ].Character ) )
            {
                int box1 = Box.GetBoxBits( Area[ y, x ].Character );
                int box2 = Box.GetBoxBits( character );

                Area[ y, x ].Character = Box.GetAsPrivateUnicode( box1 | box2 );
                Area[ y, x ].ForeColor = Screen.ForeColor;
                Area[ y, x ].BackColor = Screen.BackColor;
            }
            else
            {
                Area[ y, x ].Character = character;
                Area[ y, x ].ForeColor = Screen.ForeColor;
                Area[ y, x ].BackColor = Screen.BackColor;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets color attributes for existing characters in screen buffer area.
        /// </summary>
        /// 
        public void SetColors( int x, int y, Color backColor, Color foreColor )
        {
            x += this.offset.X;
            y += this.offset.Y;

            if ( x < this.clipStart.X || x >= this.clipEnd.X ) return;
            if ( y < this.clipStart.Y || y >= this.clipEnd.Y ) return;

            IsRowDirty[ y ] = true;

            Area[ y, x ].ForeColor = foreColor;
            Area[ y, x ].BackColor = backColor;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Copies the contents from other screen buffer area to this screen buffer area.
        /// </summary>
        ///
        public void CopyFrom( ScreenBuffer source )
        {
            int xStart = Math.Max( this.clipStart.X , this.offset.X                 );
            int yStart = Math.Max( this.clipStart.Y , this.offset.Y                 );
            int xEnd   = Math.Min( this.clipEnd.X   , this.offset.X + source.Width  );
            int yEnd   = Math.Min( this.clipEnd.Y   , this.offset.Y + source.Height );

            for( int y = yStart; y < yEnd; ++y )
            {
                bool updated = false;

                for ( int x = xStart; x < xEnd; ++x )
                {
                    Element src = source.Area[ y - this.Offset.Y, x - this.Offset.X  ];
                    Element dst = Area[ y, x ];

                    if (   src.Character != dst.Character 
                        || src.ForeColor != dst.ForeColor
                        || src.BackColor != dst.BackColor )
                    {
                        updated = true;
                        Area[ y, x ] = src;
                    }
                }

                IsRowDirty[ y ] |= updated;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Resets the whole <see cref="IsRowDirty"/> array to false.
        /// </summary>
        /// 
        public void ResetDirtyRows ()
        {
            for( int y = 0; y < Height; ++y )
            {
                IsRowDirty[ y ] = false;
            }
        }

        /// <summary>
        /// Clears the single row of the sreen buffer. 
        /// </summary>
        /// 
        public void ClearRow( int y, char character = ' ' )
        {
            if ( character == '\0' )
            {
                character = ' ';
            }

            bool updated = false;

            for ( int x = this.clipStart.X; x < this.clipEnd.X; ++x )
            {
                if (   Area[ y, x ].Character != character
                    || Area[ y, x ].ForeColor != Screen.ForeColor
                    || Area[ y, x ].BackColor != Screen.BackColor )
                {
                    updated = true;
                    Area[ y, x ].Character = character;
                    Area[ y, x ].ForeColor = Screen.ForeColor;
                    Area[ y, x ].BackColor = Screen.BackColor;
                }
            }

            IsRowDirty[ y ] |= updated;
        }

        /// <summary>
        /// Clears the sreen buffer. 
        /// </summary>
        /// 
        public void Clear( char character = ' ' )
        {
            for( int y = this.clipStart.Y; y < this.clipEnd.Y; ++y )
            {
                ClearRow( y, character );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}