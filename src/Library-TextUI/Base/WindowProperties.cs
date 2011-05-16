/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       WindowProperties.cs
 *  Created:    2011-03-24
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Mbk.Commons;

namespace TextUI
{
    /// <remarks>
    /// WindowProperties.cs implements window position, size, colors,
    /// border and other visual styles properties.
    /// </remarks>
    /// 
    public partial class Window
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties not influencing Invalidated property ]

        /// <summary>
        /// Gets window unique identifier. Window ID is auto-incremented for every
        /// new window.
        /// </summary>
        /// 
        public int ID { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the name of the window.
        /// </summary>
        public virtual string Name { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the name of the window. 
        /// </summary>
        /// 
        public virtual object Tag { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets indicator if the window contents is invalidated and should be repainted.
        /// </summary>
        /// 
        public virtual bool Invalidated { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the window handles itself 
        /// erasing of its background.
        /// </summary>
        /// 
        public virtual bool OwnErase { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the window handles istelf 
        /// drawing of its border.
        /// </summary>
        /// 
        public virtual bool OwnDrawBorder { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to 
        /// this window using the TAB key. 
        /// </summary>
        /// 
        public virtual bool TabStop { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the tab order of the window within its parent tree. 
        /// </summary>
        /// 
        public virtual int TabIndex { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the access key (underlined letter) that allows user to quickly 
        /// navigate to the window.
        /// </summary>
        /// 
        public virtual AccessKey AccessKey { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Window Contents and Visibility Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether the window and all its child 
        /// windows are displayed. 
        /// </summary>
        /// 
        public virtual bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                InvalidateIf( value != this.visible );
                this.visible = value;
            }
        }

        private bool visible;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the text associated with this window.
        /// </summary>
        /// 
        public virtual string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                InvalidateIf( value != this.text );
                this.text = value;
            }
        }

        private string text;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the caption associated with this window. Captions are
        /// displayed as window titles in window border.
        /// </summary>
        /// 
        public virtual string Caption
        {
            get
            {
                return this.caption;
            }
            set
            {
                InvalidateIf( value != this.caption );
                this.caption = value;

                CaptionVisible = value != null;
            }
        }

        private string caption;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating if the caption is displayed.
        /// </summary>
        /// 
        public virtual bool CaptionVisible
        {
            get
            {
                return this.captionVisible;
            }
            set
            {
                InvalidateIf( value != this.captionVisible );
                this.captionVisible = value;
            }
        }

        private bool captionVisible;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the alignment (left, centered right) of the caption in
        /// window title.
        /// </summary>
        /// 
        public virtual TextAlign CaptionTextAlign
        {
            get
            {
                return this.captionTextAlign;
            }
            set
            {
                InvalidateIf( value != this.captionTextAlign );
                this.captionTextAlign = value;
            }
        }

        private TextAlign captionTextAlign;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets indentation of the caption in window title.
        /// </summary>
        /// 
        public virtual int CaptionIndent
        {
            get
            {
                return this.captionIndent;
            }
            set
            {
                InvalidateIf( value != this.captionIndent );
                this.captionIndent = value;
            }
        }

        private int captionIndent;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets tool tip text usually displayed at the bottom in the status
        /// bar when the window is in focus.
        /// </summary>
        /// 
        public virtual string ToolTipText
        {
            get
            {
                return this.toolTipText;
            }
            set
            {
                InvalidateIf( value != this.toolTipText );
                this.toolTipText = value;
            }
        }

        private string toolTipText;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Window Position and Size Properties ]

        /// <summary>
        /// Gets or sets the distance between the left edge of the window and the left 
        /// edge of its parent's client area. 
        /// </summary>
        /// 
        public virtual int Left
        {
            get
            {
                return this.position.X;
            }
            set
            {
                InvalidateParentIf( value != this.position.X );
                this.position.X = value;
            }
        }
        /// <summary>
        /// Gets or sets the distance between the top edge of the window and the top 
        /// edge of its parent's client area. 
        /// </summary>
        /// 

        public virtual int Top
        {
            get
            {
                return this.position.Y;
            }
            set
            {
                InvalidateParentIf( value != this.position.Y );
                this.position.Y = value;
            }
        }

        private Point position;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the height and width of the client area of the window.  
        /// </summary>
        /// 
        public virtual Size ClientSize
        {
            get
            {
                return this.size;
            }
            set
            {
                InvalidateIf( this.size.Height != value.Height );
                InvalidateIf( this.size.Width  != value.Width  );
                this.size = value;
                OnResize ();
            }
        }

        private Size size;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// 
        public virtual int Width
        {
            get
            {
                return this.ClientSize.Width;
            }
            set
            {
                ClientSize = new Size( value, this.size.Height );
            }
        }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// 
        public virtual int Height
        {
            get
            {
                return this.ClientSize.Height;
            }
            set
            {
                ClientSize = new Size( this.size.Width, value );
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the width of the client area of the window.
        /// </summary>
        /// 
        public virtual int ClientWidth
        {
            get { return VerticalScrollBar ? Width - 2 : Width; }
        }

        /// <summary>
        /// Gets the height of the client area of the window.
        /// </summary>
        /// 
        public virtual int ClientHeight
        {
            get { return Height; }
        }

        /// <summary>
        /// Gets height of the space above the client area consisting of height
        /// of the border and window title (caption).
        /// </summary>
        /// 
        public virtual int ExtraTop
        {
            get { return ! Border ? 0 : 1 + ( ! CaptionVisible ? 0 : 2 ); }
        }

        /// <summary>
        /// Gets height of the space bellow the client area consisting of height
        /// of the border.
        /// </summary>
        /// 
        public virtual int ExtraBottom
        {
            get { return ! Border ? 0 : 1; }
        }

        /// <summary>
        /// Gets width of the space left of the client area consisting of width
        /// of the border.
        /// </summary>
        /// 
        public virtual int ExtraLeft
        {
            get { return ! Border ? 0 : 1; }
        }

        /// <summary>
        /// Gets height of the space right of the client area consisting of width
        /// of the border and window's vertical scroll bar
        /// </summary>
        /// 
        public virtual int ExtraRight
        {
            get { return ! Border ? 0 : 1; }
        }

        /// <summary>
        /// Gets the width of the window including the border width and the width 
        /// of the vertical scroll bar.
        /// </summary>
        /// 
        public virtual int TotalWidth
        {
            get { return Width + ExtraLeft + ExtraRight; }
        }

        /// <summary>
        /// Gets the height of the window including the border height and the height
        /// of the caption (window title).
        /// </summary>
        /// 
        public virtual int TotalHeight
        {
            get { return Height + ExtraTop + ExtraBottom; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Cursor ]

        /// <summary>
        /// Gets or sets a value indicating whether the cursor is displayed.
        /// </summary>
        /// 
        public virtual bool CursorVisible
        {
            get
            {
                return this.cursorVisible;
            }
            set
            {
                InvalidateIf( value != this.cursorVisible );
                this.cursorVisible = value;
            }
        }

        private bool cursorVisible;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the distance between the left edge of the window and 
        /// the cursor.
        /// </summary>
        /// 
        public virtual int CursorLeft
        {
            get
            {
                return this.cursor.X;
            }
            set
            {
                InvalidateIf( value != this.cursor.X );
                this.cursor.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the distance between the top edge of the window and 
        /// the cursor.
        /// </summary>
        /// 
        public virtual int CursorTop
        {
            get
            {
                return this.cursor.Y;
            }
            set
            {
                InvalidateIf( value != this.cursor.Y );
                this.cursor.Y = value;
            }
        }

        private Point cursor;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Border ]

        /// <summary>
        /// Gets or sets a value indicating whether the border is displayed.
        /// </summary>
        /// 
        public virtual bool Border
        {
            get
            {
                return this.border;
            }
            set
            {
                InvalidateIf( value != this.border );
                this.border = value;
            }
        }

        private bool border;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Colors ]

        /// <summary>
        /// Gets or sets the background color of the window border, used when the window
        /// is in the focus.
        /// </summary>
        /// 
        public virtual Color BorderForeColor
        {
            get
            {
                return this.borderForeColor;
            }
            set
            {
                InvalidateIf( value != this.borderForeColor );
                this.borderForeColor = value;
            }
        }

        private Color borderForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the window border, used when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color BorderBackColorInact
        {
            get
            {
                return this.borderBackColorInact;
            }
            set
            {
                InvalidateIf( value != this.borderBackColorInact );
                this.borderBackColorInact = value;
            }
        }

        private Color borderBackColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the window border, used when the
        /// window is out of the focus.
        /// </summary>
        /// 
        public virtual Color BorderForeColorInact
        {
            get
            {
                return this.borderForeColorInact;
            }
            set
            {
                InvalidateIf( value != this.borderForeColorInact );
                this.borderForeColorInact = value;
            }
        }

        private Color borderForeColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the window border, used when the 
        /// window is in the focus.
        /// </summary>
        /// 
        public virtual Color BorderBackColor
        {
            get
            {
                return this.borderBackColor;
            }
            set
            {
                InvalidateIf( value != this.borderBackColor );
                this.borderBackColor = value;
            }
        }

        private Color borderBackColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the window's title.
        /// </summary>
        /// 
        public virtual Color CaptionForeColor
        {
            get
            {
                return this.captionForeColor;
            }
            set
            {
                InvalidateIf( value != this.captionForeColor );
                this.captionForeColor = value;
            }
        }

        private Color captionForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the window's title.
        /// </summary>
        /// 
        public virtual Color CaptionBackColor
        {
            get
            {
                return this.captionBackColor;
            }
            set
            {
                InvalidateIf( value != this.captionBackColor );
                this.captionBackColor = value;
            }
        }

        private Color captionBackColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the window's title, when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color CaptionForeColorInact
        {
            get
            {
                return this.captionForeColorInact;
            }
            set
            {
                InvalidateIf( value != this.captionForeColorInact );
                this.captionForeColorInact = value;
            }
        }

        private Color captionForeColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the window's title, when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color CaptionBackColorInact
        {
            get
            {
                return this.captionBackColorInact;
            }
            set
            {
                InvalidateIf( value != this.captionBackColorInact );
                this.captionBackColorInact = value;
            }
        }

        private Color captionBackColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the window, used when the window
        /// is in the focus.
        /// </summary>
        /// 
        public virtual Color ForeColor
        {
            get
            {
                return this.foreColor;
            }
            set
            {
                InvalidateIf( value != this.foreColor );
                this.foreColor = value;
            }
        }

        private Color foreColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the window, used when the window
        /// is in the focus.
        /// </summary>
        /// 
        public virtual Color BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                InvalidateIf( value != this.backColor );
                this.backColor = value;
            }
        }

        private Color backColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the window, used when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color ForeColorInact
        {
            get
            {
                return this.foreColorInact;
            }
            set
            {
                InvalidateIf( value != this.foreColorInact );
                this.foreColorInact = value;
            }
        }

        private Color foreColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the window, used when the window
        /// is in the focus.
        /// </summary>
        /// 
        public virtual Color BackColorInact
        {
            get
            {
                return this.backColorInact;
            }
            set
            {
                InvalidateIf( value != this.backColorInact );
                this.backColorInact = value;
            }
        }

        private Color backColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the access key.
        /// </summary>
        /// 
        public virtual Color AccessKeyForeColor
        {
            get
            {
                return this.accessKeyForeColor;
            }
            set
            {
                InvalidateIf( value != this.accessKeyForeColor );
                this.accessKeyForeColor = value;
            }
        }

        private Color accessKeyForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the scroll bar.
        /// </summary>
        /// 
        public virtual Color ScrollBarForeColor
        {
            get
            {
                return this.scrollBarForeColor;
            }
            set
            {
                InvalidateIf( value != this.scrollBarForeColor );
                this.scrollBarForeColor = value;
            }
        }

        private Color scrollBarForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the scroll bar, when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color ScrollBarForeColorInact
        {
            get
            {
                return this.scrollBarForeColorInact;
            }
            set
            {
                InvalidateIf( value != this.scrollBarForeColorInact );
                this.scrollBarForeColorInact = value;
            }
        }

        private Color scrollBarForeColorInact;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ ScrollBar ]

        /// <summary>
        /// Gets or sets a value indicating whether the window has a vertical scroll bar.
        /// </summary>
        /// 
        public virtual bool VerticalScrollBar
        {
            get
            {
                return this.verticalScrollBar; 
            }
            set
            {
                InvalidateIf ( value != this.verticalScrollBar );
                this.verticalScrollBar = value;
            }
        }

        private bool verticalScrollBar;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets lower bound of the vertical scroll bar values. Should be overriden.
        /// </summary>
        /// 
        public virtual int VerticalScrollBarFirstItem
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets upper bound of the vertical scroll bar values. Should be overriden.
        /// </summary>
        /// 
        public virtual int VerticalScrollBarLastItem
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets current value of the vertical scrollb ar position. Should be overriden.
        /// </summary>
        /// 
        public virtual int VerticalScrollBarItemCount
        {
            get { return 1; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}