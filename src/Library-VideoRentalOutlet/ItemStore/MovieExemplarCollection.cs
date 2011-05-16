/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.ItemStore
 *  File:       MovieExemplarCollection.cs
 *  Created:    2011-03-12
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
    /// Encapsulates VideoRentalOutlet's library of rentable items i.e. movie exemplars.
    /// </summary>
    /// 
    [Serializable]
    public class MovieExemplarCollection 
        : GenericObjectCollection<MovieExemplar>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Auto-incremented Exemplar ID ]

        /// <summary>
        /// Auto-incremented generator of movie exemplar unique IDs.
        /// </summary>
        ///
        private int lastExemplarID;

        /// <summary>
        /// Gets an auto-incremented exemplar unique ID.
        /// </summary>
        /// 
        internal int NextExemplarID
        {
            get { return ++this.lastExemplarID; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets number of rented exemplars in collection.
        /// </summary>
        /// 
        public int RentedCount
        {
            get
            {
                int count = 0;

                foreach( MovieExemplar exemplar in this )
                {
                    count += ( exemplar.IsRented ? 1 : 0 );
                }

                return count;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        /// <summary>
        /// Instance of the <see cref="Movie"/> class that owns this collection of 
        /// movie exemplars.
        /// </summary>
        /// 
        public Movie Movie { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the MovieExemplaCollection class that is
        /// owned by a movie.
        /// </summary>
        /// <remarks>
        /// <see cref="MovieExemplar"/> is a weak class, which always belongs by some 
        /// instance of the <see cref="Movie"/>. As database contains movies, which
        /// further contains exemplars, it would be rather difficult to search among all
        /// exemplars directly, so this database contains a common collection of all 
        /// exemplars at database level. Then an exemplar is added to some movie,
        /// it is automatically added to database's exemplar collection and vice versa.
        /// Similary, removing some exemplar removes it both from movie's collection
        /// as well from common database's collection of exemplars.
        /// <seealso cref="MovieExemplarCollection.Add"/>
        /// <seealso cref="MovieExemplarCollection.Remove"/>
        /// </remarks>
        ///
        public MovieExemplarCollection( Movie movie )
            : base( movie.Database, "Movie Exemplars", "Movie Exemplar" )
        {
            this.Movie = movie;
            this.lastExemplarID = 0;

            ForwardChangedEventsToDatabase = true;
        }

        /// <summary>
        /// Initializes a new instance of the MovieExemplaCollection class that is
        /// owned by a database. For remarks explaining 'ownership' of exemplars
        /// <see cref="MovieExemplarCollection.MovieExemplarCollection(Movie movie)"/> 
        /// constructor.
        /// </summary>
        ///
        public MovieExemplarCollection( VideoRentalOutlet database )
            : base( database, "Movie Exemplars", "Movie Exemplar" )
        {
            this.Movie = null;
            this.lastExemplarID = 0;

            ForwardChangedEventsToDatabase = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Virtual Methods ]

        /// <summary>
        /// Adds exemplar to exemplar collection.
        /// </summary>
        /// 
        internal override void Add( MovieExemplar exemplar )
        {
            if ( exemplar == null )
            {
                return; // nothing to add
            }

            if ( this.Items.Contains( exemplar ) )
            {
                throw new ArgumentException( "Cannot duplicate items." );
            }

            if ( this.Movie == null )
            {
                exemplar.Movie.MovieExemplars.Add( exemplar );
                return;
            }

            this.Items.Add( exemplar );
            Database.MovieExemplars.Items.Add( exemplar );

            OnAddedNew( exemplar );
        }

        /// <summary>
        /// Removes exemplar from collection.
        /// </summary>
        /// 
        public override bool Remove( MovieExemplar exemplar )
        {
            // We can remove only existing exemplars
            //
            if ( exemplar == null || ! this.Items.Contains( exemplar ) ) 
            {
                return false;
            }
            
            if ( this.Movie == null )
            {
                return exemplar.Movie.MovieExemplars.Remove( exemplar );
            }

            exemplar.OnRemove ();

            Database.MovieExemplars.Items.Remove( exemplar );

            bool rc = this.Items.Remove( exemplar );

            if ( rc )
            {
                OnRemoved( exemplar );
            }

            return rc;

        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Executed when a new object is added to database.
        /// </summary>
        ///
        public override void OnAddedNew( GenericObject item, string reason = null )
        {
            base.OnAddedNew( item, reason );

            if ( Movie != null )
            {
                Database.MovieExemplars.OnAddedNew( item, reason );

                // Changes made to movie's exemplar collection are considered
                // as updates of the movie itself and propagated to the movie.
                //
                Database.Movies.OnUpdated( Movie, "Added Exemplar" );
            }
        }

        /// <summary>
        /// Executed when existing object is removed from database.
        /// </summary>
        ///
        public override void OnRemoved( GenericObject item, string reason = null )
        {
            base.OnRemoved( item, reason );

            if ( Movie != null )
            {
                Database.MovieExemplars.OnRemoved( item, reason );

                // Changes made to movie's exemplar collection are considered
                // as updates of the movie itself and propagated to the movie.
                //
                Database.Movies.OnUpdated( Movie, "Removed Exemplar" );
            }
        }

        /// <summary>
        /// Executed when existing object is updated.
        /// </summary>
        ///
        public override void OnUpdated( GenericObject item, string reason = null )
        {
            base.OnUpdated( item, reason );

            if ( Movie != null )
            {
                Database.MovieExemplars.OnUpdated( item, reason );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ ToString() and FullInfo() Virtual Methods ]

        /// <summary>
        /// Gets a brief contents of movie exemplar collection.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            if ( this.Items.Count == 0 )
            {
                sb.AppendLine( "Movie exemplar collection is empty." );
            }
            else
            {
                sb.AppendLine( "Movie Exemplars:" );
                sb.Append( base.ToString () );
            }

            return sb.ToString ();
        }

        /// <summary>
        /// Gets a thorough contents of movie exemplar collection.
        /// </summary>
        /// 
        public override string FullInfo ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.AppendTitle( 
                Movie != null ? "Movie Exemplars of " + Movie.FullTitle 
                              : "Movie Exemplar Database"
                );

            if ( this.Items.Count != 0 )
            {
                sb.AppendLine ()
                  .Append( "Number of exemplars: " ).Append( this.Items.Count )
                  .Append( ", Rented exemplars: " ).Append( RentedCount )
                  .AppendLine ();
            }

            sb.Append( base.FullInfo () );

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}