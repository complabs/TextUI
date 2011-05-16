/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       DetailsForm.cs
 *  Created:    2011-04-03
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.ComponentModel;

using Mbk.Commons;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
#endif

using VROLib;

/// <summary>
/// Represents a base form class from which all details forms (displaying single record
/// details) are derived in this application.
/// </summary>
/// 
internal class DetailsForm<TM,T> : FormBase
    where T:  VROLib.GenericObject
    where TM: VROLib.MonitoredObject
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Protected Properties and Private Fields ]

    /////////////////////////////////////////////////////////////////////////////////////

    // Buttons appearing at the bottom in the form

    protected MyButton Button_OK     { get; private set; }
    protected MyButton Button_Locker { get; private set; }
    protected MyButton Button_Delete { get; private set; }
    protected MyButton Button_Link   { get; private set; }
    protected MyButton Button_Info   { get; private set; }
    protected MyButton Button_Cancel { get; private set; }

    private MyButton[] buttonCollection; // array that will hold buttons listed above

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Horizontal flow layout panel with buttons (should be docked at the bottom above
    /// Status strip).
    /// </summary>
    /// 
    #if ! TEXTUI
    protected FlowLayoutPanel ButtonPanel { get; private set; }
    #endif

    /////////////////////////////////////////////////////////////////////////////////////

    // First field in the form (which should be in the focus when the form is open).
    //
    protected Control FirstField { get; set; }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets auto-incremented index tab for to be assigned to the next form's control.
    /// </summary>
    /// 
    protected int NextTabIndex
    {
        get { return this.tabIndex++; }
    }

    private int tabIndex = 0; // Holds tabIndex assigned to the last control

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the monitored database object (collection) from which
    /// this form may receive database Changed events.
    /// </summary>
    /// 
    private TM Collection { get; set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets database record tied to this form.
    /// </summary>
    /// 
    public T Record { get; set; }

    /// <summary>
    /// Gets or sets master database record owning the form's Record.
    /// </summary>
    /// 
    public object MasterRecord { get; set; }

    /// <summary>
    /// Gets a value indicating whether the form is in AddNew mode.
    /// </summary>
    /// <remarks>
    /// The form is in AddNew mode if the <see cref="Record"/> is null.
    /// </remarks>
    /// 
    public override bool IsAddNew
    {
        get { return Record == null; }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets a value indicating whether deleting records is enabled in the form.
    /// </summary>
    /// 
    public bool EnabledDeleteButton
    { 
        get { return IsActive && Record != null && Button_Delete.TabStop != false; }
    }

    /// <summary>
    /// Gets a value indicating whether link button (enabling navigation to other forms)
    /// is enabled in the form.
    /// </summary>
    /// 
    public bool EnabledLinkButton
    {
        get { return IsActive && Record != null && Button_Link.TabStop != false; }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Events ]

    /// <summary>
    /// Ocurrs when user clicks (or presses) Delete button.
    /// </summary>
    /// 
    public event EventHandler DeleteClick = null;

    /// <summary>
    /// Occurs when user clicks (or presses) Link button.
    /// </summary>
    public event EventHandler LinkClick = null;

    /// <summary>
    /// Occurs when user clicks (o presses) Info button.
    /// </summary>
    /// 
    public event EventHandler InfoClick = null;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Protected Constructor ]

    /// <summary>
    /// Creates a new instance of the DetailsForm class with a specified caption
    /// and dimensions.
    /// </summary>
    /// <remarks>
    /// Note that protected constructor forbidds instantiation of the DetailsForm objects
    /// outside the derived classes from DetailsForm.
    /// </remarks>
    /// 
    protected DetailsForm( string caption, int width, int height )
        : base ( caption, width, height )
    {
        this.Record = null;
        this.MasterRecord = null;
        this.Collection = null;

        /////////////////////////////////////////////////////////////////////////////////

        InitializeMdiForm ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /// <summary>
    /// Initializes the MdiForm.
    /// </summary>
    /// 
    private void InitializeMdiForm ()
    {
        MdiForm.Caption = "Details Form";

        if ( Em.IsGUI )
        {
            MdiForm.Width += 2 * Em.Width;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Occurs when VRO database collection has been changed.
    /// </summary>
    /// <remarks>
    /// Updates the contents of the form, in case that database record has been updated,
    /// or closes the form, in case that database record was deleted.
    /// </remarks>
    /// 
    private void Collection_Changed( GenericObject item, ChangeType how, string reason )
    {
        Debug.TraceLine( "{0} >>>> {1} {2}{3}", TraceID, how, item.GetType().Name,
            reason != null ? ", Reason: " + reason : "" );

        T record = item as T;

        if ( record != null && this.Record == record )
        {
            if ( how == ChangeType.Removed )
            {
                QuitForm ();
            }
            else // updated or added
            {
                // Reload data into details form. This will also clean ContentsChanged 
                // property for all fields in DetailsForm.
                //
                OnLoadData ();
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Selects the first control with the tab stop.
    /// </summary>
    /// 
    private void FocusOnFirstTabStop ()
    {
        if ( FirstField.TabStop )
        {
            FirstField.Focus ();
        }
        else
        {
            MdiForm.SelectNextControl( FirstField, /*forward*/ true,
                /*tabStopOnly*/ true, /*nested*/ true, /*wrap*/ true );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets the read-only property for the control (and its all nested controls)
    /// to a specified value.
    /// </summary>
    /// 
    private void SetReadOnly( Control c, bool readOnly )
    {
        foreach( Control child in c.Controls )
        {
            if ( child is MyGroupBox )
            {
                SetReadOnly( child, readOnly );
            }
            else if ( child is MyTextBox )
            {
                ( child as MyTextBox).ReadOnly = readOnly;
            }
            else if ( child is MyComboBox )
            {
                ( child as MyComboBox).ReadOnly = readOnly;
            }
            else if ( child is MyCheckBox )
            {
                ( child as MyCheckBox).ReadOnly = readOnly;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Layouts ButtonPanel.
    /// </summary>
    /// <remarks>
    /// In TextUI mode, dynamically layouts button panel horizontally.
    /// </remarks>
    /// 
    protected void LayoutButtonPanel ()
    {
#if TEXTUI
        int buttonLeft = 3 * Em.Width;
        int buttonTop = MdiForm.ClientSize.Height - Em.Height;

        foreach( var button in this.buttonCollection )
        {
            if ( button.TabStop )
            {
                button.Top  = buttonTop - button.Height;
                button.Left = buttonLeft;
                buttonLeft += button.TotalWidth + Em.Width;
            }
        }
#endif
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Raises the DeleteClick event.
    /// </summary>
    /// 
    public void RaiseDeleteClick ()
    {
        if ( DeleteClick != null )
        {
            DeleteClick( this, EventArgs.Empty );
        }
    }

    /// <summary>
    /// Raises the LinkClick event.
    /// </summary>
    /// 
    public void RaiseLinkClick ()
    {
        if ( LinkClick != null )
        {
            LinkClick( this, EventArgs.Empty );
        }
    }

    /// <summary>
    /// Raises the InfoClick event.
    /// </summary>
    /// 
    public void RaiseInfoClick ()
    {
        if ( InfoClick != null )
        {
            InfoClick( this, EventArgs.Empty );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Called when the form data record needs to be (re)loaded from database.
    /// </summary>
    /// <remarks>
    /// Updates caption of the MdiForm.
    /// </remarks>
    /// 
    protected override void OnLoadData ()
    {
        Debug.TraceLine( "{0} LoadData: {1}", TraceID, 
            IsAddNew ? "Add New" : Record.ToString () );

        MdiForm.Caption = FormTitle;

        base.OnLoadData ();
    }

    /// <summary>
    /// Called when the form data record needs to be saved to database.
    /// </summary>
    /// 
    protected override void OnSaveData ()
    {
        Debug.TraceLine( "{0} SaveData: {1}", TraceID, 
            IsAddNew ? "Add New" : Record.ToString () );

        base.OnSaveData ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes UI components of the DetailsForm.
    /// </summary>
    /// <remarks>
    /// Sets the FirstField control and creates ButtonPanel.
    /// </remarks>
    /// 
    protected override void InitializeComponents ()
    {
        FirstField = MdiForm.Children[ 0 ] as Control;

        foreach( var child in MdiForm.Children )
        {
            Control ctrl = child as Control;
            if ( ctrl != null && ctrl.TabStop )
            {
                FirstField = ctrl;
                break;
            }
        }

        FirstField.Focus ();

        /////////////////////////////////////////////////////////////////////////////////

        if ( Em.IsTextUI )
        {
            MdiForm.ForeColor = Color.DarkCyan;
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Buttons

        Button_OK     = new MyButton () { AutoSize = true, TabStop = false };
        Button_Locker = new MyButton () { AutoSize = true, TabStop = false };
        Button_Delete = new MyButton () { AutoSize = true, TabStop = false };
        Button_Link   = new MyButton () { AutoSize = true, TabStop = false };
        Button_Info   = new MyButton () { AutoSize = true, TabStop = false };
        Button_Cancel = new MyButton () { AutoSize = true, TabStop = false };

        /////////////////////////////////////////////////////////////////////////////////

        Button_OK.Click += delegate 
        {
            if ( ReadOnly ) QuitForm (); 
            else SaveAndLock ();
        };

        Button_Locker.Click += ( sender, e ) => ToggleReadOnlyMode ();
        Button_Delete.Click += ( sender, e ) => RaiseDeleteClick ();
        Button_Link  .Click += ( sender, e ) => RaiseLinkClick ();
        Button_Info  .Click += ( sender, e ) => RaiseInfoClick ();
        Button_Cancel.Click += ( sender, e ) => QuitForm ();

        /////////////////////////////////////////////////////////////////////////////////

        buttonCollection = new MyButton[] 
        {
            Button_OK, 
            Button_Locker,
            Button_Delete,
            Button_Link,
            Button_Info,
            Button_Cancel,
        };

        /////////////////////////////////////////////////////////////////////////////////

        #if TEXTUI

            MdiForm.DrawFrame( 1, MdiForm.Height - 4, MdiForm.Width - 2, 1 );

        #else

            MdiForm.Controls.Add( new MyLineSeparator () { Dock = DockStyle.Bottom } );

            ButtonPanel = new FlowLayoutPanel ()
            {
                Dock = DockStyle.Bottom, AutoSize = true,
                Padding = new Padding( 2 * Em.Width, Em.Height/2, 0, Em.Height/2 ),
                TabStop = false, TabIndex = 10000,
            };

            MdiForm.Controls.Add( ButtonPanel );

        #endif

        /////////////////////////////////////////////////////////////////////////////////

        base.InitializeComponents ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Subscribes the Changed event handler to a specified database collection.
    /// </summary>
    ///
    protected void ConnectTo( TM collection )
    {
        DisconnectFromCollection ();

        Collection = collection;

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
        if ( Collection != null )
        {
            Debug.TraceLine( "{0} Disonnected: {1}", TraceID, Collection.ClassName );

            Collection.Changed -= Collection_Changed;

            Collection = null;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens the form in specified mode (browsing, searching, editing or add new).
    /// </summary>
    ///
    public override void OpenForm( OpenMode mode )
    {
        if ( mode == OpenMode.AddNew )
        {
            Record = null;
        }

        MdiForm.SuspendLayout ();

        if ( IsAddNew )
        {
            mode = OpenMode.Edit; // Force edit mode if adding new records
        }

        base.OpenForm( mode );

        FocusOnFirstTabStop ();

        MdiForm.ResumeLayout ();
    }

    /// <summary>
    /// Gets the caption for the form composed from current read-only status and
    /// mode ("Editing", "Add New" etc) and string representation of the current
    /// record (e.g. "Customer #1: ..." ).
    /// </summary>
    /// 
    protected override string FormTitle 
    { 
        get
        {
            return IsAddNew ? "Add New " + Caption 
                 : ReadOnly ? Record.ToString () : "Edit " + Record.ToString ();
        }
    }

    /// <summary>
    /// Sets the ReadOnly property of the form.
    /// </summary>
    /// <remarks>
    /// Adjusts UI elements (buttons, caption, status info etc) to reflect a new value 
    /// of the read-only property.
    /// </remarks>
    ///
    protected override void SetReadOnly( bool readOnly )
    {
        for ( int i = 0; i < this.buttonCollection.Length; ++i )
        {
            this.buttonCollection[ i ].TabIndex = this.tabIndex + i;
        }

        if ( ! readOnly )
        {
            Button_OK    .Text = IsAddNew ? "&Insert" : "&Update";
            Button_Locker.Text = "&Lock";
            Button_Delete.Text = "&Delete";
            Button_Cancel.Text = "Cancel";

            Button_OK    .TabStop = true;
            Button_Locker.TabStop = ! IsAddNew;
            Button_Delete.TabStop = ! IsAddNew && DeleteClick != null;
            Button_Link  .TabStop = ! IsAddNew && LinkClick != null;
            Button_Info  .TabStop = ! IsAddNew && InfoClick != null;
            Button_Cancel.TabStop = true;
        }
        else
        {
            Button_OK    .Text = "Cl&ose";
            Button_Delete.Text = "Delete";
            Button_Locker.Text = "Un&lock";
            Button_Cancel.Text = "Cancel";

            Button_OK    .TabStop = true;
            Button_Locker.TabStop = true;
            Button_Delete.TabStop = false;
            Button_Link  .TabStop = ! IsAddNew && LinkClick != null;
            Button_Info  .TabStop = ! IsAddNew && InfoClick != null;
            Button_Cancel.TabStop = false;
        }

        // Setup visual appearance of visual components

        if ( readOnly )
        {
            MdiForm.ToolTipText = "Press F2 to enter edit mode or Escape to quit...";

            #if TEXTUI
            MdiForm.CaptionForeColor = Application.Theme.CaptionForeColor;
            #endif
        }
        else if ( IsAddNew )
        {
            MdiForm.ToolTipText = "Press F2 to exit edit mode or Escape to quit...";

            MdiForm.CaptionForeColor = Color.Red;
        }
        else
        {
            MdiForm.ToolTipText = "Press F2 to exit edit mode or Escape to quit...";

            MdiForm.CaptionForeColor = Color.Red;
        }

        // Setup read-only property recursivelly for all "My"-subcontrols.
        //
        SetReadOnly( MdiForm, readOnly );

        // Update button-stripe according to visibility (buttons are considered to
        // be visible if they are flagged with tab stops.
        //
        #if TEXTUI
            foreach ( var button in this.buttonCollection )
            {
                if ( ! button.TabStop ) {
                    button.Parent = null;
                }
                else {
                    button.AddBefore( FirstField );
                }
            }
        #else
            ButtonPanel.Controls.Clear ();
            foreach ( var button in this.buttonCollection )
            {
                if ( button.TabStop )
                {
                    ButtonPanel.Controls.Add( button );
                }
            }
        #endif

        LayoutButtonPanel ();

        // Change Browsing
        //
        base.SetReadOnly( readOnly );

        // Finally, form title (after we have changed Browsing flag)
        //
        MdiForm.Caption = FormTitle;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Factory Methods for UI Components ]

    /// <summary>
    /// Creates a new MyGroupBox control with caption and at specified location.
    /// </summary>
    /// <remarks>
    /// The control is added to the form container with its tab-index property 
    /// automatically assigned (see <see cref="NextTabIndex"/>).
    /// </remarks>
    /// 
    protected MyGroupBox NewGroupBox( string text, 
        float left, float top, float width, float height )
    {
        return new MyGroupBox ()
        {
            Left      = (int)( left   * Em.Width  ), 
            Top       = (int)( top    * Em.Height ), 
            Width     = (int)( width  * Em.Width  ),
            Height    = (int)( height * Em.Height ),
            TabStop   = true,
            TabIndex  = NextTabIndex,
            Text      = text,
            ForeColor = MdiForm.ForeColor, // fixes GUI blue color to black
            Parent    = MdiForm,
        };
    }

    /// <summary>
    /// Creates a new static text at specified location.
    /// </summary>
    /// <remarks>
    /// In TextUI mode a static text is drawed on MdiForm canvas.
    /// In GUI mode a new MyLabel control is created instead.
    /// </remarks>
    /// 
    protected void NewStaticText( float left, float top, string text )
    {
        #if TEXTUI
            MdiForm.At( (int)left, (int)top ).Write( text );
        #else
            new MyLabel ()
            {
                Left      = (int)( left  * Em.Width  ), 
                Top       = (int)( top   * Em.Height ),
                TabStop   = false,
                TabIndex  = NextTabIndex,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize  = true, Text = text,
                Parent    = MdiForm, 
            };
        #endif
    }

    /// <summary>
    /// Creates a new MyLabel control at specified location.
    /// </summary>
    /// <remarks>
    /// The control is added to the form container with its tab-index property 
    /// automatically assigned (see <see cref="NextTabIndex"/>).
    /// </remarks>
    /// 
    protected MyLabel NewLabel( float left, float top, float width )
    {
        return new MyLabel ()
        {
            AutoSize = false,
            Left     = (int)( left  * Em.Width  ), 
            Top      = (int)( top   * Em.Height ), 
            Width    = (int)( width * Em.Width  ),
            TabStop  = false,
            TabIndex = NextTabIndex,
            Parent   = MdiForm, 
        };
    }

    /// <summary>
    /// Creates a new MyLabel control with caption and at specified location.
    /// </summary>
    /// <remarks>
    /// The control is added to the form container with its tab-index property 
    /// automatically assigned (see <see cref="NextTabIndex"/>).
    /// </remarks>
    /// 
    protected MyLabel NewLabel( MyGroupBox box, float left, float top, string text )
    {
        return new MyLabel ()
        {
            AutoSize = true,
            Left     = (int)( left  * Em.Width  ), 
            Top      = (int)( top   * Em.Height ), 
            TabStop  = false,
            TabIndex = NextTabIndex,
            Text     = text,
            Parent   = box, 
        };
    }

    /// <summary>
    /// Creates a new MyTextBox control at specified location.
    /// </summary>
    /// <remarks>
    /// The control is added to the form container with its tab-index property 
    /// automatically assigned (see <see cref="NextTabIndex"/>).
    /// </remarks>
    /// 
    protected MyTextBox NewTextField( float left, float top, float width )
    {
        return new MyTextBox ()
        {
            Left     = (int)( left  * Em.Width  ), 
            Top      = (int)( top   * Em.Height ) - ( Em.IsGUI ? 3 : 0 ), 
            Width    = (int)( width * Em.Width  ),
            TabStop  = true,
            TabIndex = NextTabIndex,
            Parent   = MdiForm,
        };
    }

    /// <summary>
    /// Creates a new MyTextBox control inside MyGroupBox container at specified location.
    /// </summary>
    /// <remarks>
    /// The control is added to the form container with its tab-index property 
    /// automatically assigned (see <see cref="NextTabIndex"/>).
    /// </remarks>
    /// 
    protected MyTextBox NewTextField( MyGroupBox box, float left, float top, float width )
    {
        return new MyTextBox ()
        {
            Left     = (int)( left  * Em.Width  ), 
            Top      = (int)( top   * Em.Height ) - ( Em.IsGUI ? 3 : 0 ), 
            Width    = (int)( width * Em.Width  ),
            TabStop  = true,
            TabIndex = NextTabIndex,
            Parent   = box,
        };
    }

    /// <summary>
    /// Creates a new MyComboBox control associated to specified Enum type and 
    /// at some location.
    /// </summary>
    /// <remarks>
    /// The control is added to the form container with its tab-index property 
    /// automatically assigned (see <see cref="NextTabIndex"/>).
    /// </remarks>
    /// 
    protected MyComboBox NewEnumField( float left, float top, float width, Type type )
    {
        if ( Em.IsTextUI )
        {
            --left;

            if ( width > 0 )
            {
                ++width;
            }
        }

        MyComboBox combo = new MyComboBox ()
        {
            AutoSize = width <= 0 ? true : false, 
            Left     = (int)( left  * Em.Width  ), 
            Top      = (int)( top   * Em.Height ) - ( Em.IsGUI ? 3 : 0 ), 
            TabStop  = true,
            TabIndex = NextTabIndex,
            Parent   = MdiForm, 
        };

        if ( width > 0 )
        {
            combo.Width = (int)( width * Em.Width );
        }

        combo.AddEnum( type );

        return combo;
    }

    /// <summary>
    /// Creates a new MyComboBox control inside MyGroupBox container, associated
    /// to a specified Enum type and at some location.
    /// </summary>
    /// <remarks>
    /// The control is added to the form container with its tab-index property 
    /// automatically assigned (see <see cref="NextTabIndex"/>).
    /// </remarks>
    /// 
    protected MyComboBox NewEnumField( MyGroupBox box, float left, float top, 
        float width, Type type )
    {
        if ( Em.IsTextUI ) { --left; }

        MyComboBox combo = new MyComboBox ()
        {
            AutoSize = width <= 0 ? true : false,
            Left     = (int)( left * Em.Width  ), 
            Top      = (int)( top  * Em.Height ) - ( Em.IsGUI ? 3 : 0 ), 
            TabStop  = true,
            TabIndex = NextTabIndex,
            Parent   = box,
        };

        if ( width > 0 )
        {
            combo.Width = (int)( width * Em.Width );
        }

        combo.AddEnum( type );

        return combo;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Validation Helper Methods ]

    /// <summary>
    /// Validates text value not to be null or empty.
    /// </summary>
    /// 
    public void ValidateNotNull( string fieldName, string value, CancelEventArgs e )
    {
        if ( e.Cancel )
        {
            return; // Already cancelled
        }

        // MdiForm.ErrorMessage = null;

        if ( ReadOnly )
        {
            return; // Assume always data valid while browsing
        }

        value = value == null ? null : value.Trim ();

        if ( string.IsNullOrEmpty( value ) )
        {
            MdiForm.ErrorMessage = fieldName + " must not be empty or null.";
            MdiForm.Beep ();
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Validates text value to be a valid HTTP or HTTPS Universal Resource Identifier
    /// (URI).
    /// </summary>
    /// 
    public void ValidateHttpURI( string fieldName, string value, CancelEventArgs e )
    {
        if ( e.Cancel )
        {
            return; // Already cancelled
        }

        // MdiForm.ErrorMessage = null;

        if ( ReadOnly )
        {
            return; // Assume always data valid while browsing
        }

        value = value == null ? null : value.Trim ();

        // Accept NULL values always as valid. Not null must be validated separatelly.
        //
        if ( string.IsNullOrEmpty( value ) )
        {
            return;
        }

        // Parse URI and accept only URIs having scheme either http or https
        //
        try
        {
            string scheme = new Uri( value.Trim () ).Scheme.ToLower ();
            if ( scheme != "http" && scheme != "https" )
            {
                throw new Exception( fieldName + " must be either http or https URI." );
            }
        }
        catch( Exception ex )
        {
            MdiForm.ErrorMessage = ex.Message;
            MdiForm.Beep ();
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Validates text value to be a valid E-Mail address (consisting of at least
    /// one '@' and '.' charaters where '@' preceedes '.').
    /// </summary>
    /// 
    public void ValidateEMailAddress( string fieldName, string value, CancelEventArgs e )
    {
        if ( e.Cancel )
        {
            return; // Already cancelled
        }

        // MdiForm.ErrorMessage = null;

        if ( ReadOnly )
        {
            return; // Assume always data valid while browsing
        }

        value = value == null ? null : value.Trim ();

        // Accept NULL values always as valid. Not null must be validated separatelly.
        //
        if ( string.IsNullOrEmpty( value ) )
        {
            return;
        }

        // Confirm that there is an "@" and a "." in the e-mail address, and in 
        // the correct order.
        //
        int monkey = value.IndexOf( "@" );
        if ( monkey < 0 || value.IndexOf( ".", monkey ) < monkey )
        {
            MdiForm.ErrorMessage = fieldName 
                + " must be valid e-mail address format.";
            MdiForm.Beep ();
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Validates text value to be a valid integer and, if validation succedes,
    /// returns the parsed value.
    /// </summary>
    /// 
    public void ValidateInteger( string fieldName, string value, CancelEventArgs e, 
        ref int parsedValue )
    {
        if ( e.Cancel )
        {
            return; // Already cancelled
        }

        // MdiForm.ErrorMessage = null;

        if ( ReadOnly )
        {
            return; // Assume always data valid while browsing
        }

        value = value == null ? null : value.Trim ();

        // Accept NULL values always as valid. Not null must be validated separatelly.
        //
        if ( string.IsNullOrEmpty( value ) )
        {
            return;
        }

        // Parse integer
        //
        try
        {
            parsedValue = int.Parse( value );
        }
        catch( Exception )
        {
            MdiForm.ErrorMessage = fieldName + " must be a valid integer.";
            MdiForm.Beep ();
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Validates text value to be a valid decimal number and, if validation succedes,
    /// returns the parsed value.
    /// </summary>
    /// 
    public void ValidateDecimal( string fieldName, string value, CancelEventArgs e, 
        ref decimal parsedValue )
    {
        if ( e.Cancel )
        {
            return; // Already cancelled
        }

        // MdiForm.ErrorMessage = null;

        if ( ReadOnly )
        {
            return; // Assume always data valid while browsing
        }

        value = value == null ? null : value.Trim ();

        // Accept NULL values always as valid. Not null must be validated separatelly.
        //
        if ( string.IsNullOrEmpty( value ) )
        {
            return;
        }

        // Parse integer
        //
        try
        {
            parsedValue = decimal.Parse( value );
        }
        catch( Exception )
        {
            MdiForm.ErrorMessage = fieldName + " must be a valid decimal number.";
            MdiForm.Beep ();
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Validates text value to be a valid date in specified format and, if validation 
    /// succedes, returns the parsed value.
    /// </summary>
    /// 
    public DateTime ValidateDate( string fieldName, string value, string format,
        string infoFormat, CancelEventArgs e )
    {
        if ( e.Cancel )
        {
            return DateTime.MinValue; // Already cancelled
        }

        // MdiForm.ErrorMessage = null;

        if ( ReadOnly )
        {
            return DateTime.MinValue; // Assume always data valid while browsing
        }

        value = value == null ? null : value.Trim ();

        // Accept NULL values always as valid. Not null must be validated separatelly.
        //
        if ( string.IsNullOrEmpty( value ) )
        {
            return DateTime.MinValue;
        }

        // Parse integer
        //
        try
        {
            if( format != null )
            {
                return DateTime.ParseExact( value, format,
                               System.Globalization.CultureInfo.InvariantCulture );
            }
            else
            {
                return DateTime.Parse( value, 
                               System.Globalization.CultureInfo.InvariantCulture );
            }
        }
        catch( Exception )
        {
            MdiForm.ErrorMessage = fieldName + " must be a valid date" + infoFormat;
            MdiForm.Beep ();
            e.Cancel = true;
        }

        return DateTime.MinValue;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}