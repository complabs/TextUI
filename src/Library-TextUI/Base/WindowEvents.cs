/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       WindowEvents.cs
 *  Created:    2011-03-24
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.ComponentModel;

using Mbk.Commons;

namespace TextUI
{
    using TextUI.Drawing;

    /// <remarks>
    /// WindowEvents.cs contains event definitions and method used to raise events.
    /// </remarks>
    /// 
    public partial class Window
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handlers ]

        /// <summary>
        /// Occurs when a key is pressed while the control has focu
        /// </summary>
        /// 
        public event KeyEventHandler KeyDown = null;

        /// <summary>
        /// Occurs when the client area of the window is redrawn.
        /// </summary>
        /// 
        public event DrawEventHandler DrawContents = null;

        /// <summary>
        /// Occurs when the border of the window is redrawn. 
        /// </summary>
        public event DrawEventHandler DrawBorder = null;

        /// <summary>
        /// Occurs when the background of the window is being erased. 
        /// </summary>
        /// 
        public event DrawEventHandler EraseBackground = null;

        /// <summary>
        /// Occurs when the windows is loaded (becomes child of parent).
        /// </summary>
        /// 
        public event EventHandler WindowLoaded = null;

        /// <summary>
        /// Occurs when the window is unloaded (becomes disconnected from its parent).
        /// </summary>
        /// 
        public event EventHandler WindowUnloaded = null;

        /// <summary>
        /// Occurs when the control is resized.
        /// </summary>
        /// 
        public event EventHandler Resize = null;

        /// <summary>
        /// Occurs when the window is validating. 
        /// </summary>
        public event CancelEventHandler Validating = null;

        /// <summary>
        /// Occurs when the window is finished validating. 
        /// </summary>
        public event EventHandler Validated = null;

        /// <summary>
        /// Occurs when the window loses focus. 
        /// </summary>
        public event EventHandler LostFocus = null;

        /// <summary>
        /// Occurs when the window receives focus.
        /// </summary>
        /// 
        public event EventHandler GotFocus = null;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handler Raising Methods ]

        /// <summary>
        /// Raises the WindowLoaded event.
        /// </summary>
        /// 
        protected virtual void OnLoad ()
        {
            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT WindowLoaded -> {0}", Name );
            } );

            if ( WindowLoaded != null )
            {
                WindowLoaded( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the WindowUnloaded event.
        /// </summary>
        /// 
        protected virtual void OnUnload ()
        {
            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT WindowUnLoaded -> {0}", Name );
            } );

            if ( WindowUnloaded != null )
            {
                WindowUnloaded( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// 
        protected virtual void OnResize ()
        {
            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT Resize -> {0}", Name );
            } );

            if ( Resize != null )
            {
                Resize( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the GotFocus event.
        /// </summary>
        /// 
        protected virtual void OnGotFocus ()
        {
            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT GotFocus -> {0}", Name );
            } );

            if ( GotFocus != null )
            {
                GotFocus( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the LostFocus event.
        /// </summary>
        /// 
        protected virtual void OnLostFocus ()
        {
            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT LostFocus -> {0}", Name );
            } );

            if ( LostFocus != null )
            {
                LostFocus( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the Validating event.
        /// </summary>
        /// <returns>false if some of Validating event handlers returns Cancel</returns>
        /// 
        protected virtual bool OnValidating ()
        {
            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT Validating -> {0}", Name );
            } );

            Application.ErrorMessage = null;

            if ( Validating != null )
            {
                CancelEventArgs e = new CancelEventArgs () { Cancel = false }; 
                Validating( this, e );
                return ! e.Cancel;
            }

            return true;
        }

        /// <summary>
        /// Raises the Validated event.
        /// </summary>
        /// 
        protected virtual void OnValidated ()
        {
            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT Validated -> {0}", Name );
            } );

            if ( Validated != null )
            {
                Validated( this, EventArgs.Empty );
            }
        }

        /// <summary>
        /// Raises the DrawBorder event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected virtual void OnDrawBorder( Screen screen, bool hasFocus )
        {
            if ( screen == null )
            {
                return;
            }

            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT DrawBorder -> {0}", Name );
            } );

            if ( ! OwnDrawBorder )
            {
                DefaultDrawBorder( screen, hasFocus );
            }

            if ( DrawBorder != null )
            {
                DrawBorder( this, new DrawEventArgs () { Screen = screen } );
            }
        }

        /// <summary>
        /// Raises the EraseBackground event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected virtual void OnEraseBackground( Screen screen )
        {
            if ( screen == null )
            {
                return;
            }

            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT EraseBackground -> {0}", Name );
            } );

            if ( ! OwnErase )
            {
                screen.Clear ();
            }

            if ( EraseBackground != null )
            {
                EraseBackground( this, 
                    new DrawEventArgs () { Screen = screen }
                    );
            }
        }

        /// <summary>
        /// Raises the DrawContents event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected virtual void OnDrawContents( Screen screen, bool hasFocus )
        {
            if ( screen == null )
            {
                return;
            }

            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT DrawContents -> {0}", Name );
            } );

            if ( DrawContents != null )
            {
                DrawContents( this, new 
                    DrawEventArgs () { Screen = screen, HasFocus = hasFocus } );
            }
        }

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// <param name="source">An object source for the event (if not this window)
        /// </param>
        /// 
        protected virtual void OnKeyDown( KeyEventArgs e, object source = null ) 
        {
            if ( e.Handled )
            {
                return;
            }

            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( 
                    "EVENT KeyDown -> {0}, Key = {1}, Char = {2}", 
                    Name, e.KeyCode.ToString (), (int)e.Character );
            } );

            if ( source == null )
            {
                source = this;
            }

            if ( KeyDown != null )
            {
                KeyDown( source, e );
            }
        }

        /// <summary>
        /// Executed after the KeyDown event was raised but not handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected virtual void OnAfterKeyDown( KeyEventArgs e )
        {
            if ( e.Handled )
            {
                return;
            }

            Debug.IfTracing( TraceFlag.Events, delegate
            {
                Debug.TraceLine( "EVENT AfterKeyDown -> {0}, Key = {1}, Char = {2}", 
                    Name, e.KeyCode.ToString (), (int)e.Character );
            } );

            // Default behaviour:
            //
            if ( e.KeyCode == Keys.Attention )
            {
                throw new QuitMessageLoop () 
                { 
                    Reason = string.Format( "Exit on {0}", e.ControlEvent )
                };
            }
            else if ( e.Control && e.KeyCode == Keys.R )
            {
                Root.Invalidate ();
                e.StopHandling  ();
            }
            else if ( e.Control && e.KeyCode == Keys.L )
            {
                Application.FullRepaint ();
                e.StopHandling  ();
            }
            else if ( e.Modifiers == 0 && e.KeyCode == Keys.Tab )
            {
                if ( Parent != null )
                {
                    Parent.SelectNextControl( this );
                    e.StopHandling  ();
                }
            }
            else if ( e.Shift && e.KeyCode == Keys.Tab )
            {
                if ( Parent != null )
                {
                    Parent.SelectNextControl( this, /*forward*/ false );
                    e.StopHandling  ();
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Methods Exposing Event Handlers Internally (inside the library) ]

        // As all event raiser methods are protected, they cannot be seen
        // or called outside from derived classes. All the following event handlers
        // are raised somewhere internally (i.e. sources are outside the Window class
        // but somewhere inside the library).

        /// <summary>
        /// Raises the WindowLoad event.
        /// </summary>
        /// <remarks>
        /// Visible internally in library.
        /// </remarks>
        /// 
        internal void RaiseLoadEvent ()
        {
            OnLoad ();
        }

        /// <summary>
        /// Raises the WindowUnload event.
        /// </summary>
        /// <remarks>
        /// Visible internally in library.
        /// </remarks>
        /// 
        internal void RaiseUnloadEvent ()
        {
            OnUnload ();
        }

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        /// <remarks>
        /// Visible internally in library.
        /// </remarks>
        /// 
        internal void RaiseKeyDown( KeyEventArgs e, object source = null )
        {
            OnKeyDown( e, source );
        }

        /// <summary>
        /// Raises the AfterKeyDown event.
        /// </summary>
        /// <remarks>
        /// Visible internally in library.
        /// </remarks>
        /// 
        internal void RaiseAfterKeyDown( KeyEventArgs e )
        {
            OnAfterKeyDown( e );
        }

        #endregion
    }
}