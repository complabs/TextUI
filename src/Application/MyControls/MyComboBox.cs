/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyComboBox.cs
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
    using System.ComponentModel;
#endif

using Mbk.Commons;
using VROLib;

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a combo box control that is used in this application. 
/// </summary>
/// <remarks>
/// Derived either from TextUI or System.Windows.Forms combo box control, depending on 
/// used UI library.
/// </remarks>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class MyComboBox : ComboBox
{
    /////////////////////////////////////////////////////////////////////////////////

    #region [ Common Public Methods for TextUI and GUI mode ]

    /// <summary>
    /// Adds the Verbose attributes of the specified to the items collection tagged
    /// with the corresponding Enum values.
    /// </summary>
    ///
    public void AddEnum( Type enumType )
    {
        if ( ! enumType.IsEnum )
        {
            return;
        }

        this.ClearItems ();

        foreach( Enum item in Enum.GetValues( enumType ) )
        {
            this.AddItem( new TaggedText( item.Verbose (), item ) );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

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
            this.HeaderBackColor = value ? Color.DarkBlue : Color.DarkMagenta;
            this.HeaderForeColor = Color.White;
            base.ReadOnly  = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the MyComboBox class.
    /// </summary>
    /// 
    public MyComboBox ()
        : base ()
    {
    }

#else
    /////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets the current selected item.
    /// </summary>
    /// 
    public TaggedText Current 
    { 
        get 
        { 
            TaggedText? current = this.SelectedItem as TaggedText?;
            return current.HasValue ? current.Value : TaggedText.Empty;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether control has a border.
    /// Does nothing in GUI mode.
    /// </summary>
    /// 
    public bool Border { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether contents of the control was changed
    /// by the user.
    /// </summary>
    /// 
    public bool ContentsChanged { get; set; }

    /////////////////////////////////////////////////////////////////////////////////

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
            this.BackColor = this.readOnly ? Parent.BackColor : Color.White;
        }
    }

    private bool readOnly;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the MyComboBox class.
    /// </summary>
    /// <remarks>
    /// Combo box GUI style is set to DropDownList by default.
    /// </remarks>
    /// 
    public MyComboBox ()
        : base ()
    {
        this.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Intercepts Windows messages.
    /// </summary>
    /// <remarks>
    /// Prevents combobox to respond to left mouse button keys and wheel
    /// in read-only mode. Mouse wheel events are forwarded to MyMdiForm parent.
    /// </remarks>
    /// 
    protected override void WndProc ( ref Message m )
    {
        if ( ReadOnly ) // Ignore left mouse button and forward mouse wheel to parent
        {
            switch ( m.Msg )
            {
                case 0x0201: // WM_LBUTTONDOWN
                case 0x0202: // WM_LBUTTONUP
                case 0x0203: // WM_LBUTTONDBLCLK
                    return;

                case 0x020A: // WM_MOUSEWHEEL
                    MyMdiForm parent = Parent as MyMdiForm;
                    if ( parent != null )
                    {
                        int delta = ( (int)m.WParam ) >> 16; // high-order word  
                        int xpos  = ( (int)m.LParam ) & 0xFFFF; // low-order word
                        int ypos  = ( (int)m.LParam ) >> 16; // high-order word 
                        MouseEventArgs e = new MouseEventArgs( 0, 0, xpos, ypos, delta );
                        parent.ProcessMouseWheel( e );
                    }
                    return;
            }
        }
        else // Ignore mouse wheel when not read-only
        {
            switch ( m.Msg )
            {
                case 0x020A: // WM_MOUSEWHEEL
                    return;
            }
        }

        base.WndProc( ref m );
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the SelectedItemChanged event.
    /// Sets ContentsChanged to true if not in read-only mode.
    /// </summary>
    /// 
    protected override void OnSelectedItemChanged( EventArgs e )
    {
        if ( ! ReadOnly )
        {
            this.ContentsChanged = true;
        }
        base.OnSelectedItemChanged( e );
    }

    /// <summary>
    /// Raises the SelectedIndexChanged event.
    /// Sets ContentsChanged to true if not in read-only mode.
    /// </summary>
    /// 
    protected override void OnSelectedIndexChanged( EventArgs e )
    {
        if ( ! ReadOnly )
        {
            this.ContentsChanged = true;
        }
        base.OnSelectedIndexChanged( e );

        OnValidating( new CancelEventArgs () );
    }

    /// <summary>
    /// Raises the SelectedValueChanged event.
    /// Sets ContentsChanged to true if not in read-only mode.
    /// </summary>
    /// 
    protected override void OnSelectedValueChanged( EventArgs e )
    {
        if ( ! ReadOnly )
        {
            this.ContentsChanged = true;
        }
        base.OnSelectedValueChanged( e );
    }

    /// <summary>
    /// Raises the KeyDown event.
    /// Toggles DroppedDown on Space if not in read-only mode.
    /// </summary>
    /// 
    protected override void OnKeyDown ( KeyEventArgs e )
    {
        if ( ! ReadOnly && e.KeyCode == Keys.Space && e.Modifiers == 0 )
        {
            DroppedDown = ! DroppedDown;
            return;
        }

        base.OnKeyDown( e );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Removes all items from the combo box.
    /// </summary>
    /// 
    public void ClearItems ()
    {
        this.Items.Clear ();
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Adds a TaggedText item to the list of items for a ComboBox.
    /// </summary>
    /// 
    public int AddItem( TaggedText t )
    {
        return this.Items.Add( t );
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Selects current TaggedText item by its TaggedText tag.
    /// </summary>
    /// 
    public void SelectItem( object tag )
    {
        for( int i = 0; i < this.Items.Count; ++i )
        {
            TaggedText? t = this.Items[ i ] as TaggedText?;

            if ( t.HasValue && t.Value.Tag.Equals( tag ) )
            {
                this.SelectedItem = t;
                break;
            }
        }

        this.ContentsChanged = false;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////
#endif
}