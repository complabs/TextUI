/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 *  (TextUI Part Only)
 * --------------------------------------------------------------------------------------
 *  File:       MainFormTextUI.cs
 *  Created:    2011-04-29
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if TEXTUI // <------ !!!

using System;
using System.Timers;

using Mbk.Commons;

using TextUI;

/////////////////////////////////////////////////////////////////////////////////////////
// TextUI specific part of the MainForm

public partial class MainForm : Form
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Initialization of the Visual Components ]

    /// <summary>
    /// Initializes visual components of the MainForm and configures event handlers.
    /// </summary>
    /// 
    private void InitializeComponents ()
    {
        this.WindowLoaded += new EventHandler( MainForm_Loaded );

        this.WindowUnloaded += ( sender, e ) => StopClockInTitleTimer ();

        DrawMdiClientWindow ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Draws MdiClient background and application version info.
    /// </summary>
    /// 
    private void DrawMdiClientWindow ()
    {
        MdiClient.BorderBackColor      = Color.Black;
        MdiClient.BorderBackColorInact = Color.Black;
        MdiClient.BorderForeColor      = Color.DarkCyan;
        MdiClient.BorderForeColorInact = Color.DarkCyan;
        MdiClient.BackColor            = Color.Black;
        MdiClient.ForeColor            = Color.DarkCyan;

        MdiClient.FillRectangle( 0, 0, MdiClient.Width, MdiClient.Height, ' ' );

        /////////////////////////////////////////////////////////////////////////////////

        // Display application version info.
        //
        string info = GetVerboseVersionInfo ();
        TaggedTextCollection lines = TaggedText.SplitTextInLines( info );

        int left = MdiClient.Width  - 2 - lines.MaxTextLength;
        int top = MdiClient.Height - 1 - lines.Count;

        for ( int i = 0; i < lines.Count; ++i )
        {
            MdiClient.At( left, top + i ).Write( lines[i].Text );
        }

        // Display information about tracing, if any.
        //
        Debug.IfTracingExecute( () =>
        {
            MdiClient.ForeColor = Color.DarkMagenta;

            MdiClient.At( 2, MdiClient.Height - 3 );

            MdiClient.Write( "Tracing to: " );
            MdiClient.Write( System.IO.Path.GetFileName( Debug.TraceFile.Name ) );

            MdiClient.At( 2, MdiClient.Height - 2 );

            MdiClient.Write( "Trace flags: " );
            MdiClient.Write( Debug.TraceFlags.Verbose () );
        } );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ MainForm Load Event Handler (with UI Unhandled Exceptions Handler) ]

    private Exception LastException = null;

    /// <summary>
    /// Occurs when the MainForm is loaded. 
    /// </summary>
    /// <remarks>
    /// Loads database, then shows the last exception 
    /// (if any) and establishes new exception handler. 
    /// Starts clock timer shown in console window title.
    /// </remarks>
    /// 
    private void MainForm_Loaded( object sender, EventArgs e )
    {
        try
        {
            ClockInTitleTimer_Elapsed( null, null );

            LoadDatabase( this.databaseFilename, 
                "Video Rental Outlet Database", /*silent*/ true );
        }
        catch 
        {
            // Guard against all exceptions until the last exception is shown
        }

        // Show the last exception, if any ...
        //
        if ( LastException != null )
        {
            if ( DialogResult.OK == MessageBox.Show( 
                LastException.Message + "\n\nView Details?", 
                LastException.GetType().Name, MessageBoxButtons.OKCancel ) )
            {
                MessageBox.Show( 
                    "\n" + LastException.ToString (),
                    LastException.Message, 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation
                    );
            }
        }

        // ... then establish new exception handler that should restart application
        // in case of any serious error. 
        //
        Application.ExceptionHandler = delegate( Exception ex )
        {
            if ( clockInTitle != null )
            {
                clockInTitle.Stop ();
                clockInTitle = null;
            }

            MainForm form = new MainForm ();
            form.LastException = ex;

            Application.NewRootWindow( form );
        };

        // Start timer that will update clock shown in console title
        //
        clockInTitle = new System.Timers.Timer( 0.25 );

        clockInTitle.Elapsed += new ElapsedEventHandler( ClockInTitleTimer_Elapsed );

        clockInTitle.Start ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Timer behind clock in Application Title ]

    private Timer clockInTitle = null;

    /// <summary>
    /// Elapsed event handler for the clock in title Timer.
    /// </summary>
    /// 
    private void ClockInTitleTimer_Elapsed( object sender, ElapsedEventArgs e )
    {
        lock( Application.Screen )
        {
            string newTitle = ApplicationTitle + ", " 
                            + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" );

            if ( newTitle != this.Text )
            {
                this.Text = newTitle;
                Console.Title = newTitle;
            }
        }
    }

    /// <summary>
    /// Stops clock in title timer.
    /// </summary>
    /// 
    private void StopClockInTitleTimer ()
    {
        if ( clockInTitle != null )
        {
            clockInTitle.Stop ();
            clockInTitle = null;
        }

        this.Text = ApplicationTitle + "; Done.";
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Sudoku Form ]

    private SudokuForm sudokuForm = null;

    /// <summary>
    /// Opens existing (previously hidden) or creates new Sudoku puzzle form.
    /// </summary>
    /// 
    private void OpenSudoku ()
    {
        MdiClient.IfValidateOk( () =>
        {
            if ( sudokuForm == null )
            {
                sudokuForm = new SudokuForm () 
                { 
                    Parent = this, FileName = "sudoku.txt",
                };

                sudokuForm.LostFocus += ( sender, e ) => sudokuForm.Unload ();

                sudokuForm.Center ();
                sudokuForm.LoadAiEscargot ();
            }

            sudokuForm.Parent = this;
        } );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}

#endif