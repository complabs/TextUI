/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyLineSeparator.cs
 *  Created:    2011-04-29
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if ! TEXTUI

using System;

using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Represents a horizontal line static control.
/// </summary>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class MyLineSeparator : Control
{
    /// <summary>
    /// Initializes a new instance of the MyLineSeparator.
    /// </summary>
    /// <remarks>
    /// Instances of the MyLineSeparator may be used to vertically separate 
    /// components in a container control (e.g. when docking at the bottom).
    /// </remarks>
    /// 
    public MyLineSeparator ()
    {
        this.Height = 2;
        this.TabStop = false;
        this.TabIndex = 10000;

        // Repaints window contents: a line two pixels heigh where upper color
        // is darker than the lower.
        //
        this.Paint += ( sender, e ) =>
        {
            Graphics g = e.Graphics;

            int left = 5;
            int width = this.Width - 2 * left;

            g.DrawLine( Pens.DarkGray, new Point( left, 0 ), new Point( width, 0 ) );
            g.DrawLine( Pens.White,    new Point( left, 1 ), new Point( width, 1 ) );
        };
    }
}

#endif