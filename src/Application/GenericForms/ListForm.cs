/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       ListForm.cs
 *  Created:    2011-04-03
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Mbk.Commons;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
    using VideoRentalOutlet_GUI.Properties;
#endif

using VROLib;

/// <summary>
/// Represents a base form class from which all list forms (displaying collections)
/// are derived in this application.
/// </summary>
/// 
internal class ListForm<TC,T> : FormBase, IListSource
    where T:  VROLib.GenericObject
    where TC: VROLib.MonitoredObject, IList<T>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Fields and Properties ]

    /// <summary>
    /// Gets or sets current TaggedText collection of cached database records.
    /// </summary>
    /// <remarks>
    /// The Records cache holds database records with their fields converted to strings
    /// and placed as columns/fields in TaggedText, where Text property of tagged-text
    /// contains single line with all columns concatenated. Holding a cache (syncrhonized
    /// with database via Changed event handler) speeds up displaying and searching
    /// of collection in MyListView. It also provides efficient way to dynamically
    /// filter records, as Records contains always all records while MyListView 
    /// contains only references to filtered records.
    /// </remarks>
    /// 
    private TaggedTextCollection Records { get; set; }

    /// <summary>
    /// Gets or sets the database collection from which the form gets data records.
    /// </summary>
    /// 
    private TC Collection { get; set; }

    /// <summary>
    /// Gets or sets delegate function that converts database record T into
    /// a string array (columns) of values.
    /// </summary>
    /// 
    private Func<T,string[]> RecordToString { get; set; }

    /// <summary>
    /// Gets or sets the MyListView control used to display database records.
    /// </summary>
    /// 
    protected MyListView ListView { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Records"/> tagged text 
    /// collection should be fully reloaded when new records are added to the 
    /// database <see cref="Collection"/>.
    /// </summary>
    /// 
    protected bool ReloadOnAddNewRecord { get; set; }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets a value indicating whether search panel is visible.
    /// </summary>
    /// 
    private bool IsSearchMode
    {
        get
        {
            #if TEXTUI
                return Filter.IsInApplicationFocus;
            #else
                return Filter.Parent != null 
                     ? Filter.Parent.Visible
                     : false;
            #endif
        }
    }

    /// <summary>
    /// Gets or sets search filter text box (holding the current search filter text
    /// that may be modified by the user).
    /// </summary>
    /// 
    private MyTextBox Filter { get; set; }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the collection of all column headers that appear in the control.
    /// </summary>
    /// 
    protected TextFieldCollection Columns { get; set; }

    /// <summary>
    /// Gets the collection of footer rows that appear in the control.
    /// </summary>
    /// <remarks>
    /// Applicable to TextUI mode only. In GUI mode, status bar is used instead.
    /// </remarks>
    /// 
    protected string[] Footer { get; set; }

    /// <summary>
    /// Gets or sets the caption for the single database record (vs the caption
    /// of the form that describes a collection of records).
    /// </summary>
    /// <remarks>
    /// If the form's Caption is e.g. "Customers" or "Customer Database" then 
    /// the CaptionSingular property should be "Customer".
    /// </remarks>
    /// 
    protected string CaptionSingular { get; set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets the instance of the DetailsForm displaying (or editing) the current record.
    /// </summary>
    /// 
    public DetailsForm<TC,T> DetailsForm { get; private set; }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the numer of cached database records.
    /// </summary>
    /// 
    public int RecordCount 
    {
        get { return Records == null ? 0 : Records.Count; }
    }

    /// <summary>
    /// Gets the current record (i.e. selected item in the list view).
    /// </summary>
    /// 
    public T CurrentRecord
    {
        get { return ListView.CurrentTag as T; }
    }

    /// <summary>
    /// Gets a value indicating whether the form is active and the current record
    /// is not null (i.e. there is selected item in the list view).
    /// </summary>
    /// 
    public override bool IsActive
    {
        get { return base.IsActive && CurrentRecord != null; }
    }

    /// <summary>
    /// Gets a value indicating whether the form is active and have current record
    /// or there is a DetailsForm with enabled Delete button.
    /// </summary>
    /// 
    public bool EnabledDeleteButton
    {
        get { return IsActive 
                  || ( DetailsForm != null && DetailsForm.EnabledDeleteButton ); }
    }

    /// <summary>
    /// Gets a value indicating whether the form is active and have current record
    /// or there is a DetailsForm with enabled Link button.
    /// </summary>
    /// 
    public bool EnabledLinkButton
    {
        get { return IsActive 
                  || ( DetailsForm != null && DetailsForm.EnabledLinkButton ); }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the common filter applyed to database collection <em>before</em>
    /// fetching records into <see cref="Records"/> cache.
    /// </summary>
    /// 
    public Predicate<T> CommonFilter
    {
        get
        {
            return this.commonFilter;
        }
        set
        {
            Records = null; // Force refresh of all records
            this.commonFilter = value;
        }
    }

    private Predicate<T> commonFilter;

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the search filter applyed to <see cref="Records"/> cache.
    /// (Filtered records are displayed in the list view.)
    /// </summary>
    /// 
    public string SearchFilter
    {
        get
        {
            return Filter != null ? Filter.Text : null;
        }
        private set
        {
            if ( Filter != null )
            {
                Filter.Text = value;
            }

            // As in TextUI mode TextChanged is not raised when Text is set,
            // we must do that manually.
            //
            if ( Em.IsTextUI )
            {
                ApplyFilter( SearchFilter );
                OnUiStateChanged ();
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets action performed when the user selects the current record.
    /// </summary>
    /// <remarks>
    /// Setting OnSelected delegate marks the form as modal (dialog).
    /// </remarks>
    /// 
    public Action<T> OnSelected { get; set; }

    /// <summary>
    /// Gets a value indicating whether the form is running modal (stand-alone dialog).
    /// </summary>
    /// 
    protected override bool IsModal
    {
        get { return OnSelected != null; }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Protected Constructor ]

    /// <summary>
    /// Creates a new instance of the ListForm class with caption, caption in singular
    /// and dimensions.
    /// </summary>
    /// <remarks>
    /// Note that protected constructor forbidds instantiation of the ListForm objects
    /// outside the derived classes ListForm.
    /// </remarks>
    /// 
    protected ListForm( string caption, string captionInSingular, 
            int width, int height )
        : base( caption, width, height )
    {
        this.Footer               = null;
        this.CaptionSingular      = captionInSingular;
        this.Collection           = null;
        this.Records              = new TaggedTextCollection ();
        this.ReloadOnAddNewRecord = false;
        this.CommonFilter         = null;
        this.Columns              = new TextFieldCollection ();
        this.DetailsForm          = null;

        /////////////////////////////////////////////////////////////////////////////////

        InitializeMdiForm ();
        InitializeListView ();
        InitializeSearchPanel ();
        InitializeContextMenu ();
        InitializeToolStrip ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Components Initialization ]

    /// <summary>
    /// Initializes the MdiForm.
    /// </summary>
    /// <remarks>
    /// Adds key down event handler to turn on search mode on Ctrl+F
    /// </remarks>
    /// 
    private void InitializeMdiForm ()
    {
        MdiForm.Caption = "List Form";

        MdiForm.KeyDown += ( sender, e ) =>
        {
            if ( e.KeyCode == Keys.F && e.Control )
            {
                TurnOnSearchMode ();
                UpdateFormFooter ();
                e.Handled = true;
            }
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the ListView property.
    /// </summary>
    /// 
    private void InitializeListView ()
    {
        this.ListView = new MyListView () 
        {
            Caption = this.Caption, TabStop = true,
            Parent = MdiForm, 
        };

        this.ListView.KeyDown += ( sender, e ) =>
        {
            switch( e.KeyCode )
            {
                case Keys.Enter:
                    DisplaySelectedItem ();
                    e.Handled = true;
                    break;

                case Keys.Insert:
                    AddNewRecord ();
                    e.Handled = true;
                    break;

                case Keys.Back:
                case Keys.Delete:
                    RemoveCurrentRecord_IfNotReadOnly ();
                    e.Handled = true;
                    break;
            }
        };

        this.UiStateChanged += delegate
        {
            UpdateFormFooter ();
        };

#if TEXTUI
        ListView.Maximize ();

        // ListBox fully covers form's client area, so the form doesn't have border
        // and doesn't need to be erased.
        //
        MdiForm.Border = false;
        MdiForm.OwnErase = true;
#else
        this.ListView.Dock = DockStyle.Fill;
        this.ListView.MouseDoubleClick += ( sender, e ) => DisplaySelectedItem ();

        this.MdiForm.FontChanged += ( sender, e ) => ListView.FitContents ();
        this.MdiForm.Resize      += ( sender, e ) => ListView.FitContents ();
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the context menu for the MdiForm.
    /// </summary>
    /// 
    private void InitializeContextMenu ()
    {
#if TEXTUI
        // Conetext menu doesn't exist in TextUi mode
#else
        ContextMenuStrip contextMenu = new ContextMenuStrip ();

        //-------------------------------------------------------------------------------
        ToolStripMenuItem miOpenRecord = new ToolStripMenuItem ()
        {
            Text = "&Open",
            Image = Resources.Import16,
        };

        ToolStripMenuItem miSearchRecords = new ToolStripMenuItem ()
        {
            Text = "&Find " + this.CaptionSingular,
            Image = Resources.Search16,
        };

        ToolStripMenuItem miDeleteRecord = new ToolStripMenuItem ()
        {
            Text = "&Delete " + CaptionSingular,
            Image = Resources.Delete16,
        };

        ToolStripMenuItem miAddNewRecord = new ToolStripMenuItem ()
        {
            Text = "&Add New " + CaptionSingular + "...",
            Image = Resources.DocumentAdd16,
        };

        ToolStripMenuItem miLockUnlock = new ToolStripMenuItem ()
        {
            Text = "Locked/Unlocked",
        };

        //-------------------------------------------------------------------------------
        miOpenRecord   .Click += ( sender, e ) => DisplaySelectedItem ();
        miSearchRecords.Click += ( sender, e ) => TurnOnSearchMode ();
        miAddNewRecord .Click += ( sender, e ) => OpenDetailsForm( null );
        miDeleteRecord .Click += ( sender, e ) => RemoveRecord( Collection, CurrentRecord );
        miLockUnlock   .Click += ( sender, e ) => ToggleReadOnlyMode ();

        //-------------------------------------------------------------------------------
        contextMenu.Opening += ( sender, e ) =>
        {
            miOpenRecord.Enabled = CurrentRecord != null;
            miOpenRecord.Text = IsModal ? "&Select" : ReadOnly ? "&View" : "&Edit";
            miOpenRecord.Image = IsModal ? Resources.Return16 : Resources.Import16;

            miDeleteRecord.Enabled = ! ReadOnly && CurrentRecord != null;

            miLockUnlock.Text = ReadOnly ? "Un&lock" : "&Lock";
            miLockUnlock.Image = ReadOnly ? Resources.LockUnlock16
                : Resources.LockLock16;
        };

        //-------------------------------------------------------------------------------

        contextMenu.Items.Add( miOpenRecord );
        contextMenu.Items.Add( miSearchRecords );
        
        if ( ! IsModal )
        {
            contextMenu.Items.Add( new ToolStripSeparator () );
            contextMenu.Items.Add( miDeleteRecord );
            contextMenu.Items.Add( miAddNewRecord );
            contextMenu.Items.Add( new ToolStripSeparator () );
            contextMenu.Items.Add( miLockUnlock );
        }

        this.ListView.ContextMenuStrip = contextMenu;
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the tool strip bar for the MdiForm.
    /// </summary>
    /// 
    private void InitializeToolStrip ()
    {
#if TEXTUI
        // Conetext menu doesn't exist in TextUi mode
#else
        ToolStrip toolStrip = new ToolStrip ()
        {
            Dock = DockStyle.Top, 
        };

        //-------------------------------------------------------------------------------
        ToolStripMenuItem miOpenRecord = new ToolStripMenuItem ()
        {
            Text = IsModal ? "Select" : ReadOnly ? "&View" : "&Edit",
            Image = IsModal ? Resources.Return16 : Resources.Import16,
            ToolTipText = "Open " + CaptionSingular,
        };

        ToolStripMenuItem miSearchRecords = new ToolStripMenuItem ()
        {
            Text = "&Find",
            Image = Resources.Search16,
            ToolTipText = "Find " + CaptionSingular,
        };

        ToolStripMenuItem miDeleteRecord = new ToolStripMenuItem ()
        {
            Text = "&Delete",
            Image = Resources.Delete16,
            ToolTipText = "Delete " + CaptionSingular,
        };

        ToolStripMenuItem miAddNewRecord = new ToolStripMenuItem ()
        {
            Text = "&New " + CaptionSingular,
            Image = Resources.DocumentAdd16,
            ToolTipText = "Add New " + CaptionSingular,
        };

        ToolStripMenuItem miLockUnlock = new ToolStripMenuItem ()
        {
            Text = "Un&lock",
            Image = Resources.Edit16,
            ToolTipText = "Lock/Unlock collection for editing...",
            Alignment = ToolStripItemAlignment.Right
        };

        //-------------------------------------------------------------------------------
        miOpenRecord   .Click += ( sender, e ) => DisplaySelectedItem ();
        miSearchRecords.Click += ( sender, e ) => TurnOnSearchMode( ! IsSearchMode );
        miDeleteRecord .Click += ( sender, e ) => RemoveRecord( Collection, CurrentRecord );
        miAddNewRecord .Click += ( sender, e ) => OpenDetailsForm( null );

        miLockUnlock.Click += delegate
        {
            ToggleReadOnlyMode ();
        };
/*
        ListView.SelectedIndexChanged += delegate
        {
            OnUiStateChanged ();
        };
*/
        ListView.ItemSelectionChanged += delegate
        {
            OnUiStateChanged ();
        };

        ListView.VirtualItemsSelectionRangeChanged += delegate
        {
            OnUiStateChanged ();
        };

        //-------------------------------------------------------------------------------

        toolStrip.Items.Add( miOpenRecord );
        toolStrip.Items.Add( miSearchRecords );

        if ( ! IsModal )
        {
            toolStrip.Items.Add( miAddNewRecord );
            toolStrip.Items.Add( miDeleteRecord );
            toolStrip.Items.Add( miLockUnlock );
        }

        this.MdiForm.ToolStrip = toolStrip;

        //-------------------------------------------------------------------------------

        this.UiStateChanged += delegate
        {
            miOpenRecord.Text = IsModal ? "Select" : ReadOnly ? "&View" : "&Edit";
            miOpenRecord.Image = IsModal ? Resources.Return16 : Resources.Import16;
            miOpenRecord.Enabled = CurrentRecord != null;

            miLockUnlock.Text = ReadOnly ? "Un&lock" : "&Lock";
            miLockUnlock.Image = ReadOnly ? Resources.Edit16 : Resources.LockEdit16;
            miDeleteRecord.Enabled = ! ReadOnly && CurrentRecord != null;
        };
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes the search panel for the MdiForm.
    /// </summary>
    /// 
    private void InitializeSearchPanel ()
    {
#if TEXTUI
        MyLabel filterLabel = new MyLabel ()
        {
            Left = 1, Top = 1, 
            AutoSize = true,
            Text = " Search " + Caption + ": ", 
            ForeColorInact = Color.White,
            Parent = MdiForm, 
        };

        Filter = new MyTextBox ()
        {
            Parent = MdiForm, 
            Left = filterLabel.Left + filterLabel.Width, Top = 1, 
            Width = MdiForm.Width - 2 - filterLabel.Width, Height = 1,
            Multiline = false, TabStop = false, Border = false,
            BackColor = Color.DarkMagenta, ForeColor = Color.White, 
            ToolTipText = "You may use Space to delimit words...",
            ForeColorInact = Color.Gray,
        };

        Filter.KeyDown += ( sender, e ) =>
        {
            switch ( e.KeyCode )
            {
                case Keys.Escape:

                    if ( ! string.IsNullOrEmpty( SearchFilter ) 
                           && ListView.ItemCount == 0 )
                    {
                        SearchFilter = null;
                    }

                    ListView.Focus ();
                    e.StopHandling ();

                    break;
            }
        };
#else
        Panel searchPanel = new Panel ()
        {
            Dock = DockStyle.Top, Height = Em.Height * 25/10,
            Padding = new Padding( Em.Width, Em.Height / 2, Em.Width, Em.Height / 2 ),
            Visible = false
        };

        Filter = new MyTextBox ()
        {
            Parent = searchPanel, Dock = DockStyle.Fill,
            Multiline = false, TabStop = true, Border = false,
            ToolTipText = "You may use '#' to delimit columns:\n"
                + "E.g. #15# will search column matching 15.\n"
                + "Note that search is case insensitive.",
            MinimumSize = new Size( 0, Em.Height ),
        };

        MyLabel filterLabel = new MyLabel ()
        {
            Parent = searchPanel, Dock = DockStyle.Left, 
            AutoSize = true, Text = "Search " + Caption + ": ", 
            TextAlign = ContentAlignment.MiddleLeft,
        };

        MdiForm.Controls.Add( searchPanel );

        filterLabel.MinimumSize = new Size( 0, Filter.Height + 2 );

        MdiForm.ToolTip.SetToolTip( Filter, Filter.ToolTipText );
        MdiForm.ToolTip.SetToolTip( filterLabel, Filter.ToolTipText );

        bool filterToolTipNotShown = true;

        Filter.Click += delegate
        {
            Filter.SelectAll ();
        };

        Filter.Enter += delegate
        {
            Filter.SelectAll ();

            if ( filterToolTipNotShown ) // show tool tip only once
            {
                filterToolTipNotShown = false;
                MdiForm.ToolTip.Show( Filter.ToolTipText, Filter, 2000 );
            }
        };
#endif

        Filter.TextChanged += ( sender, e ) =>
        {
            ApplyFilter( SearchFilter );
            OnUiStateChanged ();
        };
    }

    /// <summary>
    /// Sets the search filter visibility according specified value.
    /// </summary>
    /// 
    private void ShowSearchPanel( bool visible )
    {
        // In GUI mode, Filter is part of the Panel (while in TextUI
        // mode the Filter text box is bellow ListView in Z-order and automatically
        // hidden if it is not in focus).
        //
        if ( Em.IsGUI && Filter.Parent != null )
        {
            Filter.Parent.Visible = visible;
        }

        if ( visible )
        {
            Filter.Select  ();
        }
        else
        {
            ListView.Select ();
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Collection Changed Event Handler ]

    /// <summary>
    /// Sinks Changed events from <see cref="Collection"/>.
    /// </summary>
    /// <remarks>
    /// The event handler is notifed when collection changes (with some new records 
    /// added to the  colleciton, some records being deleted or contents of some records
    /// being modified).
    /// </remarks>
    /// 
    private void Collection_Changed( GenericObject item, ChangeType how, string reason )
    {
        // Ignore changs when invisible. When ListForm becomes visible it should
        // reload data anyway...
        //
        if ( MdiForm.Parent == null )
        {
            return;
        }

        Debug.TraceLine( "{0} >>>> {1} {2}{3}", TraceID, how, item.GetType().Name,
            reason != null ? ", Reason: " + reason : "" );

        T record = item as T;

        if ( record == null )
        {
            return;
        }

        using ( MyListView.Updater updater = ListView.GetUpdater () )
        {
            if ( how == ChangeType.Removed )
            {
                int pos = Records.FindIndex( tt => tt.Tag == record );
                if ( pos >= 0 )
                {
                    Records.RemoveAt( pos );
                }

                ListView.RemoveItemWithTag( record );

                OnUiStateChanged ();
            }
            else if ( how == ChangeType.Added && ReloadOnAddNewRecord ) // Add new
            {
                Records = null; // clear records cache to force reloading

                // Reload records if current record is not found and ReloadOnAddNewRecord
                // is turned on. This case is usefull when our monitored collection is 
                // sorted and we would like to preserve sorting order.
                //
                OnLoadData ();
                ApplyFilter( SearchFilter );
                OnUiStateChanged ();

                MainForm.InfoMessage = "Reloaded " + Caption + "...";
            }
            else // add new or reload
            {
                string[] fields = RecordToString( record );
                string newText = Columns.Format( fields );
                TaggedText t = new TaggedText( newText, record, fields );

                int pos = Records.FindIndex( tt => tt.Tag == record );

                if ( pos >= 0 )
                {
                    Records[ pos ] = t;
                    ListView.UpdateItemWithTag( t );
                }
                else // Add new record.
                {
                    if ( CommonFilter == null || CommonFilter( record ) )
                    {
                        // This code adds item at the end of the list. This may cause a 
                        // problem if the list should be sorted or if it was filtered.
                        //
                        Records.Add( t );
                        int index = ListView.Append( t );
                        ListView.SelectItem( index );
                    }
                }
            }
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ DetailsForm Functionallity ]

    /// <summary>
    /// Opens the DetailsForm in AddNew record mode.
    /// </summary>
    /// 
    private void AddNewRecord ()
    {
        if ( ! ReadOnly )
        {
            OpenDetailsForm( null );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens the DetailsForm for the current record.
    /// </summary>
    /// <remarks>
    /// If current record is not specifified, opens the DetaislForm in 
    /// AddNew record mode.
    /// </remarks>
    /// 
    private bool OpenDetailsForm( T record )
    {
        bool ok = false;

        MdiForm.IfValidateOk( () =>
        {
            if ( Em.IsGUI && DetailsForm != null && record == DetailsForm.Record )
            {
                DetailsForm.MdiForm.Focus ();
                ok = true;
                return;
            }

            DetailsForm<TC,T> form = OnCreateDetailsForm ();

            if ( form != null )
            {
                form.MasterRecord = null;
                form.Record = record;

                OnBeforeOpenDetailsForm( form );

                form.OpenForm( record == null ? OpenMode.Edit 
                    : ReadOnly ? OpenMode.Browse : OpenMode.Edit );


                DetailsForm = form;

                ok = true;
            }
        } );

        return ok;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called before the DetailsForm is open.
    /// </summary>
    /// <remarks>
    /// Does nothing in the ListForm class; should be overriden in derived classes.
    /// </remarks>
    /// 
    protected virtual void OnBeforeOpenDetailsForm( DetailsForm<TC,T> form )
    {
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens details form for the current item.
    /// </summary>
    /// 
    private void DisplaySelectedItem ()
    {
        T record = CurrentRecord;
        if ( record == null )
        {
            return;
        }

        if ( IsModal )
        {
            OnSelected( record );
            return;
        }

        if ( ! OpenDetailsForm( record ) )
        {
            MessageBox.Show( "\n" + record.ToString (), CaptionSingular + " Details", 
                MessageBoxButtons.OK, MessageBoxIcon.Information );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of DetailsForm class.
    /// </summary>
    /// <remarks>
    /// Add event handlers to the instance of DetailsForm so it can navigate through 
    /// the records from this form.
    /// </remarks>
    /// 
    private DetailsForm<TC,T> OnCreateDetailsForm ()
    {
        if ( DetailsForm != null && Em.IsTextUI )
        {
            return DetailsForm; // Don't allow multiple details forms in TextUI mode
        }

        DetailsForm<TC,T> form = CreateDetailsForm ();

        if ( form == null )
        {
            return null;
        }

        Debug.TraceLine( "{0} Created {1}", TraceID, form.TraceID );

        /////////////////////////////////////////////////////////////////////////////////

        form.MdiForm.FormClosed += ( source, e ) =>
        {
            // When user quits form, clear also ListForm's reference to it.
            //
            if ( form != null )
            {
                Debug.TraceLine( "{0} Closed {1}", TraceID, form.TraceID );

                if ( this.DetailsForm == form )
                {
                    this.DetailsForm = null;
                }
            }
        };

        /////////////////////////////////////////////////////////////////////////////////

        // We need to take a snapshot of referenced collection so we could
        // remove record from collection when this.Collection becomes null.
        // (this.Collection becomes null when ListForm is closed)

        TC collectionSnapshot = Collection;

        // Form should use our (ListForm's) methods to remove records,
        // because details forms collection is connected to MonitoredObject.
        // ListForm's collection is connected to both MonitoredObject and IList
        // interface, which enables ListForm to remove objects contrary to 
        // form. Thy is not form's collection is not IList? -- well,
        // because form may be connected to the *single* object (like
        // VideoRentalOutlet database itself).
        //
        form.DeleteClick += delegate
        {
            form.MdiForm.IfValidateOk( () =>
            {
                if ( form != null )
                {
                    form.ExecuteWithoutFormValidation( () =>
                    {
                        this.RemoveRecord( collectionSnapshot, form.Record );
                    } );
                }
            } );
        };

        // Add navigation keys (page-up, down etc) and Ctrl+F to details form
        //
        form.MdiForm.KeyDown += ( sender, e ) =>
        {
            // Note: this.Collection becomes null when ListForm is closed

            if ( form != null && this.Collection != null )
            {
                // Respond to PageUp/Down etc if not adding new record
                //
                if ( ! form.IsAddNew )
                {
                    this.NavigateByKeyDown( form, e );
                }

                // Search records on Ctrl+F
                //
                if ( e.KeyCode == Keys.F  && e.Control )
                {
                    form.MdiForm.IfValidateOk( () =>
                    {
                        this.TurnOnSearchMode ();
                    } );
                    e.Handled = true;
                }
            }
        };

        // Respond to mouse wheel (in details form in GUI mode) by changing current
        // record (in this list form).
        //
        #if ! TEXTUI
        form.MdiForm.MouseWheel += ( sender, e ) => 
        {
            // Note: this.Collection becomes null when ListForm is closed

            if ( form != null && this.Collection != null )
            {
                if ( ! form.IsDirty () && ! form.IsAddNew )
                {
                    this.NavigateInListItems( form, () =>
                    {
                        ListView.CurrentItem += e.Delta < 0 ? +1 : -1;
                    } );
                }
            }
        };
        #endif

        /////////////////////////////////////////////////////////////////////////////////

        return form;
    }

    /// <summary>
    /// Navigates through the ListView records depending on key in KeyEventArgs.
    /// </summary>
    /// <remarks>
    /// Handles PageUp, PageDown, Ctrl+Home and Ctrl+End keys.
    /// </remarks>
    /// 
    private void NavigateByKeyDown( DetailsForm<TC,T> form, KeyEventArgs e )
    {
        // Don't navigate through records when the focus is in a multi-line text-box
        // as it will interfere with navigation between pages in the text box.
        //
        if ( Em.IsTextUI && form.MdiForm.ActiveChild != null )
        {
            MyTextBox textBox = form.MdiForm.ActiveChild as MyTextBox;
            if ( textBox != null && textBox.Multiline )
            {
                return;
            }
        }

        // Record Navigation: 
        // Page Up/Down == previous/next record
        // Ctrl+Home/End == first/last record.
        //
        if ( e.KeyCode == Keys.PageDown && e.Modifiers == 0 )
        {
            // Next record on PageDown
            //
            this.NavigateInListItems( form, () =>
            {
                ++ListView.CurrentItem;
            } );
            e.Handled = true;
        }
        else if ( e.KeyCode == Keys.End && e.Control )
        {
            // Last record on Ctrl+Home
            //
            this.NavigateInListItems( form, () =>
            {
                ListView.CurrentItem = int.MaxValue/2;
            } );
            e.Handled = true;
        }
        else if ( e.KeyCode == Keys.PageUp && e.Modifiers == 0 )
        {
            // Previous record on PageUp
            //
            this.NavigateInListItems( form, () =>
            {
                --ListView.CurrentItem;
            } );
            e.Handled = true;
        }
        else if ( e.KeyCode == Keys.Home && e.Control )
        {
            // First record on Ctrl+Home
            //
            this.NavigateInListItems( form, () =>
            {
                ListView.CurrentItem = 0;
            } );
            e.Handled = true;
        }
    }

    /// <summary>
    /// Executes action that changes the current item in the ListView.
    /// </summary>
    /// <remarks>
    /// Reloads the contents of the existing DetailsForm (if any) after the current
    /// record changes.
    /// </remarks>
    /// 
    private void NavigateInListItems( DetailsForm<TC,T> form,
        Action actionThatSetsCurrentRow )
    {
        if ( form == null )
        {
            return;
        }

        form.MdiForm.IfValidateOk( () =>
        {
            T record = form.Record;

            if ( record == null )
            {
                return;
            }

            int position = ListView.FindTag( record );

            if ( position >= 0 )
            {
                ListView.SelectItem( position );
            }

            if ( actionThatSetsCurrentRow != null )
            {
                try { actionThatSetsCurrentRow (); } catch {}
            }

            if ( record == CurrentRecord )
            {
                return;
            }

            record = CurrentRecord;

            if ( record != null )
            {
                form.MasterRecord = null;
                form.Record = record;

                OnBeforeOpenDetailsForm( form );

                form.OpenForm( ReadOnly ? OpenMode.Browse : OpenMode.Edit );
            }
            else
            {
                form.QuitForm ();
            }
        } );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the caption, list view colors and status bar info, depending
    /// on the <see cref="ReadOnly"/> and <see cref="IsModal"/> values.
    /// </summary>
    /// 
    private void UpdateFormTitle ()
    {
        ListView.Caption = FormTitle;

        if ( IsModal )
        {
            #if TEXTUI
                ListView.CaptionForeColor = Color.Yellow;
            #else
                ListView.BackColor = Color.White;
                ListView.ForeColor = Color.Blue;
            #endif

            MdiForm.InfoMessage = "Select " + CaptionSingular + "...";
            SetStatusBarInfo( MdiForm.InfoMessage );
        }
        else if ( ReadOnly )
        {
            #if TEXTUI
                ListView.CaptionForeColor = Color.White;
            #else
                ListView.BackColor = Color.Ivory;
                ListView.ForeColor = SystemColors.ControlText;
            #endif

            SetStatusBarInfo( "Browsing " + Caption.ToLowerInvariant () 
                + "; Press F2 to enter edit mode..." );
        }
        else
        {
            #if TEXTUI
                ListView.CaptionForeColor = Color.Red;
            #else
                ListView.BackColor = SystemColors.ControlLightLight;
                ListView.ForeColor = Color.DarkBlue;
            #endif

            if ( ! string.IsNullOrEmpty( SearchFilter ) )
            {
                SetStatusBarInfo( "Editing " + Caption.ToLowerInvariant () 
                    + " matching '" + SearchFilter 
                    + "'; Press F2 to exit edit mode..." );
            }
            else
            {
                SetStatusBarInfo( "Editing " + CaptionSingular.ToLowerInvariant ()
                    + " collection; Press F2 to exit edit mode..." );
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates footer info displaying the number of records selected by the
    /// current filter.
    /// </summary>
    /// 
    private void UpdateFormFooter ()
    {
        string info = null;

        if ( ! ReadOnly && MdiForm.ActiveChild != Filter )
        {
            info = "INS = Add New, DEL = Delete, Enter = Edit, Ctrl+F = Search";
        }
        else if ( string.IsNullOrEmpty( SearchFilter ) )
        {
            if ( MdiForm.ActiveChild != Filter && ! IsModal )
            {
                info = "Enter = View " 
                     + CaptionSingular.ToLowerInvariant () + " details"
                     + ", Ctrl+F = Search";
            }
            else if ( MdiForm.ActiveChild != Filter ) // and is modal
            {
                info = null; // no footer
            }
            else // Search filter
            {
                info = string.Format( "{0} record{1}", 
                    ListView.ItemCount == 0 ? "No" : ListView.ItemCount.ToString(), 
                    ListView.ItemCount == 1 ? "" : "s" );
            }
        }
        else
        {
            info = string.Format( "{0} record{1} matching '{2}'", 
                ListView.ItemCount == 0 ? "No" : ListView.ItemCount.ToString (), 
                ListView.ItemCount == 1 ? "" : "s",
                SearchFilter );
        }

        #if TEXTUI

            if ( info == null )
            {
                ListView.Footer = null;
            }
            else
            {
                ListView.Footer = new string[]
                { 
                    ListBoxBase.WideDivider,
                    " " + info
                };
            }

        #else

            if ( ! string.IsNullOrEmpty( SearchFilter ) )
            {
                SetStatusBarInfo( info );
            }
            else
            {
                UpdateFormTitle ();
            }

        #endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the informational message to be displayed in the status bar.
    /// </summary>
    /// 
    private void SetStatusBarInfo( string info )
    {
        #if TEXTUI
            ListView.ToolTipText = info;
        #else
            MdiForm.ToolTipText = info;
            MdiForm.ErrorMessage = null;
        #endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Executes <see cref="RemoveRecord"/> if the form is not <see cref="ReadOnly"/>.
    /// </summary>
    /// 
    private void RemoveCurrentRecord_IfNotReadOnly ()
    {
        if ( ! ReadOnly )
        {
            int currentItem = ListView.CurrentItem;

            RemoveRecord( Collection, CurrentRecord );

            ListView.CurrentItem = currentItem;
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ IListSource Implementation ]

    /// <summary>
    /// Gets a value indicating whether the collection is a collection of IList objects.
    /// </summary>
    /// 
    public bool ContainsListCollection
    {
        get { return Collection is IList; }
    }

    /// <summary>
    /// Returns an IList that can be bound to a data source from an object that does 
    /// not implement an IList itself. 
    /// </summary>
    /// 
    public IList GetList ()
    {
        return Collection as IList;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// The factory method that creates the DetailsForm based.
    /// Should be overriden.
    /// </summary>
    ///
    public virtual DetailsForm<TC,T> CreateDetailsForm ()
    {
        return null;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns true if a specified record can be removed.
    /// Should be overriden.
    /// </summary>
    /// 
    public virtual bool CanRemoveRecord( T record )
    {
        return true;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Selects the current item in the ListView associated to a specified record.
    /// </summary>
    /// 
    public void SelectAsCurrentRecord( T record )
    {
        if ( record == null )
        {
            return;
        }

        int position = ListView.FindTag( record );

        if ( position >= 0 )
        {
            ListView.SelectItem( position );
        }
        else
        {
            TurnOnSearchMode ();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Turns on/off search mode (shows/hides search panel) according specified value.
    /// </summary>
    /// <remarks>
    /// When the search is turned off, the search filter is cleared.
    /// </remarks>
    /// 
    public void TurnOnSearchMode( bool turnOn = true )
    {
        if ( ! turnOn )
        {
            SearchFilter = null;
        }

        ShowSearchPanel( /*visible=*/ turnOn );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Populates the ListView items from the Records using a specified filter.
    /// </summary>
    /// <remarks>
    /// If the filter is not specified (or null), the ListView will be populated with 
    /// the all records found in the cache.
    /// </remarks>
    ///
    public void ApplyFilter( string filter = null )
    {
        if ( Records == null )
        {
            return;
        }

        if ( filter != null )
        {
            filter = filter.ToLowerInvariant ();
        }

        using ( MyListView.Updater updater = ListView.GetUpdater () )
        {
            ListView.ClearItems ();

            foreach( TaggedText t in Records )
            {
                if ( string.IsNullOrEmpty( filter ) 
                    || t.Text.ToLowerInvariant ().Contains( filter ) )
                {
                    ListView.Append( t );
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Removes a specified record from a specified collection.
    /// </summary>
    /// <remarkds>
    /// Checks whether it is allowed to remove records by calling 
    /// <see cref="CanRemoveRecord"/> first.
    /// </remarkds>
    /// 
    public bool RemoveRecord( TC collection, T record )
    {
        if ( collection == null )
        {
            MainForm.InfoMessage = Caption + " collection is not connected...";
            return false;
        }

        bool ok = false;

        if ( record != null && CanRemoveRecord( record ) )
        {
            try
            {
                ok = collection.Remove( record );
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Hand );
            }
        }

        return ok;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Delete Current Record ]

    /// <summary>
    /// Removes the current record either shown in the DetalsForm or selected in 
    /// the ListView (depending on whichform is active).
    /// </summary>
    /// <remarks>
    /// Raises the DeleteClick in DetailsForm, if the DetailsForm is open,
    /// or deletes current record.
    /// </remarks>
    /// 
    public void DeleteCurrentRecord ()
    {
        MainForm.MdiClient.IfValidateOk( () =>
        {
            if ( CurrentRecord != null 
                && DetailsForm != null && DetailsForm.Record != null )
            {
                if ( DetailsForm.IsActive )
                {
                    DetailsForm.RaiseDeleteClick ();
                }
                else
                {
                    MdiForm.Activate ();
                    RemoveRecord( Collection, CurrentRecord );
                }
            }
            else if ( CurrentRecord != null )
            {
                MdiForm.Activate ();
                RemoveRecord( Collection, CurrentRecord );
            }
            else if ( DetailsForm != null && DetailsForm.EnabledDeleteButton )
            {
                DetailsForm.MdiForm.Activate ();
                DetailsForm.RaiseDeleteClick ();
            }
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens form as modal, executing specified action when the current record
    /// is selected.
    /// </summary>
    /// 
    public void OpenModal( T record, Action<T> onSelected )
    {
        OnSelected = onSelected;

        #if TEXTUI

            MdiForm.ForwadKeysToParent = false; // makes form Modal
            OpenForm( OpenMode.Browse );
            SelectAsCurrentRecord( record );

        #else

            InitializeContextMenu ();
            InitializeToolStrip ();

            MdiForm.Load += delegate
            {
                OpenForm( OpenMode.Browse );
                SelectAsCurrentRecord( record );
            };

            MdiForm.StartPosition = FormStartPosition.CenterParent;
            MdiForm.ShowDialog ();

        #endif
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Edit Sub-Items (open sub-items form for current record) ]

    /// <summary>
    /// Opens new a sub-items form for the current record (e.g. Customer rental(s)
    /// for the current Customer in Customer List View).
    /// </summary>
    /// 
    public void OpenSubItemsForm( OpenMode mode )
    {
        MainForm.MdiClient.IfValidateOk( () =>
        {
            if ( CurrentRecord != null 
                && DetailsForm != null && DetailsForm.EnabledLinkButton )
            {
                if ( DetailsForm != null && DetailsForm.IsActive )
                {
                    OpenSubItemsForm( DetailsForm.Record, mode );
                }
                else
                {
                    MdiForm.Activate ();
                    OpenSubItemsForm( CurrentRecord, mode );
                }
            }
            else if ( CurrentRecord != null )
            {
                MdiForm.Activate ();
                OpenSubItemsForm( CurrentRecord, mode );
            }
            else if ( DetailsForm != null && DetailsForm.EnabledLinkButton )
            {
                DetailsForm.MdiForm.Activate ();
                OpenSubItemsForm( DetailsForm.Record, mode );
            }
        } );
    }

    /// <summary>
    /// Virtual method that opens sub-item form for specified record.
    /// Should be overriden for <see cref="OpenSubItemsForm"/> to function properly.
    /// </summary>
    /// 
    protected virtual void OpenSubItemsForm( T record, OpenMode mode )
    {
    }

    #endregion

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Subscribes the Changed event handler to a specified database collection.
    /// </summary>
    ///
    protected void ConnectTo( TC collection, Func<T,string[]> textInfoGetter )
    {
        if ( collection == Collection )
        {
            return; // already connected...
        }

        DisconnectFromCollection ();

        Collection = collection;
        RecordToString = textInfoGetter;

        if ( Collection != null )
        {
            Debug.TraceLine( "{0} Connected to: {1}", TraceID, Collection.ClassName );

            Collection.Changed += Collection_Changed;
        }
    }

    /// <summary>
    /// Unsubscribes the Changed event handler from currently subscribed database 
    /// collection.
    /// </summary>
    ///
    protected override void DisconnectFromCollection ()
    {
        if ( DetailsForm != null )
        {
            DetailsForm.QuitForm ();
            DetailsForm = null;
        }

        if ( Collection != null )
        {
            Debug.TraceLine( "{0} Disonnected: {1}", TraceID, Collection.ClassName );

            Collection.Changed -= Collection_Changed;

            Collection = null;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the caption for the form displaying form's read-only status and
    /// modality.
    /// </summary>
    /// 
    protected override string FormTitle 
    { 
        get
        {
            return IsModal  ? "Select a " + CaptionSingular + "..."
                 : ReadOnly ? Caption + " (read-only)"
                            : "Edit " + Caption;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens the form in specified mode (browsing, searching, editing or add new).
    /// </summary>
    ///
    public override void OpenForm( OpenMode mode )
    {
        if ( IsModal )
        {
            #if ! TEXTUI // Disable context menu strip in GUI mode
            MdiForm.ContextMenuStrip = null;
            #endif
        }

        SearchFilter = null;
        ShowSearchPanel( mode == OpenMode.Search );

        base.OpenForm( mode );

        if ( mode == OpenMode.AddNew )
        {
            AddNewRecord ();
        }
   }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Closes the form without asking to save (potentially) dirty data.
    /// </summary>
    /// 
    public override void QuitForm ()
    {
        if ( DetailsForm != null )
        {
            DetailsForm.QuitForm ();
            DetailsForm = null;
        }

        base.QuitForm ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes UI components of the MdiForm.
    /// </summary>
    /// <remarks>
    /// Creates the ListView member with header columns based on <see cref="Columns"/>.
    /// </remarks>
    /// 
    protected override void InitializeComponents ()
    {
        if ( ListView == null )
        {
            return;
        }

#if TEXTUI
        ListView.Header = new string[] 
        { 
            this.Columns.Header (), 
            ListBoxBase.ShortDivider 
        };
#else
        this.Columns.DontFormat = true;
        this.Columns.Separator = "#";

        using ( MyListView.Updater updater = new MyListView.Updater( ListView ) )
        {
            int totalWidth = 0;
            for ( int i = 0; i < this.Columns.Count; ++i )
            {
                var col = this.Columns[ i ];

                int width = Math.Abs( col.Width ) * Em.Width + 20;
                totalWidth += width;

                if ( i == this.Columns.Count - 1 )
                {
                    // set 'fill-rest' width for the last column
                    width = ListView.ClientSize.Width - totalWidth;
                }

                ListView.Columns.Add( new ColumnHeader ()
                {
                    Text      = col.Title, 
                    Width     = width,
                    TextAlign = col.Width <= 0 ? HorizontalAlignment.Left 
                              : HorizontalAlignment.Right,
                } );
            }
        }

#endif
        base.InitializeComponents();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the ReadOnly property of the form.
    /// </summary>
    /// <remarks>
    /// Updates form title and footer to reflect changes.
    /// </remarks>
    ///
    protected override void SetReadOnly( bool readOnly )
    {
        base.SetReadOnly( readOnly );

        UpdateFormTitle ();
        UpdateFormFooter ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the form data (record or records) should be loaded (refreshed)
    /// from connected VRO object collection.
    /// </summary>
    /// <remarks>
    /// Populates the <see cref="Reccors"/> and <see cref="ListView"/> items 
    /// from connected <see cref="Collection"/>.
    /// </remarks>
    /// 
    protected override void OnLoadData ()
    {
        // Don't (re)load data only if visible and have some records in cache.
        // When opening form, OnLoadData will be called *before* setting parent
        // to something, so if the parent was not null (and form already visible)
        // nothing should be reloaded as ListForm was respoding on-database-changed
        // events. See Collection_Changed() event handler.
        //
        if ( MdiForm.Parent != null && RecordCount > 0 )
        {
            return;
        }

        Records = new TaggedTextCollection ();

        using( MyListView.Updater updater = ListView.GetUpdater () )
        {
            ListView.ClearItems ();

            if ( Collection == null || RecordToString == null )
            {
                MainForm.InfoMessage = Caption + " collection is not connected...";
                return;
            }

            MdiForm.Refresh ();

            foreach( T record in Collection )
            {
                if ( CommonFilter != null && ! CommonFilter( record ) )
                {
                    continue;
                }

                string[] fields = RecordToString( record );

                TaggedText t = new TaggedText( Columns.Format( fields ), record, fields );
                Records.Add( t );
                ListView.Append( t );

                // Flush first page to screen so the user gets impression that something
                // is going on. Without this, if Collection is lengthy, the user
                // will stare at an anoying blank screen for a long(er) time.
                //
                if ( RecordCount == 2000 ) 
                {
                    updater.Refresh ();
                }
            }

            ListView.FitContents ();
        }

        ListView.SelectItem( 0 );

        MainForm.InfoMessage = "Loaded " + RecordCount + " " 
            + Caption.ToLowerInvariant () + "...";


        base.OnLoadData ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}