/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.ItemStore
 *  File:       Movie.cs
 *  Created:    2011-03-12
 *  Modified:   2011-04-29
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

namespace VROLib.ItemStore
{
    [Serializable]
    public class Movie : TransactionalObject<Movie>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Mandatory Properties ]

        public string Title
        {
            get { return this.title; }
            set { ThrowExceptionIfNotMutable (); this.title = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Optional Properties ]

        /////////////////////////////////////////////////////////////////////////////////

        public DateTime FirstRelease
        {
            get { return this.firstRelease; }
            set { ThrowExceptionIfNotMutable (); this.firstRelease = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public TimeSpan Duration
        {
            get { return this.duration; }
            set { ThrowExceptionIfNotMutable (); this.duration = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string Country
        {
            get { return this.country; }
            set { ThrowExceptionIfNotMutable (); this.country = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string Language
        {
            get { return this.language; }
            set { ThrowExceptionIfNotMutable (); this.language = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string IMDB
        {
            get { return this.imdb; }
            set { ThrowExceptionIfNotMutable (); this.imdb = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string PgRate
        {
            get { return this.pgRate; }
            set { ThrowExceptionIfNotMutable (); this.pgRate = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public Genre Genre
        {
            get { return this.genre; }
            set { ThrowExceptionIfNotMutable (); this.genre = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Calculated Properties ]

        /// <summary>
        /// Gets full title in the form "Title (year) duration" where
        /// year and duration are optional. E.g. "Platoon (1986) 120 min"
        /// </summary>
        /// 
        public string FullTitle
        {
            get
            {
                StringBuilder sb = new StringBuilder ();

                sb.Append( this.Title );

                if ( ! this.FirstRelease.Equals( DateTime.MinValue ) )
                {
                    sb.Append( " (" )
                      .Append( this.FirstRelease.ToString( "yyyy" ) )
                      .Append( ")" );
                }

                if ( ! this.FirstRelease.Equals( DateTime.MinValue ) )
                {
                    sb.Append( ", " )
                      .Append( this.Duration.TotalMinutes.ToString( "N0" ) )
                      .Append( " min" );
                }

                return sb.ToString ();
            }
        }

        /// <summary>
        /// Gets movie duration in hours-minutes form like 1h 30', 2h 00' or 59'
        /// or an empty string if duration is not specified.
        /// </summary>
        /// 
        public string VerboseDuration
        {
            get
            {
                if ( this.Duration == TimeSpan.MinValue )
                {
                    return string.Empty;
                }
                else if ( this.Duration.TotalHours < 1 )
                {
                    return string.Format( "{0}'", this.Duration.Minutes );
                }

                return string.Format( "{0}h {1:00}'", (int)this.Duration.TotalHours,
                    (int)this.Duration.Minutes );
            }
        }

        /// <summary>
        /// Gets years of movie's first release in format 'yyyy' or an empty string
        /// if first release is not specified.
        /// </summary>
        /// 
        public string VerboseFirstRelease
        {
            get
            {
                return this.FirstRelease == DateTime.MinValue
                    ? string.Empty
                    : this.FirstRelease.ToString( "yyyy" );
            }
        }

        /// <summary>
        /// Gets list of actor's real names and roles one per line.
        /// </summary>
        /// 
        public string VerboseActorsList
        {
            get
            {
                StringBuilder sb = new StringBuilder ();

                foreach( MovieActor actor in this.Actors )
                {
                    if ( sb.Length != 0 )
                    {
                        sb.AppendLine ();
                    }
                    sb.Append( actor.ToString () );
                }

                return sb.ToString ();
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        public List<MovieActor> Actors { get; set; }

        public List<string> Directors { get; set; }

        public List<string> Writers { get; set; }

        public MovieExemplarCollection MovieExemplars { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Fields ]

        // Private fields behind public properties 
        //
        private string   title        ;
        private DateTime firstRelease ;
        private TimeSpan duration     ;
        private string   country      ;
        private string   language     ;
        private string   imdb         ;
        private string   pgRate       ;
        private Genre    genre        ;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new (mutable) instance of the Move class either as an empty
        /// (without argument) or as a copy-constructed object from an existing 
        /// (immutable)  instance from database.
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
        public Movie( Movie original = null )
            : base( original, "Movie" )
        {
            if ( ! IsNew ) // get fields from original
            {
                this.ID = BaseRecord.ID;

                this.SetFieldsFrom( BaseRecord );

                this.MovieExemplars = BaseRecord.MovieExemplars;
            }
            else // initialize default fields
            {
                this.FirstRelease = DateTime.MinValue;
                this.Duration     = TimeSpan.MinValue;
                this.Genre        = Genre.None;

                // Add default relationships:
                //
                this.Actors       = new List<MovieActor> ();
                this.Directors    = new List<string> ();
                this.Writers      = new List<string> ();

                // Note: we should not instantiate this.MovieExemplars collection
                // as we are still not connected to database. Instantiation is delayed
                // until AddTo() is called.
                //
                this.MovieExemplars = null;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ]

        /// <summary>
        /// Executed when record is about to be removed from database.
        /// Removes also all exemplars belonging to this movie.
        /// </summary>
        /// 
        internal override void OnRemove ()
        {
            if ( MovieExemplars != null )
            {
                MovieExemplars.Clear ();
            }
        }

        /// <summary>
        /// Copies fields from source record. This is low-level method that
        /// disregards if destination record (i.e. this record) is immutable.
        /// </summary>
        ///
        protected override void SetFieldsFrom( Movie source )
        {
            this.title        = source.title        ;
            this.firstRelease = source.firstRelease ;
            this.duration     = source.duration     ;
            this.country      = source.country      ;
            this.language     = source.language     ;
            this.imdb         = source.imdb         ;
            this.pgRate       = source.pgRate       ;
            this.genre        = source.genre        ;

            this.Actors       = source.Actors       ;
            this.Directors    = source.Directors    ;
            this.Writers      = source.Writers      ;
        }

        /// <summary>
        /// Verifies fields integrity of the record.
        /// </summary>
        /// 
        protected override void VerifyIntegrity ()
        {
            if ( string.IsNullOrEmpty( Title ) )
            {
                throw new ArgumentException( "Movie title must not be null" );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Inserts newly created instance into database.
        /// </summary>
        /// 
        public Movie AddTo( VideoRentalOutlet database )
        {
            VerifyAddTo( database );

            Database = database;

            ID = Database.NextMovieID;

            this.MovieExemplars = new MovieExemplarCollection( this );

            LockProperties (); // make record immutable

            Database.Movies.Add( this );

            return this;
        }

        /// <summary>
        /// Updates an instance obtained through copy constructor into database.
        /// </summary>
        /// 
        public Movie Update ()
        {
            VerifyUpdate ();

            BaseRecord.SetFieldsFrom( this );

            Database.Movies.OnUpdated( BaseRecord );

            foreach( MovieExemplar exemplar in MovieExemplars )
            {
                MovieExemplars.OnUpdated( exemplar, "Updated Movie" );
            }

            return BaseRecord;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ToString() and FullInfo() ]

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append( "Movie #" ).Append( this.ID )
              .Append( ": " ).Append( this.Title );

            if ( ! this.FirstRelease.Equals( DateTime.MinValue ) )
            {
                sb.Append( " (" )
                  .Append( this.FirstRelease.ToString( "yyyy" ) )
                  .Append( ")" );
            }

            if ( ! this.FirstRelease.Equals( DateTime.MinValue ) )
            {
                sb.Append( ", " )
                  .Append( this.Duration.TotalMinutes.ToString( "N0" ) )
                  .Append( " min" );
            }

            if ( this.MovieExemplars != null && this.MovieExemplars.Count > 0 )
            {
                sb.Append( " (" )
                  .Append( this.MovieExemplars.Count )
                  .Append( " ex)" );
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

            sb.Append( "Movie ID .............: " )
              .Append( this.ID )
              .AppendLine ();

            sb.Append( "Title ................: " ).AppendLine( this.Title );

            sb.Append( "Movie Exemplars ......: " )
              .Append( this.MovieExemplars.Count )
              .AppendLine ();

            sb.Append( "Country ..............: " ).AppendLine( this.Country )
              .Append( "Language .............: " ).AppendLine( this.Language )
              .Append( "PG Rate ..............: " ).AppendLine( this.PgRate );

            sb.Append( "Genre ................: " )
              .Append( this.Genre.Verbose () )
              .AppendLine ();

            if ( ! this.FirstRelease.Equals( DateTime.MinValue ) )
            {
                sb.Append( "First Release ........: " )
                  .Append( this.FirstRelease.ToString( "yyyy-MM-dd" ) )
                  .AppendLine ();
            }

            if ( ! this.Duration.Equals( DateTime.MinValue ) )
            {
                sb.Append( "Duration .............: " )
                  .Append( this.Duration.TotalMinutes.ToString( "N0" ) )
                  .Append( " min (" );
                  

                if ( this.Duration.TotalHours >= 1 )
                {
                    sb.Append( (int)this.Duration.TotalHours )
                      .Append( "h" );
                }

                if ( this.Duration.Minutes != 0 )
                {
                    sb.Append( this.Duration.TotalHours >= 1 ? " " : string.Empty )
                      .Append( (int)this.Duration.Minutes )
                      .Append( "'" );
                }

                sb.AppendLine( ")" );
            }

            if ( this.Directors.Count > 0 )
            {
                if ( this.Directors.Count == 1 ) 
                {
                    sb.Append( "Director .............: " );
                }
                else
                {
                    sb.Append( "Directors ............: " );
                }

                int seqNo = 0;
                foreach( string name in this.Directors )
                {
                    sb.Append( seqNo++ == 0 ? string.Empty : ", " )
                      .Append( name );
                }

                sb.AppendLine ();
            }

            if ( this.Writers.Count > 0 )
            {
                if ( this.Writers.Count == 1 ) 
                {
                    sb.Append( "Writer ...............: " );
                }
                else
                {
                    sb.Append( "Writers ..............: " );
                }

                int seqNo = 0;
                foreach( string name in this.Writers )
                {
                    sb.Append( seqNo++ == 0 ? string.Empty : ", " )
                      .Append( name );
                }

                sb.AppendLine ();
            }

            if ( this.Actors.Count > 0 )
            {
                if ( this.Actors.Count == 1 ) 
                {
                    sb.Append( "Actor ................: " );
                }
                else
                {
                    sb.Append( "Actors ...............: " );
                }

                int seqNo = 0;
                foreach( MovieActor actor in this.Actors )
                {
                    sb.Append( seqNo == 0 ? string.Empty : string.Empty.PadRight( 24 ) )
                      .Append( actor.ToString () ).AppendLine ();
                    ++seqNo;
                }
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}