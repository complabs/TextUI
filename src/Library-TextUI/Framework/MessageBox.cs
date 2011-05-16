/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       MessageBox.cs
 *  Created:    2011-03-29
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Text;

using Mbk.Commons;

namespace TextUI
{
    using TextUI.Controls;
    using TextUI.Drawing;

    /// <summary>
    /// Displays a message box that can contain text, buttons, and symbols that inform 
    /// and instruct the user.
    /// </summary>
    /// 
    public class MessageBox : Window
    {
        #region [ Fields ]

        // Displayed text
        //
        private TaggedTextCollection lines;
        private int maxLineWidth;

        // Buttons
        //
        private Button buttonOK;
        private Button buttonCancel;
        private Button buttonAbort;
        private Button buttonRetry;
        private Button buttonIgnore;
        private Button buttonYes;
        private Button buttonNo;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets or sets vertical spacing between text and buttons.
        /// </summary>
        /// 
        public int VerticalSpacing { get; set; }

        /// <summary>
        /// Gets or sets horizontal spacing between buttons.
        /// </summary>
        /// 
        public int ButtonSpacing { get; set; }

        /// <summary>
        /// Gets or sets horizontal padding between vertical edges of the window and 
        /// the text.
        /// </summary>
        /// 
        public int HorizontalPadding { get; set; }

        /// <summary>
        /// Gets or sets vertical padding between horizontal edges of the window and 
        /// the text.
        /// </summary>
        /// 
        public int VerticalPadding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the width and height of the control 
        /// automatically adjusts when the text assigned to the control is changed. 
        /// </summary>
        /// 
        public bool AutoSize { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the text associated with this MessageBox.
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
                base.Text = value;

                // Setup maximum line length according to screen dimensions
                // but at least 40
                //
                if ( Application.Screen != null )
                {
                    this.maxLineWidth = Application.Screen.Width - 20;
                }

                this.maxLineWidth = Math.Max( 40, this.maxLineWidth );

                // Get lines, splited with specified maximum line width.
                // At the end, maxLineWidth will contain actual max line width.
                //
                this.lines = TaggedText.SplitTextInLines( value, ref this.maxLineWidth );

                // Resize window to fit the contents, if auto-resize is turned on.
                //
                if ( AutoSize )
                {
                    Width  = maxLineWidth + 2 * HorizontalPadding;
                    Height = lines.Count  + 2 * VerticalPadding 
                            + VerticalSpacing + /*button size:*/ 3;
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Constructor ]

        /// <summary>
        /// Initializes a new instance of the MessageBox class. 
        /// Note that instances may be created only by Show() factory methods.
        /// </summary>
        ///
        private MessageBox( string text, string caption,
                MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton
            ) : base ()
        {
            string savedErrorMessage = Application.ErrorMessage;

            ButtonSpacing     = 4;
            HorizontalPadding = 2;
            VerticalPadding   = 1;
            VerticalSpacing   = 0;
            Border            = true;
            AutoSize        = true;
            Caption           = caption;
            Text              = text; // Setting text will also resize window

            // Instantiate menu items simulating all possible buttons
            //
            this.buttonOK     = new Button( "   &OK   " ) { Tag = DialogResult.OK     };
            this.buttonCancel = new Button( " Cancel "  ) { Tag = DialogResult.Cancel };
            this.buttonAbort  = new Button( " &Abort "  ) { Tag = DialogResult.Abort  };
            this.buttonRetry  = new Button( " &Retry "  ) { Tag = DialogResult.Retry  };
            this.buttonIgnore = new Button( " &Ignore " ) { Tag = DialogResult.Ignore };
            this.buttonYes    = new Button( "  &Yes  "  ) { Tag = DialogResult.Yes    };
            this.buttonNo     = new Button( "  &No  "   ) { Tag = DialogResult.No     };

            // Add selected buttons to children list.
            //
            switch( buttons )
            {
                case MessageBoxButtons.OK:
                    this.buttonOK.Parent = this;
                    ToolTipText = "Press OK to continue...";
                    break;

                case MessageBoxButtons.OKCancel:
                    this.buttonOK.Parent = this;
                    this.buttonCancel.Parent = this;
                    ToolTipText = "Please, choose OK or Cancel...";
                    break;

                case MessageBoxButtons.AbortRetryIgnore:
                    this.buttonAbort.Parent = this;
                    this.buttonRetry.Parent = this;
                    this.buttonIgnore.Parent = this;
                    ToolTipText = "Please, choose Abort, Retry or Ignore...";
                    break;

                case MessageBoxButtons.YesNoCancel:
                    this.buttonYes.Parent = this;
                    this.buttonNo.Parent = this;
                    this.buttonCancel.Parent = this;
                    ToolTipText = "Please, choose Yes, No or Cancel...";
                    break;

                case MessageBoxButtons.YesNo:
                    this.buttonYes.Parent = this;
                    this.buttonNo.Parent = this;
                    ToolTipText = "Please, choose Yes or No...";
                    break;

                case MessageBoxButtons.RetryCancel:
                    this.buttonRetry.Parent = this;
                    this.buttonCancel.Parent = this;
                    ToolTipText = "Please, choose Retry or Cancel...";
                    break;
            }

            // Count number of buttons in (button)stripe as well total stripe width.
            //
            int buttonStripeWidth = 0;
            int buttonStripeCount = 0;

            foreach( Window child in Children )
            {
                Button button = child as Button;
                if ( button != null )
                {
                    ++buttonStripeCount;
                    buttonStripeWidth += button.Width;
                }
            }

            buttonStripeWidth += ( buttonStripeCount - 1 ) * ButtonSpacing;

            // Resize MessageBox window to fit its contents (text and buttons).
            //
            if ( AutoSize )
            {
                Width = Math.Max( Width, buttonStripeWidth + 2 * HorizontalPadding );

                if ( CaptionVisible )
                {
                    Width = Math.Max( Width, Caption.Length + 2 );
                }

                // Ensure that message box always fits the sreen
                //
                if ( Application.Screen != null )
                {
                    Width  = Math.Min( Width,  Application.Screen.Width  - 2 );
                    Height = Math.Min( Height, Application.Screen.Height - 4 );
                }
            }

            // Position buttons in the button-stripe and assign events to buttons
            // as well default visual properties.
            //
            int nextButtonLeft = ( Width - buttonStripeWidth ) / 2;

            foreach( Window w in Children )
            {
                Button button = w as Button;
                if ( button != null )
                {
                    button.BackColor =  Application.Theme.MenuItemBackColor;
                    button.ForeColor =  Application.Theme.MenuItemForeColor;
                    button.ToolTipText   =  ToolTipText;
                    button.Top       =  Height - 1 - VerticalPadding;
                    button.Left      =  nextButtonLeft;
                    nextButtonLeft   += button.Width + ButtonSpacing;

                    button.Click += ( sender, e ) =>
                    {
                        throw new QuitMessageLoop () { Result = button.Tag };
                    };
                }
            }

            // Put focus on defaultButton if specified; otherwise on first button.
            //
            switch ( defaultButton )
            {
                case MessageBoxDefaultButton.Button1:
                    Children[0].Focus ();
                    break;
                case MessageBoxDefaultButton.Button2:
                    if ( Children.Count >= 2 )
                    {
                        Children[1].Focus ();
                    }
                    break;
                case MessageBoxDefaultButton.Button3:
                    if ( Children.Count >= 3 )
                    {
                        Children[2].Focus ();
                    }
                    break;
            }

            // Position Message Box centered on screen
            //
            if ( Application.RootWindow != null )
            {
                Left = ( Application.RootWindow.Width  - TotalWidth  ) / 2;
                Top  = ( Application.RootWindow.Height - TotalHeight ) / 2  + ExtraTop;
            }

            Application.ErrorMessage = savedErrorMessage;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        private DialogResult DoModal ()
        {
            return Application.DoModal( this );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Displays a message box with specified text. 
        /// </summary>
        /// 
        public static DialogResult Show( string text )
        {
            MessageBox messageBox = new MessageBox( text, null, MessageBoxButtons.OK,
                MessageBoxDefaultButton.Button1 );
            return messageBox.DoModal ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Displays a message box in front of the specified object and with the 
        /// specified text, button and default button. 
        /// </summary>
        /// 
        public static DialogResult Show( string text, 
                MessageBoxButtons buttons,
                MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1
            )
        {
            MessageBox messageBox = new MessageBox( 
                text, null, buttons, defaultButton );
            
            return messageBox.DoModal ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Displays a message box in front of the specified object and with the 
        /// specified text, caption, buttons, and default button. 
        /// </summary>
        /// 
        public static DialogResult Show( string text, 
                string caption = null, 
                MessageBoxButtons buttons = MessageBoxButtons.OK,
                MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1
            )
        {
            MessageBox messageBox = new MessageBox( 
                text, caption, buttons, defaultButton );
            
            return messageBox.DoModal ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Displays a message box in front of the specified object and with the 
        /// specified text, caption, buttons, icon, and default button. 
        /// </summary>
        /// 
        public static DialogResult Show( string text, string caption, 
                MessageBoxButtons buttons, MessageBoxIcon icon,
                MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1
            )
        {
            MessageBox messageBox = new MessageBox( text, caption, 
                buttons, defaultButton );

            switch( icon )
            {
                case MessageBoxIcon.Information:
                    messageBox.CaptionForeColor = Color.White;
                    messageBox.ForeColor        = Color.Green;
                    messageBox.ForeColorInact   = Color.Gray;
                    System.Media.SystemSounds.Asterisk.Play ();
                    break;

                case MessageBoxIcon.Asterisk:
                    messageBox.CaptionForeColor = Color.White;
                    messageBox.ForeColor        = Color.Green;
                    messageBox.ForeColorInact   = Color.White;
                    System.Media.SystemSounds.Asterisk.Play ();
                    break;

                case MessageBoxIcon.Exclamation:
                    messageBox.CaptionForeColor = Color.Yellow;
                    messageBox.ForeColor        = Color.Gray;
                    messageBox.ForeColorInact   = Color.Gray;
                    System.Media.SystemSounds.Exclamation.Play ();
                    break;

                case MessageBoxIcon.Warning:
                    messageBox.CaptionForeColor = Color.Yellow;
                    messageBox.ForeColor        = Color.Red;
                    messageBox.ForeColorInact   = Color.Gray;
                    System.Media.SystemSounds.Exclamation.Play ();
                    break;

                case MessageBoxIcon.Hand:
                    messageBox.CaptionForeColor = Color.Yellow;
                    messageBox.ForeColor        = Color.Red;
                    messageBox.ForeColorInact   = Color.Gray;
                    System.Media.SystemSounds.Hand.Play ();
                    break;

                case MessageBoxIcon.Stop:
                    messageBox.CaptionForeColor = Color.Yellow;
                    messageBox.ForeColor        = Color.Red;
                    messageBox.ForeColorInact   = Color.White;
                    System.Media.SystemSounds.Hand.Play ();
                    break;

                case MessageBoxIcon.Error:
                    messageBox.CaptionForeColor = Color.Yellow;
                    messageBox.ForeColor        = Color.Red;
                    messageBox.ForeColorInact   = Color.Yellow;
                    System.Media.SystemSounds.Hand.Play ();
                    break;

                case MessageBoxIcon.Severe:
                    messageBox.CaptionForeColor = Color.Yellow;
                    messageBox.ForeColor        = Color.Red;
                    messageBox.ForeColorInact   = Color.Red;
                    System.Media.SystemSounds.Hand.Play ();
                    break;

                default:
                    messageBox.CaptionForeColor = Color.White;
                    messageBox.ForeColor        = Color.Gray;
                    messageBox.ForeColorInact   = Color.DarkGray;
                    break;
            }
            
            return messageBox.DoModal ();
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
            bool inHeader = true;

            for ( int i = 0; i < this.lines.Count; ++i )
            {
                if ( string.IsNullOrEmpty( this.lines[ i ].Text ) )
                {
                    inHeader = false;

                    if ( i == 0 )
                    {
                        --topStart;
                    }
                }

                screen.CursorTop  = topStart + i;
                screen.ForeColor  = inHeader ? ForeColor : ForeColorInact;
                screen.CursorLeft = inHeader ? ( Width - this.lines[i].Text.Length ) / 2
                                             : HorizontalPadding;

                // If there are too many lines (not fitting the window) show elipsis.
                //
                if ( i >= Height - 2 - 2 * VerticalPadding )
                {
                    screen.CursorLeft = HorizontalPadding;
                    screen.Write( "..." );
                    break;
                }

                screen.Write( this.lines[ i ].Text );
            }

            base.OnDrawContents( screen, hasFocus );
        }

        /// <summary>
        /// Executed after the KeyDown event was raised but not handled.
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data.</param>
        /// 
        protected override void OnAfterKeyDown ( KeyEventArgs e )
        {
            if (   e.KeyCode == Keys.Attention 
                || e.KeyCode == Keys.Escape
                || e.KeyCode == Keys.F4 && e.Alt
                )
            {
                throw new QuitMessageLoop () { Result = DialogResult.None };
            }

            if ( ! char.IsControl( e.Character ) )
            {
                Window nextInFocus = FindChild( w => w.AccessKey == e.Character );

                if ( nextInFocus != null )
                {
                    nextInFocus.Focus ();

                    if ( nextInFocus is Button )
                    {
                        ( (Button)nextInFocus ).OnClick ();
                    }

                    e.StopHandling ();
                }
            }

            base.OnAfterKeyDown( e );
        }

        #endregion
    }
}