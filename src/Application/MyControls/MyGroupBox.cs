/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyGroupBox.cs
 *  Created:    2011-05-01
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
#endif

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a group box control that is used in this application. 
/// </summary>
/// <remarks>
/// Derived either from TextUI or System.Windows.Forms group box control, depending on 
/// used UI library.
/// </remarks>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class MyGroupBox : GroupBox
{
    /// <summary>
    /// Initializes a new instance of the MyGroupBox class.
    /// </summary>
    /// 
    public MyGroupBox ()
        : base ()
    {
    }
}