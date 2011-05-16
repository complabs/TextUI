/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       Debug.cs
 *  Created:    2011-04-26
 *  Modified:   2011-04-28
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

namespace Mbk.Commons
{
    #region [ TraceFlag data-type ]

    /// <summary>
    /// Trace channels.
    /// </summary>
    /// 
    [Flags]
    public enum TraceFlag : int
    {
        None        = 0x0000,
        Updates     = 0x0001,
        UpdatesPlus = 0x0002,
        Focus       = 0x0004,
        Events      = 0x0008,
        Methods     = 0x0010
    }

    #endregion

    /// <summary>
    /// Provides debugging functionallity to a trace file.
    /// </summary>
    /// 
    public static class Debug
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Properties ]

        /// <summary>
        /// Active trace channels.
        /// </summary>
        /// 
        public static TraceFlag TraceFlags { get; set; }

        /// <summary>
        /// Gets a value indicating whether if trace file is open.
        /// </summary>
        /// 
        public static bool IsTraceOpen
        {
            get
            {
                #if DEBUG
                    return TraceStream != null;
                #else
                    return false;
                #endif
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private and Read-Only Properties ]

        public static FileStream TraceFile { get; private set; }

        private static StreamWriter TraceStream { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes static fields of the Debug class.
        /// </summary>
        /// 
        static Debug ()
        {
            TraceFile   = null;
            TraceStream = null;
            TraceFlags  = TraceFlag.None;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Executes action delegate if trace flags are met.
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void IfTracing( TraceFlag flags, Action method )
        {
            if ( TraceStream != null && ( ( TraceFlags & flags ) != 0 ) )
            {
                method ();
            }
        }

        /// <summary>
        /// Executes action delegate if tracing is active.
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void IfTracingExecute( Action method )
        {
            if ( TraceStream != null )
            {
                method ();
            }
        }

        /// <summary>
        /// Writes a line terminator to the trace file.
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void TraceLine ()
        {
            if ( TraceStream == null )
            {
                return;
            }
            
            TraceStream.WriteLine ();
            System.Diagnostics.Trace.WriteLine( "" );
        }

        /// <summary>
        /// Writes time stamp to the trace file.
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void TraceTimeStamp ()
        {
            if ( TraceStream == null )
            {
                return;
            }

            TraceStream.Write( "{0:yyyy-MM-dd HH:mm:ss.fff}  ", DateTime.Now );
        }

        /// <summary>
        /// Writes a formatted string and a new line to the trace file, 
        /// using the same semantics as Format. 
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void TraceLine( string format, params Object[] args )
        {
            if ( TraceStream == null || format == null )
            {
                return;
            }

            string info = string.Format( format, args );

            TraceTimeStamp ();
            TraceStream.WriteLine( info );
            System.Diagnostics.Trace.WriteLine( info );
        }

        /// <summary>
        /// Writes a formatted string to the trace file, using the same 
        /// semantics as Format. 
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void Trace( string format, params Object[] args )
        {
            if ( TraceStream == null || format == null )
            {
                return;
            }

            string info = string.Format( format, args );
            
            TraceStream.Write( info );
            System.Diagnostics.Trace.Write( info );
        }

        /// <summary>
        /// Opens a new trace file (and closes the old one, if already open).
        /// File name is "Trace (t).log" where (t) is a time-stamp in ISO format.
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void OpenTraceFile ()
        {
            CloseTraceFile ();

            string filename = 
                "Trace " + DateTime.Now.ToString( "yyyy-MM-dd-HHmmss" ) + ".log";

            try
            {
                TraceFile = new FileStream( filename, FileMode.Create );
                TraceStream = new StreamWriter( TraceFile, Encoding.UTF8 );
                TraceStream.AutoFlush = true;
            }
            catch
            {
                if ( TraceFile != null )
                {
                    TraceFile.Close ();
                }
                TraceFile = null;
                TraceStream = null;
            }

            TraceLine( "*** BEGIN ***" );
        }

        /// <summary>
        /// Closes the trace file (if open).
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void CloseTraceFile ()
        {
            if ( TraceStream != null )
            {
                TraceLine( "*** END ***" );
                TraceStream.Close ();
                TraceFile.Close ();
                TraceStream = null;
                TraceFile = null;
            }
        }

        /// <summary>
        /// Toggles the trace file on/off i.e. opens a new, if closed, and closes the
        /// current, if open.
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void ToggleTraceOnOff ()
        {
            if ( TraceStream != null )
            {
                CloseTraceFile ();
            }
            else
            {
                OpenTraceFile ();
            }
        }

        /// <summary>
        /// Returns names of the methods found in stack trace of the caller.
        /// </summary>
        /// 
        public static string StackTrace ()
        {
            StringBuilder sb = new StringBuilder ();

            try
            {
                bool showFrame = false;

                StackTrace st = new System.Diagnostics.StackTrace ();

                foreach( StackFrame frame in st.GetFrames () )
                {
                    var method = frame.GetMethod ();
                    if ( method.Name.Equals( "StackTrace" ) )
                    {
                        showFrame = true;
                        continue;
                    }
                    else if ( ! showFrame )
                    {
                        continue;
                    }

                    sb.Append( " at " );

                    if ( method.ReflectedType != null )
                    {
                        sb.Append( method.ReflectedType.Name );
                        sb.Append( "." );
                    }

                    sb.Append( method.Name );

                    if ( frame.GetFileName () != null )
                    {
                        sb.Append( ": " );
                        sb.Append( frame.GetFileName () );
                        sb.Append( ", Line " );
                        sb.Append( frame.GetFileLineNumber () );
                    }

                    sb.AppendLine ();
                }
            }
            catch {}

            return sb.ToString ();
        }

        #endregion
    }
}