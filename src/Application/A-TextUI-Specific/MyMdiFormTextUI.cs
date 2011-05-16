/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 *  (TextUI Part Only)
 * --------------------------------------------------------------------------------------
 *  File:       MyMdiFormTextUI.cs
 *  Created:    2011-04-29
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if TEXTUI // <------ !!!

using System;
using System.ComponentModel;

using TextUI;
using TextUI.Controls;

using Mbk.Commons;

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a visual part of the <see cref="FormBase"/> class in TextUI mode.
/// </summary>
/// 
internal class MyMdiForm : MdiForm
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets a value indicating if the form is running modal as a dialog.
    /// </summary>
    /// 
    public bool Modal
    { 
        get { return this.ForwadKeysToParent == false; }
    }

    /// <summary>
    /// Occurs when after the form is closed.
    /// </summary>
    /// 
    public event EventHandler FormClosed = null;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the MyMdiForm with optionally specified dimensions.
    /// </summary>
    /// 
    public MyMdiForm( int width = 0, int height = 0 )
        : base( width, height )
    {
        this.Border = true;
        this.TabStop = false;
        this.ForwadKeysToParent = true;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /// <summary>
    /// Adjusts colors for the window and its children in case the MdiForm
    /// is inactive.
    /// </summary>
    /// 
    private static void SetupInactiveColors( Window w )
    {
        if ( w == null )
        {
            return;
        }

        w.BackColorInact = Color.Black;
        w.BorderBackColorInact = Color.Black;
        w.CaptionBackColorInact = Color.Black;
        w.BorderForeColorInact = Color.DarkGray;

        foreach( Window child in w.Children )
        {
            child.AccessKeyForeColor = Color.Gray;
            child.BorderBackColorInact = Color.Black;
            child.BackColorInact = Color.Black;
            child.CaptionBackColorInact = Color.Black;
            child.BorderForeColorInact = Color.DarkGray;

            if ( child is MyGroupBox )
            {
                SetupInactiveColors( child );
                child.ForeColorInact = Color.DarkGray;
            }
            else if ( child is MyTextBox )
            {
                MyTextBox field = child as MyTextBox;
                field.BorderForeColorInact = field.Multiline 
                    ? Color.DarkGray : Color.Black;
                //field.BorderForeColorInact = Color.Black;
                field.BorderForeColor = Color.Black;
                field.ForeColorInact = Color.Gray;
            }
            else if ( child is MyComboBox )
            {
                MyComboBox field = child as MyComboBox;
                field.HeaderBackColorInact = Color.Black;
                field.HeaderForeColorInact = Color.Gray;
                field.BorderForeColorInact = Color.Black;
                field.ScrollBarForeColorInact = Color.Black;
            }
            else if ( child is MyListView )
            {
                MyListView field = child as MyListView;
                field.ForeColorInact = Color.DarkGray;
                field.HeaderBackColorInact = Color.Black;
                field.FooterBackColorInact = Color.Black;
                field.CurrentRowBackColorInact = Color.DarkGray;
                field.CurrentRowForeColorInact = Color.Black;
                field.HeaderForeColorInact = Color.DarkGray;
                field.FooterForeColorInact = Color.DarkGray;
            }
            else if ( child is MyCheckBox )
            {
                MyCheckBox field = child as MyCheckBox;
                field.ForeColorInact = Color.DarkGray;
                field.ForeColor = Color.DarkGray;
                field.CaptionForeColorInact = Color.Gray;
                field.CaptionForeColor = Color.Gray;
            }
            else if ( child is MyLabel )
            {
                child.AccessKeyForeColor = Color.DarkGray;
                child.ForeColorInact = Color.DarkGray;
            }
        }
    }

    /// <summary>
    /// Adjusts colors for the window and its children in case the MdiForm
    /// is active (in focus).
    /// </summary>
    /// 
    private static void SetupActiveColors( Window w )
    {
        if ( w == null )
        {
            return;
        }

        w.BackColorInact = w.BackColor;
        w.BorderBackColorInact = w.BackColor;
        w.CaptionBackColorInact = w.BackColor;
        w.BorderForeColorInact = Application.Theme.BorderForeColorInact;

        foreach( Window child in w.Children )
        {
            child.AccessKeyForeColor = Application.Theme.MenuAccessKeyForeColor;
            child.BorderBackColorInact = w.BackColor;
            child.BackColorInact = w.BackColor;
            child.CaptionBackColorInact = w.BackColor;
            child.BorderForeColorInact = w.BorderForeColorInact;

            if ( child is MyGroupBox )
            {
                SetupActiveColors( child );
                child.ForeColorInact = Color.DarkCyan;
                child.ForeColor = Color.DarkCyan;
                child.BorderForeColor = child.BorderForeColorInact;
            }
            else if ( child is MyTextBox )
            {
                MyTextBox field = child as MyTextBox;
                field.BorderForeColorInact = field.Multiline 
                    ? Color.DarkCyan : w.BackColor;
                field.BorderForeColor = Color.DarkCyan;
                field.ForeColorInact = Color.Gray;
                //field.ForeColorInact = field.TabStop ? Color.Gray : Color.Cyan;
            }
            else if ( child is MyComboBox )
            {
                MyComboBox field = child as MyComboBox;
                field.HeaderForeColorInact = Color.Gray;
                field.HeaderBackColor = field.ReadOnly ? Color.DarkCyan : Color.DarkMagenta;
                field.CurrentRowBackColor = field.HeaderBackColor;
                field.CurrentRowForeColor = field.HeaderForeColor;
                field.HeaderBackColorInact = w.BackColor;
                field.BorderForeColorInact = w.BackColor;
                field.BorderForeColor = Color.DarkCyan;
                field.ScrollBarForeColorInact = w.BackColor;
            }
            else if ( child is MyListView )
            {
                MyListView field = child as MyListView;
                field.ForeColorInact = field.ForeColor;
                field.HeaderBackColorInact = w.BackColor;
                field.FooterBackColorInact = w.BackColor;
                field.CurrentRowBackColorInact = Color.DarkGray;
                field.CurrentRowForeColorInact = Color.White;
                field.BorderForeColorInact = field.BorderForeColor;
                field.HeaderForeColorInact = field.HeaderForeColor;
                field.FooterForeColorInact = field.FooterForeColor;
            }
            else if ( child is MyCheckBox )
            {
                MyCheckBox field = child as MyCheckBox;
                field.ForeColorInact = Color.DarkCyan;
                field.ForeColor = Color.Gray;
                field.CaptionForeColorInact = Color.Gray;
                field.CaptionForeColor = Color.White;
            }
            else if ( child is MyLabel )
            {
                child.AccessKeyForeColor = Application.Theme.MenuAccessKeyForeColor;
                child.ForeColorInact = Color.DarkCyan;
            }
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Adjusts colors of the form and its controls depending on whether form is active
    /// (has focus) or not.
    /// </summary>
    /// 
    public void UpdateVisualStatus ()
    {
        if ( IsInApplicationFocus )
        {
            OnGotFocus ();
        }
        else
        {
            OnLostFocus ();
        }
    }

    /// <summary>
    /// Activates the MdiForm.
    /// </summary>
    /// 
    public void Activate ()
    {
        Focus ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Dummy method (does nothing); needed for compatibility with GUI mode.
    /// </summary>
    /// 
    public void SuspendLayout ()
    {
    }

    /// <summary>
    /// Dummy method (does nothing); needed for compatibility with GUI mode.
    /// </summary>
    /// 
    public void ResumeLayout ()
    {
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Raises the LostFocus event and adjusts colors of the visual components.
    /// </summary>
    /// 
    protected override void OnLostFocus ()
    {
        SetColors( 0, 0, Width, Height, Color.Black, Color.DarkGray );

        SetupInactiveColors( this );

        base.OnLostFocus ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the LostFocus event and adjusts colors of the visual components.
    /// </summary>
    /// 
    protected override void OnGotFocus()
    {
        SetColors( 0, 0, Width, Height, BackColor, ForeColor );
        SetColors( 0, Height - 4, Width, 1, BackColor, Color.Cyan );

        SetupActiveColors( this );

 	    base.OnGotFocus();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the WindowUnloaded and FormClosed events.
    /// </summary>
    /// 
    protected override void OnUnload ()
    {
        base.OnUnload ();

        if ( FormClosed != null )
        {
            FormClosed( this, EventArgs.Empty );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Raises the KeyDown event and handles navigation keys.
    /// </summary>
    /// 
    protected override void OnKeyDown( KeyEventArgs e, object sender )
    {
        Window w = sender as Window;

        switch( e.KeyCode )
        {
            case Keys.F4:
                if ( e.Control ) // Ctrl+F4
                {
                    goto case Keys.Escape;
                }
                break;

            case Keys.Escape:
                this.IfValidateOk( () => this.Unload () );
                e.StopHandling ();
                break;

            case Keys.Tab:
                if ( e.Modifiers == 0 )
                {
                    this.SelectNextControl( w );
                    e.StopHandling ();
                }
                else if ( e.Shift )
                {
                    this.SelectNextControl( w, /*forward*/ false );
                    e.StopHandling ();
                }
                break;

            case Keys.Enter:
                if ( ! ( w is Button ) 
                  && ! ( w is ComboBox && ! ( (ComboBox)w ).ReadOnly ) )
                {
                    goto case Keys.Down;
                }
                break;

            case Keys.Up:
            case Keys.Down:
                if ( e.Modifiers == 0 )
                {
                    if (   w is Button || w is ComboBox || w is GroupBox
                        || w is TextBox && ! ( (TextBox) w ).Multiline
                        || w is CheckBox )
                    {
                        bool forward = e.KeyCode != Keys.Up;
                        this.SelectNextControl( w, forward );
                        e.StopHandling ();
                    }
                }
                break;

            default:
                if ( char.IsLetterOrDigit( e.Character ) && e.Alt )
                {
                    // Search character as an access key among children and
                    // set child in focus if found. If focused child is a Button, 
                    // also raise a Click event on it.
                    //
                    Window nextInFocus = this.FindChild( 
                        child => child.AccessKey == e.Character );

                    if ( nextInFocus != null )
                    {
                        if ( nextInFocus is Button )
                        {
                            ( (Button)nextInFocus ).OnClick ();
                        }
                        else
                        {
                            nextInFocus.Focus ();
                        }
                        e.StopHandling ();
                    }
                }
                else if ( char.IsLetterOrDigit( e.Character ) && e.Modifiers == 0
                    && ReadOnly && ! Modal )
                {
                    InfoMessage = "Press F2 to enter edit mode!";
                    break;
                }
                break;
        }

 	    base.OnKeyDown( e, sender );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}

#endif