/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyListView.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using VideoRentalOutlet_GUI.Properties;
#endif

using Mbk.Commons;

/////////////////////////////////////////////////////////////////////////////////////////

#if TEXTUI

    /// <summary>
    /// Represents a list view control that is used in this application. 
    /// </summary>
    /// <remarks>
    /// Derived from TextUI ListBox control in Text UI mode.
    /// </remarks>
    /// 
    internal class MyListView : ListBox
    {
        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the MyListView class.
        /// </summary>
        /// 
        public MyListView ()
            : base ()
        {
            base.AutoScrollBar = true;
            base.Border = true;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Resizes columns to fit the contents. Not implemented in TextUI mode.
        /// </summary>
        /// 
        public void FitContents ()
        {
        }

        /// <summary>
        /// Factory method that returns a new instance of the Updater class for
        /// this MyListView.
        /// </summary>
        ///
        public Updater GetUpdater ()
        {
            return new Updater( this );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ MyListView.Updater Class ]

        /// <summary>
        /// Represents an updater used to suppress updating process of the control.
        /// In TextUI mode does nothing. See GUI mode implementation for more details.
        /// </summary>
        /// 
        public class Updater : IDisposable
        {
            public Updater( MyListView listView )
            {
                // listView.BeginUpdate ();
            }

            public void Dispose ()
            {
                // listView.EndUpdate ();
            }

            public void Refresh ()
            {
                Application.StatusBarWindow.Text = Application.InfoMessage;
                Application.StatusBarWindow.ForeColorInact = Color.Yellow;
                Application.Screen.UpdateScreen ();
            }
        }

        #endregion
    }

#else

    /// <summary>
    /// Represents a list view control that is used in this application. 
    /// </summary>
    /// <remarks>
    /// Derived from System.Windows.Forms ListView control in GUI mode.
    /// Note that MyListView is implemented as a "virtual" list view i.e.
    /// providing and handling maintenance of its own items instead of 
    /// letting the underlying ListView to do so.
    /// </remarks>
    /// 
    [System.ComponentModel.DesignerCategory("Code")]
    internal class MyListView : ListView
    {
        #region [ Fields ]

        // Keeps tracking the depth of BeginUpdate()/EndUpdate() calls.
        // The ondly methods allowed to change this field are BeginUpdate and EndUpdate,
        // which masks base methods with the same name.
        //
        private int insideUpdate;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Protected Properties ]

        /// <summary>
        /// Gets or sets a collection containing all tagged items in the control. 
        /// </summary>
        /// 
        protected TaggedTextCollection TaggedItems { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the caption (window title) associated with the control.
        /// </summary>
        /// 
        public string Caption
        {
            get { return Parent != null ? Parent.Text : Text; }
            set { if ( Parent != null ) Parent.Text = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        /// 
        public int ItemCount 
        {
            get { return TaggedItems == null ? 0 : TaggedItems.Count; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the index of the current item.
        /// </summary>
        /// 
        public int CurrentItem
        {
            get {  return this.currentItem; }
            set { SelectItem( value ); }
        }

        private int currentItem = -1;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the tag of the current item.
        /// </summary>
        /// 
        public object CurrentTag
        {
            get
            { 
                return currentItem < 0 || currentItem >= ItemCount ? null 
                    : TaggedItems[ this.currentItem ].Tag;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the sorting TaggedText comparer for the control. 
        /// </summary>
        /// 
        public IComparer<TaggedText> TaggedItemSorter
        { 
            get
            {
                return this.taggedItemSorter;
            }
            set
            {
                this.taggedItemSorter = value;

                if ( this.taggedItemSorter != null )
                {
                    using ( Updater updater = GetUpdater () )
                    {
                        object savedTag = CurrentTag;

                        TaggedItems.Sort( this.taggedItemSorter );

                        if ( savedTag != null )
                        {
                            SelectItem( FindTag( savedTag ) );
                        }
                    }
                }
            }
        }

        private IComparer<TaggedText> taggedItemSorter;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the MyListView class. 
        /// </summary>
        /// 
        public MyListView ()
            : base ()
        {
            // The list view is in Virtual Mode and provides its own data-management 
            // operations. It holds its items in TaggedItems, which is a collection 
            // of TaggedText.
            //
            this.TaggedItems = new TaggedTextCollection ();
            this.TaggedItemSorter = null;
            base.RetrieveVirtualItem += EH_RetrieveVirtualItem;
            base.VirtualMode = true;

            // Setup details view mode with full row select (without hiding selection
            // on lost focus).
            //
            base.View           = View.Details;
            base.FullRowSelect  = true;
            base.MultiSelect    = false;
            base.HideSelection  = false;
            base.DoubleBuffered = true;

            // Add leading column that will hold an empty string. This is because
            // of the ListView's "feature" that the first column cannot be right aligned.
            // Note that we also need to disable resizing ability for the first column.
            // See OnColumnWidthChanging and OnColumnWidthChanged overriden methods.
            //
            base.Columns.Add( string.Empty, 0 );

            this.insideUpdate = 0;
            UpdateVirtualSize ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Occurs when the ListView is in virtual mode and requires a ListViewItem.
        /// Returns ListViewItem created from TaggedText.
        /// </summary>
        /// 
        private void EH_RetrieveVirtualItem( object sender,
            RetrieveVirtualItemEventArgs e )
        {
            TaggedText tag = TaggedItems[ e.ItemIndex ];

            ListViewItem lvi = new ListViewItem()
            {
                Text = tag.Fields == null ? tag.Text : tag.Fields[0],
                Tag  = tag.Tag,
            };

            if ( tag.Fields != null )
            {
                for ( int i = 0; i < tag.Fields.Length; ++i )
                {
                    lvi.SubItems.Add( tag.Fields[i] );
                }
            }

            e.Item = lvi;
        }

        /// <summary>
        /// Prevents the control from drawing until the EndUpdate method is called.
        /// </summary>
        /// <remarks>
        /// Hides base BeginUpdate.
        /// </remarks>
        /// 
        private new void BeginUpdate ()
        {
            if ( this.insideUpdate == 0 )
            {
                base.Cursor = Cursors.WaitCursor;
            }

            base.BeginUpdate ();

            ++this.insideUpdate;
        }

        /// <summary>
        /// Resumes drawing of the list view control after drawing is suspended by the
        /// BeginUpdate method. 
        /// </summary>
        /// <remarks>
        /// Hides base EndUpdate!
        /// </remarks>
        /// 
        private new void EndUpdate ()
        {
            --this.insideUpdate;

            if ( this.insideUpdate == 0 )
            {
                base.Cursor = Cursors.Default;
            }

            UpdateVirtualSize ();

            base.EndUpdate ();
        }

        /// <summary>
        /// Updates virtual list size based on actual TaggedText item count.
        /// </summary>
        /// 
        private void UpdateVirtualSize ()
        {
            if ( this.insideUpdate <= 0 )
            {
                VirtualListSize = ItemCount;

                // Update also background image to display "Empty" when the list is empty
                //
                if ( ItemCount == 0 && BackgroundImage == null )
                {
                    BackgroundImage = Resources.EmptyList;
                    BackgroundImageTiled = true;
                }
                else if ( ItemCount != 0 && BackgroundImage != null )
                {
                    BackgroundImage = null;
                    BackgroundImageTiled = false;
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Fits the contents of the control resizing column widths appropriatelly.
        /// </summary>
        /// 
        public void FitContents ()
        {
            if ( ItemCount != VirtualListSize )
            {
                VirtualListSize = ItemCount;
            }

            for ( int i = 1; i < Columns.Count - 1; ++i )
            {
                if ( Columns[i].Width < 0 )
                {
                    Columns[i].AutoResize( ColumnHeaderAutoResizeStyle.ColumnContent );
                }
            }

            if ( Columns.Count > 0 )
            {
                Columns[ Columns.Count - 1 ].Width = -2;
            }
        }

        /// <summary>
        /// Factory method that returns a new instance of the Updater class for
        /// this MyListView.
        /// </summary>
        ///
        public Updater GetUpdater ()
        {
            return new Updater( this );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// 
        public void ClearItems ()
        {
            TaggedItemSorter = null;

            TaggedItems.Clear ();

            UpdateVirtualSize ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Finds a TaggedText item in collection having a specified tag.
        /// </summary>
        /// 
        public int FindTag( object tag )
        {
            if ( tag is TaggedText )
            {
                tag = ( (TaggedText)tag ).Tag;
            }

            return TaggedItems.FindIndex( item => item.Tag == tag );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Select item at specified index as current.
        /// </summary>
        ///
        public void SelectItem( int index )
        {
            using ( Updater updater = GetUpdater () )
            {
                if ( this.Items.Count != ItemCount )
                {
                    VirtualListSize = ItemCount;
                }

                index = Math.Max( 0, Math.Min( index, base.Items.Count - 1 ) );

                if ( index >= 0 && index < base.Items.Count )
                {
                    this.currentItem = index;

                    base.Items[ index ].Selected = true;
                    base.Items[ index ].Focused = true;
                    base.Items[ index ].EnsureVisible ();
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds a new TaggedText to the end of the items collection.
        /// </summary>
        /// 
        public int Append( TaggedText taggedText )
        {
            TaggedItems.Add( taggedText );
            int index = ItemCount - 1;

            if ( TaggedItemSorter != null )
            {
                TaggedItems.Sort( TaggedItemSorter );
                index = FindTag( taggedText.Tag );
            }

            UpdateVirtualSize ();

            return index;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Synchronizes TaggedText item with the item in the list.
        /// </summary>
        /// <remarks>
        /// Finds a TaggedText item with a tag found in Tag property of a
        /// specified tagged text instance, and updates other properties of the found
        /// TaggedText item.
        /// </remarks>
        ///
        public void UpdateItemWithTag( TaggedText taggedText )
        {
            using ( Updater updater = GetUpdater () )
            {
                int position = FindTag( taggedText.Tag );

                if ( position >= 0 )
                {
                    object savedTag = CurrentTag;

                    TaggedItems[ position ] = taggedText;

                    if ( TaggedItemSorter != null )
                    {
                        TaggedItems.Sort( this.taggedItemSorter );

                        if ( savedTag != null )
                        {
                            CurrentItem = FindTag( savedTag );
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Removes TaggedText item from the collecion having a specified tag.
        /// </summary>
        /// 
        public void RemoveItemWithTag( object tag )
        {
            using ( Updater updater = GetUpdater () )
            {
                int position = FindTag( tag );

                if ( position >= 0 )
                {
                    TaggedItems.RemoveAt( position );
                    UpdateVirtualSize ();

                    if ( this.currentItem >= ItemCount )
                    {
                        this.currentItem = -1;
                    }
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Raises the ColumnClick event.
        /// Sorts clicked column using TaggedItemSorter.
        /// </summary>
        ///
        protected override void OnColumnClick ( ColumnClickEventArgs e )
        {
            Parent.Cursor = Cursors.WaitCursor;

            ItemSorter itemSorter = TaggedItemSorter as ItemSorter;

            SortOrder sortOrder = SortOrder.Ascending;

            if ( itemSorter != null && itemSorter.Column == e.Column )
            {
                // Toggle sorting order if clicked on the same as the last column.
                //
                sortOrder = itemSorter.Sorting == SortOrder.Ascending 
                            ? SortOrder.Descending : SortOrder.Ascending;
            }

            // Set the ListViewItemSorter property to a new ItemSorter object. Setting 
            // this property immediately sorts the ListView using the ItemSorter object.
            // 
            TaggedItemSorter = new ItemSorter( e.Column, sortOrder );

            base.OnColumnClick( e );

            Parent.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Raises the ColumnWidthChanging event.
        /// Suppresses changes of the width for the first column.
        /// </summary>
        ///
        protected override void OnColumnWidthChanging( ColumnWidthChangingEventArgs e )
        {
            if ( e.ColumnIndex == 0 )
            {
                e.Cancel = true;
            }

            base.OnColumnWidthChanging( e );
        }

        /// <summary>
        /// Raises the ColumnWidthChanged event.
        /// If the width of the column 0 was changed, sets its width back to 0.
        /// </summary>
        /// 
        protected override void OnColumnWidthChanged( ColumnWidthChangedEventArgs e )
        {
            if ( e.ColumnIndex == 0 && base.Columns.Count > 0 
                && base.Columns[0].Width != 0 )
            {
                base.Columns[0].Width = 0;
                return;
            }

            base.OnColumnWidthChanged( e );
        }

        /// <summary>
        /// Raises the ItemSelectionChanged event.
        /// Sets the current item.
        /// </summary>
        ///
        protected override void OnItemSelectionChanged(
            ListViewItemSelectionChangedEventArgs e )
        {
            this.currentItem = e.ItemIndex;

            base.OnItemSelectionChanged( e );
        }

        /// <summary>
        /// Raises the irtualItemsSelectionRangeChanged event.
        /// Resets the current item.
        /// </summary>
        ///
        protected override void OnVirtualItemsSelectionRangeChanged(
            ListViewVirtualItemsSelectionRangeChangedEventArgs e )
        {
            if ( this.currentItem >= e.StartIndex && this.currentItem <= e.EndIndex )
            {
                if ( ! e.IsSelected )
                {
                    this.currentItem = -1;
                }
            }

            base.OnVirtualItemsSelectionRangeChanged( e );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ MyListView.Updater Class ]

        /// <summary>
        /// Represents an updater used to suppress updating process of the control.
        /// </summary>
        /// </remarks>
        /// The Updater class is meant to be used instead of error ListView's error
        /// prone BeginUpdate/EndUpdate methods. The Updater class implements
        /// IDisposable and it should be instantiated inside using-statement.
        /// </remarkds>
        /// 
        public class Updater : IDisposable
        {
            private MyListView listView;

            /// <summary>
            /// Creates a new instance of the Updater for a specified MyListView object
            /// and begins update on the list view.
            /// </summary>
            /// 
            public Updater( MyListView listView )
            {
                this.listView = listView;
                if ( this.listView != null )
                {
                    this.listView.BeginUpdate ();
                }
            }

            /// <summary>
            /// Ends updates on MyListView object.
            /// </summary>
            /// 
            public void Dispose ()
            {
                if ( this.listView != null )
                {
                    this.listView.EndUpdate ();
                    this.listView = null;
                }
            }

            /// <summary>
            /// Temporarily ends updates and refreshes associated list view.
            /// </summary>
            /// 
            public void Refresh ()
            {
                if ( this.listView != null )
                {
                    this.listView.EndUpdate ();
                    Application.DoEvents ();
                    this.listView.BeginUpdate ();
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ MyListView.ItemSorter Class ]

        /// <summary>
        /// Represents TaggedText comparer used to sort TaggedText items by column.
        /// </summary>
        /// 
        private class ItemSorter : IComparer<TaggedText>
        {
            public int Column { get; private set; }
            public SortOrder Sorting { get; private set; }

            public ItemSorter( int column, SortOrder sortOrder = SortOrder.Ascending )
            {
                this.Sorting = sortOrder;
                this.Column = column;
            }

            public int Compare( TaggedText x, TaggedText y )
            {
                if ( Column < 1 || Column >= x.Fields.Length + 1 )
                {
                    return 0;
                }

                int result = StringLogicalComparer.Compare( 
                    x.Fields[ Column - 1 ], y.Fields[ Column - 1 ] );

                // If the sort order is descending then return inverted
                // value that was returned by String.Compare.
                //
                return Sorting == SortOrder.Descending ? -result : result;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }

#endif
