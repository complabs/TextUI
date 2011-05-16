/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       DataTypes.cs
 *  Created:    2011-03-16
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Text;

using Mbk.Commons;

namespace TextUI
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Point Structure ]

    /// <summary>
    /// Represents an ordered pair of integer x- and y-coordinates that defines a point 
    /// in a two-dimensional plane.
    /// </summary>
    /// 
    public struct Point
    {
        /// <summary>
        /// Gets or sets the x-coordinate of this Point. 
        /// </summary>
        /// 
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of this Point. 
        /// </summary>
        /// 
        public int Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the Point class with the specified coordinates. 
        /// </summary>
        /// 
        public Point( int x, int y )
            : this ()
        {
            X = x; Y = y;
        }

        /// <summary>
        /// Translates a Point by a given size.
        /// </summary>
        /// 
        public Point Translate( int x, int y )
        {
            return new Point( X + x, Y + y );
        }

        /// <summary>
        /// Converts this Point to a human-readable string.
        /// </summary>
        /// 
        public override string ToString ()
        {
            return string.Format( "Point( X = {0}, Y = {1} )", X, Y );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Size Structure ]

    /// <summary>
    /// Stores an ordered pair of integers, which specify a Height and Width.
    /// </summary>
    /// 
    public struct Size
    {
        /// <summary>
        /// Gets or sets the horizontal component of this Size structure. 
        /// </summary>
        /// 
        public int Width  { get; set; }

        /// <summary>
        /// Gets or sets the vertical component of this Size structure.
        /// </summary>
        /// 
        public int Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the Size structure from the specified 
        /// dimensions. 
        /// </summary>
        /// 
        public Size( int width, int height )
            : this ()
        {
            Width = width; Height = height;
        }

        /// <summary>
        /// Converts this Point to a human-readable string.
        /// </summary>
        /// 
        public override string ToString ()
        {
            return string.Format( "Size( W = {0}, H = {1} )", Width, Height );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Rectangle Structure ]

    /// <summary>
    /// Stores a set of four integers that represent the location and size of a 
    /// rectangle.
    /// </summary>
    /// 
    public struct Rectangle
    {
        /// <summary>
        /// Gets the x-coordinate of the left edge of this Rectangle structure. 
        /// </summary>
        /// 
        public int Left   { get; set; }

        /// <summary>
        /// Gets the y-coordinate of the top edge of this Rectangle structure. 
        /// </summary>
        /// 
        public int Top    { get; set; }

        /// <summary>
        /// Gets or sets the width of this Rectangle structure. 
        /// </summary>
        /// 
        public int Width  { get; set; }

        /// <summary>
        /// Gets or sets the height of this Rectangle structure. 
        /// </summary>
        /// 
        public int Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the Rectangle class with the specified 
        /// location and size. 
        /// </summary>
        ///
        public Rectangle( int x, int y, int width, int height ) 
            : this ()
        { 
            Left = x; Top = y; Width = width; Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the Rectangle class with the specified 
        /// location and size.
        /// </summary>
        ///
        public Rectangle( Point p1, Point p2 )
            : this ()
        {
            Left   = p1.X; 
            Top    = p1.Y;
            Width  = p2.X - p1.X; 
            Height = p2.Y -p1.Y;
        }

        /// <summary>
        /// Converts the attributes of this Rectangle to a human-readable string.
        /// </summary>
        ///
        public override string ToString ()
        {
            return string.Format( "Rectangle( X = {0}, Y = {1}, W = {2}, H = {3} )", 
                Left, Top, Width, Height );
        }
    }


    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ AccessKey Structure ]

    /// <summary>
    /// Represents a mnemonic used as access key to speed up navigation to a control.
    /// </summary>
    /// 
    public struct AccessKey
    {
        #region [ Properties ]

        /// <summary>
        /// Provides read-only access to the mnemonic character.
        /// </summary>
        /// 
        public char Key { get; private set; }

        /// <summary>
        /// Position of the mnemonic in the text.
        /// </summary>
        /// 
        public int Position { get; private set; }

        /// <summary>
        /// Gets the text holding the mnemonic.
        /// </summary>
        /// 
        public string Text { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the AccessKey class by parsing mnemonic
        /// as a character immediatelly folloing amparsand ('&') character.
        /// </summary>
        ///
        public AccessKey( string text )
            : this ()
        {
            this.Key = '\0';
            this.Position = -1;

            StringBuilder sb = new StringBuilder ();

            for ( int i = 0; text != null && i < text.Length; ++i )
            {
                if ( this.Position < 0 && text[ i ] == '&' && i + 1 < text.Length )
                {
                    // If the 'key' char is followed by normal char, take the 
                    // current position (which is current output length).
                    //
                    if ( text[ i + 1 ] != '&' )
                    {
                        this.Key = char.ToLowerInvariant( text[ i + 1 ] );
                        this.Position = sb.Length;
                    }

                    ++i; // eat 'key' character
                }

                sb.Append( text[ i ] );
            }

            this.Text = sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Gets a value indicating whether access key holds a mnemonic.
        /// </summary>
        /// 
        public static implicit operator bool( AccessKey accessKey )
        {
            return accessKey.Key != '\0';
        }

        /// <summary>
        /// Compares mnemonic of the access key with a character.
        /// </summary>
        /// 
        public static bool operator == ( AccessKey accessKey, char character )
        {
            return accessKey.Key == char.ToLowerInvariant( character );
        }

        /// <summary>
        /// Compares mnemonic of the access key with a character.
        /// </summary>
        /// 
        public static bool operator != ( AccessKey accessKey, char character )
        {
            return accessKey.Key != char.ToLowerInvariant( character );
        }

        /// <summary>
        /// Compares mnemonic of the access key with mnemonic of other access key
        /// or character.
        /// </summary>
        /// 
        public override bool Equals( object obj )
        {
            if ( obj != null )
            {
                if ( obj is char )
                {
                    return this.Key == char.ToLowerInvariant( (char) obj );
                }
                else if ( obj is AccessKey )
                {
                    return this.Key == ( ( AccessKey) obj ).Key;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        ///
        public override int GetHashCode ()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Box Static Class ]

    /// <summary>
    /// Represents bitmapped box elements placed in private unicode area that may be
    /// logically OR-ed and thus intersected.
    /// </summary>
    /// <remarks>
    /// This box elements are kept in cur.Area[] and later mapped by Sinc(x,y) 
    /// into real-world box characters. Why do we keep this fake box elements in 
    /// cur.Area? Because we can OR them when drawing lines i.e. we can easily join
    /// lines that cross each other.
    /// <seealso cref="http://en.wikipedia.org/wiki/Code_page_850"/>
    /// <pre>
    /// 
    ///      (U)       e.g. (U)       equals Box._UssR == '\uE009'
    ///       |              │        which maps to unicode boxElements[9] == '\u2514'
    ///  (L)--+--(R)         └──(R)   i.e. character '└' in codepage 850.
    ///       |                       
    ///      (D)                      Note that: Box._UssR == Box._Usss | Box._sssR
    /// 
    /// </pre>
    /// </remarks>
    /// <example>
    ///                                           ┌─┬┐╔═╦╗
    ///        Box._UDLR      Box._UssR           │ ││║ ║║       ┌────│────┬────┐
    ///                                           ├─┼┤╠═╬╣       │┌┐  │    │    │
    ///            U              U               └─┴┘╚═╩╝       │└█  │    │    │
    ///                                           ┌─────────┐    ─────│────┼─────
    ///            │              │               │  ╔═══╗  │    │    │    │    │
    ///       L  ──┼──  R         └──  R          │  ╚═╦═╝  │    ├────│────┼────┤
    ///            │                              ╞═╤══╩══╤═╡    │    │    │    │
    ///                                           │ ├──┬──┤ │    └────│────┼────┘
    ///            D                              │ └──┴──┘ │
    ///                                           └─────────┘
    /// </example>
    /// 
    public static class Box
    {
        public const char _ssss = '\uE000'; // no wings of the cross i.e. empty space
        public const char _sssR = '\uE001'; // right wing of the cross
        public const char _ssLs = '\uE002'; // left wing of the cross
        public const char _ssLR = '\uE003'; // right & left wings of the cross
        public const char _sDss = '\uE004'; // upper wing of the cross
        public const char _sDsR = '\uE005'; // etc.
        public const char _sDLs = '\uE006'; // ...
        public const char _sDLR = '\uE007';
        public const char _Usss = '\uE008';
        public const char _UssR = '\uE009';
        public const char _UsLs = '\uE00A';
        public const char _UsLR = '\uE00B';
        public const char _UDss = '\uE00C';
        public const char _UDsR = '\uE00D';
        public const char _UDLs = '\uE00E'; // ...
        public const char _UDLR = '\uE00F'; // all wings of the cross

        // Miscellaneous shading and drawing characters
        //
        public const char Rectangle  = '\u2588'; // █
        public const char Square     = '\u25A0'; // ■
        public const char Shade      = '\u2592'; // ▒
        public const char ShadeAlt1  = '\u2591'; // ░
        public const char ShadeAlt2  = '\u2593'; // ▓
        public const char ArrowUp    = '\u2191'; // ↑
        public const char ArrowDown  = '\u2193'; // ↓
        public const char ArrowLeft  = '\u2190'; // ←
        public const char ArrowRight = '\u2192'; // →
        public const char Up         = '\u25B2'; // ▲
        public const char Down       = '\u25BC'; // ▼
        public const char Left       = '\u25C4'; // ◄
        public const char Right      = '\u25BA'; // ►

        // Visible box elements mapped from boxed elements in private unicode area
        // used by Sync(x,y), which maps characters from cur.Area[] into Console 
        // (visible in real-world) characters.
        //
        public static char[] Elements =
        {
            '\u0020',  //   - - - -   '\uE000'  no wings of the cross i.e. empty space
            '\u2500',  //   - - - R   '\uE001'  right wing of the cross
            '\u2500',  //   - - L -   '\uE002'  left wing of the cross
            '\u2500',  //   - - L R   '\uE003'  right & left wings of the cross
            '\u2502',  //   - D - -   '\uE004'  upper wing of the cross
            '\u250C',  //   - D - R   '\uE005'  etc.
            '\u2510',  //   - D L -   '\uE006' 
            '\u252C',  //   - D L R   '\uE007' 
            '\u2502',  //   U - - -   '\uE008' 
            '\u2514',  //   U - - R   '\uE009' 
            '\u2518',  //   U - L -   '\uE00A' 
            '\u2534',  //   U - L R   '\uE00B' 
            '\u2502',  //   U D - -   '\uE00C' 
            '\u251C',  //   U D - R   '\uE00D' 
            '\u2524',  //   U D L -   '\uE00E'  
            '\u253C'   //   U D L R   '\uE00F'  all wings of the cross are present
        };

        /// <summary>
        /// Returns true if character is a private unicode.
        /// </summary>
        /// 
        public static bool IsPrivateUnicode( char character )
        {
            return ( ( (int)character ) & 0xE000 ) == 0xE000;
        }

        /// <summary>
        /// Gets the four lowest bits of the character ('box'-bits).
        /// </summary>
        /// 
        public static int GetBoxBits( char character )
        {
            return ( (int)character ) & 0x000F;
        }

        /// <summary>
        /// Converts box bits to private unicode character.
        /// </summary>
        /// 
        public static char GetAsPrivateUnicode( int boxBits )
        {
            return (char)( 0xE000 | boxBits );
        }

        /// <summary>
        /// Converts a private unicode character to real-word unicode character
        /// with a box-drawing element.
        /// </summary>
        /// 
        public static char Map( char character )
        {
            return ( (int)character & 0xE000 ) != 0xE000 ? character
                : Box.Elements[ ( (int)character ) & 0x000F ];
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Enumerations ]

    /// <summary>
    /// Specifies constants that define foreground and background colors for the screen. 
    /// </summary>
    /// 
    [Flags]
    public enum Color : int
    {
        Black        = 0x0000,  //  ----  The color black.
        DarkBlue     = 0x0001,  //  ---B  The color dark blue.
        DarkGreen    = 0x0002,  //  --G-  The color dark green.
        DarkCyan     = 0x0003,  //  --GB  The color dark cyan (dark blue-green).
        DarkRed      = 0x0004,  //  -R--  The color dark red.
        DarkMagenta  = 0x0005,  //  -R-B  The color dark magenta (dark purplish-red).
        DarkYellow   = 0x0006,  //  -RG-  The color dark yellow (ochre).
        DarkGray     = 0x0007,  //  -RGB  The color dark gray.
        Gray         = 0x0008,  //  H---  The color gray.
        Blue         = 0x0009,  //  H--B  The color blue.
        Green        = 0x000A,  //  H-G-  The color green.
        Cyan         = 0x000B,  //  H-GB  The color cyan (blue-green).
        Red          = 0x000C,  //  HR--  The color red.
        Magenta      = 0x000D,  //  HR-B  The color magenta (purplish-red).
        Yellow       = 0x000E,  //  HRG-  The color yellow.
        White        = 0x000F   //  HRGB  The color white.
    }

    /// <summary>
    /// Specifies whether box drawing lines are joined (when crossed) or not.
    /// </summary>
    /// 
    public enum BoxLines : int
    {
        NotJoined = 0,
        Joined
    }

    /// <summary>
    /// Specifies constants defining which buttons to display on a MessageBox.
    /// </summary>
    /// 
    public enum MessageBoxButtons : int
    {
        OK = 0,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel
    }

    /// <summary>
    /// Specifies identifiers to indicate the return value of a dialog box.
    /// </summary>
    /// 
    public enum DialogResult : int
    {
        None = 0,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    /// <summary>
    /// Specifies constants defining which information to display.
    /// </summary>
    /// 
    public enum MessageBoxIcon : int
    {
        None = 0,
        Information, Asterisk,      // Informational level
        Exclamation, Warning,       // Warning level
        Hand, Stop, Error, Severe   // Error level
    }

    /// <summary>
    /// Specifies constants defining the default button on a <see cref="MessageBox"/>.
    /// </summary>
    /// 
    public enum MessageBoxDefaultButton : int
    {
        Button1 = 0, // The first button on the message box is the default button. 
        Button2 = 1, // The second button on the message box is the default button. 
        Button3 = 2  // The third button on the message box is the default button. 
    }

    /// <summary>
    /// Specifies the layout of multiple document interface (MDI) child windows in 
    /// an MDI parent window.
    /// </summary>
    /// 
    public enum MdiLayout : int
    {
        Cascade        = 0,
        TileHorizontal = 1,
        TileVertical   = 2,
    }

    /// <summary>
    /// Specifies shortcut keys that can be used by menu items.
    /// (Compatible with System.Windows.Forms.Shortcut enumeration.)
    /// </summary>
    /// 
    public enum Shortcut : int
    {
        [Verbose( ""             )]  None         = 0,
        [Verbose( "Alt"          )]  Alt          = ConsoleModifiers.Alt     << 16,
        [Verbose( "Ctrl"         )]  Ctrl         = ConsoleModifiers.Control << 16,
        [Verbose( "Shift"        )]  Shift        = ConsoleModifiers.Shift   << 16,
        [Verbose( "F1"           )]  F1           = Keys.F1,
        [Verbose( "F2"           )]  F2           = Keys.F2,
        [Verbose( "F3"           )]  F3           = Keys.F3,
        [Verbose( "F4"           )]  F4           = Keys.F4,
        [Verbose( "F5"           )]  F5           = Keys.F5,
        [Verbose( "F6"           )]  F6           = Keys.F6,
        [Verbose( "F7"           )]  F7           = Keys.F7,
        [Verbose( "F8"           )]  F8           = Keys.F8,
        [Verbose( "F9"           )]  F9           = Keys.F9,
        [Verbose( "F10"          )]  F10          = Keys.F10,
        [Verbose( "F11"          )]  F11          = Keys.F11,
        [Verbose( "F12"          )]  F12          = Keys.F12,
        [Verbose( "Ctrl+F1"      )]  CtrlF1       = Keys.F1  | Ctrl,
        [Verbose( "Ctrl+F2"      )]  CtrlF2       = Keys.F2  | Ctrl,
        [Verbose( "Ctrl+F3"      )]  CtrlF3       = Keys.F3  | Ctrl,
        [Verbose( "Ctrl+F4"      )]  CtrlF4       = Keys.F4  | Ctrl,
        [Verbose( "Ctrl+F5"      )]  CtrlF5       = Keys.F5  | Ctrl,
        [Verbose( "Ctrl+F6"      )]  CtrlF6       = Keys.F6  | Ctrl,
        [Verbose( "Ctrl+F7"      )]  CtrlF7       = Keys.F7  | Ctrl,
        [Verbose( "Ctrl+F8"      )]  CtrlF8       = Keys.F8  | Ctrl,
        [Verbose( "Ctrl+F9"      )]  CtrlF9       = Keys.F9  | Ctrl,
        [Verbose( "Ctrl+F10"     )]  CtrlF10      = Keys.F10 | Ctrl,
        [Verbose( "Ctrl+F11"     )]  CtrlF11      = Keys.F11 | Ctrl,
        [Verbose( "Ctrl+F12"     )]  CtrlF12      = Keys.F12 | Shift,
        [Verbose( "Shift+F1"     )]  ShiftF1      = Keys.F1  | Shift,
        [Verbose( "Shift+F2"     )]  ShiftF2      = Keys.F2  | Shift,
        [Verbose( "Shift+F3"     )]  ShiftF3      = Keys.F3  | Shift,
        [Verbose( "Shift+F4"     )]  ShiftF4      = Keys.F4  | Shift,
        [Verbose( "Shift+F5"     )]  ShiftF5      = Keys.F5  | Shift,
        [Verbose( "Shift+F6"     )]  ShiftF6      = Keys.F6  | Shift,
        [Verbose( "Shift+F7"     )]  ShiftF7      = Keys.F7  | Shift,
        [Verbose( "Shift+F8"     )]  ShiftF8      = Keys.F8  | Shift,
        [Verbose( "Shift+F9"     )]  ShiftF9      = Keys.F9  | Shift,
        [Verbose( "Shift+F10"    )]  ShiftF10     = Keys.F10 | Shift,
        [Verbose( "Shift+F11"    )]  ShiftF11     = Keys.F11 | Shift,
        [Verbose( "Shift+F12"    )]  ShiftF12     = Keys.F12 | Shift,
        [Verbose( "Alt+F1"       )]  AltF1        = Keys.F1  | Alt,
        [Verbose( "Alt+F2"       )]  AltF2        = Keys.F2  | Alt,
        [Verbose( "Alt+F3"       )]  AltF3        = Keys.F3  | Alt,
        [Verbose( "Alt+F4"       )]  AltF4        = Keys.F4  | Alt,
        [Verbose( "Alt+F5"       )]  AltF5        = Keys.F5  | Alt,
        [Verbose( "Alt+F6"       )]  AltF6        = Keys.F6  | Alt,
        [Verbose( "Alt+F7"       )]  AltF7        = Keys.F7  | Alt,
        [Verbose( "Alt+F8"       )]  AltF8        = Keys.F8  | Alt,
        [Verbose( "Alt+F9"       )]  AltF9        = Keys.F9  | Alt,
        [Verbose( "Alt+F10"      )]  AltF10       = Keys.F10 | Alt,
        [Verbose( "Alt+F11"      )]  AltF11       = Keys.F11 | Alt,
        [Verbose( "Alt+F12"      )]  AltF12       = Keys.F12 | Alt,
        [Verbose( "Ctrl+L"       )]  CtrlL        = Keys.L   | Ctrl,
        [Verbose( "Ctrl+P"       )]  CtrlP        = Keys.P   | Ctrl,
        [Verbose( "Ctrl+S"       )]  CtrlS        = Keys.S   | Ctrl,
        [Verbose( "Ctrl+Shift+0" )]  CtrlShift0   = Keys.D0  | Ctrl | Shift,
        [Verbose( "Ctrl+Shift+1" )]  CtrlShift1   = Keys.D1  | Ctrl | Shift,
        [Verbose( "Ctrl+Shift+2" )]  CtrlShift2   = Keys.D2  | Ctrl | Shift,
    }

    /// <summary>
    /// Key codes enumeration compatible with System.Windows.Forms.Keys' names
    /// (but having System.ConsoleKey values).
    /// </summary>
    /// 
    public enum Keys : int
    {
        Back               = ConsoleKey.Backspace,
        Tab                = ConsoleKey.Tab,
        Clear              = ConsoleKey.Clear,
        Enter              = ConsoleKey.Enter,
        Pause              = ConsoleKey.Pause,
        Escape             = ConsoleKey.Escape,
        Spacebar           = ConsoleKey.Spacebar,
        PageUp             = ConsoleKey.PageUp,
        PageDown           = ConsoleKey.PageDown,
        End                = ConsoleKey.End,
        Home               = ConsoleKey.Home,
        Left               = ConsoleKey.LeftArrow,
        Up                 = ConsoleKey.UpArrow,
        Right              = ConsoleKey.RightArrow,
        Down               = ConsoleKey.DownArrow,
        Select             = ConsoleKey.Select,
        Print              = ConsoleKey.Print,
        Execute            = ConsoleKey.Execute,
        PrintScreen        = ConsoleKey.PrintScreen,
        Insert             = ConsoleKey.Insert,
        Delete             = ConsoleKey.Delete,
        Help               = ConsoleKey.Help,
        D0                 = ConsoleKey.D0,
        D1                 = ConsoleKey.D1,
        D2                 = ConsoleKey.D2,
        D3                 = ConsoleKey.D3,
        D4                 = ConsoleKey.D4,
        D5                 = ConsoleKey.D5,
        D6                 = ConsoleKey.D6,
        D7                 = ConsoleKey.D7,
        D8                 = ConsoleKey.D8,
        D9                 = ConsoleKey.D9,
        A                  = ConsoleKey.A,
        B                  = ConsoleKey.B,
        C                  = ConsoleKey.C,
        D                  = ConsoleKey.D,
        E                  = ConsoleKey.E,
        F                  = ConsoleKey.F,
        G                  = ConsoleKey.G,
        H                  = ConsoleKey.H,
        I                  = ConsoleKey.I,
        J                  = ConsoleKey.J,
        K                  = ConsoleKey.K,
        L                  = ConsoleKey.L,
        M                  = ConsoleKey.M,
        N                  = ConsoleKey.N,
        O                  = ConsoleKey.O,
        P                  = ConsoleKey.P,
        Q                  = ConsoleKey.Q,
        R                  = ConsoleKey.R,
        S                  = ConsoleKey.S,
        T                  = ConsoleKey.T,
        U                  = ConsoleKey.U,
        V                  = ConsoleKey.V,
        W                  = ConsoleKey.W,
        X                  = ConsoleKey.X,
        Y                  = ConsoleKey.Y,
        Z                  = ConsoleKey.Z,
        LeftWindows        = ConsoleKey.LeftWindows,
        RightWindows       = ConsoleKey.RightWindows,
        Applications       = ConsoleKey.Applications,
        Sleep              = ConsoleKey.Sleep,
        NumPad0            = ConsoleKey.NumPad0,
        NumPad1            = ConsoleKey.NumPad1,
        NumPad2            = ConsoleKey.NumPad2,
        NumPad3            = ConsoleKey.NumPad3,
        NumPad4            = ConsoleKey.NumPad4,
        NumPad5            = ConsoleKey.NumPad5,
        NumPad6            = ConsoleKey.NumPad6,
        NumPad7            = ConsoleKey.NumPad7,
        NumPad8            = ConsoleKey.NumPad8,
        NumPad9            = ConsoleKey.NumPad9,
        Multiply           = ConsoleKey.Multiply,
        Add                = ConsoleKey.Add,
        Separator          = ConsoleKey.Separator,
        Subtract           = ConsoleKey.Subtract,
        Decimal            = ConsoleKey.Decimal,
        Divide             = ConsoleKey.Divide,
        F1                 = ConsoleKey.F1,
        F2                 = ConsoleKey.F2,
        F3                 = ConsoleKey.F3,
        F4                 = ConsoleKey.F4,
        F5                 = ConsoleKey.F5,
        F6                 = ConsoleKey.F6,
        F7                 = ConsoleKey.F7,
        F8                 = ConsoleKey.F8,
        F9                 = ConsoleKey.F9,
        F10                = ConsoleKey.F10,
        F11                = ConsoleKey.F11,
        F12                = ConsoleKey.F12,
        F13                = ConsoleKey.F13,
        F14                = ConsoleKey.F14,
        F15                = ConsoleKey.F15,
        F16                = ConsoleKey.F16,
        F17                = ConsoleKey.F17,
        F18                = ConsoleKey.F18,
        F19                = ConsoleKey.F19,
        F20                = ConsoleKey.F20,
        F21                = ConsoleKey.F21,
        F22                = ConsoleKey.F22,
        F23                = ConsoleKey.F23,
        F24                = ConsoleKey.F24,
        BrowserBack        = ConsoleKey.BrowserBack,
        BrowserForward     = ConsoleKey.BrowserForward,
        BrowserRefresh     = ConsoleKey.BrowserRefresh,
        BrowserStop        = ConsoleKey.BrowserStop,
        BrowserSearch      = ConsoleKey.BrowserSearch,
        BrowserFavorites   = ConsoleKey.BrowserFavorites,
        BrowserHome        = ConsoleKey.BrowserHome,
        VolumeMute         = ConsoleKey.VolumeMute,
        VolumeDown         = ConsoleKey.VolumeDown,
        VolumeUp           = ConsoleKey.VolumeUp,
        MediaNext          = ConsoleKey.MediaNext,
        MediaPrevious      = ConsoleKey.MediaPrevious,
        MediaStop          = ConsoleKey.MediaStop,
        MediaPlay          = ConsoleKey.MediaPlay,
        LaunchMail         = ConsoleKey.LaunchMail,
        LaunchMediaSelect  = ConsoleKey.LaunchMediaSelect,
        LaunchApp1         = ConsoleKey.LaunchApp1,
        LaunchApp2         = ConsoleKey.LaunchApp2,
        Oem1               = ConsoleKey.Oem1,
        OemPlus            = ConsoleKey.OemPlus,
        OemComma           = ConsoleKey.OemComma,
        OemMinus           = ConsoleKey.OemMinus,
        OemPeriod          = ConsoleKey.OemPeriod,
        Oem2               = ConsoleKey.Oem2,
        Oem3               = ConsoleKey.Oem3,
        Oem4               = ConsoleKey.Oem4,
        Oem5               = ConsoleKey.Oem5,
        Oem6               = ConsoleKey.Oem6,
        Oem7               = ConsoleKey.Oem7,
        Oem8               = ConsoleKey.Oem8,
        Oem102             = ConsoleKey.Oem102,
        Process            = ConsoleKey.Process,
        Packet             = ConsoleKey.Packet,
        Attention          = ConsoleKey.Attention,
        CrSel              = ConsoleKey.CrSel,
        ExSel              = ConsoleKey.ExSel,
        EraseEndOfFile     = ConsoleKey.EraseEndOfFile,
        Play               = ConsoleKey.Play,
        Zoom               = ConsoleKey.Zoom,
        NoName             = ConsoleKey.NoName,
        Pa1                = ConsoleKey.Pa1,
        OemClear           = ConsoleKey.OemClear,
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}
