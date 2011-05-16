/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       TextBox.cs
 *  Created:    2011-03-16
 *  Modified:   2011-04-30
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

namespace TextUI.Controls
{
    using TextUI.Drawing;

    /// <summary>
    /// Represents a TextUI text box control.
    /// </summary>
    /// 
    public class TextBox : Control
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the current text in the TextBox.
        /// </summary>
        /// 
        public override string Text
        {
            // Note resets contents changed to false

            get
            {
                return TaggedText.Join( "\r\n", this.Items );
            }
            set
            {
                // Reset text position and lines collection to defaults

                CursorLeft     = 0;
                CursorTop      = 0;
                CurrentRow     = 0;
                CurrentColumn  = 0;
                ViewFromRow    = 0;
                ViewFromColumn = 0;

                if ( value == null )
                {
                    this.Items = new TaggedTextCollection ();
                    ContentsChanged = false;
                    return;
                }

                if ( Multiline )
                {
                    this.Items = TaggedText.SplitTextInLines( value );
                }
                else
                {
                    // Add text as single line, cleared from CRLFs and other
                    // control characters.
                    //
                    this.Items = new TaggedTextCollection ();
                    this.Items.Add( TaggedText.ClearFromControlCharacters( value ) );
                }

                ContentsChanged = false;
                Invalidate ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the TaggedText collection (tagged text lines) of the TextBox.
        /// </summary>
        /// <remarks>
        /// <pre>
        ///                    ┌────────────────────────────────────────  ViewFromColumn
        ///    Items[]         │                        
        ///                    │            ┌───────────────────────────  CurrentColumn
        ///         Column     ▼            ▼
        ///         0    5    10   15   20  | ...                   ┌───  ViewFromColumn
        /// Row  0  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒     |                       │     + ClientWidth
        ///      1  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒|▒▒▒▒▒                  │
        ///      2  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ |                       ▼
        ///      3  ▒▒▒▒▒▒▒▒▒▒┌─ Window ────|───────────────────────┐
        ///      4  ▒▒▒▒▒▒▒▒▒▒│═════════════|═══════════════        │ ◄─  ViewFromRow
        ///      5  ▒▒▒▒▒▒▒▒▒▒│═════════════|════════════           │
        ///      6  ▒▒▒▒▒▒▒▒▒▒│═════════════|══════                 │
        ///      7  ▒▒▒▒▒▒▒▒▒▒│═════════════|══════════════         │
        ///      8  ▒▒▒▒▒▒▒▒▒▒│═════════════♦═══════════--------------◄─  CurrentRow
        ///      9  ▒▒▒▒▒▒▒▒▒▒│══════════════════                   │
        ///     10  ▒▒▒▒▒▒▒▒▒▒│════════════════════════════════     │
        ///     11  ▒▒▒▒▒▒▒▒▒▒│══════════════════════════           │
        ///     12  ▒▒▒▒▒▒▒▒▒▒│══════════════════════               │
        ///     13  ▒▒▒▒▒▒▒▒▒▒└─────────────────────────────────────┘ ◄─  ViewFromRow
        ///     14  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                    + ClientHeight
        ///    ...  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
        ///  (last) ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ ◄───────────────────────  Items.Count - 1
        ///     
        /// </pre>
        /// </remarks>
        /// 
        protected TaggedTextCollection Items { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets tool tip text usually displayed at the bottom in the status
        /// bar when the window is in focus.
        /// </summary>
        /// 
        public override string ToolTipText
        {
            get
            {
                return ToolTipWithLineInfo;
            }
            set
            {
                base.ToolTipText = value;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets tool tip text together with information about the read-only status,
        /// OVR/INS mode and current lien and column.
        /// </summary>
        /// 
        public string ToolTipWithLineInfo
        {
            get
            {
                if ( Application.StatusBarWindow != null )
                {
                    string lineInfo = ( ReadOnly ? "RO" : string.Empty );
                    
                    if ( Multiline && SelectFullRows )
                    {
                        lineInfo += string.Format( "  Ln {0,2}", CurrentRow + 1 );
                    }
                    else if ( Multiline )
                    {
                        lineInfo += string.Format( "  {0}  Ln {1,2}  Col {2,2}",
                            ( Application.Overwrite ? "OVR" : "INS" ),
                            CurrentRow + 1, CurrentColumn + 1 );
                    }
                    else if ( ! SelectFullRows )
                    {
                        lineInfo += string.Format( "  {0}  Col {1,2}", 
                            ( Application.Overwrite ? "OVR" : "INS" ),
                            CurrentColumn + 1 );
                    }

                    int leftWidth = Application.StatusBarWindow.Width - lineInfo.Length;
                    if ( leftWidth < 0 )
                    {
                        leftWidth = 0;
                    }

                    string leftInfo = base.ToolTipText != null 
                                    ? base.ToolTipText : Application.DefaultStatusBarText;

                    if ( leftInfo.Length > leftWidth )
                    {
                        leftInfo = leftInfo.Substring( 0, leftWidth );
                    }

                    return leftInfo.PadRight( leftWidth ) + lineInfo;
                }
                else
                {
                    return base.ToolTipText;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the lines of text in a text box control.
        /// </summary>
        /// 
        public virtual string[] Lines 
        {
            get
            {
                List<string> stringCollection = new List<string> ();
                foreach( TaggedText t in this.Items )
                {
                    stringCollection.Add( t.Text );
                }
                return stringCollection.ToArray ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether this is a multiline TextBox control.
        /// </summary>
        /// 
        public virtual bool Multiline
        {
            get
            {
                return this.multiline;
            }
            set
            {
                if ( value != this.multiline )
                {
                    string contents = Text;

                    this.multiline = value;

                    Text = contents;
                }
            }
        }

        private bool multiline;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether scroll bars should automatically
        /// appear  in a multiline TextBox control. 
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
        /// Gets or sets a value indicating whether full rows should be shown as
        /// selected as alternative to that only cursor (current character) is shown.
        /// </summary>
        /// 
        public virtual bool SelectFullRows
        {
            get
            {
                return this.selectFullRows;
            }
            set
            {
                int row = CurrentRow;
                int col = CurrentColumn;

                InvalidateIf( value != this.selectFullRows );
                this.selectFullRows = value;

                // Revalidate current row and column
                //
                CurrentRow = row;
                CurrentColumn = col;
            }
        }

        private bool selectFullRows;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color of the current line.
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
        /// Gets or sets the foreground color of the current line.
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
        /// Gets or sets the background color of the current line, used when the window
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
        /// Gets or sets the foreground color of the current line, used when the window
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
        /// Gets the current text in the TextBox with all leading and trailing 
        /// white-space characters removed from the text. 
        /// </summary>
        /// 
        public string TrimmedText
        {
            get
            {
                string t = this.Text.Trim ();
                return string.IsNullOrEmpty( t ) ? null : t;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether contents of the TextBox is read-only.
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
                int row = CurrentRow;
                int col = CurrentColumn;

                base.ReadOnly = value;

                // Revalidate current row and column
                //
                CurrentRow = row;
                CurrentColumn = currentColumn;
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
                if ( AutoScrollBar )
                {
                    return this.Multiline && this.RowCount > this.ClientHeight;
                }
                else
                {
                    return base.VerticalScrollBar && this.Multiline; 
                }
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
            get { return ViewFromRow; }
        }

        /// <summary>
        /// Gets upper bound of the vertical scroll bar values.
        /// </summary>
        /// 
        public override int VerticalScrollBarLastItem
        {
            get { return ViewFromRow + ClientHeight; }
        }

        /// <summary>
        /// Gets current value of the vertical scrollb ar position.
        /// </summary>
        /// 
        public override int VerticalScrollBarItemCount
        {
            get { return RowCount; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a value indicating whether the cursor is displayed. 
        /// Returns always false.
        /// </summary>
        /// 
        public override bool CursorVisible
        {
            get
            {
                return base.CursorVisible && ! SelectFullRows;
            }
            set
            {
                base.CursorVisible = value;
            }
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
        /// Gets the number of lines of text in the TextBox.
        /// </summary>
        /// 
        public int RowCount
        {
            get
            {
                return this.Items == null ? 0 : this.Items.Count;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets line number of the first line visible in the TextBox.
        /// </summary>
        /// 
        public int ViewFromRow
        {
            get
            {
                return this.viewFromRow;
            }
            set
            {
                int lastRow = RowCount - 1 
                            + ( Multiline && ! ReadOnly && ! SelectFullRows ? 1 : 0 );

                // Keep position betwen 0 and line count reduced for (window.height - 1)
                //
                value = Math.Max( 0, Math.Min( lastRow - ClientHeight + 1, value ) );

                if ( value != this.viewFromRow )
                {
                    this.viewFromRow = value;
                    UpdateCursorPosition ();
                }
            }
        }

        private int viewFromRow;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets column number of the first character in line visible in the TextBox.
        /// </summary>
        /// 
        public int ViewFromColumn
        {
            get
            {
                return this.viewFromColumn;
            }
            set
            {
                int lineWidth = Current.Text.Length;

                // Keep position between 0 and current line width reduced for 
                // ( window width - 1 )
                //
                value = Math.Max( 0, Math.Min( lineWidth - ClientWidth + 1, value ) );

                if ( value != this.viewFromColumn )
                {
                    this.viewFromColumn = value;
                    UpdateCursorPosition ();
                }
            }
        }

        private int viewFromColumn;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current line number.
        /// </summary>
        /// 
        public int CurrentRow
        {
            get
            {
                return this.currentRow;
            }
            set
            {
                // The current row should be between 0 and Items.Count (*inclsuive* 
                // Items.Count if in multiline mode and not in read only mode and not 
                // selecting full rows).
                //
                int lastRow = RowCount - 1 
                            + ( Multiline && ! ReadOnly && ! SelectFullRows ? 1 : 0 );

                value = Math.Max( 0, Math.Min( lastRow, value ) );

                InvalidateIf( this.currentRow != value );

                this.currentRow = value;

                UpdateViewAfterCurrentRowChange ();
            }
        }

        private int currentRow;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current column.
        /// </summary>
        /// 
        public int CurrentColumn
        {
            get
            {
                // Get adjusted current horizontal position. Current column is kept 
                // "as is" and may point to position to the right (outside) of the 
                // current line.
                //
                int lastColumn = Current.Text.Length; // - 1 + ( ReadOnly ? 0 : 1 );

                return Math.Max( 0, Math.Min( lastColumn, this.currentColumn ) );
            }
            set
            {
                // The current column should be between 0 and current line length 
                // (*inclsuive* line length if in multiline and not in read only mode).
                //
                int lastColumn = Current.Text.Length; // - 1 + ( ReadOnly ? 0 : 1 );

                value = Math.Max( 0, Math.Min( lastColumn, value ) );

                InvalidateIf( value != this.currentColumn );

                this.currentColumn = value;

                UpdateViewAfterCurrentColumnChange ();
            }
        }

        private int currentColumn;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the TaggedText value of the current line.
        /// </summary>
        /// 
        public TaggedText Current
        {
            get
            {
                return this.Items != null && CurrentRow >= 0 && CurrentRow < RowCount 
                     ? this.Items[ CurrentRow ] : (TaggedText) string.Empty;
            }
            set
            {
                if ( this.Items == null )
                {
                    this.Items = new TaggedTextCollection ();
                }

                if ( ReadOnly )
                {
                    return;
                }

                if ( this.Items.Count == 0 || CurrentRow >= RowCount )
                {
                    this.Items.Add( value );
                }
                else if ( CurrentRow >= 0 && CurrentRow < RowCount )
                {
                    this.Items[ CurrentRow ] = value;
                }

                UpdateCursorPosition ();
                Invalidate ();
            }
        }

        #endregion 

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the TextBox class.
        /// </summary>
        /// 
        public TextBox () 
            : base ()
        {
            // Setup defult flags
            //
            CursorVisible       = true;
            Multiline           = false;
            AutoScrollBar       = false;
            ReadOnly            = false;
            SelectFullRows      = false;
            Height              = 1;

            // Setup default colors
            //
            CurrentRowBackColor       = Application.Theme.CurrentRowBackColor;
            CurrentRowForeColor       = Application.Theme.CurrentRowForeColor;
            CurrentRowBackColorInact  = Application.Theme.CurrentRowBackColorInact;
            CurrentRowForeColorInact  = Application.Theme.CurrentRowForeColorInact;

            // Modifying Text also resets current position & view to 0.
            //
            Text = null;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets the window's cursor position from current line and column.
        /// </summary>
        /// 
        private void UpdateCursorPosition ()
        {
            CursorTop  = CurrentRow    - ViewFromRow;
            CursorLeft = CurrentColumn - ViewFromColumn;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates ViewFromRow and ViewFromColumn so the cursor is always visible.
        /// Should be called after current line has been changed.
        /// </summary>
        /// 
        private void UpdateViewAfterCurrentRowChange ()
        {
            UpdateCursorPosition ();

            // Adjust current vertical position, if cursor wanders vertically
            //
            if ( CursorTop >= ClientHeight )
            {
                ViewFromRow += CursorTop - ClientHeight + 1;
            }
            else if ( CursorTop < 0 )
            {
                ViewFromRow += CursorTop;
            }

            // Adjust current horizontal position, if cursor wanders horizontally
            //
            if ( CursorLeft >= ClientWidth )
            {
                ViewFromColumn += CursorLeft - ClientWidth + 1;
            }
            else if ( CursorLeft < 0 )
            {
                // Jump to first column if the complete line can fit the window
                //
                if ( Current.Text.Length < ClientWidth )
                {
                    ViewFromColumn = 0;
                }
                else
                {
                    ViewFromColumn += CursorLeft;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Updates ViewFromRow and ViewFromColumn so the cursor is always visible.
        /// Should be called after current column has been changed.
        /// </summary>
        /// 
        private void UpdateViewAfterCurrentColumnChange ()
        {
            UpdateCursorPosition();

            if ( CursorLeft >= ClientWidth )
            {
                ViewFromColumn += CursorLeft - ClientWidth + 1;
            }
            else if ( CursorLeft < 0 )
            {
                ViewFromColumn += CursorLeft;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Appends text of the text box. In multiline mode adds new line, while
        /// in singleline mode appends current line.
        /// </summary>
        ///
        public void AppendText( string text )
        {
            if ( this.Items == null )
            {
                // Reset text position and lines collection to defaults

                CursorLeft     = 0;
                CursorTop      = 0;
                CurrentRow     = 0;
                CurrentColumn  = 0;
                ViewFromRow    = 0;
                ViewFromColumn = 0;

                this.Items = new TaggedTextCollection ();
            }

            if ( text == null )
            {
                return;
            }

            if ( Multiline )
            {
                this.Items.Add( TaggedText.ClearFromControlCharacters( text ) );
            }
            else // append current line
            {
                Current += TaggedText.ClearFromControlCharacters( text );
            }

            Invalidate ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets text of the TextBox from string collection.
        /// </summary>
        /// 
        public void SetText( List<string> array )
        {
            Text = null;
            AppendText( array );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the lines of text in a text box control as string collection.
        /// </summary>
        /// 
        public List<string> ToStringList ()
        {
            List<string> array = new List<string> ();

            foreach ( TaggedText t in this.Items )
            {
                array.Add( t.Text );
            }

            return array;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Appends text from string collection.
        /// </summary>
        ///
        public void AppendText( List<string> array )
        {
            if ( this.Items == null )
            {
                // Reset text position and lines collection to defaults

                CursorLeft     = 0;
                CursorTop      = 0;
                CurrentRow     = 0;
                CurrentColumn  = 0;
                ViewFromRow    = 0;
                ViewFromColumn = 0;

                this.Items = new TaggedTextCollection ();
            }

            if ( array == null )
            {
                return;
            }
            
            if ( Multiline )
            {
                foreach( string line in array )
                {
                    this.Items.Add( TaggedText.ClearFromControlCharacters( line ) );
                }
            }
            else // append current line
            {
                Current += TaggedText.ClearFromControlCharacters( 
                    string.Join( " ", array.ToArray () ) );
            }

            Invalidate ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Removes character under the cursor. 
        /// </summary>
        /// 
        public void DeleteCurrentCharacter ()
        {
            if ( ReadOnly )
            {
                return;
            }

            if ( CurrentRow >= RowCount )
            {
                return; // nothing to delete if at the end of textbox
            }

            CurrentColumn += 0; // Make CurrentColumn in valid bounds.

            string line = Current.Text;

            if ( CurrentColumn < line.Length )
            {
                // Cursor is in the middle of line, so simply eat the character under
                // the cursor (without moving cursor).
                //
                this.Items[ CurrentRow ] = this.Items[ CurrentRow ]
                                         .Replace( line.Remove( CurrentColumn, 1 ) );
            }
            else if ( Multiline )
            {
                // Cursor is at the end of line, so join with the next line and move
                // cursor (if not already at the last line).
                //
                if ( CurrentRow + 1 < RowCount )
                {
                    this.Items[ CurrentRow ] += this.Items[ CurrentRow + 1 ];
                    this.Items.RemoveAt( CurrentRow + 1 );
                }
            }

            ContentsChanged = true;
            Invalidate ();
            OnTextChanged ();
        }

        /// <summary>
        /// Removes character previous to the cursor.
        /// </summary>
        /// 
        public void DeletePreviousCharacter ()
        {
            if ( ReadOnly )
            {
                return;
            }

            if ( CurrentRow >= RowCount )
            {
                // If current position is after the last line, the only we should do
                // is to move cursor at the end of previous line (if not already 
                // at the first line).
                //
                if ( CurrentRow > 0 )
                {
                    --CurrentRow;
                    CurrentColumn = int.MaxValue/2;
                }
            }
            else if ( CurrentColumn > 0 )
            {
                // If in the middle of line, simply move cursor left and remove character
                // under the cursor.
                //
                --CurrentColumn;
                Current = Current.Replace( Current.Text.Remove( CurrentColumn, 1 ) );
            }
            else if ( CurrentRow > 0 )
            {
                // If at the beginning of line and it is not the first line, 
                // move cursor to the previous line and join with the next line.
                //
                --CurrentRow;
                CurrentColumn = int.MaxValue/2;
                Current += this.Items[ CurrentRow + 1 ].Text;
                this.Items.RemoveAt( CurrentRow + 1 );
            }

            // If cursor hits left edge, shift view 10 positions to the right
            //
            if ( CurrentColumn <= ViewFromColumn )
            {
                if ( Current.Text.Length > CurrentColumn )
                {
                    ViewFromColumn -= 10;
                }
            }

            ContentsChanged = true;
            OnTextChanged ();
        }

        /// <summary>
        /// Removes current line.
        /// </summary>
        /// 
        public void DeleteCurrentLine ()
        {
            if ( ReadOnly )
            {
                return;
            }

            if ( CurrentRow >= RowCount )
            {
                return; // nothing to delete if at the end of textbox
            }

            int current = CurrentRow;

            CurrentColumn = 0;

            this.Items.RemoveAt( CurrentRow );

            CurrentRow = current; // this forces revalidation of the CurrentRow

            ContentsChanged = true;
            Invalidate ();
            OnTextChanged ();
        }

        /// <summary>
        /// Adds new line at current character position (splitting line in two).
        /// </summary>
        ///
        public void InsertNewLine( bool inPlace = true )
        {
            if ( ReadOnly || ! Multiline )
            {
                return;
            }

            if ( ! inPlace )
            {
                CurrentColumn = 0;
            }

            if ( CurrentRow >= RowCount )
            {
                // Current position is after the last line, so simply add an empty line.
                //
                this.Items.Add( (TaggedText) string.Empty );

                if ( inPlace )
                {
                    ++CurrentRow;
                }
            }
            else
            {
                // Split current line in two.
                //
                string line = Current.Text;
                Current = Current.Replace( line.Substring( 0, CurrentColumn ) );

                this.Items.Insert( CurrentRow + 1, 
                    new TaggedText( line.Substring( CurrentColumn ), Current.Tag )
                    );

                if ( inPlace )
                {
                    ++CurrentRow;
                    CurrentColumn = 0;
                }
            }

            ContentsChanged = true;
            OnTextChanged ();
        }

        /// <summary>
        /// Inserts characters at current position.
        /// </summary>
        ///
        public void InsertString( string str )
        {
            if ( ReadOnly )
            {
                return;
            }

            if ( CurrentRow >= RowCount )
            {
                // Current position is after the last line, simply add a new line
                // containing string to be inserted.
                //
                this.Items.Add( (TaggedText) str );

                CurrentColumn += str.Length;
            }
            else
            {
                // Insert string at current position.
                //
                Current = Current.Replace( Current.Text.Insert( CurrentColumn, str ) );

                CurrentColumn += str.Length;
            }

            // If cursor hits right edge, shift view 10 positions to the left
            //
            if ( CurrentColumn >= ViewFromColumn + ClientWidth - 1 )
            {
                if ( Current.Text.Length > CurrentColumn )
                {
                    ViewFromColumn += 10;
                }
            }

            ContentsChanged = true;
            OnTextChanged ();
        }

        /// <summary>
        /// Overwrites characters starting at current position.
        /// </summary>
        ///
        public void ReplaceString( string str )
        {
            if ( ReadOnly )
            {
                return;
            }

            if ( CurrentRow >= RowCount )
            {
                // Current position is after the last line, simply add a new line
                // containing string to be replaced.
                //
                this.Items.Add( (TaggedText) str );

                CurrentColumn += str.Length;
            }
            else
            {
                // Replace string at current position.
                //
                int count = Math.Min( str.Length, Current.Text.Length - CurrentColumn );

                string newText = Current.Text.Remove( CurrentColumn, count )
                                        .Insert( CurrentColumn, str );

                Current = Current.Replace( newText );

                CurrentColumn += str.Length;
            }

            // If cursor hits right edge, shift view 10 positions to the left
            //
            if ( CurrentColumn >= ViewFromColumn + ClientWidth - 1 )
            {
                if ( Current.Text.Length > CurrentColumn )
                {
                    ViewFromColumn += 10;
                }
            }

            ContentsChanged = true;
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

            // Visible rows belongs to half-open range [ startRow, lastRow )
            //
            int startRow = Math.Max( 0, ViewFromRow );
            int lastRow  = Math.Min( RowCount, ViewFromRow + ClientHeight );

            for ( int row = startRow; row < lastRow; ++row )
            {
                string line = row >= Items.Count ? string.Empty : this.Items[ row ].Text;

                // Visible characters belongs half-open range [ startCol, lastCol )
                //
                int startCol = Math.Max( 0, ViewFromColumn );
                int lastCol  = Math.Min( line.Length, ViewFromColumn + ClientWidth );

                // Setup line colors for highlighted line
                //
                if ( row == CurrentRow && SelectFullRows )
                {
                    if ( hasFocus )
                    {
                        screen.BackColor = CurrentRowBackColor;
                        screen.ForeColor = CurrentRowForeColor;
                    }
                    else
                    {
                        screen.BackColor = CurrentRowBackColorInact;
                        screen.ForeColor = CurrentRowForeColorInact;
                    }
                }

                screen.CursorTop  = row - ViewFromRow;
                screen.CursorLeft = startCol - ViewFromColumn;

                // Display visible characters, if visible length > 0
                //
                if ( startCol < lastCol )
                {
                    screen.Write( line.Substring( startCol, lastCol - startCol ) );
                }

                // Pad highlighted line and reset colors
                //
                if ( row == CurrentRow && SelectFullRows )
                {
                    screen.Write( 
                        string.Empty.PadRight( ClientWidth - ( lastCol - startCol ) )
                        );

                    if ( hasFocus )
                    {
                        screen.BackColor = BackColor;
                        screen.ForeColor = ForeColor;
                    }
                    else
                    {
                        screen.BackColor = BackColorInact;
                        screen.ForeColor = ForeColorInact;
                    }
                }
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

                    if ( e.Control )
                    {
                        --ViewFromRow;
                    }

                    --CurrentRow;

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Down:

                    if ( e.Control )
                    {
                        ++ViewFromRow;
                    }

                    ++CurrentRow;

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Left:

                    if ( e.Control || SelectFullRows )
                    {
                        --ViewFromColumn;
                    }

                    if ( ! SelectFullRows )
                    {
                        if ( CurrentColumn > 0 )
                        {
                            --CurrentColumn;
                        }
                        else if ( CurrentRow > 0 )
                        {
                            --CurrentRow;
                            CurrentColumn = int.MaxValue/2;
                        }
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Right:

                    if ( e.Control || SelectFullRows )
                    {
                        ++ViewFromColumn;
                    }

                    if ( ! SelectFullRows )
                    {
                        if ( CurrentColumn < Current.Text.Length )
                        {
                            ++CurrentColumn;
                        }
                        else if ( Multiline && CurrentRow <= RowCount )
                        {
                            ++CurrentRow;
                            CurrentColumn = 0;
                        }
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Home:

                    if ( e.Control ) // goto first line first column
                    {
                        ViewFromRow = 0;
                        CurrentRow  = 0;
                        CurrentColumn = 0;
                    }
                    else if ( e.Modifiers == 0 ) // first column in current line
                    {
                        CurrentColumn = 0;
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.End:

                    if ( e.Control && Multiline ) // goto last line first column
                    {
                        ViewFromRow = RowCount - ClientHeight + ( SelectFullRows ? 0 : 1 );
                        CurrentRow  = RowCount + ( SelectFullRows ? -1 : 0 );
                        CurrentColumn = 0;
                    }
                    else if ( e.Modifiers == 0 ) // last column in current line
                    {
                        CurrentColumn = int.MaxValue/2;
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.PageUp:

                    ViewFromRow -= ClientHeight;
                    CurrentRow  -= ClientHeight;

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.PageDown:

                    ViewFromRow += ClientHeight;
                    CurrentRow  += ClientHeight;

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Back:

                    if ( ! SelectFullRows )
                    {
                        DeletePreviousCharacter ();
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Delete:

                    if ( e.Control || SelectFullRows )
                    {
                        DeleteCurrentLine ();
                    }
                    else
                    {
                        DeleteCurrentCharacter ();
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Insert:

                    if ( e.Control || SelectFullRows )
                    {
                        InsertNewLine( false );
                    }
                    else
                    {
                        Application.Overwrite = ! Application.Overwrite;
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Enter:

                    if ( ! Multiline )
                    {
                        if ( Parent != null )
                        {
                            Parent.SelectNextControl( this );
                        }
                    }
                    else if ( ReadOnly ) // if multiline and read-only
                    {
                        if ( Parent != null )
                        {
                            Parent.SelectNextControl( this );
                        }
                    }
                    else if ( ! SelectFullRows ) // multiline and not selected full rows
                    {
                        // if multiline, not read-only and not selected full rows
                        InsertNewLine ();
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                default:

                    if ( ! char.IsControl( e.Character ) && ! SelectFullRows )
                    {
                        if ( Application.Overwrite )
                        {
                            ReplaceString( string.Empty + e.Character );
                        }
                        else
                        {
                            InsertString( string.Empty + e.Character );
                        }

                        e.StopHandling ();
                    }
                    break;
            }

            base.OnAfterKeyDown( e );
        }

        #endregion
    }
}