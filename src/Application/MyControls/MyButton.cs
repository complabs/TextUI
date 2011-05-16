/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyButton.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-29
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
/// Represents a button control that is used in this application. 
/// </summary>
/// <remarks>
/// Derived either from TextUI or System.Windows.Forms button control, depending on 
/// used UI library.
/// </remarks>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class MyButton : Button
{
#if TEXTUI

    /// <summary>
    /// Initializes a new instance of the MyButton class. 
    /// </summary>
    /// 
    public MyButton ()
        : base ()
    {
        this.HorizontalPadding = 2;
    }

#else

    /// <summary>
    /// Initializes a new instance of the MyButton class. 
    /// </summary>
    /// 
    public MyButton ()
        : base ()
    {
        this.TextAlign = ContentAlignment.MiddleCenter;
        this.Padding = new Padding( Em.Width, 0, Em.Width, 0 );
    }

    /// <summary>
    /// Determines whether the specified key is a regular input key or a special key
    /// that requires preprocessing. Ignores keys Up and Down as special keys.
    /// </summary>
    ///
    protected override bool IsInputKey( Keys keyData )
    {
        if ( keyData == Keys.Down || keyData == Keys.Up )
        {
            return true;
        }

        return base.IsInputKey( keyData );
    }

#endif
}