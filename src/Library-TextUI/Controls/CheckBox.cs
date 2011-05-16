/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  File:       CheckBox.cs
 *  Created:    2011-03-22
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace TextUI.Controls
{
    using TextUI.Drawing;

    /// <summary>
    /// Represents a TextUI CheckBox.
    /// </summary>
    /// 
    public class CheckBox : Control
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or set a value indicating whether the CheckBox is in the checked state.
        /// </summary>
        /// 
        public virtual bool Checked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                InvalidateIf( value != this.isChecked );
                this.isChecked = value;
                ContentsChanged = false;
            }
        }

        private bool isChecked;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the CheckBox class.
        /// </summary>
        /// 
        public CheckBox () 
            : base ()
        {
            Checked = false;
            Border    = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Toggles checked value.
        /// </summary>
        /// 
        private void ToggleValue ()
        {
            if ( ! ReadOnly )
            {
                Checked = ! Checked;

                ContentsChanged = true;
                OnTextChanged ();
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

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

            screen.ForeColor = hasFocus ? ForeColor : ForeColorInact;

            screen.Write( "[" );
            
            screen.ForeColor = hasFocus ? CaptionForeColor : CaptionForeColorInact;

            screen.Write( "" + ( Checked ? Box.Square : ' ' ) );
            
            screen.ForeColor = hasFocus ? ForeColor : ForeColorInact;

            screen.Write( "] " );
            screen.Write( Text );

            base.OnDrawContents( screen, hasFocus );
        }

        /// <summary>
        /// Executed after the KeyDown event was raised but not handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected override void OnAfterKeyDown ( KeyEventArgs e )
        {
            switch ( e.KeyCode )
            {
                case Keys.Enter:
                    if ( Parent != null )
                    {
                        Parent.SelectNextControl( this );
                        e.StopHandling ();
                    }
                    break;

                case Keys.Spacebar:
                    ToggleValue ();
                    e.StopHandling ();
                    break;
            }

            base.OnAfterKeyDown( e );
        }

        #endregion
    }
}