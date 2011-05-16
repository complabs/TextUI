/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyReportBox.cs
 *  Created:    2011-04-29
 *  Modified:   2011-05-09
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.IO;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
#endif

#if TEXTUI

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Represents a report box dialog. Implemented as modal TextUI TextBox.
    /// </summary>
    /// 
    internal static class MyReportBox
    {
        /// <summary>
        /// Displays a text report with specified contents and caption. 
        /// </summary>
        /// 
        public static void ShowReport( string caption, string report, 
            int width = 0, int height = 0 )
        {
            TextBox reportBox = new TextBox ()
            {
                Name = "reportBox", Parent = Application.Screen.RootWindow, 
                Caption = caption,
                Multiline = true, AutoScrollBar = true, Border = true, ReadOnly = true,
                ForeColor = Color.Gray, ScrollBarForeColor = Color.Gray,
                CaptionForeColor = Color.Yellow,
                ToolTipText = "Press Escape to continue... "
            };

            reportBox.Maximize ();
            --reportBox.Height;

            reportBox.LostFocus += ( sender, e ) =>
            {
                reportBox.Unload ();
            };

            reportBox.KeyDown += ( sender, e ) =>
            {
                Window w = sender as Window;

                if ( e.Alt && e.KeyCode == Keys.F4 )
                {
                    w.Unload ();
                    e.StopHandling ();
                    return;
                }

                switch ( e.KeyCode )
                {
                    case Keys.Enter:
                        e.StopHandling ();
                        break;

                    case Keys.Escape:
                        w.Unload ();
                        e.StopHandling ();
                        break;
                }
            };

            reportBox.Text = report;
        }
    }

#else

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Represents a report box dialog. 
    /// Implemented as a Windows form with the inner WebBrowser control.
    /// </summary>
    /// 
    [System.ComponentModel.DesignerCategory("Code")]
    internal class MyReportBox : Form
    {
        #region [ Fields ]

        private WebBrowser webBrowser; // used to display report

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods (the constructor is also here!) ]

        /// <summary>
        /// Initializes a new instance of the MyReportBox with caption
        /// </summary>
        /// 
        private MyReportBox( string caption, int width, int height )
        {
            this.Text = caption;
            this.KeyPreview = true;
            this.MinimumSize = new Size( 400, 300 );
            this.Size = new Size( width, height );
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = VideoRentalOutlet_GUI.Properties.Resources.Document;

            this.webBrowser = new WebBrowser () 
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGoldenrodYellow, ForeColor = Color.Black,
            };

            this.Controls.Add( this.webBrowser );

            webBrowser.PreviewKeyDown += ( sender, e ) =>
            {
                if ( e.KeyCode == Keys.Escape )
                {
                    Close ();
                }
            };
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Shows the report in web browser. If the report is not formatted in HTML, 
        /// surrounds the report with the HTML code.
        /// </summary>
        ///
        private void ShowText( string report )
        {
            this.Cursor = Cursors.WaitCursor;

            // Convert plain text to HTML, if it's not already
            //
            if ( ! report.StartsWith( "<" ) )
            {
                report = new System.Text.StringBuilder ()
                    .Append(
@"<?xml version='1.0' encoding='UTF-8'?>
<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>
<html><head>
<meta http-equiv='Content-Type' content='text/html'; charset='UTF-8'/>
<style type='text/css'>
  body { background-color: #FFFFF5; }
  pre { font-family: Consolas, 'Courier New'; font-size: 11pt; }
</style>
<title>" )
                    .Append( this.Text )
                    .Append( "</title></head><body><pre>" )
                    .Append( report )
                    .Append( "</pre></body></html>" )
                    .ToString ();
            }

            // Note that setting webBrowser.DocumentText does not create
            // webBrowser.Document (or webBrowser.DocumentTitle), so we set
            // webBrowser.DocumentStream instead (which does parsing properly!).
            //
            webBrowser.DocumentStream = new MemoryStream( 
                System.Text.Encoding.UTF8.GetBytes( report ) );

            if ( ! string.IsNullOrEmpty( webBrowser.DocumentTitle ) )
            {
                this.Text = webBrowser.DocumentTitle;
            }

            this.Show ();

            this.Cursor = Cursors.Default;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Static Methods ]

        /// <summary>
        /// Displays a text or HTML report with specified contents and caption. 
        /// </summary>
        /// 
        public static void ShowReport( string caption, string report,
            int width = 640, int height = 480 )
        {
            MyReportBox form = new MyReportBox( caption, width, height );
            form.ShowText( report );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }

#endif