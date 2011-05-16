/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       FormBase.cs
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

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Specifies how the form should be opened.
/// </summary>
/// 
internal enum OpenMode : int
{
    Browse = 0,  // Read-only mode (browsing)
    Search,      // Read-only mode with search panel initially visible
    Edit,        // Edit record(s)
    AddNew       // Add new record
}

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a base form class from which <strong>all MDI forms</strong> are derived
/// in this application.
/// </summary>
/// <remarks>
/// Note that FormBase is not derived from TextUI or Windows form (it has a lot of 
/// members with common names which could interfere with TextUI/Windows form).
/// Underlying TextUI/Windows form is accessable via <see cref="MdiForm"/> property
/// and the MdiForm is tagged with the FormBase object's reference.
/// </remarks>
/// 
internal class FormBase
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Events ]

    /// <summary>
    /// Occurs when read-only state changes.
    /// </summary>
    /// 
    public event EventHandler UiStateChanged = null;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Fields and Properties ]

    /// <summary>
    /// Gets or sets a value indicating whether to suppress form validation.
    /// </summary>
    /// 
    private bool IgnoreRecordChanges { get; set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets the main form, which MdiClient is the container of all FormBase's MdiForms.
    /// </summary>
    /// 
    public static MainForm MainForm { get; internal set; }

    /// <summary>
    /// Gets or sets the caption describing the name of database class to which
    /// the form is connected to.
    /// </summary>
    /// 
    public string Caption { get; set; }

    /// <summary>
    /// Gets the instance of the MyMdiForm class, which holds visual UI components
    /// for the form.
    /// </summary>
    /// 
    public MyMdiForm MdiForm { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the form is read-only (locked) or not.
    /// </summary>
    /// 
    public bool ReadOnly { get; private set; }

    /// <summary>
    /// Gets the unique identifier used to distinguish FormBase objects when tracing.
    /// </summary>
    /// 
    public string TraceID { get; private set; }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets a value indicating whether derived class implements IListSource interface
    /// or not.
    /// </summary>
    /// <remarks>
    /// Used to distinguish ListForm from DetailsForm.
    /// </remarks>
    /// 
    public bool IsListForm
    {
        get { return this is IListSource; }
    }

    /// <summary>
    /// Gets a value indicating whether this form is active (is in focus).
    /// </summary>
    /// 
    public virtual bool IsActive
    {
        get { return MdiForm.Parent != null && MainForm.ActiveMdiChild == MdiForm; }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Protected Constructor ]

    /// <summary>
    /// Initializes a new instance of the FormBase with specified caption and dimensions.
    /// </summary>
    /// <remarks>
    /// Note that protected constructor forbidds instantiation of the FormBase objects
    /// outside the dervied classes from FormBase.
    /// </remarks>
    /// 
    protected FormBase( string caption, int width, int height )
    {
        Caption  = caption;
        ReadOnly = true;
        IgnoreRecordChanges = false;

        /////////////////////////////////////////////////////////////////////////////////

        MdiForm = new MyMdiForm( width, height ) { Tag = this, ReadOnly = true };

        TraceID = MdiForm.Name + "-[" + Caption + "]";

        InitializeMdiForm ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /// <summary>
    /// Initializes event handlers of the MdiForm and its visual components.
    /// </summary>
    /// 
    private void InitializeMdiForm ()
    {
        // Hook our event handlers

        this.UiStateChanged += ( sender, e ) =>
        {
            MdiForm.UpdateVisualStatus ();
        };

        MdiForm.Validating += ( sender, e ) =>
        {
            e.Cancel = ! IsSafeToLeaveTheForm ();
        };

        MdiForm.FormClosed += ( sender, e ) =>
        {
            DisconnectFromCollection ();
        };

        MdiForm.KeyDown += ( sender, e ) =>
        {
            if ( e.KeyCode == Keys.F2 && e.Modifiers == 0 )
            {
                ToggleReadOnlyMode ();
                e.Handled = true;
            }
        };

        // In GUI mode we need also to initialize MdiForm's font and icon
        // and, in case of list form, adjust MdiForm's width/height ratio.

        #if ! TEXTUI

        MdiForm.Font = MainForm.Font;
        MdiForm.SetupIcon( this.Caption );

        if ( IsListForm )
        {
            MdiForm.Width = (int) ( MdiForm.Width * 1.25f );
            MdiForm.Height = (int) ( MdiForm.Height * 0.75f );
            MdiForm.MinimumSize = MdiForm.Size;
        }

        #endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Displays the MdiForm.
    /// </summary>
    /// 
    private void ShowMdiForm ()
    {
        if ( Em.IsTextUI )
        {
            MdiForm.Center( MainForm.MdiClient );
            MdiForm.MdiParent = MainForm;

            if ( ! IsModal )
            {
                MainForm.LayoutMdi( MdiLayout.Cascade );
            }
        }
        else // GUI
        {
            if ( MainForm.ActiveMdiChild == null )
            {
                MdiForm.Center( MainForm.MdiClient );
            }

            if ( ! IsModal )
            {
                MdiForm.MdiParent = MainForm;
                MdiForm.Visible = true;
                MdiForm.Focus();
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Validates the form (if validation is not suppressed by IgnoreRecordChanges)
    /// and returns true if it is safe to leave the form i.e. if changes done to the
    /// form record(s) are saved.
    /// </summary>
    ///
    private bool IsSafeToLeaveTheForm ()
    {
        // User may always leave the form if changes were ignored or the record 
        // is not modified (i.e. not dirty == clean).
        //
        if ( IgnoreRecordChanges )
        {
            Debug.TraceLine( "{0} Ignored Record Changes", TraceID );
            return true;
        }

        bool contentsChanged = this.IsDirty ();

        if ( ! contentsChanged )
        {
            Debug.TraceLine( "{0} Record is Clean", TraceID );

            return true;
        }

        Debug.TraceLine( "{0} Record is Dirty", TraceID );

        // Otherwise, we should ask user to commit changes...

        bool validationOK = true;

        MdiForm.ErrorMessage = Caption + " record has been modified...";

        bool lostFocus =  Em.IsGUI && MainForm.ActiveMdiChild != MdiForm;
        if ( lostFocus )
        {
            MdiForm.Activate ();
        }

        DialogResult rc = MessageBox.Show( "Do you want to save changes?", 
            MdiForm.ErrorMessage, 
            lostFocus ? MessageBoxButtons.YesNo : MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Exclamation );

        if ( rc == DialogResult.Yes )
        {
            // Save record and catch any property violations
            //
            try
            {
                Debug.TraceLine( "{0} Saving data...", TraceID );

                OnSaveData ();
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message, "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Hand );
                validationOK = false;
            }

            if ( validationOK && ! ReadOnly )
            {
                SetReadOnly( true );
            }
        }
        else if ( rc == DialogResult.No )
        {
            // Undo (cancel) changes
            //
            OnLoadData ();

            if ( IsAddNew )
            {
                QuitForm ();
            }
            else if ( ! ReadOnly )
            {
                SetReadOnly( true );
            }
        }
        else // Cancel change of focus i.e. don't leave the form
        {
            validationOK = false; // This works in TextUI, however ...
        }

        // Clear our error message if user choosed to save/undo data
        //
        if ( validationOK )
        {
            MdiForm.ErrorMessage = null; // refresh MDI status message
        }

        return validationOK;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Protected and Public Virtual Methods ]

    /// <summary>
    /// Connects the form to Video Rental Outlet's object collection.
    /// </summary>
    /// 
    protected virtual void ConnectToCollection ()
    {
        throw new NotImplementedException (); // must be implemented
    }

    /// <summary>
    /// Disconnects the form from Video Rental Outlet's collection.
    /// </summary>
    /// 
    protected virtual void DisconnectFromCollection ()
    {
        throw new NotImplementedException (); // must be implemented
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets form title.
    /// </summary>
    /// 
    protected virtual string FormTitle 
    { 
        get { return this.Caption; }
    }
    
    /// <summary>
    /// Gets a value indicating whether the form is modal (shown as a dialog).
    /// </summary>
    /// 
    protected virtual bool IsModal
    {
        get { return false; }
    }

    /// <summary>
    /// Gets a value indicating whether the form is in AddNew mode.
    /// </summary>
    /// 
    public virtual bool IsAddNew
    {
        get { return false; }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the form data (record or records) should be loaded (refreshed)
    /// from connected VRO object collection.
    /// </summary>
    /// 
    protected virtual void OnLoadData ()
    {
    }

    /// <summary>
    /// Gets a value indicating whether the form data is changed by the user.
    /// </summary>
    ///
    public virtual bool IsDirty ()
    {
        return false;
    }

    /// <summary>
    /// Called when the form data (record or records) should be saved to VRO database.
    /// </summary>
    /// 
    protected virtual void OnSaveData ()
    {
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the ReadOnlyStateChanged event.
    /// </summary>
    /// 
    protected virtual void OnUiStateChanged ()
    {
        if ( UiStateChanged != null )
        {
            UiStateChanged( this, EventArgs.Empty );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes UI components of the MdiForm.
    /// </summary>
    /// 
    protected virtual void InitializeComponents ()
    {
        #if ! TEXTUI
            MdiForm.Controls.Add( MdiForm.StatusStrip );
        #endif
    }

    /// <summary>
    /// Sets the ReadOnly property of the form.
    /// </summary>
    ///
    protected virtual void SetReadOnly( bool readOnly )
    {
        if ( IsModal )
        {
            readOnly = true;
        }

        Debug.TraceLine( "{0} *** Setup {1} Mode", TraceID, readOnly ? "View" : "Edit" );

        MdiForm.InvalidateIf( readOnly != ReadOnly );

        ReadOnly = readOnly;
        MdiForm.ReadOnly = readOnly;

        if ( Em.IsGUI && ! IsListForm )
        {
            MainForm.InfoMessage = this.FormTitle;
        }

        if ( Em.IsGUI )
        {
            MdiForm.ErrorMessage = null; // refresh MDI status message
        }

        OnUiStateChanged ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Saves data and sets the form read-only (locks the form for changes).
    /// </summary>
    /// 
    protected virtual void SaveAndLock ()
    {
        ExecuteWithoutFormValidation( () =>
        {
            MdiForm.IfValidateOk( () =>
            {
                try
                {
                    Debug.TraceLine( "{0} Saving data...", TraceID );

                    OnSaveData ();

                    SetReadOnly( true );
                }
                catch( Exception ex ) // Catch any property violations
                {
                    MessageBox.Show( ex.Message, "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Hand );
                }
            } );
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens the form in specified mode (browsing, searching, editing or add new).
    /// </summary>
    ///
    public virtual void OpenForm( OpenMode mode )
    {
        ReadOnly = true;

        bool readOnly = mode == OpenMode.Browse
                     || mode == OpenMode.Search
                     || IsModal;

        Debug.TraceLine( "{0} Open Form; RO {1}, {2}", TraceID, readOnly, mode );

        ConnectToCollection ();

        if ( IsListForm )
        {
            SetReadOnly( readOnly );
            ShowMdiForm ();
            OnLoadData ();
        }
        else
        {
            OnLoadData ();
            SetReadOnly( readOnly );
            ShowMdiForm ();
        }

        IgnoreRecordChanges = false;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Closes the form without asking to save (potentially) dirty data.
    /// </summary>
    /// 
    public virtual void QuitForm ()
    {
        this.ExecuteWithoutFormValidation( () =>
        {
            MdiForm.IfValidateOk( () =>
            {
                Debug.TraceLine( "{0} Unloading...", TraceID );
                MdiForm.Unload ();
            } );
        } );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Executes action without validating the form data record(s).
    /// </summary>
    ///
    public void ExecuteWithoutFormValidation( Action method )
    {
        Debug.TraceLine( "{0} ExecuteWithoutFormValidation", TraceID );

        bool savedIgnoreRecordChanges = IgnoreRecordChanges;
        IgnoreRecordChanges = true;

        try
        {
            method ();
        }
        finally
        {
            IgnoreRecordChanges = savedIgnoreRecordChanges;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Toggles read-only mode on/off.
    /// </summary>
    /// 
    public void ToggleReadOnlyMode ()
    {
        if ( IsModal )
        {
            return;
        }

        // Note that IfValidateOk () can trigger validation
        // which may change Browsing mode, so it is good to remember current browsing
        // mode so we could detect if it was changed.
        //
        bool wasReadOnly = ReadOnly;

        if ( MdiForm.IfValidateOk () ) // here we lose focus temporarily if we can
        {
            // Focus could be lost, so we may change editing mode...
            //
            if ( ReadOnly == wasReadOnly ) // i.e. not changed while validating
            {
                SetReadOnly( ! wasReadOnly ); // toggle mode
            }
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}