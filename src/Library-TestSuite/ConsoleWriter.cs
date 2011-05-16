/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  VRO Test Suite Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VRO_TestSuite
 *  File:       ConsoleWriter.cs
 *  Created:    2011-04-07
 *  Modified:   2011-04-28
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace VRO_TestSuite
{
    /// <summary>
    /// Implements ITestClientWriter interface used to display results of th eVRO tests
    /// on console or to write results to the standard output stream, if application
    /// is not interactive.
    /// </summary>
    /// 
    public class ConsoleWriter : ITestClientWriter
    {
        private int maxLineWidth = 90;
        private bool isInteractive = false;

        /// <summary>
        /// Displays boxed header with name of the current test
        /// </summary>
        ///
        public void Title( string title )
        {
            if ( this.isInteractive )
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
            }

            if ( title.Length > maxLineWidth - 4 )
            {
                title = title.Substring( 0, maxLineWidth - 4 );
            }
            else
            {
                title = string.Empty.PadLeft( ( maxLineWidth - 4 - title.Length ) / 2 ) 
                      + title;
            }

            Console.WriteLine ();

            Console.Write( "\u250C".PadRight( maxLineWidth - 2, '\u2500' ) );
            Console.WriteLine( "\u2510" );

            Console.Write( "\u2502 " );
            Console.Write( title.PadRight( maxLineWidth - 4 ) );
            Console.WriteLine( "\u2502" );

            Console.Write( "\u2514".PadRight( maxLineWidth - 2, '\u2500' ) );
            Console.WriteLine( "\u2518" );

            Console.WriteLine ();

            if ( this.isInteractive )
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// Setup console colors and determine if the console is interactive
        /// </summary>
        /// 
        public void Begin ()
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Clear ();
                Console.SetWindowSize( 100, 40 );

                // Mono doesn't support SetBufferSize
                try {  Console.SetBufferSize( 100, 1000 ); } catch {}

                Console.TreatControlCAsInput = true;
                this.isInteractive = true;
            }
            catch
            {
                this.isInteractive = false;
            }
        }

        /// <summary>
        /// Writes the current line terminator to the standard output stream.
        /// </summary>
        /// 
        public void WriteLine ()
        {
            Console.WriteLine ();
        }

        /// <summary>
        /// Writes the text representation of the specified object, followed by the
        /// current line terminator, to the standard output stream. 
        /// </summary>
        /// 
        public void WriteLine( object obj )
        {
            Console.WriteLine( obj );
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects, followed 
        /// by the current line terminator, to the standard output stream using the 
        /// specified format information. 
        /// </summary>
        /// 
        public void WriteLine( string format, params object[] args )
        {
            Console.WriteLine( format, args );
        }

        /// <summary>
        /// Clean-up console after test (does nothing).
        /// </summary>
        /// 
        public void End ()
        {
        }
    }
}