/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       ListBoxBase.cs
 *  Created:    2011-03-20
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;

using Mbk.Commons;

namespace TextUI.Controls
{
    using TextUI.Drawing;

    /// <summary>
    /// Represents a TextUI control to display a list of items.
    /// </summary>
    /// 
    public class ListBoxBase : Control
    {
        #region [ Public Constants ]

        /// <summary>
        /// Wide divider (part as a part of footer/header collection).
        /// Wid divider is connected to the window border.
        /// </summary>
        /// 
        public static readonly string WideDivider = "<-wide->";

        /// <summary>
        /// Short divider (part as a part of footer/header collection).
        /// Short divider is not connected to the window border.
        /// </summary>
        /// 
        public static readonly string ShortDivider = "<-short->";

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets the items of the ListBox.
        /// </summary>
        /// <remarks>
        /// <pre>
        ///                
        ///    Items[]     
        ///                
        ///         Column
        ///         0    5    10   15   20    ...
        /// Row  0  ▒▒▒▒▒▒▒▒                             ClientWidth
        ///      1  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                  │
        ///      2  ▒▒▒▒▒▒▒▒▒▒▒▒                         ▼
        ///      3 ┌─ Window ────────────────────────────┐
        ///      4 │═════════════════════════════        │ ◄─  ViewFrom
        ///      5 │══════════════════════════           │
        ///      6 │════════════════════                 │
        ///      7 │════════════════════════════         │
        ///      8 │═════════════════════════--------------◄─  CurrentItem
        ///      9 │══════════════════                   │
        ///     10 │════════════════════════════════     │
        ///     11 │══════════════════════════           │
        ///     12 │══════════════════════               │
        ///     13 └─────────────────────────────────────┘ ◄─  ViewFrom + ViewHeight
        ///     14  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
        ///    ...  ▒▒▒▒▒
        ///  (last) ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ ◄───────────────────────  Items.Count - 1
        ///     
        /// </pre>
        /// </remarks>
        /// 
        protected TaggedTextCollection Items { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the text of ALL items in the ListBox.
        /// </summary>
        /// 
        public override string Text
        {
            // Note resets contents changed to false

            get
            {
                return Items == null ? string.Empty
                     : TaggedText.Join( "\r\n", Items );
            }
            set
            {
                // Reset text position and items collection to defaults

                CursorTop    = 0;
                CursorLeft   = 0;
                CurrentItem  = 0;
                ViewFrom     = 0;

                if ( value == null )
                {
                    Items = new TaggedTextCollection ();
                    return;
                }

                Items = TaggedText.SplitTextInLines( value );

                ContentsChanged = false;
                Invalidate ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value of indicatore whether vertical scroll bar is
        /// automatically visible when items could not fit the window client area. 
        /// </summary>
        /// 
        public virtual bool AutoScrollBar
        {
            get
            {
                return this.autoScrollBar;
            }
            set
            {
                InvalidateIf( value != this.autoScrollBar );
                this.autoScrollBar = value;
            }
        }

        private bool autoScrollBar;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets horizontal padding within the control.  
        /// </summary>
        /// 
        public virtual int HorizontalPadding
        {
            get
            {
                return this.horizontalPadding;
            }
            set
            {
                InvalidateIf( value != this.horizontalPadding );
                this.horizontalPadding = value;
            }
        }

        private int horizontalPadding;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the current item.
        /// </summary>
        /// 
        public virtual Color CurrentRowBackColor
        {
            get
            {
                return this.currentRowBackColor;
            }
            set
            {
                InvalidateIf( value != this.currentRowBackColor );
                this.currentRowBackColor = value;
            }
        }

        private Color currentRowBackColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the current item.
        /// </summary>
        /// 
        public virtual Color CurrentRowForeColor
        {
            get
            {
                return this.currentRowForeColor;
            }
            set
            {
                InvalidateIf( value != this.currentRowForeColor );
                this.currentRowForeColor = value;
            }
        }

        private Color currentRowForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the current item, used when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color CurrentRowBackColorInact
        {
            get
            {
                return this.currentRowBackColorInact;
            }
            set
            {
                InvalidateIf( value != this.currentRowBackColorInact );
                this.currentRowBackColorInact = value;
            }
        }

        private Color currentRowBackColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the current item, used when the window
        /// is out of the focus
        /// </summary>
        /// 
        public virtual Color CurrentRowForeColorInact
        {
            get
            {
                return this.currentRowForeColorInact;
            }
            set
            {
                InvalidateIf( value != this.currentRowForeColorInact );
                this.currentRowForeColorInact = value;
            }
        }

        private Color currentRowForeColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the header.
        /// </summary>
        /// 
        public virtual Color HeaderBackColor
        {
            get
            {
                return this.headerBackColor;
            }
            set
            {
                InvalidateIf( value != this.headerBackColor );
                this.headerBackColor = value;
            }
        }

        private Color headerBackColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the header.
        /// </summary>
        /// 
        public virtual Color HeaderForeColor
        {
            get
            {
                return this.headerForeColor;
            }
            set
            {
                InvalidateIf( value != this.headerForeColor );
                this.headerForeColor = value;
            }
        }

        private Color headerForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the header, used when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color HeaderBackColorInact
        {
            get
            {
                return this.headerBackColorInact;
            }
            set
            {
                InvalidateIf( value != this.headerBackColorInact );
                this.headerBackColorInact = value;
            }
        }

        private Color headerBackColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the header, used when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color HeaderForeColorInact
        {
            get
            {
                return this.headerForeColorInact;
            }
            set
            {
                InvalidateIf( value != this.headerForeColorInact );
                this.headerForeColorInact = value;
            }
        }

        private Color headerForeColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the height of the header.
        /// </summary>
        /// 
        public virtual int HeaderHeight
        {
            get 
            { 
                return this.header == null ? 0 : this.header.Length;
            }
        }

        /// <summary>
        /// Gets or sets the list header.
        /// </summary>
        /// 
        public virtual string[] Header
        {
            get
            {
                return this.header;
            }
            set
            {
                this.header = value;
                Invalidate ();
            }
        }

        private string[] header;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the footer.
        /// </summary>
        /// 
        public virtual Color FooterBackColor
        {
            get
            {
                return this.footerBackColor;
            }
            set
            {
                InvalidateIf( value != this.footerBackColor );
                this.footerBackColor = value;
            }
        }

        private Color footerBackColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the footer.
        /// </summary>
        /// 
        public virtual Color FooterForeColor
        {
            get
            {
                return this.footerForeColor;
            }
            set
            {
                InvalidateIf( value != this.footerForeColor );
                this.footerForeColor = value;
            }
        }

        private Color footerForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the footer, used when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color FooterBackColorInact
        {
            get
            {
                return this.footerBackColorInact;
            }
            set
            {
                InvalidateIf( value != this.footerBackColorInact );
                this.footerBackColorInact = value;
            }
        }

        private Color footerBackColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color of the footer, used when the window
        /// is out of the focus.
        /// </summary>
        /// 
        public virtual Color FooterForeColorInact
        {
            get
            {
                return this.footerForeColorInact;
            }
            set
            {
                InvalidateIf( value != this.footerForeColorInact );
                this.footerForeColorInact = value;
            }
        }

        private Color footerForeColorInact;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the height of the footer.
        /// </summary>
        /// 
        public virtual int FooterHeight
        {
            get 
            { 
                return this.footer == null ? 0 : this.footer.Length;
            }
        }

        /// <summary>
        /// Gets or sets the footer of the ListBox.
        /// </summary>
        /// 
        public virtual string[] Footer
        {
            get
            {
                return this.footer;
            }
            set
            {
                this.footer = value;
                Invalidate ();
            }
        }

        private string[] footer;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets number of visible items in the window.
        /// </summary>
        /// 
        public virtual int ItemsPerPage
        {
            get { return ViewHeight; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether contents of the control is read-only.
        /// </summary>
        /// 
        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                int index = CurrentItem;

                base.ReadOnly = value;

                // Revalidate current item
                //
                CurrentItem = index;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a value indicating whether the window handles itself 
        /// erasing of its background. Returns always true.
        /// </summary>
        /// 
        public sealed override bool OwnErase
        {
            get { return true; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the window has a vertical scroll bar.
        /// </summary>
        /// 
        public override bool VerticalScrollBar
        {
            get
            { 
                return AutoScrollBar ? this.ItemCount > this.ClientHeight
                                     : base.VerticalScrollBar; 
            }
            set
            {
                base.VerticalScrollBar = value;
            }
        }

        /// <summary>
        /// Gets lower bound of the vertical scroll bar values.
        /// </summary>
        /// 
        public override int VerticalScrollBarFirstItem
        {
            get { return ViewFrom; }
        }

        /// <summary>
        /// Gets upper bound of the vertical scroll bar values.
        /// </summary>
        /// 
        public override int VerticalScrollBarLastItem
        {
            get { return ViewFrom + ViewHeight; }
        }

        /// <summary>
        /// Gets current value of the vertical scrollb ar position.
        /// </summary>
        /// 
        public override int VerticalScrollBarItemCount
        {
            get { return ItemCount; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a value indicating whether the cursor is displayed. 
        /// Returns always false.
        /// </summary>
        /// 
        public sealed override bool CursorVisible
        {
            get { return false; }
        }

        /////////////////////////////////////////////////////////////////////////////////

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
                CurrentRowForeColor = value;
                base.BackColor = value;
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
                CurrentRowBackColor = value;
                base.ForeColor = value;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        /// 
        public int ItemCount
        {
            get
            {
                return Items == null ? 0 : Items.Count;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets view height.
        /// </summary>
        /// 
        public int ViewHeight
        {
            get { return ClientHeight - HeaderHeight - FooterHeight; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the index of the first item in the view.
        /// </summary>
        /// 
        public int ViewFrom
        {
            get
            {
                return this.viewFrom;
            }
            set
            {
                // Keep position betwen 0 and item count reduced for window.height
                //
                value = Math.Max( 0, Math.Min( ItemCount - ViewHeight, value ) );

                if ( value != this.viewFrom )
                {
                    this.viewFrom = value;
                    UpdateCursorPosition ();
                }
            }
        }

        private int viewFrom;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the index of the current item.
        /// </summary>
        /// 
        public int CurrentItem
        {
            get
            {
                return this.currentItem;
            }
            set
            {
                // The current item should be between 0 and Items.Count - 1.
                //
                int lastRow = ItemCount - 1;

                value = Math.Max( 0, Math.Min( lastRow, value ) );

                InvalidateIf( this.currentItem != value );

                this.currentItem = value;

                UpdateViewBounds ();
            }
        }

        private int currentItem;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the TaggedText value of the current item.
        /// </summary>
        /// 
        public TaggedText Current
        {
            get
            {
                return Items != null && CurrentItem >= 0 && CurrentItem < ItemCount 
                     ? Items[ CurrentItem ] : TaggedText.Empty ;
            }
            set
            {
                if ( Items == null )
                {
                    Items = new TaggedTextCollection ();
                }

                if ( Items.Count == 0 || CurrentItem >= ItemCount )
                {
                    Items.Add( value );
                }
                else if ( CurrentItem >= 0 && CurrentItem < ItemCount )
                {
                    Items[ CurrentItem ] = value;
                }

                UpdateCursorPosition ();
                Invalidate ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets Tag part of the current item.
        /// </summary>
        /// 
        public object CurrentTag
        {
            get { return Current.Tag; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a maximum length of the text part of all items.
        /// </summary>
        /// 
        public int MaxItemLength
        {
            get { return Items == null ? 0 : Items.MaxTextLength; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ListBoxBase class. 
        /// </summary>
        /// 
        public ListBoxBase () 
            : base ()
        {
            // Setup defult flags
            //
            ReadOnly = false;
            HorizontalPadding = 0;

            // Setup default colors
            //
            CurrentRowBackColor       = Application.Theme.CurrentRowBackColor;
            CurrentRowForeColor       = Application.Theme.CurrentRowForeColor;
            CurrentRowBackColorInact  = Application.Theme.CurrentRowBackColorInact;
            CurrentRowForeColorInact  = Application.Theme.CurrentRowForeColorInact;
            HeaderBackColor           = Application.Theme.HeaderBackColor;
            HeaderForeColor           = Application.Theme.HeaderForeColor;
            HeaderBackColorInact      = Application.Theme.HeaderBackColorInact;
            HeaderForeColorInact      = Application.Theme.HeaderForeColorInact;
            FooterBackColor           = Application.Theme.FooterBackColor;
            FooterForeColor           = Application.Theme.FooterForeColor;
            FooterBackColorInact      = Application.Theme.FooterBackColorInact;
            FooterForeColorInact      = Application.Theme.FooterForeColorInact;

            // Modifying Text also resets current position & view to 0 and
            // initialized this.liens to list.
            //
            Text = null;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Updates cursor position.
        /// </summary>
        /// 
        private void UpdateCursorPosition ()
        {
            CursorTop = CurrentItem - ViewFrom;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates ViewFrom property based on the cursor position.
        /// </summary>
        /// 
        private void UpdateViewBounds ()
        {
            UpdateCursorPosition ();

            // Adjust current vertical position, if cursor wanders vertically
            //
            if ( CursorTop >= ViewHeight )
            {
                ViewFrom += CursorTop - ViewHeight + 1;
            }
            else if ( CursorTop < 0 )
            {
                ViewFrom += CursorTop;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Selects current item by its index.
        /// </summary>
        ///
        public void SelectItem( int item )
        {
            CurrentItem = item;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clears all items.
        /// </summary>
        /// 
        public void ClearItems ()
        {
            Text = null;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds new item.
        /// </summary>
        /// 
        public int Append( TaggedText taggedText )
        {
            Items.Add( taggedText );

            Invalidate ();

            return Items.Count - 1;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Searches for an item by its Tag.
        /// </summary>
        /// 
        public int FindTag( object tag )
        {
            return Items.FindIndex( item => item.Tag == tag );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates the contents of the item, based on the Tag.
        /// </summary>
        /// 
        public void UpdateItemWithTag( TaggedText tag, bool move = false )
        {
            int position = FindTag( tag.Tag );

            if ( position >= 0 )
            {
                Items[ position ] = tag;

                if ( move )
                {
                    CurrentItem = position;
                }

                Invalidate ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Removes item with the tag.
        /// </summary>
        ///
        public void RemoveItemWithTag( object tag )
        {
            int position = FindTag( tag );

            if ( position >= 0 )
            {
                Items.RemoveAt( position );

                CurrentItem += 0; // force revalidation of the CurrentItem

                Invalidate ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Removes current item.
        /// </summary>
        /// 
        public void DeleteCurrentItem ()
        {
            if ( CurrentItem >= ItemCount )
            {
                return; // nothing to delete if at the end of textbox
            }

            Items.RemoveAt( CurrentItem );

            CurrentItem += 0; // force revalidation of the CurrentItem

            ContentsChanged = true;
            Invalidate ();
            OnTextChanged ();
        }

        /// <summary>
        /// Inserts a new item before the current item.
        /// </summary>
        ///
        public void InsertItem( string text = "" )
        {
            if ( CurrentItem >= ItemCount )
            {
                // Current position is after the last item, so simply add an empty item.
                //
                Items.Add( (TaggedText) text );
            }
            else
            {
                Items.Insert( CurrentItem, (TaggedText) text );
            }

            CurrentItem += 0; // force revalidation of the CurrentItem

            ContentsChanged = true;
            Invalidate ();
            OnTextChanged ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Raises the EraseBackground event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnEraseBackground ( Screen screen )
        {
            if ( screen == null )
            {
                return;
            }

            screen.Clear ();

            base.OnEraseBackground( screen );
        }

        /// <summary>
        /// Raises the DrawBorder event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnDrawBorder ( Screen screen, bool hasFocus )
        {
            if ( screen == null )
            {
                return;
            }

            base.OnDrawBorder( screen, hasFocus );

            int left = - ExtraLeft;
            int width = ClientWidth + ExtraLeft + ExtraRight; // widthout scrollbar

            // Draw 'wide'-dividers from header
            //
            for ( int row = 0; row < HeaderHeight; ++row )
            {
                if ( Header[ row ] == WideDivider )
                {
                    int top = row;
                    screen.DrawRectangle( left, top, width, 1, BoxLines.Joined );
                }
            }

            // Draw 'wide'-dividers from footer
            //
            for ( int row = 0; row < FooterHeight; ++row )
            {
                if ( Footer[ row ] == WideDivider )
                {
                    int top = ClientHeight - FooterHeight + row;
                    screen.DrawRectangle( left, top, width, 1, BoxLines.Joined );
                }
            }
        }

        /// <summary>
        /// Raises the DrawContents event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnDrawContents( Screen screen, bool hasFocus )
        {
            if ( screen == null )
            {
                return;
            }

            // Setup item colors for header
            //
            if ( hasFocus )
            {
                screen.BackColor = HeaderBackColor;
                screen.ForeColor = HeaderForeColor;
            }
            else
            {
                screen.BackColor = HeaderBackColorInact;
                screen.ForeColor = HeaderForeColorInact;
            }

            // Draw header
            //
            for ( int row = 0; row < HeaderHeight; ++row )
            {
                string text = Header[ row ];

                if ( text == WideDivider ) // 'wide'-divider drawn in border
                {
                    continue;
                }
                else if ( text == ShortDivider ) // 'short'-divider
                {
                    text = string.Empty.PadRight( ClientWidth, '─' );
                }

                text = TaggedText.AlignedText( text, ClientWidth,
                            TextAlign.Left, HorizontalPadding, HorizontalPadding );

                screen.CursorTop  = row;
                screen.CursorLeft = 0;
                screen.Write( text );
            }

            // Visible rows belongs to half-open range [ startRow, lastRow )
            //
            int startRow = Math.Max( 0, ViewFrom );
            int lastRow  = Math.Min( ItemCount, ViewFrom + ViewHeight );

            for ( int row = startRow; row < lastRow; ++row )
            {
                string text = row >= Items.Count ? string.Empty : Items[ row ].Text;

                text = TaggedText.AlignedText( text, ClientWidth,
                            TextAlign.Left, HorizontalPadding, HorizontalPadding );

                // Setup item colors
                //
                if ( hasFocus )
                {
                    if ( row == CurrentItem )
                    {
                        screen.BackColor = CurrentRowBackColor;
                        screen.ForeColor = CurrentRowForeColor;
                    }
                    else
                    {
                        screen.BackColor = BackColor;
                        screen.ForeColor = ForeColor;
                    }
                }
                else if ( row == CurrentItem )
                {
                    screen.BackColor = CurrentRowBackColorInact;
                    screen.ForeColor = CurrentRowForeColorInact;
                }
                else
                {
                    screen.BackColor = BackColorInact;
                    screen.ForeColor = ForeColorInact;
                }

                screen.CursorTop  = HeaderHeight + row - ViewFrom;
                screen.CursorLeft = 0;
                screen.Write( text );
            }

            // Setup item colors for footer
            //
            if ( hasFocus )
            {
                screen.BackColor = FooterBackColor;
                screen.ForeColor = FooterForeColor;
            }
            else
            {
                screen.BackColor = FooterBackColorInact;
                screen.ForeColor = FooterForeColorInact;
            }

            // Draw footer
            //
            for ( int row = 0; row < FooterHeight; ++row )
            {
                string text = Footer[ row ];

                if ( text == WideDivider ) // 'wide'-divider drawn in border
                {
                    continue;
                }
                else if ( text == ShortDivider ) // 'short'-divider
                {
                    text = string.Empty.PadRight( ClientWidth, '─' );
                }

                text = TaggedText.AlignedText( text, ClientWidth,
                            TextAlign.Left, HorizontalPadding, HorizontalPadding );

                screen.CursorTop  = ClientHeight - FooterHeight + row;
                screen.CursorLeft = 0;
                screen.Write( text );
            }

            base.OnDrawContents( screen, hasFocus );
        }

        /// <summary>
        /// Executed after the KeyDown event was raised but not handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected override void OnAfterKeyDown( KeyEventArgs e )
        {
            switch( e.KeyCode )
            {
                /////////////////////////////////////////////////////////////////////////

                case Keys.Up:

                    if ( ! ReadOnly )
                    {
                        if ( e.Control )
                        {
                            --ViewFrom;
                        }

                        --CurrentItem;
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Down:

                    if ( ! ReadOnly )
                    {
                        if ( e.Control )
                        {
                            ++ViewFrom;
                        }

                        ++CurrentItem;
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Home:

                    if ( ! ReadOnly )
                    {
                        ViewFrom = 0;
                        CurrentItem  = 0;
                        e.StopHandling ();
                    }

                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.End:

                    if ( ! ReadOnly )
                    {
                        ViewFrom = ItemCount - ViewHeight;
                        CurrentItem  = ItemCount - 1;
                        e.StopHandling ();
                    }

                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.PageUp:

                    if ( ! ReadOnly )
                    {
                        ViewFrom -= ViewHeight;
                        CurrentItem -= ViewHeight;
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.PageDown:

                    if ( ! ReadOnly )
                    {
                        ViewFrom += ViewHeight;
                        CurrentItem  += ViewHeight;
                    }

                    e.StopHandling ();
                    break;
            }

            base.OnAfterKeyDown( e );
        }

        #endregion
    }
}