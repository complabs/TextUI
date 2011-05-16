/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyCheckBox.cs
 *  Created:    2011-04-29
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
/// Represents a check box control that is used in this application. 
/// </summary>
/// <remarks>
/// Derived either from TextUI or System.Windows.Forms check box control, depending on 
/// used UI library.
/// </remarks>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class MyCheckBox : CheckBox
{
#if TEXTUI

    /// <summary>
    /// Gets or sets a value indicating whether contents of the control is read-only.
    /// Adjusts the colors accordingly read-only mode. (Overrides base method.) 
    /// </summary>
    /// 
    public override bool ReadOnly
    {
        set
        {
            this.Border    = false;
            this.BackColor = value ? Color.DarkBlue : Color.DarkMagenta;
            this.ForeColor = Color.White;
            base.ReadOnly  = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the MyCheckBox class.
    /// </summary>
    /// 
    public MyCheckBox ()
        : base ()
    {
    }

#else

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets a value indicating whether contents of the control is read-only
    /// and adjusts the colors accordingly read-only mode.
    /// </summary>
    /// 
    public bool ReadOnly
    { 
        get 
        {
            return this.readOnly;
        }
        set
        {
            this.readOnly = value;
            this.ForeColor = this.readOnly ? Color.DarkSlateGray : Color.Black;
        }
    }

    private bool readOnly;

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets a value indicating whether contents of the control was changed
    /// by the user.
    /// </summary>
    /// 
    public bool ContentsChanged { get; set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the MyCheckBox class.
    /// </summary>
    /// 
    public MyCheckBox ()
        : base ()
    {
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Processes Windows messages.
    /// </summary>
    /// <remarks>
    /// Prevents check box to respond to left mouse button keys and key down space 
    /// events in read only mode.
    /// </remarks>
    /// 
    protected override void WndProc ( ref Message m )
    {
        if ( ReadOnly )
        {
            switch ( m.Msg )
            {
                // Prevent checkbox to respond to left mouse button keys
                //
                case 0x0201: // WM_LBUTTONDOWN
                case 0x0202: // WM_LBUTTONUP
                case 0x0203: // WM_LBUTTONDBLCLK
                    return;

                // Prevent checkbox to respont to pressed spacebar
                //
                case 0x0100: // WM_KEYDOWN
                case 0x0101: // WM_KEYUP
                case 0x0102: // WM_CHAR
                    if ( (int)m.WParam == 0x20 )
                    {
                        return; // if VK_SPACE
                    }
                    break;
            }
        }

        base.WndProc( ref m );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the CheckedChanged event.
    /// Sets ContentsChanged to true if not in read-only mode.
    /// </summary>
    /// 
    protected override void OnCheckedChanged ( EventArgs e )
    {
        if ( ! ReadOnly )
        {
            this.ContentsChanged = true;
        }
        base.OnCheckedChanged( e );
    }

    /// <summary>
    /// Raises the CheckStateChanged event.
    /// Sets ContentsChanged to true if not in read-only mode.
    /// </summary>
    /// 
    protected override void OnCheckStateChanged ( EventArgs e )
    {
        if ( ! ReadOnly )
        {
            this.ContentsChanged = true;
        }
        base.OnCheckStateChanged( e );
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

    #endregion

#endif
}