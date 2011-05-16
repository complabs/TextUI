/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  VRO Test Suite Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VRO_TestSuite
 *  File:       StringWriter.cs
 *  Created:    2011-04-07
 *  Modified:   2011-04-28
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Text;

namespace VRO_TestSuite
{
    /// <summary>
    /// Implements ITestClientWriter interface used to save results of the VRO tests
    /// in string buffer.
    /// </summary>
    /// 
    public class StringWriter : ITestClientWriter
    {
        private int maxLineWidth;
        private StringBuilder sb;

        /// <summary>
        /// Creates new instance of StringWriter class.
        /// </summary>
        /// <param name="maxLineWidth">expected maximum line width (used to
        /// display framed header titles</param>
        /// 
        public StringWriter( int maxLineWidth = 90 )
        {
            this.maxLineWidth = maxLineWidth;
            this.sb = new StringBuilder ();
        }

        /// <summary>
        /// Returns contents of the VRO test suite results.
        /// </summary>
        /// 
        public override string ToString ()
        {
            return sb.ToString ();
        }

        /// <summary>
        /// Displays boxed header with name of the current test
        /// </summary>
        ///
        public void Title( string title )
        {
            if ( title.Length > maxLineWidth - 4 )
            {
                title = title.Substring( 0, maxLineWidth - 4 );
            }
            else
            {
                title = string.Empty.PadLeft( ( maxLineWidth - 4 - title.Length ) / 2 ) 
                      + title;
            }

            sb.AppendLine ();

            sb.Append( "\u250C".PadRight( maxLineWidth - 2, '\u2500' ) );
            sb.AppendLine( "\u2510" );

            sb.Append( "\u2502 " );
            sb.Append( title.PadRight( maxLineWidth - 4 ) );
            sb.AppendLine( "\u2502" );

            sb.Append( "\u2514".PadRight( maxLineWidth - 2, '\u2500' ) );
            sb.AppendLine( "\u2518" );

            sb.AppendLine ();
        }

        /// <summary>
        /// Setup string buffer before starting tests.
        /// </summary>
        /// 
        public void Begin ()
        {
        }

        /// <summary>
        /// Writes the current line terminator to the string buffer.
        /// </summary>
        /// 
        public void WriteLine ()
        {
            sb.AppendLine ();
        }

        /// <summary>
        /// Writes the text representation of the specified object, followed by the
        /// current line terminator, to the string buffer. 
        /// </summary>
        /// 
        public void WriteLine( object obj )
        {
            sb.AppendLine( obj.ToString () );
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects, followed 
        /// by the current line terminator, to the string buffer using the 
        /// specified format information. 
        /// </summary>
        /// 
        public void WriteLine( string format, params object[] args )
        {
            sb.AppendLine( string.Format( format, args ) );
        }

        /// <summary>
        /// Clean-up string buffer after compliting tests.
        /// </summary>
        /// 
        public void End ()
        {
        }
    }
}