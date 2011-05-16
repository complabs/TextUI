/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       Window.cs
 *  Created:    2011-03-24
 *  Modified:   2011-04-26
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Text;

using Mbk.Commons;

namespace TextUI
{
    using TextUI.Framework;

    /// <summary>
    /// Defines the base class for Text UI windows.
    /// </summary>
    /// 
    public partial class Window
    {
        #region [ Static Fields ]

        /// <summary>
        /// Gets last generated window ID. Auto-incremented for each new window.
        /// </summary>
        /// 
        private static int lastWindowID = 0;

        #endregion 

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Constructs an instance of Window class with default color theme. 
        /// </summary>
        /// 
        public Window ()
            : this( Application.Theme )
        {
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Constructs an instance of Window class using specific color theme.
        /// </summary>
        /// <remarks>
        /// If color theme is null, colors and visibility properties are not initialized.
        /// This is usefull for "fast"-creation of a window.
        /// </remarks>
        ///
        public Window( ColorTheme colorTheme )
        {
            ID = ++lastWindowID;
            Name = GetType().Name + "-" + ID.ToString( "0000" );

            Parent   = null;
            children = new List<Window> ();

            if ( colorTheme == null )
            {
                return; // Perform fast construction if no color theme is requested
            }

            Visible           = true;
            TabStop           = false;
            TabIndex          = -1;
            Invalidated       = false;

            Text              = null;
            ToolTipText       = null;
            Tag               = null;

            Caption           = null;
            CaptionVisible    = false;
            CaptionTextAlign  = TextAlign.Left;
            CaptionIndent     = 1;

            Top               = 0;
            Left              = 0;
            Width             = 0;
            Height            = 0;

            Border            = false;

            OwnErase          = false;
            OwnDrawBorder     = false;
            VerticalScrollBar = false;

            CursorVisible     = false;
            CursorLeft        = 0;
            CursorTop         = 0;

            // Setup default colors (using theme)

            BackColor               = colorTheme.BackColor;
            ForeColor               = colorTheme.ForeColor;
            BackColorInact          = colorTheme.BackColorInact;
            ForeColorInact          = colorTheme.ForeColorInact;

            BorderBackColor         = colorTheme.BorderBackColor;
            BorderForeColor         = colorTheme.BorderForeColor;
            BorderBackColorInact    = colorTheme.BorderBackColorInact;
            BorderForeColorInact    = colorTheme.BorderForeColorInact;

            CaptionForeColor        = colorTheme.CaptionForeColor;
            CaptionBackColor        = colorTheme.CaptionBackColor;
            CaptionForeColorInact   = colorTheme.CaptionForeColorInact;
            CaptionBackColorInact   = colorTheme.CaptionBackColorInact;

            AccessKeyForeColor      = colorTheme.MenuAccessKeyForeColor;

            ScrollBarForeColor      = colorTheme.ScrollBarForeColor;
            ScrollBarForeColorInact = colorTheme.ScrollBarForeColorInact;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ StructureToString() and overriden ToString()  ]

        /// <summary>
        /// Gets children tree.
        /// </summary>
        /// 
        public string StructureToString( int level = 0, StringBuilder sb = null )
        {
            if ( sb == null ) sb = new StringBuilder ();

            sb.Append( string.Empty.PadRight( level * 4 ) )
              .AppendLine( this.ToString () );

            foreach( Window subWindow in children )
            {
                subWindow.StructureToString( level + 1, sb );
            }

            return level == 0 ? sb.ToString () : null;
        }

        /// <summary>
        /// Converts the value of this instance to a String.
        /// </summary>
        /// <returns>name of the window, information about its parent,
        /// window's position and size, and finally window's Text (if not null)
        /// </returns>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();
            sb.Append( string.IsNullOrEmpty( Name ) ? base.ToString () : Name );
            sb.Append( ": " );

            sb.Append( "Parent = " );
            sb.Append( Parent == null ? "null" : Parent.Name );

            sb.Append( ", " );
            sb.Append( new Rectangle( Left, Top, Width, Height ).ToString () );

            if ( Text != null )
            {
                sb.Append( ", Text = " );
                sb.Append( Text.Length > 30 ? Text.Substring( 0, 30 ) : Text );
            }

            return sb.ToString ();
        }

        #endregion
    }
}