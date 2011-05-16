/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyProgressBar.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
#endif

using Mbk.Commons;

/////////////////////////////////////////////////////////////////////////////////////////

#if TEXTUI

    /// <summary>
    /// Represents a progress bar dialog that is used in this application. 
    /// </summary>
    /// <remarks>
    /// Derived from TextUI progress bar control in Text UI mode.
    /// </remarks>
    /// 
    internal class MyProgressBar : ProgressBar
    {
        /// <summary>
        /// Initializes a new instance of the MyProgressBar contained in a specified
        /// form and with caption.
        /// </summary>
        /// 
        public MyProgressBar( MainForm form, string progressInfo )
        {   
            form.InfoMessage = progressInfo;

            this.Parent = form;
            this.Border = true;

            this.Caption = progressInfo;

            this.Height = 1;
            this.Width = Math.Min( form.MdiClient.Width - 4, 
                         Math.Max( 50, progressInfo.Length ) );

            this.CaptionTextAlign = TextAlign.Center;
            this.CaptionBackColor = form.MdiClient.BackColor;
            this.CaptionForeColor = Color.Green;
            this.BackColor        = form.MdiClient.BackColor; 
            this.ForeColor        = Color.White;
            this.BorderBackColor  = form.MdiClient.BackColor;
            this.BorderForeColor  = Color.DarkGray;

            this.Center ();

            Application.StatusBarWindow.ForeColorInact = this.ForeColor;
            Application.StatusBarWindow.Text = progressInfo;

            this.Refresh ();
        }

        /// <summary>
        /// Closes progress bar window.
        /// </summary>
        /// 
        public void Quit ()
        {
            this.Refresh ();
            this.Unload ();
            this.Refresh ();
        }
    }

#else

    /// <summary>
    /// Represents a progress bar dialog that is used in this application. 
    /// </summary>
    /// <remarks>
    /// Implemented as a Windows form containing the inner progress bar control.
    /// </remarks>
    /// 
    [System.ComponentModel.DesignerCategory("Code")]
    internal class MyProgressBar : Form
    {
        #region [ Fields ]

        // Saved MainForm's status info, restored when MyProgressBar disappears.
        //
        private string savedStatusInfo;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Parent form owning this form.
        /// </summary>
        /// 
        public MainForm MainForm { get; private set; }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the underlying progress bar control.
        /// </summary>
        /// 
        public ProgressBar Bar { get; private set; }

        /// <summary>
        /// Implicitly converts this instance to underlying progress bar control.
        /// </summary>
        /// <remarks>
        /// Needed for compatibility with TextUI mode where MyProgressBar is
        /// derived not from the form but from the progress bar control directly.
        /// </remarks>
        /// 
        public static implicit operator ProgressBar( MyProgressBar form )
        { 
            return form.Bar;
        }

        /// <summary>
        /// Gets or sets the minimum value of the range of the MyProgressBar.
        /// </summary>
        /// 
        public int Minimum
        {
            get { return Bar.Minimum; }
            set { Bar.Minimum = value; }
        }

        /// <summary>
        /// Gets or sets the maximum value of the range of the MyProgressBar.
        /// </summary>
        /// 
        public int Maximum
        {
            get { return Bar.Maximum; }
            set { Bar.Maximum = value; }
        }

        /// <summary>
        /// Gets or sets the current position of the progress bar.
        /// </summary>
        /// 
        public int Value
        {
            get { return Bar.Value; }
            set { Bar.Value = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the MyProgressBar contained in a specified
        /// form and with caption.
        /// </summary>
        /// 
        public MyProgressBar( MainForm form, string progressInfo )
        {   
            this.MainForm = form;

            this.savedStatusInfo = this.MainForm.InfoMessage;
            this.MainForm.InfoMessage = progressInfo;

            this.MainForm.Update ();

            this.MdiParent = MainForm;
            this.Text = progressInfo;
            this.ClientSize = new Size( 80 * Em.Width, Em.Height + 2 );
            this.MinimumSize = this.Size;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Bar = new ProgressBar ()
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add( this.Bar );
            this.Show ();
            this.Bar.Refresh ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Closes progress bar window.
        /// </summary>
        /// 
        public void Quit ()
        {
            MainForm.InfoMessage = savedStatusInfo;

            Bar.Refresh ();
            Application.DoEvents ();

            this.Close ();
            Application.DoEvents ();
        }

        #endregion
    }

#endif