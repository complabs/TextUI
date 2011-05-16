/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Controls
 *  File:       ButtonBase.cs
 *  Created:    2011-03-16
 *  Modified:   2011-05-01
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
    /// Implements the basic functionality common to button controls.
    /// </summary>
    /// 
    public class ButtonBase : Control
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets horizontal padding within the control.  
        /// </summary>
        /// 
        public virtual int HorizontalPadding
        {
            get
            {
                return this.horizontalPadding;
            }
            set
            {
                InvalidateIf( value != this.horizontalPadding );
                this.horizontalPadding = value;

                ResizeToFitContents ();
            }
        }

        private int horizontalPadding;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets vertical padding within the control. 
        /// </summary>
        /// 
        public virtual int VerticalPadding
        {
            get
            {
                return this.verticalPadding;
            }
            set
            {
                InvalidateIf( value != this.verticalPadding );
                this.verticalPadding = value;

                ResizeToFitContents ();
            }
        }

        private int verticalPadding;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the width and height of the control 
        /// automatically adjusts when the text assigned to the control is changed. 
        /// </summary>
        /// 
        public virtual bool AutoSize
        {
            get
            {
                return this.autoSize;
            }
            set
            {
                InvalidateIf( value != this.autoSize );
                this.autoSize = value;

                ResizeToFitContents ();
            }
        }

        private bool autoSize;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets a value indicating whether the control interprets an ampersand 
        /// character (&) in the control's Text property to be an access key 
        /// prefix character.
        /// </summary>
        /// 
        public virtual bool UseMnemonic
        {
            get
            {
                return this.useMnemonic;
            }
            set
            {
                InvalidateIf( value != this.useMnemonic );
                this.useMnemonic = value;
            }
        }

        private bool useMnemonic;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets how text is aligned in the control.
        /// </summary>
        /// 
        public virtual TextAlign TextAlign
        {
            get
            {
                return this.textAlign;
            }
            set
            {
                InvalidateIf( value != this.textAlign );
                this.textAlign = value;
            }
        }

        private TextAlign textAlign;

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current text in the control. 
        /// </summary>
        /// 
        public override string Text
        {
            get
            {
                return TaggedText.Join( "\n", this.Lines );
            }
            set
            {
                base.Text = value;
                this.LineWithAccessKey = -1;
                this.AccessKey = new AccessKey ();

                int maxLineWidth = 0;
                this.Lines = TaggedText.SplitTextInLines( value, ref maxLineWidth );

                // Find access key and recalculate max line width.
                //
                this.MaxLineWidth = 0;

                for( int i = 0; i < this.Lines.Count; ++i )
                {
                    if ( UseMnemonic && ! this.AccessKey )
                    {
                        this.AccessKey = new AccessKey( this.Lines[ i ].Text );

                        if ( this.AccessKey )
                        {
                            this.Lines[ i ] = this.Lines[ i ]
                                            .Replace( this.AccessKey.Text );
                            this.LineWithAccessKey = i;
                        }
                    }

                    if (this.Lines[ i ].Text.Length > this.MaxLineWidth )
                    {
                        this.MaxLineWidth = this.Lines[ i ].Text.Length;
                    }
                }

                Invalidate ();
                ResizeToFitContents ();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the lines of text in a control.
        /// </summary>
        /// 
        protected TaggedTextCollection Lines { get; private set; }

        /// <summary>
        /// Gets the maximum line length in the control's Text.
        /// </summary>
        /// 
        protected int MaxLineWidth { get; private set; }

        /// <summary>
        /// Gets a line number holding highlighted AccessKey.
        /// </summary>
        /// 
        protected int LineWithAccessKey { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the ButtonBase class. 
        /// </summary>
        ///
        public ButtonBase( string text = null ) 
            : base ()
        {
            HorizontalPadding = 0;
            VerticalPadding   = 0;

            TabStop        = false;
            Border         = false;
            UseMnemonic    = false;
            AutoSize       = false;
            TextAlign      = TextAlign.Left;

            BackColor      = Application.Theme.ButtonBackColor;
            ForeColor      = Application.Theme.ButtonForeColor;
            BackColorInact = Application.Theme.ButtonBackColorInact;
            ForeColorInact = Application.Theme.ButtonForeColorInact;

            Text = text; // this will initialize maxLineWidth and lines[]
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Facilitates AutoSize property.
        /// </summary>
        /// 
        private void ResizeToFitContents ()
        {
            if ( AutoSize )
            {
                Width  = this.MaxLineWidth + 2 * HorizontalPadding;
                Height = this.Lines.Count  + 2 * VerticalPadding;
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

            int topStart = VerticalPadding;

            if ( ! Enabled )
            {
                screen.ForeColor = BorderForeColorInact;
            }

            for ( int i = 0; i < this.Lines.Count; ++i )
            {
                string text = this.Lines[ i ].AlignedText( ClientWidth, TextAlign );

                screen.CursorTop = topStart + i;
                screen.CursorLeft = 0;
                screen.Write( text );

                if ( this.LineWithAccessKey == i && AccessKey && Enabled )
                {
                    Color savedForeColor = screen.ForeColor;

                    // If the back color is dark gray, turn of highlight component of the
                    // short-key foreground color. Consider to have a separate property 
                    // for short-key color when active and inactive.
                    //
                    Color keyColor = AccessKeyForeColor;
                    if ( screen.BackColor == Color.DarkGray )
                    {
                        keyColor &= ~Color.Gray;
                    }

                    string line = this.Lines[ i ].Text;
                    int left = 0;

                    switch( TextAlign )
                    {
                        case TextAlign.Left:
                            break;

                        case TextAlign.Center:
                            left = ( ClientWidth - line.Length ) / 2;
                            break;

                        case TextAlign.Right:
                            screen.CursorLeft = ClientWidth - line.Length;
                            break;
                    }

                    screen.ForeColor = keyColor;
                    screen.CursorLeft = left + AccessKey.Position;
                    screen.Write( line.Substring( AccessKey.Position, 1 ) );

                    screen.ForeColor = savedForeColor;
                }
            }

            base.OnDrawContents( screen, hasFocus );
        }

        #endregion
    }
}