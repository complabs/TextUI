/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       MenuItem.cs
 *  Created:    2011-03-25
 *  Modified:   2011-04-26
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
    /// <summary>
    /// Represents an individual item that is displayed within a Menu or MainMenu.
    /// </summary>
    /// 
    public class MenuItem : Menu
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handlers ]

        /// <summary>
        /// Occurs when the menu item is clicked or selected using a shortcut key or 
        /// access key defined for the menu item.
        /// </summary>
        /// 
        public event EventHandler Click = null;

        /// <summary>
        /// Occurs before a menu item's list of menu items is displayed. 
        /// </summary>
        /// 
        public event EventHandler Popup = null;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether the menu item is enabled. 
        /// </summary>
        /// 
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a check mark appears next to the
        /// text of the menu item. 
        /// </summary>
        /// 
        public bool Checked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the shortcut key associated with the 
        /// menu item. 
        /// </summary>
        /// 
        public Shortcut Shortcut { get; set; }

        /// <summary>
        /// Gets a human readable representation of the shortcut key.
        /// E.g. "Ctrl+S" for Shortcut.CtrlS
        /// </summary>
        /// 
        public string VerboseShortcut
        {
            get
            {
                return Shortcut == Shortcut.None ? string.Empty : Shortcut.Verbose ();
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the MenuItem class with a specified caption
        /// for the menu item. 
        /// </summary>
        /// <param name="caption">caption for the menu item</param>
        /// 
        public MenuItem( string caption ) 
            : base ()
        {
            Text       = caption; // this will set ShortkeyPosition as well
            Enabled    = true;
            IsMainMenu = false;
            Border     = true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Protected Virtual Methods ]

        /// <summary>
        /// Raises the Click event.
        /// </summary>
        ///
        internal protected virtual void OnClick( Menu owner )
        {
            if ( Click != null )
            {
                Click( owner, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the Popup event.
        /// </summary>
        /// 
        internal protected virtual void OnPopup ()
        {
            if ( Popup != null )
            {
                Popup( this, EventArgs.Empty );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Displays menu item on the screen.
        /// </summary>
        ///
        public override void Show( Menu master = null )
        {
            OnPopup ();

            base.Show( master );
        }

        #endregion
    }
}