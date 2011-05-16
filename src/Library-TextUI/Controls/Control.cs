/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       Control.cs
 *  Created:    2011-03-25
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

    /// <summary>
    /// Defines the base class for controls, which are TextUI components with visual 
    /// representation optionally providing user interaction.
    /// </summary>
    /// 
    public class Control : Window
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether unhandled key events are forwarded
        /// to the parent of the window.
        /// </summary>
        /// 
        public virtual bool ForwadKeysToParent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether contents of the control was changed
        /// by the user.
        /// </summary>
        /// 
        public virtual bool ContentsChanged { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets the Text property of the window and clears its ContentsChanged flag.
        /// </summary>
        /// 
        public virtual string InitText
        {
            set 
            { 
                this.Text = value;
                this.ContentsChanged = false;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether contents of the control is read-only.
        /// </summary>
        /// 
        public virtual bool ReadOnly
        {
            get
            {
                return this.readOnly;
            }
            set
            {
                InvalidateIf ( value != this.readOnly );
                this.readOnly = value;
            }
        }

        private bool readOnly;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the control can respond to user 
        /// interaction.
        /// </summary>
        /// 
        public virtual bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                InvalidateIf ( value != this.enabled );
                this.enabled = value;
            }
        }

        private bool enabled;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets tool tip text usually displayed at the bottom in the status
        /// bar when the window is in focus.
        /// </summary>
        /// 
        public override string ToolTipText
        {
            get
            {
                return base.ToolTipText == null && Parent != null
                     ? Parent.ToolTipText 
                     : base.ToolTipText;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handlers ]

        /// <summary>
        /// Occurs when the Text property value changes.
        /// </summary>
        /// 
        public event EventHandler TextChanged = null;

        /// <summary>
        /// Raises the TextChanged event.
        /// </summary>
        /// 
        protected virtual void OnTextChanged ()
        {
            if ( TextChanged != null )
            {
                TextChanged( this, EventArgs.Empty );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the Control class with default settings.
        /// </summary>
        /// 
        public Control ()
            : base ()
        {
            TabStop            = true;
            ForwadKeysToParent = true;
            Enabled            = true;
            ContentsChanged    = false;
            ReadOnly           = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Activates the control.
        /// </summary>
        /// 
        public virtual void Select ()
        {
            Focus ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets validation error message (to be visible in status bar).
        /// </summary>
        /// 
        public virtual string ErrorMessage
        {
            get { return Application.ErrorMessage; }
            set { Application.ErrorMessage = value; }
        }

        /// <summary>
        /// Gets or sets ifnormational message (to be visible in status bar).
        /// </summary>
        /// 
        public virtual string InfoMessage
        {
            get { return Application.InfoMessage; }
            set { Application.InfoMessage = value; }
        }

        /// <summary>
        /// Plays the sound of a beep.
        /// </summary>
        /// 
        public virtual void Beep ()
        {
            Application.Beep ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Prevents the control from drawing until the EndUpdate method is called. 
        /// </summary>
        /// 
        public virtual void BeginUpdate ()
        {
        }

        /// <summary>
        /// Resumes drawing of the control after drawing is suspended by the 
        /// BeginUpdate method. 
        /// </summary>
        /// 
        public virtual void EndUpdate ()
        {
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// <param name="source">An object source for the event (if not this window)
        /// </param>
        /// 
        protected override void OnKeyDown( KeyEventArgs e, 
            object source )
        {
            if ( source == null )
            {
                source = this;
            }

            base.OnKeyDown( e, source );

            if ( ! e.Handled && Parent != null && ForwadKeysToParent )
            {
                Parent.RaiseKeyDown( e, source );
            }
        }

        /// <summary>
        /// Executed after the KeyDown event was raised but not handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected override void OnAfterKeyDown( KeyEventArgs e )
        {
            base.OnAfterKeyDown( e );

            if ( ! e.Handled && Parent != null && ForwadKeysToParent )
            {
                Parent.RaiseAfterKeyDown( e );
            }
        }

        #endregion
    }
}