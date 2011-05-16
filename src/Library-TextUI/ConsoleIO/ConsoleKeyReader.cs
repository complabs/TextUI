/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.ConsoleIO
 *  File:       ConsoleKeyReader.cs
 *  Created:    2011-03-16
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using Mbk.Commons;

namespace TextUI.ConsoleIO
{
    using TextUI.Drawing;

    /// <summary>
    /// Implements Key event reader for the .NET Console.
    /// </summary>
    /// 
    internal class ConsoleKeyReader
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Fields ]

        /// <summary>
        /// Message queue used to pass events to an active window (window in focus).
        /// </summary>
        /// 
        private List<EventArgs> messageQueue;

        /// <summary>
        /// Indicates if worker thread is running.
        /// </summary>
        /// 
        private volatile bool isRunning;

        /// <summary>
        /// Worker thread instance.
        /// </summary>
        /// 
        private Thread readKeyThread;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ConsoleKeyReader class with default 
        /// settings. 
        /// </summary>
        /// 
        public ConsoleKeyReader ()
        {
            this.messageQueue = null;
            this.isRunning = false;
            this.readKeyThread = null;

            this.SetupCtrlHandler( PutAttentionKeyDown );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Windows Console API interops ]

        private static readonly IntPtr STD_INPUT_HANDLE  = new IntPtr(-10);
        private static readonly IntPtr STD_OUTPUT_HANDLE = new IntPtr(-11);
        private static readonly IntPtr STD_ERROR_HANDLE  = new IntPtr(-12);

        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;

        public delegate bool CtrlHandler( CtrlEventTypes CtrlType );

        [DllImport("kernel32", SetLastError=true)]
        private static extern IntPtr GetStdHandle( IntPtr standardHandle );

        [DllImport("kernel32", SetLastError=true)]
        private static extern bool GetConsoleMode( IntPtr handle, out uint mode );

        [DllImport("kernel32", SetLastError=true)]
        private static extern bool SetConsoleMode( IntPtr handle, uint mode );

        [DllImport("kernel32", SetLastError=true)]
        private static extern bool SetConsoleCtrlHandler( CtrlHandler handler, bool add );  

        private uint savedCondoleModeFlags = 0;

        /// <summary>
        /// Restores console mode previously set by SetupConsoleMode().
        /// </summary>
        /// 
        private void RestoreConsoleMode ()
        {
            try
            {
                IntPtr handle = GetStdHandle( STD_INPUT_HANDLE );

                SetConsoleMode( handle, savedCondoleModeFlags );
            }
            catch
            {
                // be silent if extended flags could not be set
            }
        }

        /// <summary>
        /// Sets the input mode of a console's input buffer or the output mode of a
        /// console screen buffer.
        /// </summary>
        /// 
        private void SetupConsoleMode ()
        {
            try
            {
                IntPtr handle = GetStdHandle( STD_INPUT_HANDLE );
                GetConsoleMode( handle, out savedCondoleModeFlags );
                SetConsoleMode( handle, ENABLE_EXTENDED_FLAGS );
            }
            catch
            {
                // be silent if extended flags could not be get or set
            }
        }

        private CtrlHandler controlHandler = null;

        /// <summary>
        /// Setup console console CTRL handler.
        /// </summary>
        /// <remarks>
        /// We have to keep the handler routine alive during the execution of the 
        /// program, because the garbage collector will destroy it after any 
        /// CTRL event.
        /// </remarks>
        ///
        public void SetupCtrlHandler( CtrlHandler handler )
        {
            try
            {
                if ( controlHandler != null )
                {
                    SetConsoleCtrlHandler( controlHandler, /*add*/ false /*=remove*/ );
                }

                controlHandler = new CtrlHandler( handler );

                GC.KeepAlive( controlHandler );

                SetConsoleCtrlHandler( handler, /*add*/ true );
            }
            catch
            {
                // be silent if extended flags could not be get or set
            }
        }

        #endregion  

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Starts working thread reading keys from console.
        /// </summary>
        /// 
        private void Start ()
        {
            Debug.TraceLine( "ConsoleKeyReader: Starting worker thread..." );

            lock( this )
            {
                if ( this.readKeyThread == null )
                {
                    SetupConsoleMode ();

                    this.messageQueue = new List<EventArgs> ();
                    this.isRunning = true;

                    this.readKeyThread = new Thread( new ThreadStart( ReadKey_Worker ) );
                    this.readKeyThread.Start ();
                }
            }
        }

        /// <summary>
        /// Stops working thread.
        /// </summary>
        /// 
        private void Stop ()
        {
            Debug.TraceLine( "ConsoleKeyReader: Stopping worker thread..." );

            lock( this )
            {
                if ( this.readKeyThread != null )
                {
                    this.isRunning = false;
                    this.readKeyThread = null;
                
                    lock( this.messageQueue )
                    {
                        // Putting nulls in message queue releases blocked GetMessage()
                        // 
                        for ( int i = 0; i < 10; ++i )
                        {
                            messageQueue.Add( null );
                        }

                        Monitor.Pulse( this.messageQueue );
                    }

                    RestoreConsoleMode ();
                }
            }
        }

        /// <summary>
        /// Decodes Console key using current Console input encoding.
        /// </summary>
        /// 
        private static char Decode( ConsoleKeyInfo key )
        {
            Decoder decoder = Console.InputEncoding.GetDecoder ();

            byte[] src = new byte[] { (byte)key.KeyChar };
            int charCount = decoder.GetCharCount( src, 0, src.Length );

            char[] chars = new Char[ charCount ];
            int charsDecodedCount = decoder.GetChars( src, 0, src.Length, chars, 0 );

            return chars.Length >= 1 ? chars[ 0 ] : '\0';
        }

        /// <summary>
        /// ReadKey worker thread that reads keys from console and puts them into
        /// a message queue.
        /// </summary>
        /// 
        private void ReadKey_Worker ()
        {
            try
            {
                Debug.TraceLine( "ConsoleKeyReader: Worker thread begins." );

                while( this.isRunning )
                {
                    while ( this.isRunning && ! Console.KeyAvailable )
                    {
                        Thread.Sleep( 10 );
                    }

                    if ( ! this.isRunning )
                    {
                        break;
                    }

                    ConsoleKeyInfo key = Console.ReadKey( /*intercept=*/ true );

                    lock( this.messageQueue )
                    {
                        messageQueue.Add( new KeyEventArgs () { 
                            KeyInfo = key, Character = Decode( key )
                        } );

                        Monitor.Pulse( this.messageQueue );
                    }
                }
            }
            catch
            {
                Debug.TraceLine( "ConsoleKeyReader: Worker thread aborted." );
            }
            finally
            {
                Debug.TraceLine( "ConsoleKeyReader: Worker thread ends." );
            }
        }

        /// <summary>
        /// Gets a value indicating if message queue is not empty.
        /// </summary>
        /// 
        private bool IsMessageAvailable ()
        {
            if ( this.messageQueue == null )
            {
                return false;
            }

            lock( this.messageQueue )
            {
                return this.messageQueue.Count > 0;
            }
        }

        /// <summary>
        /// Gets event from the message queue.
        /// </summary>
        /// 
        private EventArgs GetMessage( int millisecondsTimeout = -1 )
        {
            if ( this.messageQueue == null || ! this.isRunning )
            {
                return null;
            }

            EventArgs message = null;

            lock( this.messageQueue )
            {
                try
                {
                    if ( this.messageQueue.Count > 0 )
                    {
                        message = this.messageQueue[ 0 ];
                    }
                    else
                    {
                        if ( millisecondsTimeout < 0 && Monitor.Wait( messageQueue ) )
                        {
                            message = this.messageQueue[ 0 ];
                        }
                        else if ( Monitor.Wait( messageQueue, millisecondsTimeout ) )
                        {
                            message = this.messageQueue[ 0 ];
                        }
                    }
                }
                catch ( SynchronizationLockException )
                {
                    message = null;
                }
                catch ( ThreadInterruptedException )
                {
                    message = null;
                }

                if ( message != null )
                {
                    this.messageQueue.Remove( message );
                }
            }

            return message;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Puts an event into a message queue.
        /// </summary>
        /// 
        public void PutMessage( EventArgs e )
        {
            if ( this.messageQueue == null )
            {
                return;
            }

            try
            {
                lock( this.messageQueue )
                {
                    messageQueue.Add( e );

                    Monitor.Pulse( this.messageQueue );
                }
            }
            catch( ThreadAbortException ) // Worker Thread aborted
            {
                lock( this.messageQueue )
                {
                    // Putting NULL in message queue releases blocked 
                    // (i.e. indefinitely waiting) GetMessage() in MessagePump.
                    // 
                    messageQueue.Add( null );
                    Monitor.Pulse( this.messageQueue );
                }
            }
        }

        /// <summary>
        /// Puts Keys.Attention KeyDown event into the message queue.
        /// </summary>
        /// 
        public bool PutAttentionKeyDown( CtrlEventTypes ctrlType )
        {
            ConsoleKeyInfo attentionKey = new ConsoleKeyInfo( 
                '\0', ConsoleKey.Attention,
                /*shift*/ false, /*alt*/ false, /*control*/ false );

            // Put 'Attention' key into message queue (several times, to be sure)
            //
            for ( int i = 0; i < 10; ++i )
            {
                this.PutMessage( new KeyEventArgs () { 
                    KeyInfo = attentionKey, ControlEvent = ctrlType
                } );
            }

            return true;
        }

        public KeyEventArgs ReadKey( int millisecondsTimeout = -1 )
        {
            KeyEventArgs readKeyEvent = null;

            while( this.isRunning )
            {
                EventArgs message = GetMessage( millisecondsTimeout );
                if ( message != null && ( message is KeyEventArgs ) )
                {
                    readKeyEvent = (KeyEventArgs)message;
                    break;
                }
            }

            return readKeyEvent;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ MessagePump(), DoModal() and Run() Methods ]

        /// <summary>
        /// Dispatches messsages (events) from message queue to the window in focus.
        /// Window in focus is determined as the ActiveDescendant of the specified
        /// modal window. If the modal window is not specified, the Root window of the
        /// screen is used as the modal window.
        /// </summary>
        /// 
        private void MessagePump( Screen screen, Window modalWindow = null )
        {
            if ( modalWindow == null )
            {
                modalWindow = screen.RootWindow;
            }

            while( this.isRunning )
            {
                if ( ! this.IsMessageAvailable () )
                {
                    lock( screen )
                    {
                        screen.UpdateScreen ();
                    }
                }

                EventArgs e = this.GetMessage ();
                if ( e == null )
                {
                    continue;
                }

                lock( screen )
                {
                    Window winInFocus = modalWindow.ActiveDescendant;

                    if ( winInFocus != null )
                    {
                        if ( e is KeyEventArgs )
                        {
                            KeyEventArgs rk = (KeyEventArgs)e;

                            // Raise key events
                            //
                            winInFocus.RaiseKeyDown( rk );
                            winInFocus.RaiseAfterKeyDown( rk );
                        }
                    }

                    Application.UpdateStatusBarWindow( modalWindow.ActiveDescendant );
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Does a message pump on the modal window.
        /// </summary>
        /// 
        public DialogResult DoModal( Screen screen, Window modalWindow )
        {
            try
            {
                // Update tooltip
                //
                Application.UpdateStatusBarWindow( screen.RootWindow.ActiveDescendant );

                // Pump messages until QuitMessageLoop exception
                //
                MessagePump( screen, modalWindow );
            }
            catch( QuitMessageLoop ex )
            {
                // DoModal() loop is normally broken by throwing QuitMessageLoop
                // exception with DialogResult as parameter.
                //
                if ( ex.Result != null && ex.Result is DialogResult )
                {
                    return (DialogResult)ex.Result;
                }

                // Exception is not for us, throw it further...
                // This might be a QuitMessageLoop thrown to quit application.
                //
                throw;
            }

            return DialogResult.None;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Does a message pump on the application's Root window.
        /// </summary>
        /// 
        public void Run( Screen screen )
        {
            this.Start (); // Start read-key worker thread (if not already started)

            try
            {
                lock( screen )
                {
                    // Display initial screen contents
                    //
                    screen.FullRepaint ();

                    // Raise WindowLoad event on root window
                    //
                    screen.RootWindow.RaiseLoadEvent ();

                    // Raise LostFocus/GotFocus events and update tooltip
                    //
                    Application.UpdateStatusBarWindow( screen.RootWindow );
                }

                // Pump messages until QuitMessageLoop exception is thrown
                // inside MessagePump().
                //
                MessagePump( screen );

                // MessagePump normally terminates with internally thrown QuitMessageLoop
                // exception or when IsRunning becomes false. Make sure that in later
                // case we always we always end-up with proper cleaning (which is done
                // while handling QuitMessageLoop exception).
                //
                throw new QuitMessageLoop() { Reason = "IsRunning set to false" };
            }
            catch( RestartMessageLoop )
            { 
                lock( screen )
                {
                    // Raise WindowUnload event on root window
                    //
                    screen.RootWindow.RaiseUnloadEvent ();

                    // Display final screen contents
                    //
                    screen.UpdateScreen ();
                }

                // Retrow exception (it should be catched by Application.Run()).
                // Exception was thrown by calling Application.NewRootWindow().
                //
                throw;
            }
            catch( QuitMessageLoop ex )
            { 
                // Stop key receiver thread
                //
                try { this.Stop (); } catch {}

                lock( screen )
                {
                    // Raise WindowUnload event on root window
                    //
                    screen.RootWindow.RaiseUnloadEvent ();

                    // Display final screen contents
                    //
                    screen.UpdateScreen ();
                }

                Application.GotoLogArea ();

                Application.WriteLine();
                Application.WriteLine( string.Empty.PadRight( screen.Width, '-' ) );

                Application.WriteLine( ex.Reason != null ? ex.Reason : "Completed."  );
            }
            catch( Exception ex )
            { 
                // Execute user's exception handler, if any.
                //
                lock( screen )
                {
                    if ( Application.ExceptionHandler != null )
                    {
                        // Reset exception handler delegate to break potential looping
                        //
                        Action<Exception> action = Application.ExceptionHandler;
                        Application.ExceptionHandler = null;

                        action( ex );
                    }
                }

                // Note that user's exception handler may throw RestartMessageLoop
                // exception so the following code won't be executed.

                // Stop key receiver thread
                //
                try { this.Stop (); } catch {}

                // Goto log area and display information about unknown exception
                //
                Application.GotoLogArea ();

                Application.WriteLine();
                Application.WriteLine( string.Empty.PadRight( screen.Width, '-' ) );

                Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine( ex.ToString () );
                Console.WriteLine ();

                Console.WriteLine( "Press any key to quit..." );

                Console.ReadKey( false );

                return;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}