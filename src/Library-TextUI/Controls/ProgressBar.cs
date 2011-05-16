/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  File:       ProgressBar.cs
 *  Created:    2011-04-03
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;

namespace TextUI.Controls
{
    using TextUI.Drawing;

    /// <summary>
    /// Represents a TextUI progress bar control.
    /// </summary>
    /// 
    public class ProgressBar : Control
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the minimum value of the range of the control.
        /// </summary>
        /// 
        public virtual double Minimum
        {
            get
            {
                return this.minimumValue;
            }
            set
            {
                InvalidateIf( value != this.minimumValue );
                this.minimumValue = value;
            }
        }

        private double minimumValue;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the maximum value of the range of the control.
        /// </summary>
        /// 
        public virtual double Maximum
        {
            get
            {
                return this.maximumValue;
            }
            set
            {
                InvalidateIf( value != this.maximumValue );
                this.maximumValue = value;
            }
        }

        private double maximumValue;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current position of the progress bar.
        /// </summary>
        /// 
        public virtual double Value
        {
            get
            {
                return this.currentValue;
            }
            set
            {
                if ( ! ReadOnly )
                {
                    InvalidateIf( value != this.currentValue );
                    this.currentValue = value;

                    ContentsChanged = true;
                    OnTextChanged ();
                }
            }
        }

        private double currentValue;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ProgressBar class.
        /// </summary>
        /// 
        public ProgressBar () 
            : base()
        {
            Minimum = 0;
            Maximum = 100;
            Value = 0;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Base Methods ]

        /// <summary>
        /// Raises the EraseBackground event.
        /// Suppresses EraseBackground as as OnDrawContents will overwrite complete area.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnEraseBackground( Screen screen )
        {
        }

        /// <summary>
        /// Raises the DrawContents event.
        /// </summary>
        /// <param name="screen">screen where the window is redrawn</param>
        /// <param name="hasFocus">true if the window is in application focus</param>
        /// 
        protected override void OnDrawContents( Screen screen, bool hasFocus )
        {
            int progressLen = (int) ( ClientWidth * ( currentValue - minimumValue ) 
                                                  / ( maximumValue - minimumValue ) );

            progressLen = Math.Max( 0, progressLen );
            progressLen = Math.Min( ClientWidth, progressLen );

            screen.Write( string.Empty.PadRight( progressLen, Box.Square ) );
            screen.Write( string.Empty.PadRight( ClientWidth - progressLen ) );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}