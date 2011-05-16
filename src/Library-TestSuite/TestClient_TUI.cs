/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  VRO Test Suite Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VRO_TestSuite
 *  File:       TestClient_TUI.cs
 *  Created:    2011-04-07
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.ComponentModel;
using System.Timers;

using Mbk.Commons;

using TextUI;
using TextUI.Controls;
using TextUI.Drawing;

namespace VRO_TestSuite
{
    /// <summary>
    /// Test suite for TextUI library.
    /// </summary>
    /// 
    public class TestClient_TUI : Form
    {
        private string applicationTitle = "Test Client for TUI Library";

        private ButtonBase winRead;

        private ButtonBase winCyan;
        private TextBox    textBoxSubCyan;
        private CheckBox   checkBoxSubCyan;

        private ButtonBase winMoveable;
        private MdiForm  drawBox;
        private TextBox    textBoxEditor;
        private ListBox    listBox;
        private Button     buttonTest;
        private Button     buttonTest2;

        /////////////////////////////////////////////////////////////////////////////////

        // Constructs sample application form.
        //
        public TestClient_TUI () 
            : base( 100, 40 )
        {
            this.Text = applicationTitle;

            this.WindowUnloaded += new EventHandler ( EH_WindowUnloaded );

            /////////////////////////////////////////////////////////////////////////////
            // Client Area default text

            MdiClient.Border = false;
            MdiClient.SetSize( Width, Height - 3 );
            --MdiClient.Left;
            --MdiClient.Top;

            MdiClient.ForeColor = Color.DarkCyan;
            MdiClient.FillRectangle( 0, 0, MdiClient.Width, MdiClient.Height, '▒' );

            MdiClient.DrawFrame( 0, 0, MdiClient.Width, MdiClient.Height );

            MdiClient.BackColor = BackColor;
            MdiClient.ForeColor = ForeColor;
            MdiClient.SetCursorPosition( 60, 3 );
            MdiClient.WriteLine( "Text User Interface Test Suite\r\n" );
            MdiClient.WriteLine( "▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒" );
            MdiClient.WriteLine( "ABCDEFGHIJKLMNOPQRSTUVWXYZÖÅÄÉ\r\n" 
                        + "abcdefghijklmnopqrstuvwxyzåäöé\n"
                        + "\b\b\b«☺»"
                        + "\r0123456789" );
            MdiClient.WriteLine( "▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒" );

            MdiClient.SetColors( 60, 5, 30, 5, Color.Black, Color.DarkGray );

            /////////////////////////////////////////////////////////////////////////////
            // Application main menu

            Menu.MenuItems.Add( new MenuItem( "-" ) ); // test invalid separator

            MenuItem miEditor = new MenuItem( "&Editor" );
            miEditor.ToolTipText = "Sets editor window in focus";
            Menu.MenuItems.Add( miEditor );
            miEditor.Click += new EventHandler( EH_SetEditorInFocus );

            Menu.MenuItems.Add( new MenuItem( "-" ) ); // test invalid separator
            Menu.MenuItems.Add( new MenuItem( "-" ) ); // test invalid separator

            MenuItem miRed = new MenuItem( "&Red" );
            miRed.ToolTipText = "Sets red window in focus.";
            Menu.MenuItems.Add( miRed );
            miRed.Click += new EventHandler( EH_SetRedInFocus );

            MenuItem miCyan = new MenuItem( "&Cyan" );
            miCyan.ToolTipText = "Sets cyan window in focus.";
            Menu.MenuItems.Add( miCyan );
            miCyan.Click += new EventHandler( EH_SetCyanInFocus );

            MenuItem miSubmenu = new MenuItem( "&Misc..." );
            miSubmenu.ToolTipText =
                "Grid test, button test and message box tests are here..." ;
            Menu.MenuItems.Add( miSubmenu );

            Menu.MenuItems.Add( new MenuItem( "-" ) ); // test invalid separator

            /////////////////////////////////////////////////////////////////////////////
            // Client window: Moveable

            winMoveable = new ButtonBase ()
            {
                Name = "winMoveable",
                Left = 50, Top = 15, Width = 25, Height = 11, Border = true,
                BorderForeColor = Color.Yellow,
                BackColor = Color.DarkBlue, ForeColor = Color.Gray,
            };

            winMoveable.DrawContents += new DrawEventHandler( EH_winMoveable_DrawContents );

            /////////////////////////////////////////////////////////////////////////////
            // Client window: Cyan and SubCyan

            winCyan = new ButtonBase ()
            {
                Name = "winCyan",  TabStop = true,
                Left = 54, Top = 20, Width = 22, Height = 7, Border = true,
                BorderForeColor = Color.Green,
                BackColor = Color.DarkBlue, ForeColor = Color.Gray,
                Text = "Cyan Window:"
            };

            textBoxSubCyan = new TextBox ()
            {
                Name = "winSubCyan", Parent = winCyan, Multiline = false,
                Left = 1, Top = 2, Width = 20, Height = 1, 
                BackColor = Color.DarkMagenta, ForeColor = Color.Cyan,
                ForeColorInact = Color.Gray,
                ToolTipText = "Cyan Window; Tests single-line text box with validation..."
            };

            textBoxSubCyan.KeyDown  += new KeyEventHandler( EH_winSubCyan_KeyDown );
            textBoxSubCyan.GotFocus += new EventHandler( EH_winSubCyan_GotFocus );

            checkBoxSubCyan = new CheckBox ()
            {
                Name = "checkBoxSubCyan", Parent = winCyan, 
                Left = 1, Top = 4, Width = 20, Height = 1, 
                BackColor = Color.DarkMagenta, ForeColor = Color.Cyan,
                ForeColorInact = Color.Gray,
                Text = "Check box", 
                ToolTipText = "Cyan Window; Checkbox to test..."
            };

            checkBoxSubCyan.KeyDown  += new KeyEventHandler( EH_winSubCyan_KeyDown );

            textBoxSubCyan.Validating += new CancelEventHandler( EH_textBoxSubCyan_Validating );

            /////////////////////////////////////////////////////////////////////////////
            // Client window: Buffered

            drawBox = new MdiForm( 21, 15 )
            {
                Name = "winBuf",   TabStop = false,
                Left = MdiClient.Width - 23, Top = MdiClient.Height - 17, Border = true
            };

            drawBox.Write( "Draw Box" );

            drawBox.ForeColor = Color.DarkMagenta;
            drawBox.DrawFrame( 3, 3, 15, 10, BoxLines.NotJoined );
            drawBox.DrawFrame( 3, 5, 15,  1,  BoxLines.Joined );
            drawBox.DrawFrame( 5, 3,  1, 10,  BoxLines.Joined );

            drawBox.ForeColor = Color.Green;
            drawBox.CursorTop = 4;
            drawBox.CursorLeft = 8;
            drawBox.Write( " Test " );

            /////////////////////////////////////////////////////////////////////////////
            // Client window: Red

            winRead = new ButtonBase ()
            {
                Name = "winRed",   TabStop = true,
                ToolTipText = "Red Window; Shift+Arrows moves only draw box, "
                        + "Control+Arrows resizes draw box",
                Left = 75, Top = 18, Width = 20, Height = 10, Border = true,
                BackColor = Color.DarkRed, ForeColor = Color.Yellow,
                BackColorInact = Color.DarkRed, ForeColorInact = Color.DarkYellow,
                Text = "Red Window:",
                ForwadKeysToParent = true
            };

            winRead.KeyDown += new KeyEventHandler( EH_winRed_KeyDown );
            winRead.GotFocus += new EventHandler( EH_winRed_GotFocus );
            winRead.LostFocus += new EventHandler( EH_winRed_LostFocus );

            /////////////////////////////////////////////////////////////////////////////
            // Client window: Editor

            textBoxEditor = new TextBox ()
            {
                Name = "winEditor", Multiline = true, 
                ToolTipText = "Press F2 to load file 'VRO-test.txt' or AppKey for ListBox ...",
                Left = 5, Top = 5, Width = 40, Height = 15,
                VerticalScrollBar = true, Border = true, 
                Caption = "TextBox as Editor",
                Text = "0 Editor\r\n"
                     + "1\r\n"
                     + "2\r\n"
                     + "3\r\n"
                     + "4\r\n"
                     + "5\r\n"
                     + "6 234567890123456789012345678901234567890123456789\r\n"
                     + "7\r\n"
                     + "8\r\n"
                     + "9\r\n"
                     + "10 sdfasdfasdfasfasfasdf1\r\n"
                     + "11\r\n"
                     + "12\r\n"
                     + "1\b3 backspace\r\n"
                     + "tabs\ta\tab\tabc\tabcd\tabcde\tabcdef\r\n"
                     + "15\r\n"
                     + "16\r\n"
                     + "17\r\n"
                     + "18\r\n"
                     + "19\r\n"
            };

            textBoxEditor.KeyDown += new KeyEventHandler( EH_winEditor_KeyDown );

            /////////////////////////////////////////////////////////////////////////////
            // List box

            listBox = new ListBox ()
            {
                Name = "listBox", 
                Left = 5, Top = 5, Width = 40, Height = 15,
                VerticalScrollBar = true, Border = true,
                CaptionForeColor = Color.Red, Caption = "List Box (copied from TextBox)",
                Header = new string[]
                { 
                    string.Empty,
                    "     Header",
                    string.Empty,
                    ListBoxBase.WideDivider,
                },
                Footer = new string []
                {
                    ListBoxBase.WideDivider,
                    "     Footer",
                }
            };

            listBox.KeyDown += new KeyEventHandler( EH_listBox_KeyDown );

            /////////////////////////////////////////////////////////////////////////////
            // Vertical submenu

            miSubmenu.MenuItems.Add( new MenuItem( "-" ) ); // test invalid sep.

            MenuItem miShowGrid = new MenuItem( "Show &grid window" );
            miShowGrid.ToolTipText = "Shows a lots of windows in a grid...";
            miSubmenu.MenuItems.Add( miShowGrid );
            miShowGrid.Click += new EventHandler( EH_ShowGridWindow );

            miSubmenu.MenuItems.Add( new MenuItem( "-" ) );

            MenuItem m13 = new MenuItem( "Test &Error Recovery" );
            m13.ToolTipText = "Throws exception";
            miSubmenu.MenuItems.Add( m13 );

            m13.Click += delegate 
            { 
                throw new Exception( "Error Recovery Test" );
            };

            miSubmenu.MenuItems.Add( new MenuItem( "-" ) );

            MenuItem miTestButton = new MenuItem( 
                "&&Long submenu item that && opens test &button (& multiple &&s) " );
            miTestButton.ToolTipText =
                "&&Long submenu item that && opens test &button (& multiple &&s) ";
            miSubmenu.MenuItems.Add( miTestButton );
            miTestButton.Click += new EventHandler( EH_SetTestButtonInFocus );

            miSubmenu.MenuItems.Add( new MenuItem( "-" ) ); // test invalid sep.

            /////////////////////////////////////////////////////////////////////////////
            // Button

            buttonTest = new Button ()
            {
                Name = "buttonTest", Border = true,
                Left = 5, Top = 10, AutoSize = true,
                HorizontalPadding = 2, // VerticalPadding = 1,
                Text = "Test Button\nwith very long line\nand the third line",
                ToolTipText = "Press ENTER to hide the button..."
            };

            buttonTest.Click += new EventHandler( EH_buttonTest_Click );

            buttonTest2 = new Button ()
            {
                Name = "buttonTest2", Border = true,
                Left = 5 + buttonTest.Width + 2, Top = 10, AutoSize = true,
                HorizontalPadding = 2, // VerticalPadding = 1,
                Text = "Test Button 2\nwith very long line\nand the third line",
                ToolTipText = "Press ENTER to hide the button...",
                TextAlign = TextAlign.Right
            };

            buttonTest2.Click += new EventHandler( EH_buttonTest_Click );
        }

        /////////////////////////////////////////////////////////////////////////////////

        void EH_SetRedInFocus( object sender, EventArgs e )
        {
            winMoveable.Parent = MdiClient;
            drawBox.Parent = MdiClient;
            winRead.Parent = MdiClient;
            winRead.Focus ();
        }

        void EH_SetCyanInFocus( object sender, EventArgs e )
        {
            winCyan.Parent = MdiClient;
            textBoxSubCyan.Focus ();
        }

        void EH_SetEditorInFocus( object sender, EventArgs e )
        {
            if ( textBoxEditor.Parent == null )
            {
                textBoxEditor.Move( 5, 5 );
                textBoxEditor.SetSize( 40, 15 );
            }

            listBox.Unload ();
            textBoxEditor.Parent = MdiClient;
            textBoxEditor.Focus ();
        }

        void EH_SetTestButtonInFocus( object sender, EventArgs e )
        {
            if ( sender != null && sender is Menu )
            {
                ( (Menu)sender ).Hide ();
            }

            DialogResult result = MessageBox.Show( 
                "Do you really want create two buttons?\n\nThis is second line...\n",
                "Text User Interface Test Suite", MessageBoxButtons.YesNoCancel
                );

            if ( result == DialogResult.Cancel || result == DialogResult.None )
            {
                return;
            }

            MessageBox.Show( "You have selected: " + result );

            if ( result == DialogResult.Yes )
            {
                buttonTest.Parent = MdiClient;
                buttonTest2.Parent = MdiClient;
                buttonTest.Focus ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_ShowGridWindow( object sender, EventArgs e )
        {
            Window winGrid = new Window ()
            {
                Name = "winTab", Parent = this, Border = true,
                Left = 1, Top = 1, Width = this.Width - 2, Height = this.Height - 3, 
                BorderForeColor = Color.Green
            };

            Random rand = new Random ();

            int columnSize = 12;
            int horizontalCount = winGrid.ClientWidth / columnSize;
            int left = ( winGrid.Width - horizontalCount * columnSize ) / 2;

            for ( int i = 0; i < winGrid.ClientHeight; ++i )
            {
                for ( int j = 0; j < horizontalCount; ++j )
                {
                    bool hasTabStop = i >= 4 && i <= 34;
                    int tabIndex = rand.Next( 20 );

                    TextBox winD = new TextBox ()
                    {
                        Name = "winTab(" + i + "," + j + ")", Parent = winGrid,
                        TabIndex = tabIndex,
                        Text = hasTabStop ? tabIndex.ToString () : "xxx",
                        Left = left + j * columnSize, Top = i, 
                        Width = columnSize - 1, Height = 1, 
                        Border = false,
                        BackColor = Color.DarkCyan, 
                        BackColorInact = hasTabStop ? Color.DarkGray : Color.DarkBlue,
                        ForeColor = Color.White,
                        ForeColorInact = hasTabStop ? Color.Black : Color.Gray,
                        TabStop = hasTabStop,
                        ToolTipText = "Grid test with 228 TextBoxes; Press Escape to exit..."
                    };

                    winD.KeyDown += new KeyEventHandler( EH_winGrid_KeyDown );
                }
            }

            winGrid.Focus ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_WindowUnloaded( object sender, EventArgs e )
        {
            this.Text = applicationTitle + "; Done.";
        }

        public override void ShortcutKeyHandler( object sender, KeyEventArgs e )
        {
            Window w = sender as Window;

            if ( e.Alt && e.KeyCode == Keys.F4 )
            {
                throw new QuitMessageLoop () { Reason = "Done." };
            }
            else if ( e.KeyCode == Keys.Attention )
            {
                throw new QuitMessageLoop () { Reason = "Ctrl event." };
            }

            switch( e.KeyCode )
            {
                case Keys.F1:
                    Menu.Focus ();
                    e.StopHandling ();
                    break;

                case Keys.F9:
                    EH_ShowGridWindow( sender, e );
                    e.StopHandling ();
                    break;

                case Keys.F11:
                    if ( e.Shift )
                    {
                        Application.GotoLogArea ();
                    }
                    else
                    {
                        Application.EnterFullScreen ();
                    }
                    Application.WriteLine();
                    Application.WriteLine( "Press any key to quit..." );
                    Application.ReadKey ();
                    Application.FullRepaint ();
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

        private void EH_sudoku_ReadKey( object sender, KeyEventArgs e )
        {
            Window w = sender as Window;

            switch( e.KeyCode )
            {
                case Keys.F9:
                    EH_ShowGridWindow( sender, e );
                    e.StopHandling ();
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

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_winMoveable_DrawContents( object sender, DrawEventArgs e )
        {
            Screen wm = e.Screen;

            int w = winMoveable.Width, h = winMoveable.Height;

            string message = string.Format( "L={0},T={1},W={2},H={3}", 
                winMoveable.Left, winMoveable.Top, w, h );

            wm.CursorTop = 0;
            wm.CursorLeft = 0;
            wm.Write( "Movable Window" );

            wm.CursorTop = h / 2;
            wm.CursorLeft = ( w - message.Length ) / 2;
            wm.ForeColor = Color.White;
            wm.WriteLine( message );
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_winRed_LostFocus( object sender, EventArgs e )
        {
            winRead.Text = "Red Window";
        }

        private void EH_winRed_GotFocus( object sender, EventArgs e )
        {
            winRead.Text = "Use Arrows to move\r\n"
                + "'movable' and 'draw \r\n"
                + "box' windows.\r\n\n"
                + "You may also use\r\n"
                + "shift & control\r\n"
                + "as modifiers...\r\n";
        }

        private void EH_winRed_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.Handled )
            {
                return;
            }

            Window w = sender as Window;

            if ( w == null )
            {
                return;
            }

            switch( e.KeyCode )
            {
                case Keys.Escape:
                    winMoveable.Unload ();
                    drawBox.Unload ();
                    winRead.Unload ();
                    Menu.Focus ();
                    e.StopHandling ();
                    break;

                case Keys.Enter:
                    textBoxSubCyan.Focus ();
                    e.StopHandling ();
                    break;

                case Keys.Left:
                    if ( e.Control )
                    {
                        if ( drawBox.Width >= 19 )
                        {
                            --drawBox.Width;
                        }
                    }
                    else if ( e.Shift )
                    {
                        --drawBox.Left;
                    }
                    else if ( winMoveable.Left > 1 )
                    {
                        --drawBox.Left;
                        --winMoveable.Left;
                    }
                    e.StopHandling ();
                    break;

                case Keys.Right:
                    if ( e.Control )
                    {
                        ++drawBox.Width;
                    }
                    else if ( e.Shift )
                    {
                        ++drawBox.Left;
                    }
                    else if ( winMoveable.Left < winMoveable.Parent.Width - winMoveable.Width - 1 )
                    {
                        ++drawBox.Left;
                        ++winMoveable.Left;
                    }
                    e.StopHandling ();
                    break;

                case Keys.Up:
                    if ( e.Control )
                    {
                        if ( drawBox.Height >= 14 )
                        {
                            --drawBox.Height;
                        }
                    }
                    else if ( e.Shift )
                    {
                        --drawBox.Top;
                    }
                    else if ( winMoveable.Top > 1 )
                    {
                        --drawBox.Top;
                        --winMoveable.Top;
                    }
                    e.StopHandling ();
                    break;

                case Keys.Down:
                    if ( e.Control )
                    {
                        ++drawBox.Height;
                    }
                    else if ( e.Shift )
                    {
                        ++drawBox.Top;
                    }
                    else if ( winMoveable.Top < winMoveable.Parent.Height - winMoveable.Height - 1 )
                    {
                        ++drawBox.Top;
                        ++winMoveable.Top;
                    }
                    e.StopHandling ();
                    break;

                default:
                    if ( char.IsControl( e.Character ) )
                    {
                        winRead.Text = "Control Key " + (int)e.Character 
                                    + "\r\nOrig Code " + (int)e.KeyInfo.KeyChar
                                    + "\r\nKey# " + e.KeyCode 
                                    + ": " + (int)e.KeyCode;
                    }
                    else
                    {
                        winRead.Text = "Key " + e.Character 
                                    + "\r\nOur Code: " + (int)e.Character
                                    + "\r\n\r\nUse Arrows to move\r\n'movable' window";
                    }
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_winSubCyan_GotFocus( object sender, EventArgs e )
        {
            textBoxSubCyan.Text = "got focus";
        }

        private void EH_textBoxSubCyan_Validating( object sender, CancelEventArgs e )
        {
            if ( string.IsNullOrEmpty( textBoxSubCyan.Text.Trim () ) )
            {
                Application.ErrorMessage = "Should not be empty";
                Application.Beep ();
                e.Cancel = true;
            }
            else
            {
                Application.ErrorMessage = null;
            }
        }

        private void EH_winSubCyan_KeyDown( object sender, KeyEventArgs e )
        {
            Window w = sender as Window;

            switch( e.KeyCode )
            {
                case Keys.Escape:
                    w.UnloadParent ();
                    Menu.Focus ();
                    e.StopHandling ();
                    break;

                case Keys.Enter:
                    MdiClient.SelectNextControl( w );
                    e.StopHandling ();
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_winGrid_KeyDown( object sender, KeyEventArgs e )
        {
            Window w = sender as Window;

            switch( e.KeyCode )
            {
                case Keys.Escape:
                    w.UnloadParent ();
                    Menu.Focus ();
                    e.StopHandling ();
                    break;

                default:
                    if ( char.IsLetterOrDigit( e.Character ) )
                    {
                        foreach( TextBox gridElement in w.Parent.Children )
                        {
                            gridElement.Text = ( (int)e.KeyInfo.KeyChar ).ToString () 
                                + "->" + ( (int)e.Character ).ToString () 
                                + ": " + string.Empty.PadRight( 20, e.Character );
                        }

                        w.Text = string.Empty + e.Character;
                        e.StopHandling ();
                    }
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_winEditor_KeyDown( object sender, KeyEventArgs e )
        {
            TextBox w = sender as TextBox;

            switch( e.KeyCode )
            {
                case Keys.Escape:
                    w.Unload ();
                    Menu.Focus ();
                    break;

                case Keys.F2:
                    w.Maximize ();
                    try 
                    {
                        w.Text = System.IO.File.ReadAllText( "VRO-test.txt" );
                    } 
                    catch ( Exception ex )
                    {
                        w.CursorVisible = true;
                        w.ReadOnly = true;
                        w.Text = "\nError while loading file VRO-test.txt\n\n"
                               + ex.ToString ();
                    }
                    break;

                case Keys.F4:
                    w.Current += "test";
                    break;

                case Keys.F5:
                    w.CursorVisible = ! w.CursorVisible;
                    break;

                case Keys.F6:
                    w.ReadOnly = ! w.ReadOnly;
                    break;

                case Keys.F7:
                    w.Multiline = ! w.Multiline;
                    break;

                case Keys.F8:
                    w.SelectFullRows = ! w.SelectFullRows;
                    break;

                case Keys.Applications:
                    w.Unload ();
                    listBox.Parent = MdiClient;
                    listBox.Text = w.Text;
                    listBox.Move( w.Left, w.Top );
                    listBox.SetSize( w.Width, w.Height );
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void EH_listBox_KeyDown( object sender, KeyEventArgs e )
        {
            ListBox w = sender as ListBox;

            switch( e.KeyCode )
            {
                case Keys.Escape:
                    w.Unload ();
                    Menu.Focus ();
                    break;

                case Keys.F2:
                    w.Maximize ();
                    try 
                    {
                        w.Text = System.IO.File.ReadAllText( "VRO-test.txt" );
                    } 
                    catch ( Exception ex )
                    {
                        w.CursorVisible = true;
                        w.ReadOnly = true;
                        w.Text = "\nError while loading file VRO-test.txt\n\n"
                               + ex.ToString ();
                    }
                    break;

                case Keys.F4:
                    w.Current += "test";
                    break;

                case Keys.F6:
                    w.ReadOnly = ! w.ReadOnly;
                    break;

                case Keys.Applications:
                    w.Unload ();
                    textBoxEditor.Parent = MdiClient;
                    textBoxEditor.Text = w.Text;
                    textBoxEditor.Move( w.Left, w.Top );
                    textBoxEditor.SetSize( w.Width, w.Height );
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        void EH_buttonTest_Click( object sender, EventArgs e )
        {
            buttonTest.Unload ();
            buttonTest2.Unload ();
            Menu.Focus ();
        }
    }
}