/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       GroupBox.cs
 *  Created:    2011-05-01
 *  Modified:   2011-05-01
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
    /// Represents a Windows control that displays a frame around a group of controls 
    /// with an optional caption.
    /// </summary>
    /// 
    public class GroupBox : Control
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets instance of the Label class holding an optional caption.
        /// </summary>
        /// 
        protected Label Label { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the access key (underlined letter) that allows user to quickly 
        /// navigate to the window.
        /// </summary>
        /// 
        public override AccessKey AccessKey
        {
            get
            {
                return Label != null ? Label.AccessKey : base.AccessKey;
            }
            set
            {
                Label.AccessKey = value;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        /// 
        public override string Text
        {
            get 
            { 
                return Label != null ? Label.Text : null;
            }
            set
            {
                if ( Label != null )
                {
                    Label.Text = " " + value + " ";
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the GroupBox class. 
        /// </summary>
        ///
        public GroupBox( string text = null ) 
            : base ()
        {
            Border = false;

            Label = new Label ()
            {
                Left = 1, Top = 0, TabStop = false,
                UseMnemonic = true, AutoSize = true, Text = text,
                Parent = this, 
            };
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

            if ( hasFocus )
            {
                screen.BackColor = BorderBackColor;
                screen.ForeColor = BorderForeColor;
            }
            else
            {
                screen.BackColor = BorderBackColorInact;
                screen.ForeColor = BorderForeColorInact;
            }

            screen.DrawRectangle( 0, 0, Width, Height );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

    }
}