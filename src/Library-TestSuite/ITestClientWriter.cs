/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  VRO Test Suite Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VRO_TestSuite
 *  File:       ITestClientWriter.cs
 *  Created:    2011-04-07
 *  Modified:   2011-04-28
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

namespace VRO_TestSuite
{
    /// <summary>
    /// Output interface where VRO test results are written to.
    /// </summary>
    /// 
    public interface ITestClientWriter
    {
        void Begin ();
        void Title( string str );
        void WriteLine ();
        void WriteLine( object obj );
        void WriteLine( string format, params object[] args );
        void End ();
    }
}