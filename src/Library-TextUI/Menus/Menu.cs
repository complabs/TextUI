/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       Menu.cs
 *  Created:    2011-03-25
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

using Mbk.Commons;

namespace TextUI
{
    using TextUI.Drawing;

    /// <summary>
    /// Represents the base functionality for all menus. 
    /// </summary>
    /// 
    public class Menu : Window
    {
        #region [ Public Constants ]

        /// <summary>
        /// Text indicating menu separator.
        /// </summary>
        /// 
        public static readonly string Separator = "-";

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handlers ]

        /// <summary>
        /// Occurs when the user exits menu.
        /// </summary>
        /// 
        public event EventHandler ExitMenu = null;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether menu is main menu of a form.
        /// MainMenu are always horizontal, while others are only vertical.
        /// </summary>
        /// 
        public bool IsMainMenu { get; set; }

        /// <summary>
        /// Gets or sets padding between the left edge of the window and the text.
        /// </summary>
        /// 
        public int LeftPadding { get; set; }

        /// <summary>
        /// Gets or sets padding between the right edge of the window and the text.
        /// </summary>
        /// 
        public int RightPadding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the width and height of the control 
        /// automatically adjusts when the text assigned to the control is changed. 
        /// </summary>
        /// 
        public bool AutoSize { get; set; }

        /// <summary>
        /// Gets or sets horizontal spacing between items in MainMenu.
        /// </summary>
        /// 
        public int HorizontalSpacing { get; set; }

        /// <summary>
        /// Gets the width of the shortcut column.
        /// </summary>
        /// 
        public int ShortcutWidth { get; private set; }

        /// <summary>
        /// Gets the width of the text column.
        /// </summary>
        /// 
        public int TextWidth { get; private set; }

        /// <summary>
        /// Gets a value indicating whether menu's items has subitems.
        /// </summary>
        /// 
        public bool HasSubsubItems { get; private set; }

        /// <summary>
        /// Gets the parent menu.
        /// </summary>
        /// 
        public Menu Master { get; private set; }

        /// <summary>
        /// Gets a value indicating the collection of MenuItem objects associated 
        /// with the menu. 
        /// </summary>
        /// 
        public MenuItems MenuItems { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the text associated with the menu.
        /// </summary>
        /// 
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                InvalidateIf( base.Text != value );

                if ( value != null )
                {
                    value = TaggedText.ClearFromControlCharacters( value ).Text;
                    this.AccessKey = new AccessKey( value );
                    base.Text = this.AccessKey.Text;
                }
                else
                {
                    base.Text = null;
                    this.AccessKey = new AccessKey ();
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the index of the current menu item.
        /// </summary>
        /// 
        public int Current
        {
            get
            {
                return this.current;
            }
            set
            {
                value = Math.Max( 0, Math.Min( MenuItems.Count - 1, value ) );
                InvalidateIf( value != this.current );
                this.current = value;
            }
        }

        private int current;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the current menu item.
        /// </summary>
        /// 
        public MenuItem CurrentItem
        {
            get 
            { 
                return MenuItems != null && MenuItems.Count > 0 
                     ? MenuItems[ Current ] : null;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the background color used for the current item.
        /// </summary>
        /// 
        public virtual Color CurrentItemBackColor
        {
            get
            {
                return this.currentItemBackColor;
            }
            set
            {
                InvalidateIf( value != this.currentItemBackColor );
                this.currentItemBackColor = value;
            }
        }

        private Color currentItemBackColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the foreground color used for the current item.
        /// </summary>
        /// 
        public virtual Color CurrentItemForeColor
        {
            get
            {
                return this.currentItemForeColor;
            }
            set
            {
                InvalidateIf( value != this.currentItemForeColor );
                this.currentItemForeColor = value;
            }
        }

        private Color currentItemForeColor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets tool tip text, usually displayed in the status bar when
        /// the menu is active.
        /// </summary>
        /// 
        public override string ToolTipText
        {
            get
            {
                return CurrentItem != null ? CurrentItem.ToolTipText : base.ToolTipText;
            }
            set
            {
                if ( CurrentItem == null )
                {
                    base.ToolTipText = value;
                }
                else
                {
                    CurrentItem.ToolTipText = value;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether usor can move the cursor. 
        /// </summary>
        /// 
        public virtual bool CanMoveCursor
        {
            get
            {
                return this.canMoveCursor;
            }
            set
            {
                InvalidateIf ( value != this.canMoveCursor );
                this.canMoveCursor = value;
            }
        }

        private bool canMoveCursor;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a value indicating whether the window handles itself 
        /// erasing of its background. Returns always true.
        /// </summary>
        /// 
        public sealed override bool OwnErase
        {
            get { return true; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the Menu class. 
        /// </summary>
        /// 
        public Menu ()
            : base ()
        {
            IsMainMenu           = false;
            Master               = null;
            MenuItems            = new MenuItems( this );
            Current              = 0;
            LeftPadding          = 2;
            RightPadding         = 2;
            HorizontalSpacing    = 2;
            AutoSize             = true;
            CanMoveCursor        = true;

            BackColor            = Application.Theme.MenuBackColor;
            ForeColor            = Application.Theme.MenuForeColor;

            BackColorInact       = Application.Theme.MenuBackColorInact;
            ForeColorInact       = Application.Theme.MenuForeColorInact;

            CurrentItemBackColor = Application.Theme.MenuItemBackColor;
            CurrentItemForeColor = Application.Theme.MenuItemForeColor;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Gets screen position of the menu item.
        /// </summary>
        /// 
        private int GetPosition( int item )
        {
            int position = 0;

            for ( int i = 0; i < item && i < MenuItems.Count; ++i )
            {
                if ( IsMainMenu )
                {
                    position += MenuItems[ i ].Text.Length 
                              + LeftPadding + RightPadding + HorizontalSpacing;
                }
                else
                {
                    ++position;
                }
            }

            return position;
        }

        /// <summary>
        /// Raises Click event of the current item.
        /// </summary>
        /// 
        private void RaiseOnClick ()
        {
            if ( CurrentItem == null )
            {
                return;
            }

            Menu sub = CurrentItem;

            if ( sub != null && sub.MenuItems.Count > 0 
                && ( sub.Master == null || sub.Master == this ) )
            {
                sub.Current = 0;
                sub.Show( this );

                CurrentItem.OnClick( this );

                return;
            }

            HideBranch ();

            CurrentItem.OnClick( this );
        }

        /// <summary>
        /// Returns true if menu item is the last one in the menu.
        /// </summary>
        /// 
        private bool IsLast( MenuItem item )
        {
            return MenuItems.Count >= 1 && item == MenuItems[ MenuItems.Count - 1 ];
        }

        /// <summary>
        /// Returns true if menu item is the first one in the menu.
        /// </summary>
        /// 
        private bool IsFirst( MenuItem item )
        {
            return MenuItems.Count >= 1 && item == MenuItems[ 0 ];
        }

        /// <summary>
        /// Returns true if menu item is the current.
        /// </summary>
        /// 
        private bool IsCurrent( MenuItem item )
        {
            return MenuItems.Count >= 1 && item == MenuItems[ Current ];
        }

        /// <summary>
        /// Moves cursor (sets curent item) to first selectable (enabled) menu item.
        /// </summary>
        /// 
        private void MoveToFirstSelectable ()
        {
            for ( int i = 0; i < MenuItems.Count; ++i )
            {
                if ( MenuItems[ i ].Enabled && MenuItems[ i ].Text != Separator )
                {
                    Current = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Advances cursor to next selectable menu item.
        /// </summary>
        /// 
        private void MoveNext ()
        {
            if ( CanMoveCursor )
            {
                for ( int i = Current + 1; i < MenuItems.Count; ++i )
                {
                    if ( MenuItems[ i ].Enabled && MenuItems[ i ].Text != Separator )
                    {
                        Current = i;
                        return;
                    }
                }

                for ( int i = 0; i < Current; ++i )
                {
                    if ( MenuItems[ i ].Enabled && MenuItems[ i ].Text != Separator )
                    {
                        Current = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Moves cursor to the previous selectable menu item.
        /// </summary>
        /// 
        private void MovePrevious ()
        {
            if ( CanMoveCursor )
            {
                for ( int i = Current - 1; i >= 0; --i )
                {
                    if ( MenuItems[ i ].Enabled && MenuItems[ i ].Text != Separator )
                    {
                        Current = i;
                        return;
                    }
                }

                for ( int i = MenuItems.Count - 1; i > Current; --i )
                {
                    if ( MenuItems[ i ].Enabled && MenuItems[ i ].Text != Separator )
                    {
                        Current = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Hides the whole branch of open menus and submenus.
        /// </summary>
        /// 
        private void HideBranch ()
        {
            string savedErrorMessage = Application.ErrorMessage;

            // Find the deepest current sub menu
            //
            Menu current = this;
            while( current.CurrentItem != null && current.CurrentItem.Parent != null )
            {
                current = current.CurrentItem;
            }

            // Reverse back (while not reaching the top menu) and unlink Menu's linked 
            // as SubMenu's in MenuItems from parent menus.
            //
            while( current.Master != null )
            {
                Menu oldMaster = current.Master;

                current.Master = null;
                current.Unload ();

                current = oldMaster;
            }

            if ( Application.ErrorMessage == null )
            {
                Application.ErrorMessage = savedErrorMessage;
            }
        }

        /// <summary>
        /// Recalculates menu size based on the menu items.
        /// </summary>
        /// 
        private void ResizeToFitContents ()
        {
            if ( ! MenuItems.StructureChanged )
            {
                return;
            }

            // Remove improperly placed separators
            //
            if ( IsMainMenu )
            {
                // Remove all separators from horizontal menu 
                //
                while( true )
                {
                    MenuItem mi = MenuItems.Find( item => item.Text == Separator );
                    if ( mi == null )
                    {
                        break;
                    }

                    MenuItems.Remove( mi );
                }
            }
            else
            {
                // Remove separators at the first and last positions in vertical menu
                //
                while( MenuItems.Count > 0 && MenuItems[ 0 ].Text == Separator )
                {
                    MenuItems.RemoveAt( 0 );
                }

                while( MenuItems.Count > 0 
                    && MenuItems[ MenuItems.Count -1  ].Text == Separator )
                {
                    MenuItems.RemoveAt( MenuItems.Count - 1 );
                }
            }

            Current += 0; // revalidate current

            MenuItems.AcknolwedgeStructureChange ();

            // Calculate maximum dimensions of menu items
            //
            TextWidth = 0;
            ShortcutWidth = 0;
            HasSubsubItems = false;
            int position = 0;

            foreach( MenuItem mi in MenuItems )
            {
                HasSubsubItems = HasSubsubItems || mi.MenuItems.Count > 0;

                if ( IsMainMenu && mi.Text != Separator )
                {
                    position += mi.Text.Length 
                              + LeftPadding + RightPadding + HorizontalSpacing;
                    TextWidth = position;
                }
                else if ( ! IsMainMenu )
                {
                    ++position;
                    TextWidth = Math.Max( TextWidth, mi.Text.Length );
                    ShortcutWidth = Math.Max( ShortcutWidth, mi.VerboseShortcut.Length );
                }
            }

            // Reset position relative to master, if any
            //
            if ( Master != null && Master.CurrentItem != null )
            {
                if ( Master.IsMainMenu )
                {
                    Left = Master.Left + Master.GetPosition( Master.Current );
                    Top  = Master.Top  + 2;
                }
                else
                {
                    Left = Master.Left + Master.Width + 1;
                    Top  = Master.Top  + Master.GetPosition( Master.Current );
                }
            }

            // Finally, resize window to reflect menu items' dimensions
            //
            if ( IsMainMenu )
            {
                Width  = Math.Max( Width,  Math.Max( position, 1 ) );
                Height = Math.Max( Height, 1 );
            }
            else
            {
                int width = TextWidth + ShortcutWidth + LeftPadding + RightPadding 
                          + ( ShortcutWidth > 0 ? HorizontalSpacing : 0 )
                          + ( HasSubsubItems ? 1 : 0 );

                if ( AutoSize )
                {
                    Width  = Math.Max( Width,  Math.Max( width,    1 ) );
                    Height = Math.Max( Height, Math.Max( position, 1 ) );
                }
                else
                {
                    ShortcutWidth = Math.Max( 0, Width - width - HorizontalSpacing );
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Hides menu (unloads menu from its parent).
        /// </summary>
        /// 
        public void Hide ()
        {
            string savedErrorMessage = Application.ErrorMessage;

            Master = null;
            Unload ();

            if ( Application.ErrorMessage == null )
            {
                Application.ErrorMessage = savedErrorMessage;
            }
        }

        /// <summary>
        /// Displays menu on the screen.
        /// </summary>
        /// 
        public virtual void Show( Menu master = null )
        {
            Master = master;

            if ( Master != null )
            {
                Parent = Master.Parent;
            }

            ResizeToFitContents ();
            MoveToFirstSelectable ();
            Focus ();
            Invalidate ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Raises Click event for subitem matching specified mnemonic character.
        /// </summary>
        /// 
        public bool ProcessAccessKey( char character )
        {
            for( int i = 0; i < MenuItems.Count; ++i )
            {
                MenuItem mi = MenuItems[ i ];

                if ( mi.AccessKey == character )
                {
                    // Close sibling menu items that are currently open
                    //
                    foreach( MenuItem j in MenuItems )
                    {
                        if ( j.MenuItems.Count > 0 )
                        {
                            j.Hide ();
                        }
                    }

                    // Setup selected menu item as current (if can change focus)
                    //
                    if ( Focus () )
                    {
                        Current = i;
                        RaiseOnClick ();
                        return true;
                    }

                    return true;
                }
            }

            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Raises Click event for subitem matching shortcut key.
        /// </summary>
        /// 
        public void ProcessShortctuKey( KeyEventArgs e )
        {
            if ( e.Handled )
            {
                return;
            }

            int shortcut = (int) e.KeyCode | ( (int)e.Modifiers << 16 );

            foreach( MenuItem mi in MenuItems )
            {
                if ( (int)mi.Shortcut == shortcut )
                {
                    mi.OnClick( this );
                    e.Handled = true;
                    return;
                }
            }

            foreach( MenuItem mi in MenuItems )
            {
                mi.ProcessShortctuKey( e );
                if ( e.Handled )
                {
                    return;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Raises the Exit event.
        /// </summary>
        /// 
        protected virtual void OnExit ()
        {
            if ( ExitMenu != null )
            {
                ExitMenu( this, EventArgs.Empty );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Recalculates window size. <see cref="Window.OnCalculateSize"/>
        /// </summary>
        ///
        protected override void OnCalculateSize( bool hasFocus )
        {
            ResizeToFitContents ();

            base.OnCalculateSize( hasFocus );
        }

        /// <summary>
        /// Raises the EraseBackground event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnEraseBackground( Drawing.Screen screen )
        {
            if ( IsMainMenu )
            {
                screen.BackColor = BackColorInact;
                screen.ForeColor = ForeColorInact;
                screen.Clear ();
            }

            base.OnEraseBackground( screen );
        }

        /// <summary>
        /// Raises the DrawContents event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnDrawContents( Screen screen, bool hasFocus )
        {
            if ( screen == null )
            {
                return;
            }

            int position = 0;

            for ( int i = 0; i < MenuItems.Count; ++i )
            {
                MenuItem mi = MenuItems[ i ];

                // Fill separator between shortcut key and menu item with
                // spaces up to a common menu width (so shortcut key info is
                // right aligned).
                //
                string text = mi.Text;

                // Setup default colors depending on focus and selected status
                //
                if ( i == Current && hasFocus )
                {
                    screen.BackColor = CurrentItemBackColor;
                    screen.ForeColor = CurrentItemForeColor;
                }
                else if ( i == Current ) // Current not in focus
                {
                    screen.BackColor = BackColor;
                    screen.ForeColor = ForeColor;
                }
                else if ( mi.Enabled && text != Separator )
                {
                    screen.BackColor = BackColorInact;
                    screen.ForeColor = ForeColorInact;
                }
                else
                {
                    screen.BackColor = BackColorInact;
                    screen.ForeColor = Color.DarkGray;
                }

                // If the back color is dark gray, turn of highlight component of the
                // short-key foreground color. Consider to have a separate property for
                // short-key color when active and inactive.
                //
                Color keyColor = AccessKeyForeColor;
                if ( screen.BackColor == Color.DarkGray )
                {
                    keyColor &= ~Color.Gray;
                }

                if ( IsMainMenu && text != Separator )
                {
                    screen.CursorTop = Height / 2;
                    screen.CursorLeft = position;

                    screen.Write( string.Empty.PadLeft( LeftPadding ) );
                    screen.Write( text );
                    screen.Write( string.Empty.PadRight( RightPadding ) );

                    if ( mi.AccessKey && mi.Enabled && text != Separator )
                    {
                        screen.ForeColor = keyColor;
                        screen.CursorTop = Height / 2;
                        screen.CursorLeft = LeftPadding + mi.AccessKey.Position
                                          + position;
                        screen.Write( text.Substring( mi.AccessKey.Position, 1 ) );
                    }

                    position += text.Length 
                              + LeftPadding + RightPadding + HorizontalSpacing;
                }
                else if ( ! IsMainMenu )
                {
                    if ( text == Separator )
                    {
                        if ( ! hasFocus )
                        {
                            screen.ForeColor = BorderForeColorInact;
                        }

                        screen.DrawRectangle( 0, position, Width, 1 );
                    }
                    else
                    {
                        string checkBox = mi.Checked ? "*" : string.Empty;
                        string shorcut = mi.VerboseShortcut;

                        screen.CursorTop = position;
                        screen.CursorLeft = 0;

                        screen.Write( checkBox.PadRight( LeftPadding ) );
                        screen.Write( text.PadRight( TextWidth ) );

                        if ( ShortcutWidth > 0 )
                        {
                            screen.Write( string.Empty.PadRight( HorizontalSpacing )  );
                            screen.Write( shorcut.PadRight( ShortcutWidth ) );
                        }

                        screen.Write( string.Empty.PadRight( RightPadding + 1 ) );
                    }

                    if ( mi.MenuItems.Count > 0 )
                    {
                        if ( i == Current && ! hasFocus )
                        {
                            screen.ForeColor = mi.BorderForeColor;
                        }

                        screen.CursorLeft = Width - 2;
                        screen.Write( string.Empty + Box.Right );
                    }

                    if ( mi.AccessKey && mi.Enabled && text != Separator )
                    {
                        screen.ForeColor = keyColor;
                        screen.CursorTop = position;
                        screen.CursorLeft = LeftPadding + mi.AccessKey.Position;
                        screen.Write( text.Substring( mi.AccessKey.Position, 1 ) );
                    }

                    ++position;
                }
            }
        }

        /// <summary>
        /// Executed after the KeyDown event was raised but not handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected override void OnAfterKeyDown ( KeyEventArgs e )
        {
            if ( e.Handled )
            {
                return;
            }

            switch( e.KeyCode )
            {
                /////////////////////////////////////////////////////////////////////////

                case Keys.Tab:

                    if ( e.Shift )
                    {
                        MovePrevious ();
                    }
                    else
                    {
                        MoveNext ();
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Left:

                    if ( IsMainMenu )
                    {
                        MovePrevious ();
                    }
                    else if ( Master != null && ! Master.IsMainMenu )
                    {
                        Hide ();
                    }
                    else if ( Master != null && Master.IsMainMenu )
                    {
                        Menu mainMenu = Master;
                        HideBranch ();
                        mainMenu.MovePrevious ();
                        mainMenu.RaiseOnClick ();
                    }
                    else
                    {
                        Hide ();
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Right:

                    if ( IsMainMenu )
                    {
                        MoveNext ();
                    }
                    else if ( ! IsMainMenu 
                        && CurrentItem != null && CurrentItem.MenuItems.Count > 0 )
                    {
                        CurrentItem.Current = 0;
                        CurrentItem.Show( this );
                    }
                    else if ( Master != null && Master.IsMainMenu )
                    {
                        Menu mainMenu = Master;
                        HideBranch ();
                        mainMenu.MoveNext ();
                        mainMenu.RaiseOnClick ();
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Up:

                    if ( ! IsMainMenu )
                    {
                        MovePrevious ();
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Down:

                    if ( ! IsMainMenu )
                    {
                        MoveNext ();
                    }
                    else if ( CurrentItem.MenuItems.Count > 0
                        && ( CurrentItem.Master == null || CurrentItem.Master == this )
                        )
                    {
                        CurrentItem.Current = 0;
                        CurrentItem.Show( this );
                    }

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Enter:

                    RaiseOnClick ();

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Escape:

                    if ( Master != null )
                    {
                        Hide ();
                    }

                    OnExit ();

                    e.StopHandling ();
                    break;

                /////////////////////////////////////////////////////////////////////////

                default:

                    if ( ! char.IsControl( e.Character ) )
                    {
                        if ( ProcessAccessKey( e.Character ) )
                        {
                            e.StopHandling ();
                        }
                    }
                    break;
            }

            if ( ! e.Handled )
            {
                // Find master at the top and execute its key event handler if it
                // is main menu.
                //
                Menu top = this;
                while ( top.Master != null )
                {
                    top = top.Master;
                }

                if ( top.IsMainMenu )
                {
                    top.OnKeyDown( e );

                    if ( e.Handled )
                    {
                        HideBranch ();
                    }
                }
            }

            base.OnAfterKeyDown( e );
        }

        #endregion
    }
}