/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 *  (GUI Part Only)
 * --------------------------------------------------------------------------------------
 *  File:       MainFormGUI.cs
 *  Created:    2011-04-29
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if ! TEXTUI // <------ !!!

using System;
using System.Drawing;
using System.Windows.Forms;

using System.Reflection;
using System.Drawing.Drawing2D;
using VideoRentalOutlet_GUI.Properties;

using Mbk.Commons;

using System.Security.Permissions;
using System.Threading;

/////////////////////////////////////////////////////////////////////////////////////////

[SecurityPermission(
    SecurityAction.Demand,
    Flags = SecurityPermissionFlag.ControlAppDomain
    ) // neded for UI Exception Handler
]
public partial class MainForm : Form
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Form Initialization and Clean-up (Dispose) ]

    /// <summary>
    /// Initializes visual components of the MainForm and configures event handlers.
    /// </summary>
    /// 
    private void InitializeComponents ()
    {
        /////////////////////////////////////////////////////////////////////////////////

        this.SetStyle( ControlStyles.UserPaint 
                     | ControlStyles.DoubleBuffer 
                     | ControlStyles.AllPaintingInWmPaint 
                     | ControlStyles.ResizeRedraw 
                     | ControlStyles.SupportsTransparentBackColor, true );

        this.IsMdiContainer = true;
        this.AutoValidate   = AutoValidate.EnablePreventFocusChange;
        this.Icon           = Resources.VideoStore;
        this.DoubleBuffered = true;
        this.Menu           = new MainMenu ();

        /////////////////////////////////////////////////////////////////////////////////

        this.Shown += new EventHandler( MainForm_Shown );
        this.FormClosing += new FormClosingEventHandler( MainForm_FormClosing );
        this.MdiChildActivate += ( sender, e ) => ValidateMdiChildActivate ();

        /////////////////////////////////////////////////////////////////////////////////

        SetupStatusBar ();

        /////////////////////////////////////////////////////////////////////////////////

        SetupMdiClientArea ();

        /////////////////////////////////////////////////////////////////////////////////

        SetupBaseFont( "Verdana", 9f );

        /////////////////////////////////////////////////////////////////////////////////

        this.MinimumSize = new Size( 1000, 550 );
        Rectangle requiredArea = new Rectangle( 0, 0, 1024, 700 );

        // Setup initial size to fit working area and setup size accordingly
        requiredArea.Intersect( Screen.PrimaryScreen.WorkingArea );
        this.Size = requiredArea.Size;

        this.CenterToScreen ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; 
    /// otherwise, false.</param>
    /// 
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( HeadingFont != null )
            {
                HeadingFont.Dispose ();
                HeadingFont = null;
            }
        }

        base.Dispose( disposing );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ MainForm - Shown and Closing Event Handlers ]

    /// <summary>
    /// Handles the Shown event. Loads database from file.
    /// </summary>
    /// 
    private void MainForm_Shown( object sender, EventArgs e )
    {
        try
        {
            LoadDatabase( this.databaseFilename, 
                "Video Rental Outlet Database", /*silent*/ true );
        }
        catch( Exception ex )
        {
            // As LoadDatabase method uses external DLL, we cannot catch
            // "could not load dll error" inside the method, so we do it here.
            // Note that the method handles itself its own exceptions.
            //
            MessageBox.Show( ex.Message, "Error Loading DLL", 
                MessageBoxButtons.OK, MessageBoxIcon.Hand );
        }
    }

    /// <summary>
    /// Handles the FormClosing event. Asks user whether to save database, if database
    /// is dirty.
    /// </summary>
    /// 
    private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
    {
        if ( ! this.VideoStore.IsDirty )
        {
            return;
        }

        DialogResult rc = MessageBox.Show( 
            "Database has been modified...\n\n"
                + "Do you want to save changes?", 
            "Video Rental Oultet",
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation );

        if ( rc == DialogResult.Yes )
        {
            SaveDatabase( this.databaseFilename, "Saving Database", /*silent*/true );
        }
        else if ( rc == DialogResult.Cancel )
        {
            e.Cancel = true;
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Application Fonts - Setup ]

    /// <summary>
    /// Gets application-level font used for headers and titles.
    /// </summary>
    /// 
    public Font HeadingFont { get; private set; }

    /// <summary>
    /// Configures base font.
    /// </summary>
    /// <remarks>
    /// Sets the Font for the MainForm and all MDI children.
    /// Configures also the <see cref="HeadingFont"/> based on the base font size.
    /// </remarks>
    /// 
    private void SetupBaseFont( string fontName, float fontSize )
    {
        Debug.TraceLine( "Old font: {0}, size {1}", this.Font.Name, this.Font.Size );

        this.SuspendLayout ();

        // Remember current fonts so they could be disposed later
        //
        Font oldHeadingFont = HeadingFont;

        // Setup new base and heading font. Note that heading font is always "Arial"
        // but with size scaled to base font.
        //
        Font baseFont = null;
        try
        {
            baseFont = new Font( fontName, fontSize );
        }
        catch
        {
            baseFont = this.Font;
        }

        HeadingFont = new Font( "Arial", fontSize * 1.2f, FontStyle.Bold );

        // Calculate an average Em-box size for the current font.
        // Everything will be manually scaled to this dimensions.
        //
        using ( Graphics g = this.CreateGraphics() )
        {
            string test = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö";
            Em.Width  = (int)( g.MeasureString( test, baseFont ).Width / test.Length );
            Em.Height = Math.Max( 8, baseFont.Height );

            // In case of fixed fonts, increase width for one pixel
            //
            SizeF iSize = g.MeasureString( "i", baseFont );
            SizeF wSize = g.MeasureString( "w", baseFont );
            if ( Math.Abs( iSize.Width - wSize.Width ) < 0.1 )
            {
                ++Em.Width;
            }

            // Set auto-scale mode to depend on font
            //
            this.AutoScaleMode = AutoScaleMode.Font;
        }

        // Update font for the main form, but save current active MDI child so 
        // it could be restored. (Setting new font will change order of MDI children.)
        //
        Form savedActiveMdiChild = ActiveMdiChild;
        this.Font = baseFont;

        // Update font for the children
        //
        ExecuteFor<FormBase>( form => form.MdiForm.Font = baseFont );

        // Reactivate active MDI child that was saved before font update
        //
        if ( savedActiveMdiChild != null )
        {
            savedActiveMdiChild.Activate ();
        }

        this.statusStrip.Font = baseFont;

        this.InfoMessage = string.Format( "Font: {0} {1}",
            this.Font.Name, this.Font.Size );

        if ( oldHeadingFont != null )
        {
            oldHeadingFont.Dispose ();
        }

        this.ResumeLayout ();

        Debug.TraceLine( "New font: {0}, size {1}", this.Font.Name, this.Font.Size );
        Debug.TraceLine( "Em-Box width= {0}, height= {1}", Em.Width, Em.Height );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ MDI Client Area - Initialization ]

    /// <summary>
    /// Gets MdiClient control that is parent to all MDI forms.
    /// </summary>
    /// 
    public MdiClient MdiClient { get; private set; }

    /// <summary>
    /// Initializes MdiClient visual components.
    /// </summary>
    /// 
    private void SetupMdiClientArea ()
    {
        // Find out MDI client area between controls
        //
        this.MdiClient = null;

        foreach( Control child in this.Controls )
        {
            this.MdiClient = child as MdiClient;
            if ( this.MdiClient != null )
            {
                break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        // Prevent flickering, on MdiClient i.e. set ControlStyles.DoubleBuffer
        // for MdiClient instance using reflection (as it is not directly available).
        //
        try
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = typeof( MdiClient ).GetMethod( "SetStyle", flags );

            object[] parameters = { ControlStyles.DoubleBuffer, true };

            method.Invoke( MdiClient, parameters );
        }
        catch ( System.Security.SecurityException )
        {
            // Ignored. Assembly doesn't have reflection permission.
            // This code is running under partially trusted context.
        }


        /////////////////////////////////////////////////////////////////////////////////
        // Setup how MDI should look like

        this.MdiClient.BackColor = Color.FromArgb( 0xAD, 0xA7, 0x5B ); // Very Dark Khaki

        this.MdiClient.Paint += EH_MdClient_Paint;

        this.MdiClient.Resize += delegate
        {
            this.MdiClient.Invalidate ();
        };
    }

    /// <summary>
    /// Draws the window of the MdiClient area.
    /// </summary>
    /// 
    private void EH_MdClient_Paint( object sender, PaintEventArgs e )
    {
        Graphics g = e.Graphics;

        //---------------------------------------------------------------------------
        #region [ Parameters ]

        string info = GetVerboseVersionInfo ();
        Image logo = Resources.KthLogo;

        Color backColorTop = Color.LightGoldenrodYellow;
        Color backColorBottom = this.MdiClient.BackColor;

        #endregion

        //---------------------------------------------------------------------------
        #region [ Repaint background with gradient brush ]

        // Create a gradient brush
        //
        Rectangle rect = new Rectangle(
            this.ClientRectangle.Left,
            this.ClientRectangle.Top,
            this.ClientRectangle.Width,
            this.ClientRectangle.Height - logo.Height / 2
            // NOTE: Part bellow logo.Height/2 is already painted in BackColor
            );

        rect.Inflate( 2, 2 ); // to completely fill the client area

        using ( LinearGradientBrush filler = new LinearGradientBrush(
            rect, backColorTop, backColorBottom, 90f ) )
        {
            g.FillRectangle( filler, rect ); // Fill the client area
        }

        #endregion

        //---------------------------------------------------------------------------
        #region [ Draw Logo and info text ]

        // Get info text dimensions, depending on used font
        //
        Size size = g.MeasureString( info, this.Font ).ToSize();

        // Calculate (centered) position for info text
        // to be placed on lower right corner in client area
        //
        Point location = new Point( ClientRectangle.Size );
        location.X -= size.Width / 2 + 4 * Em.Width;
        location.Y -= size.Height + this.statusStrip.Height + 2 * Em.Height;

        // Draw info message (centered at location)
        //
        using ( StringFormat sf = new StringFormat() )
        {
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString( info, this.Font, Brushes.White, location, sf );
        }

        // Draw logo (also centered at location, above info text)
        //
        location.Y -= logo.Height + Em.Height;
        location.X -= logo.Width / 2;
        e.Graphics.DrawImage( logo, location );

        #endregion
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ MDI Child Validation - MdiChildActivate event handler ]

    /// <summary>
    /// Holds the last MdiChild that was set to be active.
    /// </summary>
    /// 
    private Form lastMdiChild = null;

    /// <summary>
    /// Handles MdiChildActivate event handler.
    /// </summary>
    /// <remarks>
    /// Validates whether current MDI may lose the focus. If validation failes,
    /// reactivates invalid MDI that is losing the focus.
    /// </remarks>
    /// 
    private void ValidateMdiChildActivate ()
    {
        Debug.TraceLine( "MdiChildActivate:  {0}", 
            ActiveMdiChild == null ? "none" : ActiveMdiChild.Name );

        MyMdiForm myMdiForm = this.lastMdiChild as MyMdiForm;

        if ( this.lastMdiChild != ActiveMdiChild && myMdiForm != null )
        {
            FormBase mdiForm = myMdiForm.Tag as FormBase;

            if ( mdiForm != null && mdiForm.IsDirty () )
            {
                // System.Media.SystemSounds.Beep.Play ();

                mdiForm.MdiForm.Activate ();

                Debug.TraceLine( "MdiChildActivate: Reactivated as dirty: {0}", 
                    ActiveMdiChild == null ? "none" : ActiveMdiChild.Name );
            }
        }

        this.lastMdiChild = ActiveMdiChild;

        this.InfoMessage = ActiveMdiChild == null ? "Ready." : ActiveMdiChild.Text;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Status Bar - Methods, Fields and Properties ]

    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusInfo;

    /// <summary>
    /// Initializes the status bar.
    /// </summary>
    /// 
    private void SetupStatusBar ()
    {
        this.statusStrip = new StatusStrip()
        {
            AutoSize = true, Parent = this,
        };

        this.statusInfo = new ToolStripStatusLabel()
        {
            Spring = true, Text = "Ready.", ForeColor = Color.DarkBlue,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
        };

        this.statusStrip.Items.Add( this.statusInfo );
    }

    /// <summary>
    /// Gets or sets the informational message displayed in the status bar.
    /// </summary>
    /// 
    public string InfoMessage
    {
        get
        {
            return this.statusInfo.Text;
        }
        set
        {
            this.statusInfo.ForeColor = Color.DarkBlue;
            this.statusInfo.Text = value;
        }
    }

    /// <summary>
    /// Configures the Select event handler of a specified menu item to show its 
    /// tooltip text in the status bar, after being selected.
    /// </summary>
    /// 
    private void SetToolTip( MenuItem w, string text )
    {
        w.Select += ( sender, e ) =>
        {
            this.statusInfo.ForeColor = Color.Black;
            this.statusInfo.Text = text;
        };
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Hourglass cursor - Methods ]

    /// <summary>
    /// Sets the cursor to WaitCursor.
    /// </summary>
    /// 
    public void HourglassOn ()
    {
        this.Cursor = Cursors.WaitCursor;
    }

    /// <summary>
    /// Sets the cursor to default cursor.
    /// </summary>
    /// 
    public void HourglassOff ()
    {
        this.Cursor = Cursors.Default;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Screenshot to PNG image - Static Method ]
 
    public static void SnapshotAsPngImage( Control control )
    {
        try
        {
            // Generate snapshot filename as the name of control class + timestamp
            //
            string fileName = control.GetType().Name 
                + " " + DateTime.Now.ToString( "yyyy-MM-dd-HHmmss" ) + ".png";

            using ( Bitmap bmp = new Bitmap( control.Width, control.Height ) )
            {
                // Draw control to bitmap and save image
                //
                Rectangle area = new Rectangle( 0, 0, control.Width, control.Height );
                control.DrawToBitmap( bmp, area );
                bmp.Save( fileName, System.Drawing.Imaging.ImageFormat.Png );
            }
        }
        catch {}
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Sudoku Puzzle - Method ]

    /// <summary>
    /// Creates new instance of the SudokuForm class and displays it.
    /// </summary>
    /// 
    private void OpenSudoku ()
    {
        MdiClient.IfValidateOk( () =>
        {
            SudokuForm sudoku = new SudokuForm ();
            sudoku.MdiParent = this;
            sudoku.LoadAiEscargot ();
            sudoku.Show ();
        } );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Exception Handling ]

    /// <summary>
    /// Handles the UI exceptions by showing a dialog box, and asking the user whether
    /// or not they wish to abort execution.
    /// </summary>
    /// 
    public static void EH_UIThreadException ( object sender, ThreadExceptionEventArgs t )
    {
        DialogResult result = DialogResult.Cancel;
        try
        {
            Exception e = t.Exception;

            string errorMsg = 
                "An application error occurred:\n\n"
                + e.Message + "\n\nStack Trace:\n" + e.StackTrace;

            result = MessageBox.Show( errorMsg, "Video Rental Outlet - Exception",
                            MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop );
        }
        catch
        {
            Application.Exit ();
        }

        // Exits the program when the user clicks Abort.
        //
        if ( result == DialogResult.Abort )
        {
            Application.Exit ();
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}

#endif