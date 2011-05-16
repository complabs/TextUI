/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.ItemStore
 *  File:       MovieExemplar.cs
 *  Created:    2011-03-12
 *  Modified:   2011-04-29
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
    [Serializable]
    public class MovieExemplar : TransactionalObject<MovieExemplar>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Mandatory Properties ]

        public int CopyNumber { get; internal set; }

        /////////////////////////////////////////////////////////////////////////////////

        public Media Media
        {
            get { return this.media; }
            set { ThrowExceptionIfNotMutable (); this.media = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Optional Properties ]

        /////////////////////////////////////////////////////////////////////////////////

        public PriceClass PriceClass
        {
            get { return this.priceClass; }
            set { ThrowExceptionIfNotMutable (); this.priceClass = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string ISAN
        {
            get { return this.isan; }
            set { ThrowExceptionIfNotMutable (); this.isan = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public DateTime Released
        {
            get { return this.released; }
            set { ThrowExceptionIfNotMutable (); this.released = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string Subtitles
        {
            get { return this.subtitles; }
            set { ThrowExceptionIfNotMutable (); this.subtitles = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string ImageFormat
        {
            get { return this.imageFormat; }
            set { ThrowExceptionIfNotMutable (); this.imageFormat = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string Features
        {
            get { return this.features; }
            set { ThrowExceptionIfNotMutable (); this.features = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Calculated Properties ]

        /// <summary>
        /// Returns true if movie exemplar is available for rental (i.e. not rented).
        /// </summary>
        /// 
        public bool IsAvailable
        { 
            get { return this.RentedAsItem == null; }
        
        }

        /// <summary>
        /// Returns true if movie exemplar is rented to some customer.
        /// </summary>
        /// 
        public bool IsRented
        {
            get { return this.RentedAsItem != null; }
        }

        /// <summary>
        /// Returns a string representation of rental info containing customer
        /// info and rental conditions (due date and fee). If exemplar is not rented,
        /// returns an empty string.
        /// </summary>
        /// 
        public string RentalInfo
        {
            get
            {
                if ( this.RentedAsItem == null ) 
                {
                    return string.Empty;
                }

                return new StringBuilder ()
                    .Append( "    Rented to ........: " )
                    .AppendLine( this.RentedAsItem.RentedTo.ToString () )
                    .Append( "    Conditions .......: " )
                    .Append( this.RentedAsItem.VerboseConditions )
                    .AppendLine ()
                    .ToString ();
            }
        }

        /// <summary>
        /// Returns year of released date in form 'yyyy', or an empty string if 
        /// released date is not specified.
        /// </summary>
        /// 
        public string VerboseReleased
        {
            get
            {
                return this.Released.Equals( DateTime.MinValue )
                    ? string.Empty
                    : this.Released.ToString( "yyyy" );
            }
        }

        /// <summary>
        /// Returns rental due date in form 'yyyy' if exemplar is rented out, or an 
        /// empty string if exemplar is available for rental.
        /// </summary>
        /// 
        public string VerboseDueDate
        {
            get
            {
                return IsAvailable ? string.Empty : RentedAsItem.VerboseDueDate;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        public Movie Movie { get; private set; }

        public RentedItem RentedAsItem { get; internal set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Fields ]

        // Private fields behind public properties 
        //
        private Media      media      ;
        private PriceClass priceClass ;
        private string     isan       ;
        private DateTime   released   ;
        private string     subtitles  ;
        private string     imageFormat;
        private string     features   ;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new (mutable) instance of the MovieExemplar class either as an
        /// empty (without argument) or as a copy-constructed object from an existing 
        /// (immutable) instance from database.
        /// See also <see cref="AddTo"/> and <see cref="Update"/> methods.
        /// </summary>
        /// <remarks>
        /// The constructor does not insert or update database. To save changes to 
        /// database, use either <see cref="AddTo"/> method after construction without 
        /// original object, or <see cref="Update"/> method after copy-construction 
        /// from an existing original immutable object. Initially constructed objects 
        /// are all mutable until *saved to database*, after which they become immutable.
        /// To modify immutable object one must copy-construct new instance and later 
        /// call <see cref="Update"/> to commit changes. This ensures transactioncal 
        /// change of all records and proper delivery of on-changed events to event 
        /// handlers. 
        /// </remarks>
        ///
        public MovieExemplar( MovieExemplar original = null )
            : base( original, "Movie Exemplar" )
        {
            if ( ! IsNew ) // get fields from original
            {
                this.ID = original.ID;
                this.Movie = original.Movie;

                this.SetFieldsFrom( BaseRecord );

                this.RentedAsItem = BaseRecord.RentedAsItem;
            }
            else // initialize default fields
            {
                this.Movie        = null;
                this.Media        = Media.DVD;
                this.PriceClass   = PriceClass.OlderMovie;
                this.Released     = DateTime.MinValue;
                this.RentedAsItem = null;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Internal Methods ]

        /// <summary>
        /// Executed when record is about to be removed from database.
        /// If this exemplars is rented, cascades item's removal from
        /// customers' rented items.
        /// </summary>
        /// 
        internal override void OnRemove ()
        {
            if ( this.RentedAsItem != null )
            {
                this.Movie.Database.ReturnItem( this );
            }
        }

        /// <summary>
        /// Copies fields from source record. This is low-level method that
        /// disregards if destination record (i.e. this record) is immutable.
        /// </summary>
        ///
        protected override void SetFieldsFrom( MovieExemplar source )
        {
            this.media       = source.media       ;
            this.priceClass  = source.priceClass  ;
            this.isan        = source.isan        ;
            this.released    = source.released    ;
            this.subtitles   = source.subtitles   ;
            this.imageFormat = source.imageFormat ;
            this.features    = source.features    ;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Inserts newly created instance into database.
        /// </summary>
        /// 
        public MovieExemplar AddTo( Movie movie )
        {
            ThrowExceptionIfNotMutable ();

            if ( movie == null )
            {
                throw new ArgumentNullException( 
                    "Movie Exemplar must be added to some movie." );
            }
            if ( movie.Database == null )
            {
                throw new ArgumentNullException( 
                    "Movie Exemplar must be added to movie that belongs to database." );
            }

            Movie = movie;
            Database = movie.Database;

            ID  = Movie.Database.MovieExemplars.NextExemplarID;
            CopyNumber = Movie.MovieExemplars.NextExemplarID;

            LockProperties (); // make record immutable

            Movie.MovieExemplars.Add( this );

            return this;
        }

        /// <summary>
        /// Updates an instance obtained through copy constructor into database.
        /// </summary>
        /// 
        public MovieExemplar Update ()
        {
            VerifyUpdate ();

            BaseRecord.SetFieldsFrom( this );

            Movie.MovieExemplars.OnUpdated( BaseRecord );

            return BaseRecord;
        }

        /// <summary>
        /// Create additional copies of the movie exemplar
        /// </summary>
        /// <param name="numberOfCopies"></param>
        /// 
        public void CreateCopies( int numberOfCopies )
        {
            for ( int i = 0; i < numberOfCopies; ++i )
            {
                MovieExemplar newEx = new MovieExemplar( this );
                newEx.AddTo( this.Movie );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ToString() and FullInfo() ]

        /// <summary>
        /// Converts the value of this instance to its equivalent brief string 
        /// representation.
        /// </summary>
        /// <returns>a string containing exemplar#, movie briefs and rental info
        /// </returns>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append( "MovieEx #" ).Append( this.ID )
              .Append( ": " ).Append( this.Media.Verbose () )
              .Append( ", " ).Append( this.Movie.FullTitle );

            if ( this.IsRented )
            {
                sb.Append( ", Due: " )
                  .Append( this.VerboseDueDate );
            }

            return sb.ToString ();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// with full details about the record.
        /// </summary>
        /// 
        public override string FullInfo ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append( "Exemplar ID ..........: " )
              .Append( this.ID )
              .AppendLine ();

            sb.Append( "Media ................: " )
              .Append( this.Media.Verbose () )
              .AppendLine ();

            sb.Append( "Movie ................: " )
              .Append( this.Movie.FullTitle )
              .AppendLine ();

            sb.Append( "ISAN .................: " ).AppendLine( this.ISAN )
              .Append( "Subtitles ............: " ).AppendLine( this.Subtitles )
              .Append( "Image Format .........: " ).AppendLine( this.ImageFormat )
              .Append( "Features .............: " ).AppendLine( this.Features );

            if ( ! this.Released.Equals( DateTime.MinValue ) )
            {
                sb.Append( "Released .............: " )
                  .Append( this.Released.ToString( "yyyy-MM-dd" ) )
                  .AppendLine ();
            }

            sb.Append( "Price Class ..........: " )
              .Append( this.PriceClass.Verbose () )
              .AppendLine ();

            if ( this.IsRented )
            {
                sb.AppendLine().AppendLine( "  Rental Information:" )
                  .Append( this.RentalInfo );
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}