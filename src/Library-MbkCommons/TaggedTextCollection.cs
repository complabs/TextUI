/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       TaggedTextCollection.cs
 *  Created:    2011-05-08
 *  Modified:   2011-05-08
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;

namespace Mbk.Commons
{
    /// <summary>
    /// Represents collection of TaggedText items.
    /// </summary>
    /// 
    public class TaggedTextCollection : List<TaggedText>
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the maximum found length of the Text property of TaggedText items
        /// in the collection.
        /// </summary>
        /// 
        public int MaxTextLength
        {
            get
            {
                int maxTextLength = 0;

                foreach( TaggedText item in this )
                {
                    maxTextLength = Math.Max( maxTextLength, item.Text.Length );
                }

                return maxTextLength;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the TaggedTextCollection class that is empty
        /// and has the default initial capacity. 
        /// </summary>
        /// 
        public TaggedTextCollection ()
            : base ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TaggedTextCollection class that 
        /// contains elements copied from the specified collection and has
        /// sufficient capacity to accommodate the number of elements copied. 
        /// </summary>
        /// 
        public TaggedTextCollection ( IEnumerable<TaggedText> collection )
            : base( collection )
        {
        }

        #endregion
    }
}