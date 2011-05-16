/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       TextField.cs
 *  Created:    2011-04-26
 *  Modified:   2011-04-26
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mbk.Commons
{
    /// <summary>
    /// Represents a text field with the caption (title), max text width and 
    /// text alignment.
    /// </summary>
    /// 
    public class TextField
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the separator to be used between this and previous field.
        /// </summary>
        /// 
        public string Separator { get; set; }

        /// <summary>
        /// Gets or sets the caption (column title) for the field.
        /// </summary>
        /// 
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the maximum field (column) width and alignment.
        /// Negative widths left aligned.
        /// </summary>
        /// 
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use elipsis if the text
        /// cannot fit the specified field width.
        /// </summary>
        /// 
        public bool UseElipsis { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the TextField class with a specified title,
        /// and optional maximum text width, alignment and separator.
        /// </summary>
        /// 
        public TextField( string title, int width = 0, string separator = null )
        {
            this.Title = title;
            this.Width = width;
            this.Separator = separator;
            this.UseElipsis = true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Formats given text according TextField specification.
        /// </summary>
        /// 
        public string Format( string text )
        {
            text = string.IsNullOrEmpty( text ) ? string.Empty : text;

            int w = Width == 0 ? text.Length : Math.Abs( Width );

            if ( text.Length <= w )
            {
                return Width < 0 ? text.PadRight( w ) : text.PadLeft( w );
            }

            if ( w <= 3 || ! UseElipsis )
            {
                return text.Substring( 0, w );
            }

            return Width < 0
                    ? text.Substring( 0, w - 3 ) + "..."
                    : "..." + text.Substring( text.Length - ( w - 3 ), w - 3 );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}