/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       ComboBox.cs
 *  Created:    2011-03-30
 *  Modified:   2011-05-07
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
    /// Represents a TextUI combo box control. 
    /// </summary>
    /// 
    public class ComboBox : ListBoxBase
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Events ]

        /// <summary>
        /// Occurs when the CurrentItem property has changed.
        /// </summary>
        /// 
        public event EventHandler SelectedIndexChanged = null;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Fields ]

        /// <summary>
        /// Index of the currently selected item. Negative if there no items selected.
        /// </summary>
        /// 
        private int selectedItem = -1;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether the height of the control 
        /// automatically adjusts when the text assigned to the control is changed. 
        /// </summary>
        /// 
        public virtual bool AutoSize
        {
            get
            {
                return this.autoSize;
            }
            set
            {
                InvalidateIf( value != this.autoSize );
                this.autoSize = value;
            }
        }

        private bool autoSize;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the combo box is displaying its 
        /// drop-down portion.
        /// </summary>
        /// 
        public virtual bool DroppedDown
        {
            get
            {
                return this.droppedDown;
            }
            set
            {
                InvalidateIf( value != this.droppedDown );
                this.droppedDown = value;
            }
        }

        private bool droppedDown;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the maximum height of the drop-down portion of the ComboBox.
        /// </summary>
        public virtual int MaxHeight
        {
            get
            {
                return this.maxHeight;
            }
            set
            {
                InvalidateIf( value != this.maxHeight );
                this.maxHeight = value;
            }
        }

        private int maxHeight;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether unhandled key events are forwarded
        /// to the parent of the window.
        /// </summary>
        /// 
        public sealed override bool ForwadKeysToParent
        {
            get
            {
                return base.ForwadKeysToParent && ! DroppedDown;
            }
            set
            {
                base.ForwadKeysToParent = value;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ComboBox class.
        /// </summary>
        ///
        public ComboBox( string text = null ) 
            : base ()
        {
            TabStop           = true;
            Border            = true;
            AutoSize          = true;
            OwnErase          = true;

            HorizontalPadding = 1;

            Text = text; // this will initialize maxLineWidth and items[]

            Width = Math.Min( 4, MaxItemLength + 2 * HorizontalPadding + 2 );
            MaxHeight = 0;

            // Header holds current item
            //
            Header = new string [] { Current.Text, ListBoxBase.WideDivider };
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Adds a new TaggedText item to the ComboBox.
        /// </summary>
        ///
        public void AddItem( TaggedText item )
        {
            item = TaggedText.ClearFromControlCharacters( item.Text, item.Tag );

            Items.Add( item );

            Invalidate ();
        }

        /// <summary>
        /// Selects the current item by associated tag. 
        /// Note: Resets ContentsChanged to false.
        /// </summary>
        ///
        public void SelectItem( object tag )
        {
            // Note: Resets ContentsChanged to false

            for( int i = 0; i < Items.Count; ++i )
            {
                if ( Items[ i ].Tag.Equals( tag ) )
                {
                    CurrentItem = this.selectedItem = i;
                    Header[0] = Current.Text;
                    break;
                }
            }

            ContentsChanged = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Protected Methods ]

        /// <summary>
        /// Raises the SelectedIndexChanged event.
        /// </summary>
        /// 
        protected void OnSelectedIndexChanged ()
        {
            if ( SelectedIndexChanged != null )
            {
                SelectedIndexChanged( this, EventArgs.Empty );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Displays drop-down portion of the ComboBox and starts being modal.
        /// </summary>
        /// 
        private void EnterDropDown ()
        {
            DroppedDown = true; // must be first as this.Height depends on it

            OnCalculateSize( true );

            // Ensure visible CurrentItem
            //
            ViewFrom = 0;
            CurrentItem = this.selectedItem;
        }

        /// <summary>
        /// Hides drop-down portion of the ComboBox. Doesn't change selected item.
        /// </summary>
        /// 
        private void CancelDropDown ()
        {
            CurrentItem = this.selectedItem;
            Header [ 0 ] = Current.Text;

            DroppedDown = false;
        }

        /// <summary>
        /// Accepts selected item and hides drop-down portion of the ComboBox.
        /// </summary>
        /// 
        private void ExitDropDownAcceptCurrent ()
        {
            if ( this.selectedItem != CurrentItem )
            {
                ContentsChanged = true;

                this.selectedItem = CurrentItem;
                Header [ 0 ] = Current.Text;

                DroppedDown = false;

                OnSelectedIndexChanged ();
            }
            else
            {
                DroppedDown = false;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Virtual member called before redraw of the window starts, used to
        /// recalculate window size for auto-size windows.
        /// </summary>
        ///
        protected override void OnCalculateSize( bool hasFocus )
        {
            if ( AutoSize )
            {
                Width = MaxItemLength + 2 * HorizontalPadding + 2;
            }

            if ( ! DroppedDown )
            {
                Height = 1;
                AutoScrollBar = false;
            }
            else
            {
                int shownItems = MaxHeight <= 0 ? ItemCount : MaxHeight;

                if ( Parent != null )
                {
                    shownItems = Math.Min( shownItems, 
                        Parent.Height - Top - Header.Length - ExtraBottom );
                }

                Height = Header.Length + Math.Min( shownItems, ItemCount );

                AutoScrollBar = true;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Raises the DrawBorder event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnDrawBorder ( Screen screen, bool hasFocus )
        {
            base.OnDrawBorder( screen, hasFocus );

            if ( screen != null && hasFocus && Border && ! VerticalScrollBar )
            {
                // Draw vertical separator left of drop-down arrow
                //
                screen.DrawRectangle( ClientWidth - 2, -1, 1, 3, BoxLines.Joined );

                // Draw drop-down arrow
                //
                screen.ForeColor = hasFocus 
                                 ? ScrollBarForeColor 
                                 : ScrollBarForeColorInact;

                screen.CursorLeft = ClientWidth - 1;
                screen.CursorTop = 0;

                screen.Write( "" + Box.Down );
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Raises the DrawContents event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnDrawContents ( Screen screen, bool hasFocus )
        {
            if ( ! DroppedDown )
            {
                base.OnDrawContents( screen, hasFocus );
            }
            else
            {
                // When dropped down, header (which contains current item i.e. title)
                // has normal collors, while current item will be highlighted.
                // We do that by termporarily adjusting header colors before calling
                // ListBoxBase's OnDrawContents.
                //
                Color savedHeaderForeColor = HeaderForeColor;
                Color savedHeaderBackColor = HeaderBackColor;

                HeaderForeColor = ForeColor;
                HeaderBackColor = BackColor;

                base.OnDrawContents( screen, hasFocus );

                HeaderForeColor = savedHeaderForeColor;
                HeaderBackColor = savedHeaderBackColor;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Executed after the KeyDown event was raised but not handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected override void OnAfterKeyDown( KeyEventArgs e )
        {
            switch( e.KeyCode )
            {
                case Keys.Spacebar:
                case Keys.Enter:

                    if ( ! ReadOnly )
                    {
                        if ( ! DroppedDown )
                        {
                            EnterDropDown ();
                        }
                        else
                        {
                            ExitDropDownAcceptCurrent ();
                        }
                    }
                    else if ( Parent != null )
                    {
                        Parent.SelectNextControl( this );
                    }

                    e.StopHandling ();
                    break;

                case Keys.Escape:

                    CancelDropDown ();

                    e.StopHandling ();
                    break;
                    
                case Keys.Left:

                    if ( ! ReadOnly )
                    {
                        if ( DroppedDown )
                        {
                            --CurrentItem;
                        }
                        else
                        {
                            int item = CurrentItem;
                            if ( --CurrentItem != item )
                            {
                                ContentsChanged = true;
                                Header[0] = Current.Text;
                                OnSelectedIndexChanged ();
                            }
                        }
                    }

                    e.StopHandling ();
                    break;

                case Keys.Up:

                    if ( ! ReadOnly )
                    {
                        --CurrentItem;
                    }

                    e.StopHandling ();
                    break;

                case Keys.Right:

                    if ( ! ReadOnly )
                    {
                        if ( DroppedDown )
                        {
                            ++CurrentItem;
                        }
                        else
                        {
                            int item = CurrentItem;
                            if ( ++CurrentItem != item )
                            {
                                ContentsChanged = true;
                                Header[0] = Current.Text;
                                OnSelectedIndexChanged ();
                            }
                        }
                    }

                    e.StopHandling ();
                    break;

                case Keys.Down:

                    if ( ! ReadOnly )
                    {
                        ++CurrentItem;
                    }

                    e.StopHandling ();
                    break;
            }

            base.OnAfterKeyDown( e );
        }

        #endregion
    }
}