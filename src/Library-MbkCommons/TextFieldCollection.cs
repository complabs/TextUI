/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       TextFieldCollection.cs
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
    /// Represents the collection of TextField items. Used to format array of text
    /// values according to <see cref="TextField"/>s in collection.
    /// </summary>
    /// 
    public class TextFieldCollection : List<TextField>
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether to suppresses formating 
        /// specified in TextFields when formating array of field values.
        /// </summary>
        /// 
        public bool DontFormat { get; set; }

        /// <summary>
        /// Gets or sets the separator to be used between field values. 
        /// </summary>
        /// 
        public string Separator { get; set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new empty instance of the TextFieldCollection.
        /// </summary>
        /// 
        public TextFieldCollection ()
            : base ()
        {
            this.DontFormat = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Returns a header formatted according to specification from TextField items.
        /// </summary>
        ///
        public string Header ()
        {
            StringBuilder sb = new StringBuilder ();

            foreach( TextField column in this )
            {
                sb.Append( column.Separator );
                sb.Append( column.Format( column.Title ) );
            }

            return sb.ToString ();
        }

        /// <summary>
        /// Returns array of text values formatted according to specification from
        /// TextField items.
        /// </summary>
        ///
        public string Format( params string[] values )
        {
            StringBuilder sb = new StringBuilder ();

            int index = 0;

            foreach( TextField column in this )
            {
                if ( index >= values.Length ) break;

                if ( DontFormat )
                {
                    sb.Append( this.Separator );
                    sb.Append( values[ index ] );
                }
                else
                {
                    sb.Append( column.Separator );
                    sb.Append( column.Format( values[ index ] ) );
                }

                ++index;
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}