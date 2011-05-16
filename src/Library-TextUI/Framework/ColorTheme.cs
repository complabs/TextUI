/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Framework
 *  File:       ColorTheme.cs
 *  Created:    2011-03-21
 *  Modified:   2011-04-03
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace TextUI.Framework
{
    /// <summary>
    /// Represents a set of color attributes used define initial look of the window.
    /// </summary>
    /// 
    public class ColorTheme
    {
        #region [ Properties ]

        /////////////////////////////////////////////////////////////////////////////////

        public Color BorderBackColor          { get; set; }
        public Color BorderForeColor          { get; set; }

        public Color BorderBackColorInact     { get; set; }
        public Color BorderForeColorInact     { get; set; }

        public Color CaptionBackColor         { get; set; }
        public Color CaptionForeColor         { get; set; }

        public Color CaptionBackColorInact    { get; set; }
        public Color CaptionForeColorInact    { get; set; }

        public Color BackColor                { get; set; }
        public Color ForeColor                { get; set; }

        public Color BackColorInact           { get; set; }
        public Color ForeColorInact           { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Color ButtonBackColor          { get; set; }
        public Color ButtonForeColor          { get; set; }

        public Color ButtonBackColorInact     { get; set; }
        public Color ButtonForeColorInact     { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Color MenuBackColor            { get; set; }
        public Color MenuForeColor            { get; set; }

        public Color MenuBackColorInact       { get; set; }
        public Color MenuForeColorInact       { get; set; }

        public Color MenuItemBackColor        { get; set; }
        public Color MenuItemForeColor        { get; set; }

        public Color MenuAccessKeyForeColor    { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Color CurrentRowBackColor      { get; set; }
        public Color CurrentRowForeColor      { get; set; }

        public Color CurrentRowBackColorInact { get; set; }
        public Color CurrentRowForeColorInact { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Color HeaderBackColor          { get; set; }
        public Color HeaderForeColor          { get; set; }

        public Color HeaderBackColorInact     { get; set; }
        public Color HeaderForeColorInact     { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Color FooterBackColor          { get; set; }
        public Color FooterForeColor          { get; set; }

        public Color FooterBackColorInact     { get; set; }
        public Color FooterForeColorInact     { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Color ScrollBarForeColor       { get; set; }
        public Color ScrollBarForeColorInact  { get; set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Color ToolTipColor             { get; set; }
        public Color ErrorMessageColor        { get; set; }
        public Color InfoMessageColor         { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the ColorTheme class with a default
        /// 'blue-console' look.
        /// </summary>
        ///
        public ColorTheme ()
        {
            BorderBackColor          = Color.DarkBlue;
            BorderForeColor          = Color.Cyan;

            BorderBackColorInact     = Color.DarkBlue;
            BorderForeColorInact     = Color.DarkCyan;

            CaptionBackColor         = Color.DarkBlue;
            CaptionForeColor         = Color.White;

            CaptionBackColorInact    = Color.DarkBlue;
            CaptionForeColorInact    = Color.Gray;

            BackColor                = Color.DarkBlue;
            ForeColor                = Color.Gray;

            BackColorInact           = Color.DarkBlue;
            ForeColorInact           = Color.DarkGray;

            ButtonBackColor          = Color.White;
            ButtonForeColor          = Color.Black;

            ButtonBackColorInact     = Color.DarkBlue;
            ButtonForeColorInact     = Color.Gray;

            MenuBackColor            = Color.DarkGray;
            MenuForeColor            = Color.Black;

            MenuBackColorInact       = Color.DarkBlue;
            MenuForeColorInact       = Color.Gray;

            MenuItemBackColor        = Color.White;
            MenuItemForeColor        = Color.Black;

            MenuAccessKeyForeColor   = Color.Red;

            CurrentRowBackColor      = Color.DarkGray;
            CurrentRowForeColor      = Color.White;

            CurrentRowBackColorInact = Color.Gray;
            CurrentRowForeColorInact = Color.DarkBlue;

            HeaderBackColor          = Color.DarkBlue;
            HeaderForeColor          = Color.Cyan;

            HeaderBackColorInact     = Color.DarkBlue;
            HeaderForeColorInact     = Color.DarkGray;

            FooterBackColor          = Color.DarkBlue;
            FooterForeColor          = Color.Gray;

            FooterBackColorInact     = Color.DarkBlue;
            FooterForeColorInact     = Color.DarkGray;

            ScrollBarForeColor       = Color.Gray;
            ScrollBarForeColorInact  = Color.DarkGray;

            ToolTipColor             = Color.White;
            ErrorMessageColor        = Color.Red;
            InfoMessageColor         = Color.Green;
        }
    }
}