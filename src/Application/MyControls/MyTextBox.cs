/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyTextBox.cs
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
    using System.Collections.Generic;
    using System.Windows.Forms;
#endif

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a text box control that is used in this application. 
/// </summary>
/// <remarks>
/// Derived either from TextUI or System.Windows.Forms text box control, depending on 
/// used UI library.
/// </remarks>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class MyTextBox : TextBox
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
            this.Border    = true;
            this.BackColor = value ? Color.DarkBlue : Color.DarkMagenta;
            this.ForeColor = Color.White;
            base.ReadOnly  = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the MyTextBox class.
    /// </summary>
    /// 
    public MyTextBox ()
        : base ()
    {
        Height = 1;
    }

#else
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets a value indicating whether contents of the control was changed
    /// by the user.
    /// </summary>
    /// 
    public bool ContentsChanged { get; set; }

    /// <summary>
    /// Sets the Text property of the window and clears its ContentsChanged flag.
    /// </summary>
    /// 
    public string InitText
    {
        set
        {
            this.Text = value;
            this.ContentsChanged = false;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether control has a border.
    /// Does nothing in GUI mode.
    /// </summary>
    /// 
    public bool Border { get; set; }

    /// <summary>
    /// Gets or sets tool tip text usually displayed at the bottom in the status
    /// bar when the window is in focus.
    /// Does nothing in GUI mode.
    /// </summary>
    /// 
    public string ToolTipText { get; set; }

    /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the current text in the TextBox with all leading and trailing 
        /// white-space characters removed from the text. 
        /// </summary>
        /// 
    public string TrimmedText
    {
        get
        {
            string t = this.Text.Trim ();
            return string.IsNullOrEmpty( t ) ? null : t;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether scroll bars should automatically
    /// appear  in a multiline TextBox control. 
    /// </summary>
    /// 
    public bool AutoScrollBar
    {
        get
        {
            return this.ScrollBars != ScrollBars.None;
        }
        set
        {
            this.WordWrap = ! value;
            this.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the MyTextBox class.
    /// </summary>
    /// 
    public MyTextBox ()
        : base ()
    {
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Sets the Text property of the control from string collection and 
    /// clears ContentsChanged.
    /// </summary>
    ///
    public void SetText( List<string> array )
    {
        this.Text = string.Join( Environment.NewLine, array.ToArray () );
        this.ContentsChanged = false;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Raises the TextChanged event.
    /// Sets ContentsChanged to true.
    /// </summary>
    /// 
    protected override void OnTextChanged ( EventArgs e )
    {
        this.ContentsChanged = true;
        base.OnTextChanged( e );
    }

    /// <summary>
    /// Raises the KeyPress event. 
    /// Suppresses handling of Enter key presses in single-line mode.
    /// </summary>
    ///
    protected override void OnKeyPress ( KeyPressEventArgs e )
    {
        if ( e.KeyChar == '\r' && ! Multiline )
        {
            e.Handled = true;
        }

        base.OnKeyPress( e );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
#endif
}