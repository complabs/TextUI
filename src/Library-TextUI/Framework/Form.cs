/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       Form.cs
 *  Created:    2011-03-22
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.ObjectModel;

namespace TextUI
{
    using TextUI.Controls;

    /// <summary>
    /// Represents a window that makes up an application's user interface.
    /// </summary>
    /// 
    public class Form : Window
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Static Constants ]

        /// <summary>
        /// Gets a value indicating whether form is displayed on Console in Text UI mode.
        /// </summary>
        /// 
        public static readonly bool IsTextUI = true;

        /// <summary>
        /// Gets a value indicating whether form is displayed as Windows form in GUI mode.
        /// </summary>
        /// 
        public static readonly bool IsGUI    = false;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets the MdiClient window, which is used as a container of all MDI
        /// subforms.
        /// </summary>
        /// 
        public MdiForm MdiClient { get; private set; }

        /// <summary>
        /// Gets or sets the MainMenu that is displayed in the form. 
        /// </summary>
        /// 
        public MainMenu Menu { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a read-only collection of forms that represent the multiple-document 
        /// interface (MDI) child forms that are parented to this form.
        /// </summary>
        /// 
        public ReadOnlyCollection<Window> MdiChildren
        {
            get { return MdiClient.Children; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the currently active multiple-document interface (MDI) child window. 
        /// </summary>
        /// 
        public Window ActiveMdiChild
        {
            get { return MdiClient.ActiveChild; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current error message.
        /// </summary>
        /// 
        public virtual string ErrorMessage
        {
            get { return Application.ErrorMessage; }
            set { Application.ErrorMessage = value; }
        }

        /// <summary>
        /// Gets or sets the current informational message.
        /// </summary>
        /// 
        public virtual string InfoMessage
        {
            get { return Application.InfoMessage; }
            set { Application.InfoMessage = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the Form class. 
        /// </summary>
        ///
        public Form( int width = 100, int height = 40 )
            : base ()
        {
            this.Width  = width;
            this.Height = height;

            this.KeyDown += new KeyEventHandler( ShortcutKeyHandler );

            /////////////////////////////////////////////////////////////////////////////
            // Create tool tip window

            Application.StatusBarWindow = new ButtonBase ()
            {
                Name = "toolTip", Parent = this, TabStop = false,
                Left = 1, Top = this.Height - 1, Width = this.Width - 2, Height = 1, 
                Border = false, ForeColorInact = Application.Theme.ToolTipColor,
                ToolTipText = Application.DefaultStatusBarText
            };

            /////////////////////////////////////////////////////////////////////////////
            // Create MDI client area (which will own and clip all client windows)

            MdiClient = new MdiForm( Width - 2, Height - 5 )
            {
                Name = "clientArea", Parent = this,
                Left = 1, Top = 3, Border = true
            };

            MdiClient.GotFocus += new EventHandler( MdiClient_GotFocus );

            MdiClient.KeyDown += new KeyEventHandler( ShortcutKeyHandler );
            MdiClient.ForwadKeysToParent = false;

            /////////////////////////////////////////////////////////////////////////////
            // Create client area (which will own and clip all client windows)

            Menu = new MainMenu ()
            {
                Name = "mainMenu", Parent = this, 
                Left = 1, Top = 1, Width = this.Width - 2, Height = 1,
            };

            Menu.KeyDown += new KeyEventHandler( ShortcutKeyHandler );

            Menu.ExitMenu += ( sender, e ) =>
            {
                if ( this.ActiveChild == Menu && MdiClient.Children.Count != 0 )
                {
                    MdiClient.Focus ();
                }
            };

            Menu.Show ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// MdiClient GotFocus event handler. In case that MdiClients receive focus,
        /// moves focus on main Menu.
        /// </summary>
        /// 
        private void MdiClient_GotFocus( object sender, EventArgs e )
        {
            // If client area does not have any subwindow, put main menu in focus.
            //
            if ( MdiClient.Children.Count == 0 )
            {
                Menu.Focus ();
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Arranges the multiple-document interface (MDI) child forms within the MDI 
        /// parent form. 
        /// </summary>
        ///
        public void LayoutMdi( MdiLayout layout )
        {
            switch ( layout )
            {
                case MdiLayout.Cascade:
                    LayoutMdiCascade ();
                    break;

                case MdiLayout.TileHorizontal:
                    throw new NotImplementedException ();

                case MdiLayout.TileVertical:
                    throw new NotImplementedException ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Cascades the multiple-document interface (MDI) child forms within the MDI 
        /// parent form. 
        /// </summary>
        /// 
        public void LayoutMdiCascade ()
        {
            // Start position
            //
            int left = 0;
            int top  = 0;

            // Position increment
            //
            int deltaLeft = 2;
            int deltaTop = 2;
            int visibleChildCount = 0;

            // Cascade all child windows in ClientArea except the last child
            // which should be centered. Of course, skip all invisible windows.
            //
            Window lastChild = null;

            foreach( Window w in MdiClient.Children )
            {
                if ( w.Visible )
                {
                    ++visibleChildCount;

                    if ( lastChild != null )
                    {
                        lastChild.MoveBorderPosition( left, top );

                        left += deltaLeft;
                        top  += deltaTop;
                    }

                    lastChild = w;
                }

            }

            // Center child in focus, shifted down-right if it is not the only child.
            //
            if ( lastChild != null )
            {
                lastChild.Center ();

                if ( visibleChildCount > 1 )
                {
                    lastChild.Left += 2;
                    lastChild.Top  += 1;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Plays the sound of a beep.
        /// </summary>
        /// 
        public virtual void Beep ()
        {
            Application.Beep ();
        }

        /// <summary>
        /// Sets 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="text"></param>
        public virtual void SetToolTip( Window w, string text )
        {
            if ( w != null )
            {
                w.ToolTipText = text;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Turns on hourglass cursor.
        /// </summary>
        /// 
        public virtual void HourglassOn ()
        {
        }

        /// <summary>
        /// Turns off hourglass cursor
        /// </summary>
        /// 
        public virtual void HourglassOff ()
        {
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Application level default KeyDown event handler that responds to 
        /// main Menu shortcut keys.
        /// </summary>
        /// 
        public virtual void ShortcutKeyHandler( object sender, KeyEventArgs e )
        {
            Window w = sender as Window;

            Menu.ProcessShortctuKey( e );

            if ( e.Handled )
            {
                return;
            }

            // Move child window of ClientArea on shift + arrows
            //
            if ( e.Shift && w.Parent != null && w.Parent.Parent == MdiClient )
            {
                switch( e.KeyCode )
                {
                    case Keys.Left:
                        w = w.Parent;
                        if ( w.Left > w.ExtraLeft )
                        {
                            --w.Left;
                        }
                        e.StopHandling ();
                        return;

                    case Keys.Right:
                        w = w.Parent;
                        if ( w.Left < MdiClient.Width - w.Width - w.ExtraRight )
                        {
                            ++w.Left;
                        }
                        e.StopHandling ();
                        return;

                    case Keys.Up:
                        w = w.Parent;
                        if ( w.Top > w.ExtraTop )
                        {
                            --w.Top;
                        }
                        e.StopHandling ();
                        return;

                    case Keys.Down:
                        w = w.Parent;
                        if ( w.Top < MdiClient.Height - w.Height - w.ExtraBottom )
                        {
                            ++w.Top;
                        }
                        e.StopHandling ();
                        return;
                }
            }

            // General short-cuts and main menu access keys
            //
            switch( e.KeyCode )
            {
                case Keys.Applications:
                    Menu.Focus ();
                    e.StopHandling ();
                    break;

                case Keys.Tab:
                    if ( e.Control )
                    {
                        if ( MdiClient.ActiveChild != null )
                        {
                            MdiClient.ActiveChild.NextSibling.Focus ();
                        }
                        e.StopHandling ();
                    }
                    else if ( e.Shift && e.Control )
                    {
                        if ( MdiClient.ActiveChild != null )
                        {
                            MdiClient.ActiveChild.PreviousSibling.Focus ();
                        }
                        e.StopHandling ();
                    }
                    break;

                default:
                    if ( char.IsLetterOrDigit( e.Character ) && e.Alt )
                    {
                        if ( Menu.ProcessAccessKey( e.Character ) )
                        {
                            e.StopHandling ();
                        }
                    }
                    break;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// 
        protected override void OnResize ()
        {
            // Resize MdiClient, StatusBar and Menu windows

            if ( MdiClient != null )
            {
                MdiClient.SetSize( Width - 2, Height - 5 );
            }

            if ( Application.StatusBarWindow != null )
            {
                Application.StatusBarWindow.SetSize( Width - 1, 1 );
                Application.StatusBarWindow.Move( 1, Height - 1 );
            }

            if ( Menu != null )
            {
                Menu.SetSize( Width - 2, 1 );
            }

            base.OnResize ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}