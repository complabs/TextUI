/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       Label.cs
 *  Created:    2011-04-26
 *  Modified:   2011-04-26
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace TextUI.Controls
{
    /// <summary>
    /// Represents a standard TextUI label control.
    /// </summary>
    /// 
    public class Label : ButtonBase
    {
        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the Label class. 
        /// </summary>
        /// 
        public Label( string text = null ) 
            : base( text )
        {
        }

        #endregion
    }
}