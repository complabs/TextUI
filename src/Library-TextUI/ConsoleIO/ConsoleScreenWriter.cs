/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.ConsoleIO
 *  File:       ConsoleScreenWriter.cs
 *  Created:    2011-03-16
 *  Modified:   2011-04-26
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Text;

using Mbk.Commons;

namespace TextUI.ConsoleIO
{
    using TextUI.Drawing;

    /// <summary>
    /// Implements synchronization interface for the Screen class running on the
    /// .NET Console.
    /// </summary>
    /// <remarks>
    /// Syncrhonization is implemented using a double-buffering scheme, where
    /// all changes to screen are compared against the saved (old) buffer and 
    /// with only necessary changes written to the Console.
    /// </remarks>
    /// 
    public class ConsoleScreenWriter : IScreenSynchronizer
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Fields ]

        // Screen and its dimensions

        private Screen        Screen      { get; set; } // Screen being synchronized
        private int           Height      { get; set; } // Cached screen height
        private int           Width       { get; set; } // Cached screen width
        private int           LogHeight   { get; set; } // Bottom-screen log height

        // Synchronization buffers (used in double-buffering)
        // Note that Buffer points to Screen.Buffer, while OldBuffer is locally
        // created buffer.

        private ScreenBuffer  Buffer      { get; set; } // Current buffer
        private ScreenBuffer  OldBuffer   { get; set; } // Buffer with old contents

        /////////////////////////////////////////////////////////////////////////////////

        // Cached Console current position, cursor position and color.
        // Used temporarily during Update(). Initialized in BeginUpdate ()
 
        private int    X           { get; set; } // Current character x-position
        private int    Y           { get; set; } // Current character y-position
        private int    CursorLeft  { get; set; } // Current Console.CursorLeft
        private int    CursorTop   { get; set; } // Current Console.CursorTop
        private Color  BackColor   { get; set; } // Current Console.BackColor
        private Color  ForeColor   { get; set; } // Current Console.ForeColor

        /////////////////////////////////////////////////////////////////////////////////

        // Statistics 

        public uint UpdateCount      { get; private set; }
        public uint MultiWriteCount  { get; private set; }
        public uint CharWriteCount   { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Mappings between console Color enum and ConsoleColor enum ]

        private static readonly ConsoleColor[] colorMap =
        {
            ConsoleColor.Black,       // ----  The color black.
            ConsoleColor.DarkBlue,    // ---B  The color dark blue.
            ConsoleColor.DarkGreen,   // --G-  The color dark green.
            ConsoleColor.DarkCyan,    // --GB  The color dark cyan (dark blue-green).
            ConsoleColor.DarkRed,     // -R--  The color dark red.
            ConsoleColor.DarkMagenta, // -R-B  The color dark magenta (dark purplish-red)
            ConsoleColor.DarkYellow,  // -RG-  The color dark yellow (ochre).
            ConsoleColor.DarkGray,    // -RGB  The color dark gray.
            ConsoleColor.Gray,        // H---  The color gray.
            ConsoleColor.Blue,        // H--B  The color blue.
            ConsoleColor.Green,       // H-G-  The color green.
            ConsoleColor.Cyan,        // H-GB  The color cyan (blue-green).
            ConsoleColor.Red,         // HR--  The color red.
            ConsoleColor.Magenta,     // HR-B  The color magenta (purplish-red).
            ConsoleColor.Yellow,      // HRG-  The color yellow.
            ConsoleColor.White        // HRGB  The color white. 
        };

        private static readonly ConsoleColor[] inverseColorMap =
        {
            ConsoleColor.White,       // ----  The inverse of black.
            ConsoleColor.Yellow,      // ---B  The inverse of dark blue.
            ConsoleColor.Magenta,     // --G-  The inverse of dark green.
            ConsoleColor.Red,         // --GB  The inverse of dark cyan (dark blue-green).
            ConsoleColor.Cyan,        // -R--  The inverse of dark red.
            ConsoleColor.Green,       // -R-B  The inverse of dark magenta (dark purplish-red).
            ConsoleColor.Blue,        // -RG-  The inverse of dark yellow (ochre).
            ConsoleColor.Gray,        // -RGB  The inverse of dark gray.
            ConsoleColor.DarkGray,    // H---  The inverse of gray.
            ConsoleColor.DarkYellow,  // H--B  The inverse of blue.
            ConsoleColor.DarkMagenta, // H-G-  The inverse of green.
            ConsoleColor.DarkRed,     // H-GB  The inverse of cyan (blue-green).
            ConsoleColor.DarkCyan,    // HR--  The inverse of red.
            ConsoleColor.DarkGreen,   // HR-B  The inverse of magenta (purplish-red).
            ConsoleColor.DarkBlue,    // HRG-  The inverse of yellow.
            ConsoleColor.Black        // HRGB  The inverse of white. 
        };

        /// <summary>
        /// Maps Color enum to ConsoleColor
        /// </summary>
        /// 
        public static ConsoleColor Map( Color color )
        {
            return colorMap[ (int)color % colorMap.Length  ];
        }

        /// <summary>
        /// Maps Color enum to inverted ConsoleColor
        /// </summary>
        /// 
        public static ConsoleColor Inverse( Color color )
        {
            return inverseColorMap[ (int)color % inverseColorMap.Length  ];
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ConsoleScreenWriter class that will be
        /// used to syncrhonize a Screen object to Console.
        /// </summary>
        /// <remarks>
        /// Don't forget to call Screen.FullRepaint () to complete Console setup!
        /// </remarks>
        /// 
        public ConsoleScreenWriter( 
            Screen screen, int logAreaHeight,
            int windowWidth = 0, int windowHeight = 0
            )
        {
            screen.Updater  = this;

            Screen          = screen;
            Width           = Screen.Width;
            Height          = Screen.Height;
            Buffer          = Screen.Buffer;
            LogHeight       = logAreaHeight;
            OldBuffer       = new ScreenBuffer( Screen );
            UpdateCount     = 0;
            MultiWriteCount = 0;
            CharWriteCount  = 0;

            /////////////////////////////////////////////////////////////////////////////
            // Initalize Console
            //
            if ( windowWidth  == 0 ) windowWidth  = Width;
            if ( windowHeight == 0 ) windowHeight = Height;

            windowWidth  = Math.Min( windowWidth,  Console.LargestWindowWidth  );
            windowHeight = Math.Min( windowHeight, Console.LargestWindowHeight );

            Console.BackgroundColor = Map( Screen.BackColor );
            Console.ForegroundColor = Map( Screen.ForeColor );

            Console.Clear ();

            Console.SetWindowSize( windowWidth, windowHeight );

            if ( ! SetBufferSize( Width, Height ) )
            {
                // In case that we couldn't set buffer size (like on Mono)
                //
                Screen.Resize( Console.WindowWidth, Console.WindowHeight );

                Width     = Screen.Width;
                Height    = Screen.Height;
                Buffer    = Screen.Buffer;
                OldBuffer = new ScreenBuffer( Screen );
            }

            Console.TreatControlCAsInput = true;

            Console.OutputEncoding = Encoding.GetEncoding( 850 );
            Console.InputEncoding  = Encoding.GetEncoding( 850 );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ SetBufferSize() for Mono fixup ]

        /// <summary>
        /// Sets the height and width of the Console buffer area to the specified values.
        /// </summary>
        /// <remarks>
        /// One should use this method instead of Console.SetBufferSize, the later
        /// does not need to be implemented (like in Mono .NET implementation).
        /// </remarks>
        /// 
        private bool SetBufferSize( int width, int height )
        {
            try 
            { 
                Console.SetBufferSize( width, height );
            }
            catch // E.g. Mono doesn't support SetBufferSize
            {
                return false;
            }

            return true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ IScreenSynchronizer Interface implementation ]

        /// <summary>
        /// Clears screen and displays cursor.
        /// </summary>
        /// 
        public void EnterFullscreen ()
        {
            Console.BackgroundColor = Map( Screen.RootWindow.BackColor );
            Console.ForegroundColor = Map( Screen.RootWindow.ForeColor );

            SetBufferSize( Width, Height + LogHeight );

            Console.Clear ();

            Console.CursorVisible = true;
        }

        /// <summary>
        /// Navigates to the bottom of the screen.
        /// </summary>
        /// 
        public void GotoLogArea ()
        {
            Console.BackgroundColor = Map( Screen.RootWindow.BackColor );
            Console.ForegroundColor = Map( Screen.RootWindow.ForeColor );

            SetBufferSize( Width, Height + LogHeight );

            Console.SetCursorPosition( Width - 1, Height - 1 );
            Console.WriteLine ();

            Console.CursorVisible = true;
        }

        /// <summary>
        /// Clears screen.
        /// </summary>
        /// 
        public void Clear ()
        {
            OldBuffer.Clear ();

            Console.BackgroundColor = Map( Screen.BackColor );
            Console.ForegroundColor = Map( Screen.ForeColor );
            Console.Clear ();

            if ( Console.BufferHeight != Height || Console.BufferWidth != Width )
            {
                SetBufferSize( Width, Height );
            }
        }

        /// <summary>
        /// Begins update. Sets initial cursor position and initializes 
        /// (caches from Console) temporary fields used by Update().
        /// </summary>
        /// 
        public void BeginUpdate ()
        {
            Console.CursorVisible = false;

            int x = Screen.CursorLeft;
            int y = Screen.CursorTop;

            if ( x < 0 || x >= Buffer.Width || y < 0 || y >= Buffer.Height )
            {
                return;
            }

            Console.SetCursorPosition( x, y );

            Console.BackgroundColor = Map( Buffer.Area[ y, x ].BackColor );
            Console.ForegroundColor = Map( Buffer.Area[ y, x ].ForeColor );

            Console.Write( Box.Map( Buffer.Area[ y, x ].Character ) );
        }

        /// <summary>
        /// Ends update by restoreing Console cursor position and screen attributes.
        /// </summary>
        /// 
        public void EndUpdate ()
        {
            int x = Screen.CursorLeft;
            int y = Screen.CursorTop;

            if ( x < 0 || x >= Buffer.Width || y < 0 || y >= Buffer.Height )
            {
                return;
            }

            if ( x == Width - 1 && y == Height - 1 )
            {
                // Suppress writing at bottom-right position on Console by making this
                // element always up-to date. If we write at bottom-right position, 
                // the Console buffer will scroll-up!
                //
                return;
            }

            Console.SetCursorPosition( x, y );

            if ( ! Application.Overwrite )
            {
                Console.CursorVisible   = Screen.CursorVisible;
                Console.BackgroundColor = Map( Buffer.Area[ y, x ].BackColor );
                Console.ForegroundColor = Map( Buffer.Area[ y, x ].ForeColor );
            }
            else if ( Screen.CursorVisible )
            {
                Console.BackgroundColor = Inverse( Buffer.Area[ y, x ].BackColor );
                Console.ForegroundColor = Inverse( Buffer.Area[ y, x ].ForeColor );
            }

            Console.Write( Box.Map( Buffer.Area[ y, x ].Character ) );

            Console.SetCursorPosition( x, y );
        }

        /// <summary>
        /// Synchronizes Screen buffer with the Console (i.e. our OldBuffer).
        /// </summary>
        /// 
        public void Update ()
        {
            #region [ Trace ]
            Debug.IfTracing( TraceFlag.Updates | TraceFlag.UpdatesPlus, delegate
            {
                Debug.TraceLine ();
                Debug.TraceLine( "Update started..." );
            } );
            Debug.IfTracing( TraceFlag.UpdatesPlus, delegate
            {
                TraceDumpBufferContents ();
            } );
            Debug.IfTracing( TraceFlag.Updates, delegate
            {
                Debug.TraceLine ();
                Debug.TraceLine( "{0,4}{1,5}{2,7}{3,12}{4,12}", 
                    "Row", "Col", "Index", "BackColor", "ForeColor" );
            } );
            #endregion

            // Console.CursorVisible = false;

            Console.CursorLeft = CursorLeft = X = 0;
            Console.CursorTop  = CursorTop  = Y = 0;

            Console.BackgroundColor = Map( BackColor  = Color.DarkBlue );
            Console.ForegroundColor = Map( ForeColor  = Color.Gray );

            // Suppress writing at bottom-right position on Console by making this
            // element always up-to date. If we write at bottom-right position, 
            // the Console buffer will scroll-up!
            //
            OldBuffer.Area[ Height-1, Width-1 ] = Buffer.Area[ Height-1, Width-1 ];

            int charCount = 0;
            int loopCount = 0;

            while ( FindNextDifferentElement () )
            {
                SyncConsolePositionAndAttributes ();

                string difference = CollectDifferentCharactersWithSameAttributes ();

                #region [ Trace ]
                Debug.IfTracing( TraceFlag.Updates, delegate
                {
                    Debug.Trace( "{0,6} : {1}{2}", 
                        difference.Length, difference, 
                        difference.EndsWith( " " ) ? "<EOT>" : string.Empty );
                    Debug.TraceLine ();
                } );
                #endregion

                Console.Write( difference );

                charCount += difference.Length;
                ++loopCount;
            }

            ++UpdateCount;
            CharWriteCount  += (uint)charCount;
            MultiWriteCount += (uint)loopCount;

            #region [ Trace ]
            Debug.IfTracing( TraceFlag.Updates, delegate
            {
                Debug.TraceLine( 
                    "**** Calls= {0}, Writes= {1} (+{2}), Chars= {3} (+{4}) ", 
                    UpdateCount, MultiWriteCount, loopCount, CharWriteCount, charCount 
                    );
                Debug.TraceLine( "Update completed." );
                Debug.TraceLine ();
            } );
            #endregion

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition( 0, 0 );

            // Synchronize console title

            if ( Screen.RootWindow != null && Screen.RootWindow.Text != null 
                 && Screen.RootWindow.Text != Console.Title )
            {
                Console.Title = Screen.RootWindow.Text;
            }

            // Beep, if requested.

            if ( Screen.Beep )
            {
                Screen.Beep = false;
                System.Media.SystemSounds.Beep.Play ();
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Syncrhonization Private Methods ]

        #region [ Trace ]

        // Dump old contents and new contents line by line
        //
        private void TraceDumpBufferContents ()
        {
            Debug.TraceLine ();
            for ( int row = 0; row < Height; ++row )
            {
                Debug.Trace( "{0,4} {1,5}", row, 
                    Buffer.IsRowDirty[ row ] ? "Dirty" : string.Empty );

                for ( int col = 0; col < Width; ++col )
                {
                    Debug.Trace( " {0:X2}{1:X2} {2}",
                        (int)OldBuffer.Area[ row, col ].BackColor, 
                        (int)OldBuffer.Area[ row, col ].ForeColor,
                        Box.Map( OldBuffer.Area[ row, col ].Character )
                        );
                }

                Debug.TraceLine ();

                Debug.Trace( "{0,4} {1,5}", string.Empty, string.Empty );

                for ( int col = 0; col < Width; ++col )
                {
                    if ( Buffer.Area[ row, col ].ForeColor 
                            == OldBuffer.Area[ row, col ].ForeColor
                        && Buffer.Area[ row, col ].BackColor
                            == OldBuffer.Area[ row, col ].BackColor )
                    {
                        Debug.Trace( "     " );
                    }
                    else
                    {
                        Debug.Trace( " {0:X2}{1:X2}",
                            (int)Buffer.Area[ row, col ].BackColor, 
                            (int)Buffer.Area[ row, col ].ForeColor
                            );
                    }

                    if ( Buffer.Area[ row, col ].Character 
                            == OldBuffer.Area[ row, col ].Character )
                    {
                        Debug.Trace( "  " );
                    }
                    else
                    {
                        Debug.Trace( " {0}",
                            Buffer.Area[ row, col ].Character
                            );
                    }
                }
                Debug.TraceLine ();
            }
        }

        #endregion

        /// <summary>
        /// Synchronizes current Console position and color attributes.
        /// </summary>
        /// 
        private bool SyncConsolePositionAndAttributes ()
        {
            string c1 = "-", c2 = "-", c3 = "-";

            // Update Console cursor position, if different
            //
            if ( CursorTop != Y || CursorLeft != X )
            {
                Console.SetCursorPosition( CursorLeft = X, CursorTop = Y );

                #region [ Trace ]
                Debug.IfTracing( TraceFlag.Updates, delegate
                {
                    c1 = string.Format( "{0,4} {1,4} {2,6}", Y, X, Y * Width + X );
                } );
                #endregion
            }

            // Update current Console background color, if different
            //
            if ( BackColor != Buffer.Area[ Y, X ].BackColor )
            {
                Console.BackgroundColor = Map( 
                    BackColor = Buffer.Area[ Y, X ].BackColor );

                #region [ Trace ]
                Debug.IfTracing( TraceFlag.Updates, delegate
                {
                    c2 = BackColor.ToString ();
                } );
                #endregion
            }

            // Update current Console foreground color, if different
            //
            if ( ForeColor != Buffer.Area[ Y, X ].ForeColor )
            {
                Console.ForegroundColor = Map( 
                    ForeColor = Buffer.Area[ Y, X ].ForeColor );

                #region [ Trace ]
                Debug.IfTracing( TraceFlag.Updates, delegate
                {
                    c3 = ForeColor.ToString ();
                } );
                #endregion
            }

            #region [ Trace ]
            Debug.IfTracing( TraceFlag.Updates, delegate
            {
                Debug.TraceTimeStamp ();
                Debug.Trace( "{0,16}{1,12}{2,12} ", c1, c2, c3 );
            } );
            #endregion

            return true;
        }

        /// <summary>
        /// Skips over rows that are not dirty.
        /// </summary>
        /// 
        private void SkipCleanRows ()
        {
            int startY = Y;

            while ( Y < Height && ! Buffer.IsRowDirty[ Y ] )
            {
                ++Y;
            }

            #region [ Trace ]
            Debug.IfTracing( TraceFlag.Updates, delegate
            {
                if ( Y != startY )
                {
                    Debug.TraceLine( "{0,4}  clean rows", Y - startY );
                }
            } );
            #endregion
        }

        /// <summary>
        /// Searches for the next different element (i.e. buffer element different
        /// in both character and colors).
        /// </summary>
        /// 
        private bool FindNextDifferentElement ()
        {
            SkipCleanRows ();

            if ( X >= Width || Y >= Height )
            {
                return false;
            }

            while( true )
            {
                ScreenBuffer.Element dst = Buffer.Area[ Y, X ];
                ScreenBuffer.Element src = OldBuffer.Area [ Y, X ];

                if (   src.Character != dst.Character
                    || src.ForeColor != dst.ForeColor
                    || src.BackColor != dst.BackColor )
                {
                    break;
                }

                if ( ++X >= Width )
                {
                    X = 0; ++Y;

                    SkipCleanRows ();

                    if ( X >= Width || Y >= Height )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Collects different characters in a row having same attributes (colors).
        /// </summary>
        /// 
        private string CollectDifferentCharactersWithSameAttributes ()
        {
            int maxCharacterCount = 95; // Width;

            StringBuilder sb = new StringBuilder ();

            bool sameAttributes = true;

            for ( int i = 0; i < maxCharacterCount && sameAttributes; ++i )
            {
                if ( X == Width - 1 && Y == Height - 1 )
                {
                    X = 0; Y = Height;
                    return sb.ToString ();
                }

                // Private unicode characters are used as boxed elements and
                // shuld be mapped to real boxed Console characters.
                //
                sb.Append( Box.Map( Buffer.Area[ Y, X ].Character ) );

                OldBuffer.Area[ Y, X ] = Buffer.Area[ Y, X ];

                if ( ++X >= Width )
                {
                    X = 0; ++Y;
                    if ( Y >= Height || ! Buffer.IsRowDirty[ Y ] )
                    {
                        return sb.ToString ();
                    }
                }

                sameAttributes = Buffer.Area[ Y, X ].ForeColor == ForeColor
                              && Buffer.Area[ Y, X ].BackColor == BackColor;
            }

            return sb.ToString ();
        }

        #endregion
    }
}