/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       Program.cs
 *  Created:    2011-03-15
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Globalization;
using System.Threading;
using Mbk.Commons;

#if TEXTUI
    using TextUI;
#else
    using System.Windows.Forms;
#endif

/// <summary>
/// Main program entry of the Video Rental Outlet application,
/// common both to TextUI (Console) and GUI (Windows Forms) modes. 
/// </summary>
/// 
static class Program
{
    #if TEXTUI
        [MTAThread] // TextUI COM mode: Multi-threaded apartment
    #else
        [STAThread] // GUI COM mode: Single-threaded apartment
    #endif
    static void Main( string [] args )
    {
        /////////////////////////////////////////////////////////////////////////////////
        // As our application is English/US-centric, setup all system messages 
        // (i.e. exception messages) and default formattings to be in English too.
        //
        CultureInfo englishUS = new CultureInfo( "en-US" );
        Thread.CurrentThread.CurrentUICulture = englishUS; 
        Thread.CurrentThread.CurrentCulture   = englishUS;

        /////////////////////////////////////////////////////////////////////////////////
        // Configure tracing for debugging purposes. Note that tracing is effective 
        // only in DEBUG mode.
        //
        // Debug.TraceFlags |= TraceFlag.Events;
        // Debug.TraceFlags |= TraceFlag.Methods;
        // Debug.TraceFlags |= TraceFlag.Focus;
        // Debug.TraceFlags |= TraceFlag.Updates;
        // Debug.TraceFlags |= TraceFlag.UpdatesPlus;
        //
        // Debug.OpenTraceFile ();

        /////////////////////////////////////////////////////////////////////////////////

        SetupExceptionHandling ();

        /////////////////////////////////////////////////////////////////////////////////

        #if TEXTUI

            // Request extra log area (bellow application window) in case we should
            // dump extended reports directly to console.
            //
            Application.ExtraLogArea = 500;

        #else

            // Enable windows forms visual styles.
            //
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault( false );

        #endif

        /////////////////////////////////////////////////////////////////////////////////
        // Run our TextUI/GUI application
        //
        Application.Run( new MainForm () );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Exception Handling ]

    /// <summary>
    /// Setup exception handling for Windows Forms. 
    /// </summary>
    /// 
    private static void SetupExceptionHandling ()
    {
        // Add the event handler for handling non-UI thread exceptions to the event. 
        //
        AppDomain.CurrentDomain.UnhandledException += EH_UnhandledException;

        /////////////////////////////////////////////////////////////////////////////////

        #if ! TEXTUI // In GUI Mode:

        // Add the event handler for handling UI thread exceptions to the event and
        // set the unhandled exception mode to force all Windows Forms errors to go 
        // through our handler.
        //
        Application.ThreadException += MainForm.EH_UIThreadException;
        Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );

        #endif
    }

    /// <summary>
    /// Log AppDomain unhandled exceptions to screen or message box, 
    /// depending on UI mode.
    /// </summary>
    /// 
    private static void EH_UnhandledException( object sender,
        UnhandledExceptionEventArgs e )
    {
        Exception ex = (Exception) e.ExceptionObject;

        string errorMsg = "An application error occurred:\n\n"
            + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;

        if ( Em.IsTextUI )
        {
            Console.WriteLine( errorMsg );
        }
        else
        {
            MessageBox.Show( errorMsg, "Video Rental Outlet" );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}