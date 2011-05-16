/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       Application.cs
 *  Created:    2011-03-16
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace TextUI
{
    using TextUI.ConsoleIO;
    using TextUI.Drawing;
    using TextUI.Framework;

    /// <summary>
    /// Provides static methods and properties to manage an application, such as methods
    /// to start and stop an application, to process TextUI messages, and properties 
    /// to get information about an application. This class cannot be inherited.
    /// </summary>
    /// 
    public static class Application
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Properties ]

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the height of the extra log area at the bottom of the media
        /// used to display long reports or exceptions.
        /// </summary>
        /// 
        public static int ExtraLogArea { get; set; }

        /// <summary>
        /// Gets or sets the initial window width of the screen media.
        /// </summary>
        /// 
        public static int InitialWindowWidth { get; set; }

        /// <summary>
        /// Gets or sets the initial window height of the screen media.
        /// </summary>
        public static int InitialWindowHeight { get; set; }

        /// <summary>
        /// Gets or sets the current insert/overwrite mode.
        /// </summary>
        /// 
        public static bool Overwrite { get; set; }

        /// <summary>
        /// Gets or sets the current error message, usually displayed in status bar
        /// of the main form.
        /// </summary>
        /// 
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the current informational message, usually displayed in status 
        /// bar of the main form.
        /// </summary>
        /// 
        public static string InfoMessage { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets an instance of the window sused to display tool tip texts
        /// and error/info messages.
        /// </summary>
        /// 
        public static Window StatusBarWindow { get; set; }

        /// <summary>
        /// Gets or sets the default text displayed in the status bar.
        /// </summary>
        /// 
        public static string DefaultStatusBarText { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the color theme used when creating new windows.
        /// </summary>
        /// 
        public static ColorTheme Theme { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets exception handler called when an untrapped thread
        /// exception is thrown. 
        /// </summary>
        /// 
        public static Action<Exception> ExceptionHandler { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Private property. Holds current instance of the ConsoleKeyReader used
        /// to dispatch console key events.
        /// </summary>
        /// 
        private static ConsoleKeyReader MessageQueue { get; set; }

        /// <summary>
        /// Gets the instance of the screen.
        /// </summary>
        /// 
        public static Screen Screen { get; private set; }

        /// <summary>
        /// Gets the instance of the root window (a top-level window containing
        /// all windows displayed on the screen).
        /// </summary>
        /// 
        public static Window RootWindow 
        {
            get { return Screen != null ? Screen.RootWindow : null; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes static members of the Application class. (Internal)
        /// </summary>
        /// 
        static Application ()
        {
            Screen = null;
            StatusBarWindow = null;

            DefaultStatusBarText = "Ready.";
            ErrorMessage = null;

            ExtraLogArea        = 0;
            InitialWindowWidth  = 0;
            InitialWindowHeight = 0;

            Overwrite = false;

            Theme = new ColorTheme ();

            ExceptionHandler = null;

            MessageQueue = new ConsoleKeyReader ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Inner Objects Method Wrappers ]

        /// <summary>
        /// Enters full-screen (makes screen media available to the user).
        /// </summary>
        /// 
        public static void EnterFullScreen ()
        {
            if ( Screen != null && Screen.Updater != null )
            {
                Screen.Updater.EnterFullscreen ();
            }
        }

        /// <summary>
        /// Navigates to the log area on the media.
        /// </summary>
        /// 
        public static void GotoLogArea ()
        {
            if ( Screen != null && Screen.Updater != null )
            {
                Screen.Updater.GotoLogArea ();
            }
        }

        /// <summary>
        /// Erases the media and forces full synchronization of the current screen buffer
        /// with the media.
        /// </summary>
        /// 
        public static void FullRepaint ()
        {
            if ( Screen != null )
            {
                Screen.FullRepaint ();
            }
        }

        /// <summary>
        /// Plays the sound of a beep.
        /// </summary>
        /// 
        public static void Beep ()
        {
            if ( Screen != null )
            {
                Screen.Beep = true;
            }
        }

        /// <summary>
        /// Updates the contents of the status bar window, optionally using
        /// the tool tip from the window in focus.
        /// </summary>
        /// 
        internal static void UpdateStatusBarWindow( Window windowInFocus )
        {
            if ( StatusBarWindow == null )
            {
                return;
            }

            if ( ErrorMessage != null )
            {
                StatusBarWindow.Text = ErrorMessage;
                StatusBarWindow.ForeColorInact = Theme.ErrorMessageColor;
            }
            else if ( InfoMessage != null )
            {
                StatusBarWindow.Text = InfoMessage;
                StatusBarWindow.ForeColorInact = Theme.InfoMessageColor;
                InfoMessage = null;
            }
            else if ( windowInFocus != null && windowInFocus.ToolTipText != null )
            {
                StatusBarWindow.Text = windowInFocus.ToolTipText;
                StatusBarWindow.ForeColorInact = Theme.ToolTipColor;
            }
            else
            {
                StatusBarWindow.Text = DefaultStatusBarText;
                StatusBarWindow.ForeColorInact = Theme.ToolTipColor;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Write, WriteLine and ReadKey ]

        public static KeyEventArgs ReadKey( int millisecondsTimeout = -1 )
        {
            return MessageQueue.ReadKey( millisecondsTimeout );
        }

        public static void WriteLine ()
        {
            Console.WriteLine ();
        }

        public static void WriteLine( string format, params Object[] args )
        {
            if ( format == null )
            {
                Console.WriteLine ();
            }
            else
            {
                Console.WriteLine( string.Format( format, args ) );
            }
        }

        public static void Write( string format, params Object[] args )
        {
            if ( format != null )
            {
                Console.Write( string.Format( format, args ) );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Application Message Pump ]

        /// <summary>
        /// Does a message pump on the modal window.
        /// </summary>
        /// 
        public static DialogResult DoModal( Window window )
        {
            if ( RootWindow == null || MessageQueue == null )
            {
                return DialogResult.None;
            }

            RootWindow.AddChild( window, /*validateFocusChange*/ false );

            DialogResult result = MessageQueue.DoModal( Screen, window );

            RootWindow.RemoveChild( window, /*validateFocusChange*/ false );

            return result;
        }

        /// <summary>
        /// Informs the message pump to reload the Screen with the new root window.
        /// </summary>
        ///
        public static void NewRootWindow( Window rootWindow )
        {
            throw new TextUI.RestartMessageLoop () 
            { 
                NewRootWindow = rootWindow
            };
        }

        /// <summary>
        /// Informs the message pump that it must terminate. 
        /// </summary>
        /// 
        public static void Exit ()
        {
            throw new QuitMessageLoop () { Reason = "Done." };
        }

        /// <summary>
        /// Processes all TextUI messages currently in the message queue.
        /// </summary>
        /// <remarks>
        /// Yields CPU.
        /// </remarks>
        /// 
        public static void DoEvents ()
        {
            System.Threading.Thread.Sleep( 0 ); // just yield cpu
        }

        /// <summary>
        /// Creates the screen for the specified window and begins running a standard 
        /// application message loop on the current thread making the specified 
        /// window visible. 
        /// </summary>
        ///
        public static void Run( Window rootWindow )
        {
            RestartMessageLoop:

            /////////////////////////////////////////////////////////////////////////////
            // Create TUI screen context for root window
            //
            Screen newScreen = new Screen( rootWindow );

            /////////////////////////////////////////////////////////////////////////////
            // Create synchronizing interface to console
            //
            try
            {
                new ConsoleScreenWriter( newScreen, ExtraLogArea, 
                    InitialWindowWidth, InitialWindowHeight );
            }
            catch( Exception ex )
            {
                Console.WriteLine( 
                    "Application is interactive and its output must "
                    + "not be redirected to a file." );

                Console.WriteLine ();
                Console.WriteLine( ex.ToString () );
                return;
            }

            /////////////////////////////////////////////////////////////////////////////
            // Setup new screen
            //
            Application.Screen = newScreen;

            /////////////////////////////////////////////////////////////////////////////
            // Run Console message pump on screen
            //
            try
            {
                MessageQueue.Run( newScreen );
            }
            catch ( RestartMessageLoop ex )
            {
                rootWindow = ex.NewRootWindow;
                goto RestartMessageLoop;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex );
            }

            /////////////////////////////////////////////////////////////////////////////
            // Terminate application's process with all its thread and exit with the 
            // specified exit code.
            //
            System.Environment.Exit( 0 );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}