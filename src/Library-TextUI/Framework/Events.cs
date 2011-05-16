/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       Events.cs
 *  Created:    2011-03-20
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.ComponentModel;

namespace TextUI
{
    using TextUI.Drawing;
    using TextUI.Controls;
    using TextUI.ConsoleIO;

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Represents the method that will handle the KeyUp event of a TextUI window.
    /// </summary>
    /// 
    public delegate void KeyEventHandler( object sender, KeyEventArgs e );

    /// <summary>
    /// Represents the method that will handle the DrawContents event of a TextUI window. 
    /// </summary>
    /// 
    public delegate void DrawEventHandler( object sender, DrawEventArgs e );

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Console CTRL-event types. 
    /// See SetConsoleCtrlHandler() Windows API functio:
    /// http://msdn.microsoft.com/en-us/library/ms686016.aspx
    /// </summary>
    /// 
    public enum CtrlEventTypes  
    {  
        CTRL_C_EVENT        = 0,
        CTRL_BREAK_EVENT    = 1,
        CTRL_CLOSE_EVENT    = 2,
        CTRL_LOGOFF_EVENT   = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }  

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Provides data for the KeyDown or AfterKeyDown event.
    /// </summary>
    /// 
    public class KeyEventArgs : HandledEventArgs
    {
        /// <summary>
        /// Gets the ConsoleKeyInfo for a KeyDown event.
        /// </summary>
        /// 
        public ConsoleKeyInfo KeyInfo { get; set; }

        /// <summary>
        /// Gets the character for a KeyDown event.
        /// </summary>
        /// 
        public char Character { get; set; }

        /// <summary>
        /// Gets Console CTRL-event for a KeyDown event.
        /// </summary>
        /// 
        public CtrlEventTypes ControlEvent { get; set; }

        /// <summary>
        /// Gets the keyboard code for a KeyDown event. 
        /// </summary>
        /// 
        public Keys KeyCode
        {
            get { return (Keys)KeyInfo.Key; }
        }

        /// <summary>
        /// Gets the modifier flags for a KeyDown event. 
        /// The flags indicate which combination of CTRL, SHIFT, and ALT keys 
        /// was pressed. 
        /// </summary>
        /// 
        public ConsoleModifiers Modifiers
        {
            get { return KeyInfo.Modifiers; }
        }

        /// <summary>
        /// Gets a value indicating whether the CTRL key was pressed. 
        /// </summary>
        /// 
        public bool Control 
        { 
            get { return ( KeyInfo.Modifiers & ConsoleModifiers.Control ) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the ALT key was pressed. 
        /// </summary>
        /// 
        public bool Alt
        { 
            get { return ( KeyInfo.Modifiers & ConsoleModifiers.Alt ) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the SHIFT key was pressed. 
        /// </summary>
        /// 
        public bool Shift
        {
            get { return ( KeyInfo.Modifiers & ConsoleModifiers.Shift ) != 0; }
        }

        /// <summary>
        /// Sets a Handled value to true indicating that the event was handled.
        /// </summary>
        /// 
        public void StopHandling ()
        { 
            Handled = true;
            KeyInfo = new ConsoleKeyInfo ();
            Character = '\0';
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Provides data for the DrawContents event.
    /// </summary>
    /// 
    public class DrawEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the screen where the contents of the window should be drawn to.
        /// </summary>
        /// 
        public Screen Screen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether repainted window has focus.
        /// </summary>
        /// 
        public bool HasFocus { get; set; }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Represents the message used to break TextUI messsage loops.
    /// </summary>
    /// 
    public class QuitMessageLoop : Exception
    {
        /// <summary>
        /// Gets or sets the result object returned as the result of the modal
        /// message loop in dialog (e.g. in MessageBox).
        /// </summary>
        /// 
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets the termination reason.
        /// </summary>
        /// 
        public string Reason { get; set; }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Represents the message used to restarts TextUI message loops 
    /// also reloading the Screen with the new root window.
    /// </summary>
    /// 
    public class RestartMessageLoop : Exception
    {
        public Window NewRootWindow { get; set; }
    }
}