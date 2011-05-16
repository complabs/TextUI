/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       Button.cs
 *  Created:    2011-03-22
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;

using Mbk.Commons;

namespace TextUI.Controls
{
    using TextUI.Drawing;

    /// <summary>
    /// Represents a TextUI button control, which reacts to the Click event.
    /// </summary>
    /// 
    public class Button : ButtonBase
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Click Event Handler ]

        /// <summary>
        /// Occurs when a Button is clicked.
        /// </summary>
        /// 
        public event EventHandler Click = null;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Raises the Click event.
        /// </summary>
        /// 
        public virtual void OnClick ()
        {
            if ( Click != null )
            {
                Click( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Click handler is subscribed. 
        /// </summary>
        /// 
        public virtual bool HasClickHandler
        {
            get { return Click != null; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the Button class. 
        /// </summary>
        /// 
        public Button( string text = null ) 
            : base()
        {
            TabStop     = true;
            Border      = true;
            UseMnemonic = true;
            AutoSize    = true;
            TextAlign   = TextAlign.Center;
            Text        = text; // this will force resizing of the window
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ]

        /// <summary>
        /// Executed after the KeyDown event was raised and handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected override void OnAfterKeyDown( KeyEventArgs e )
        {
            switch( e.KeyCode )
            {
                /////////////////////////////////////////////////////////////////////////

                case Keys.Up:
                case Keys.Left:
                    if ( Parent != null )
                    {
                        Parent.SelectNextControl( this, /*forward*/ false );
                        e.StopHandling ();
                    }
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Down:
                case Keys.Right:
                    if ( Parent != null )
                    {
                        Parent.SelectNextControl( this );
                        e.StopHandling ();
                    }
                    break;

                /////////////////////////////////////////////////////////////////////////

                case Keys.Enter:

                    if ( Enabled )
                    {
                        OnClick ();
                    }

                    e.StopHandling ();
                    break;
            }

            base.OnAfterKeyDown( e );
        }

        #endregion
    }
}