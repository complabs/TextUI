/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.ItemStore
 *  File:       MovieCollection.cs
 *  Created:    2011-03-28
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Text;

using Mbk.Commons;

namespace VROLib.ItemStore
{
    /// <summary>
    /// Encapsulates video store movie library, but not a library of rentable items 
    /// (i.e. movie exemplars), which is kept in <see cref="MovieExemplarCollection"/>
    /// as one movie may have many different movie exemplars (copies on media).
    /// </summary>
    /// 
    [Serializable]
    public class MovieCollection
        : GenericObjectCollection<Movie>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the Library class with an empty item store.
        /// </summary>
        ///
        public MovieCollection( VideoRentalOutlet database )
            : base( database, "Movies", "Movie" )
        {
            ForwardChangedEventsToDatabase = true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ ToString() and FullInfo() Virtual Methods ]

        /// <summary>
        /// Gets a brief contents of movie collection.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            if ( this.Items.Count == 0 )
            {
                sb.Append( "Movie collection is empty." );
            }
            else
            {
                sb.AppendLine( "Movies:" );
                sb.Append( base.ToString () );
            }

            return sb.ToString ();
        }

        /// <summary>
        /// Gets a thorough contents of movie collection.
        /// </summary>
        /// 
        public override string FullInfo ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.AppendTitle( "Movie Database" );

            sb.Append( base.FullInfo () );

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}