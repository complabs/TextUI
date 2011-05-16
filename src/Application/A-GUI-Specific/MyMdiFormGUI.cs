/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 *  (GUI Part Only)
 * --------------------------------------------------------------------------------------
 *  File:       MyMdiFormGUI.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if ! TEXTUI // <------ !!!

using System;
using System.ComponentModel;

using System.Drawing;
using System.Windows.Forms;

using Mbk.Commons;
using VideoRentalOutlet_GUI.Properties;

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a visual part of the <see cref="FormBase"/> class in GUI mode.
/// </summary>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class MyMdiForm : Form
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Static Fields ]

    // Last generated form id. Used to generate unique MyMdiForm names.
    //
    private static int lastFormID = 0;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]
    
    /// <summary>
    /// Gets the collection of controls contained in the form.
    /// </summary>
    /// 
    public Control.ControlCollection Children
    {
        get { return this.Controls; }
    }

    /// <summary>
    /// Gets the active control in the form.
    /// </summary>
    /// 
    public Control ActiveChild
    {
        get { return this.ActiveControl; }
    }

    /// <summary>
    /// Gets or sets the tool tip text (default text displayed in the status bar).
    /// </summary>
    /// 
    public string ToolTipText { get; set; }

    /// <summary>
    /// Gets or sets a value whether the form is locked (read-only) or editable.
    /// </summary>
    /// 
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets caption (window title) of the form.
    /// </summary>
    /// 
    public string Caption
    {
        get { return this.Text; } 
        set { this.Text = value; }
    }

    /// <summary>
    /// Gets or sets caption foreground color (provided for compatibility with
    /// TextUI but ignored here as .NET does not allow chaning of the window title
    /// colors without villy-nilly with win32 api).
    /// </summary>
    /// 
    public Color CaptionForeColor { get; set; }

    /// <summary>
    /// Gets the primary tool tip control used in the form.
    /// </summary>
    /// 
    public ToolTip ToolTip { get; private set; }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the primary tool strip container for the form. 
    /// </summary>
    /// 
    public ToolStrip ToolStrip
    {
        get
        {
            return this.toolStrip;
        }
        set
        {
            if ( this.toolStrip == value )
            {
                return;
            }
            if ( this.toolStrip != null )
            {
                this.Controls.Remove( this.toolStrip );
            }
            this.toolStrip = value;
            if ( this.toolStrip != null )
            {
                this.Controls.Add( this.toolStrip );
            }
        }
    }

    private ToolStrip toolStrip;

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the primary status strip container for the form.
    /// </summary>
    /// 
    public StatusStrip StatusStrip { get; private set; }

    /// <summary>
    /// Gets the status info tool strip text label for the form.
    /// </summary>
    /// 
    public ToolStripStatusLabel StatusInfo { get; private set; }

    /// <summary>
    /// Gets the locker status strip icon for the form.
    /// </summary>
    /// 
    public ToolStripStatusLabel StatusLocker { get; private set; }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the error message to be displayed in red in the status bar.
    /// </summary>
    /// 
    public string ErrorMessage
    {
        get
        {
            return this.errorMessage;
        }
        set
        {
            this.errorMessage = value;

            if ( this.errorMessage != null )
            {
                this.StatusInfo.ForeColor = Color.Red;
                this.StatusInfo.Text = this.errorMessage;
            }
            else if ( this.infoMessage != null )
            {
                this.StatusInfo.ForeColor = Color.Blue;
                this.StatusInfo.Text = this.infoMessage;
            }
            else
            {
                this.StatusInfo.ForeColor = Color.DarkBlue;
                this.StatusInfo.Text = ToolTipText != null ? ToolTipText : "Ready.";
            }
        }
    }

    private string errorMessage = null;

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the informational message to be displayed in blue in the status bar.
    /// </summary>
    /// 
    public string InfoMessage
    {
        get
        {
            return this.infoMessage;
        }
        set
        {
            this.infoMessage = value;

            if ( this.errorMessage != null )
            {
                this.StatusInfo.ForeColor = Color.Red;
                this.StatusInfo.Text = this.errorMessage;
            }
            else if ( this.infoMessage != null )
            {
                this.StatusInfo.ForeColor = Color.DarkBlue;
                this.StatusInfo.Text = this.InfoMessage;
            }
            else
            {
                this.StatusInfo.ForeColor = Color.Black;
                this.StatusInfo.Text = "Ready.";
            }
        }
    }

    private string infoMessage = null;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the MyMdiForm with optionally specified dimensions.
    /// </summary>
    /// 
    public MyMdiForm( int width = 0, int height = 0 )
        : base ()
    {
        this.Name = this.GetType().Name + "-" + ( ++lastFormID ).ToString( "0000" );

        if ( width != 0 && height != 0 )
        {
            this.ClientSize = new Size() 
            { 
                Width  = width  * Em.Width,
                Height = height * Em.Height
            };

            this.MinimumSize  = this.Size;
        }

        InitializeComponents ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /// <summary>
    /// Initializes default visual style of the form and common UI components.
    /// </summary>
    /// <remarks>
    /// Every MyMdiForm is provided with the StatusStrip (with status info and locker 
    /// status icon) and ToolStrip.
    /// </remarks>
    /// 
    private void InitializeComponents ()
    {
        this.SetStyle( ControlStyles.UserPaint 
                     | ControlStyles.DoubleBuffer 
                     | ControlStyles.AllPaintingInWmPaint, true );

        this.KeyPreview   = true;
        this.AutoValidate = AutoValidate.EnablePreventFocusChange;

        /////////////////////////////////////////////////////////////////////////////////

        this.ToolTip = new ToolTip ();

        this.StatusStrip = new StatusStrip()
        {
            AutoSize = true, ShowItemToolTips = true,
            TabStop = false, TabIndex = 10000,
        };

        this.StatusInfo = new ToolStripStatusLabel()
        {
            Spring = true,
            Text = "Ready.",
            ForeColor = Color.DarkBlue,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
        };

        this.StatusLocker = new ToolStripStatusLabel()
        {
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            Image = VideoRentalOutlet_GUI.Properties.Resources.LockLock16,
            ImageScaling = ToolStripItemImageScaling.None,
            ToolTipText = "Locker.",
            Margin = new Padding( 0, 0, Em.Width, 0 )
        };

        this.StatusStrip.Items.Add( this.StatusInfo );
        this.StatusStrip.Items.Add( this.StatusLocker );

        this.FontChanged += delegate
        {
            this.StatusInfo.Font = this.Font;
        };
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Initializes the Icon property of the form based on the icon (or form) caption.
    /// </summary>
    /// 
    public void SetupIcon( string caption )
    {
        switch( caption )
        {
            case "Video Rental Outlet":  Icon = Resources.VideoStore;   break;
            case "Customer":             Icon = Resources.UserClient;   break;
            case "Customers":            Icon = Resources.UserClients;  break;
            case "Movie":                Icon = Resources.VideoFolder;  break;
            case "Movies":               Icon = Resources.VideoFolder;  break;
            case "Movie Exemplar":       Icon = Resources.VideoStore;   break;
            case "Movie Exemplars":      Icon = Resources.VideoStore;   break;
            case "Price Details":        Icon = Resources.Calculator;   break;
            case "Price List":           Icon = Resources.Gear;         break;
            case "Rented Item":          Icon = Resources.CalendarBlue; break;
            case "Rented Items":         Icon = Resources.CalendarBlue; break;
            case "Sudoku":               Icon = Resources.Sudoku;       break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Centers the form in the bounds of the parent if specified, otherwise 
    /// centeres the form on the current display.
    /// </summary>
    ///
    public void Center( MdiClient parent = null )
    {
        this.StartPosition = parent != null ? FormStartPosition.CenterParent
            : FormStartPosition.CenterScreen;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Plays the beep sound.
    /// </summary>
    /// 
    public void Beep ()
    {
        System.Media.SystemSounds.Beep.Play ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the locker status in status bar.
    /// </summary>
    /// 
    public void UpdateVisualStatus ()
    {
        this.StatusLocker.Image = ReadOnly 
            ? Resources.LockLock16
            : Resources.LockUnlock16;

        this.StatusLocker.ToolTipText = ReadOnly 
            ? "The form is locked (data cannot be modified)."
            : "Data may be modified.";
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the Validating event and executes action if validation succedes.
    /// </summary>
    /// 
    public bool IfValidateOk( Action method = null )
    {
        CancelEventArgs e = new CancelEventArgs ();

        OnValidating( e );

        if ( e.Cancel )
        {
            return false;
        }

        if ( ! this.Validate () || ! this.ValidateChildren () )
        {
            return false;
        }

        if ( method != null )
        {
            try 
            {
                method ();
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.ToString (), ex.Message );
            }
        }

        return true;
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Closes the form.
    /// </summary>
    /// 
    public void Unload ()
    {
        this.Close ();
    }

    /// <summary>
    /// Invalidates the form if condition is met.
    /// </summary>
    /// 
    public void InvalidateIf( bool condition )
    {
        if ( condition )
        {
            Invalidate ();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the MouseWheel event.
    /// </summary>
    /// 
    public void ProcessMouseWheel( MouseEventArgs e )
    {
        OnMouseWheel( e );
    }

    /// <summary>
    /// Raises the KeyDown event.
    /// </summary>
    /// 
    public void CommonKeyDown( KeyEventArgs e )
    {
        OnKeyDown( e );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Cleans up any resources being used.
    /// </summary>
    /// 
    protected override void Dispose ( bool disposing )
    {
        if ( disposing )
        {
            if ( ToolTip != null )
            {
                ToolTip.Dispose ();
            }
        }

        base.Dispose( disposing );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles and re-raises the KeyDown event.
    /// </summary>
    ///
    protected override void OnKeyDown( KeyEventArgs e )
    {
        Control    active  = this.ActiveControl;
        MyComboBox combo   = active as MyComboBox;
        MyTextBox  textBox = active as MyTextBox;

        if ( active != null && e.Modifiers == 0 )
        {
            switch( e.KeyCode )
            {
                case Keys.Escape:
                    this.Unload ();
                    e.Handled = true;
                    break;

                case Keys.Scroll:
                    this.DumpTabIndexes ();
                    break;

                case Keys.Enter:
                    if ( ! ( active is MyButton ) )
                    {
                        goto case Keys.Down;
                    }
                    break;

                case Keys.Up:
                case Keys.Down:

                    if (   active is MyButton
                        || active is MyCheckBox
                        || active is MyTextBox && ! textBox.Multiline
                        || combo != null && ( combo.ReadOnly || ! combo.DroppedDown )
                           )
                    {
                        goto case Keys.Tab;
                    }
                    break;

                case Keys.Tab:

                    bool forward = e.KeyCode != Keys.Up; // forward if not key Up

                    this.SelectNextControl( active, forward, 
                        /*tabStopOnly*/ true, /*nested*/ true, /*wrap*/ true );

                    combo = ActiveChild as MyComboBox;

                    if ( combo != null && ! combo.ReadOnly && ! combo.DroppedDown )
                    {
                        combo.DroppedDown = true;
                    }

                    e.Handled = true;
                    break;
            }
        }

        base.OnKeyDown( e );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}

#endif