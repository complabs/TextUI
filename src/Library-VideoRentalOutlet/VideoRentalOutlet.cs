/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib
 *  File:       VideoRentalOutlet.cs
 *  Created:    2011-03-12
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Mbk.Commons;

namespace VROLib
{
    using CustomerDatabase;
    using ItemStore;

    /// <summary>
    /// Video Rental Outlet database with customers, movies, movie exemplars,
    /// price list and movie exemplar rentals.
    /// <remarks>
    /// Note that instance of VideoRentalOutlet class is <see cref="MonitoredObject"/> 
    /// (and thus derived from <see cref="GenericObject"/>) so it can be manipulated
    /// as any other object (i.e. record with fields) in database.
    /// </remarks>
    /// </summary>
    /// 
    [Serializable]
    public class VideoRentalOutlet : MonitoredObject
    {
        /// <summary>
        /// VROLib version needed to resolve different database versions. Used when
        /// deserializing database from a binary file. See <see cref="IsValidVersion"/>)
        /// property and <see cref="VideoRentalOutlet.Deserialize"/> method.
        /// </summary>
        /// 
        private const string VROLibVersion = "1.19";

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        // Mandatory properties:

        public string Version { get; private set; }
        public string Name    { get; private set; }
        public string VatNo   { get; private set; }

        // Optional properties:

        public string Address   { get; set; }
        public string PostCode  { get; set; }
        public string City      { get; set; }
        public string Country   { get; set; }
        public string Phone     { get; set; }
        public string HomePage  { get; set; }
        public string EMail     { get; set; }

        // Calculated properties:

        /// <summary>
        /// Gets total number of records (customers, price lists, movies and
        /// movie exemplars) in database. 
        /// </summary>
        /// 
        public int TotalRecordsCount
        {
            get 
            { 
                return Customers.Count + PriceList.Count 
                     + Movies.Count + MovieExemplars.Count;
            }
        }

        /// <summary>
        /// Returns if database version is the same as VROLib version. 
        /// </summary>
        /// <remarks>
        /// When created, database is tagged with VROLibVersion. As user may
        /// deserialize database, this ensures that deserialized object has the same
        /// version as VRO library. This is important because deserialization might
        /// instantiate newlly added properties (between versions) to null.
        /// </remarks>
        /// 
        public bool IsValidVersion
        {
            get { return this.Version == VideoRentalOutlet.VROLibVersion; }
        }

        /// <summary>
        /// Returns if instance has been modified since last serialization to file.
        /// </summary>
        /// 
        public override bool IsDirty 
        {
            get
            {
                return base.IsDirty
                    || Customers.IsDirty || PriceList.IsDirty
                    || Movies.IsDirty    || MovieExemplars.IsDirty;
            }
            set
            {
                base.IsDirty = value;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Auto-incremented Unique ID Generators ]

        private int lastCustomerID;
        private int lastPriceSpecID;
        private int lastMovieID;

        /// <summary>
        /// Gets auto-incremented customer unique ID.
        /// </summary>
        ///
        internal int NextCustomerID
        {
            get { return ++this.lastCustomerID; }
        }

        /// <summary>
        /// Gets auto-incremented price specification unique ID.
        /// </summary>
        ///
        internal int NextPriceSpecID
        {
            get { return ++this.lastPriceSpecID; }
        }

        /// <summary>
        /// Gets auto-incremented movie unique ID.
        /// </summary>
        /// 
        internal int NextMovieID
        {
            get { return ++this.lastMovieID; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        public CustomerCollection Customers { get; private set; }

        public PriceList PriceList { get; private set; }

        public MovieCollection Movies { get; private set; }

        public MovieExemplarCollection MovieExemplars { get; private set; }

        /// <summary>
        /// Gets rented items enumerated for all customers. Returns collection
        /// using "yield return".
        /// </summary>
        /// 
        public IEnumerable<RentedItem> RentedItems
        {
            get
            {
                foreach( Customer customer in Customers )
                {
                    foreach( RentedItem rentedItem in customer.RentedItems )
                    {
                        yield return rentedItem;
                    }
                }
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the VideoRentalOutlet class with mandatory 
        /// properties.
        /// </summary>
        /// <remarks>
        /// New instance is constructed as dirty.
        /// </remarks>
        /// <param name="name">a string with video store name</param>
        /// <param name="vatNo">a string with VAT number></param>
        /// 
        public VideoRentalOutlet( string name, string vatNo )
            : base( null, "Video Rental Outlet" )
        {
            this.Database = this;
            this.Version  = VROLibVersion;

            // Mandatory properties:
            //
            SetName  ( name  );
            SetVatNo ( vatNo );

            // Default property values:
            //
            this.Address        = null;
            this.PostCode       = null;
            this.City           = null;
            this.Country        = null;
            this.Phone          = null;

            // Default relationships:
            //
            this.Customers      = new CustomerCollection( this );
            this.Movies         = new MovieCollection( this );
            this.MovieExemplars = new MovieExemplarCollection( this );
            this.PriceList      = new PriceList( this );

            // Default values:
            //
            this.lastCustomerID = 0;
            this.lastMovieID    = 0;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]
       
        /// <summary>
        /// Change company name. Throws exception if a new company name is null.
        /// </summary>
        ///
        /// 
        public void SetName( string newCompanyName )
        {
            if ( string.IsNullOrEmpty( newCompanyName ) )
            {
                throw new ArgumentException( "Company Name must not be null" );
            }

            IsDirty = true;
            Name = newCompanyName;
        }

        /// <summary>
        /// Change company's VAT Number. Throws exception if a new VAT Number is null.
        /// </summary>
        ///
        public void SetVatNo( string newVatNo )
        {
            if ( string.IsNullOrEmpty( newVatNo ) )
            {
                throw new ArgumentException( "Company's VAT Number must not be null" );
            }

            IsDirty = true;
            VatNo = newVatNo;
        }

        /// <summary>
        /// Gets suggested rental fee from price list according customer's membership 
        /// status and movie exemplar's price class. Price list (which is sorted with
        /// descending membership status, ascending price class and descending minimum 
        /// quantity) is searched until minimal qualifying price specification is found.
        /// </summary>
        /// 
        public decimal GetPrice( 
            Membership membership, PriceClass priceClass, int quantity )
        {
            decimal fee = 0;

            foreach( MinQuantityPrice price in PriceList )
            {
                fee = price.Price;

                if ( membership >= price.Membership && priceClass <= price.PriceClass
                    && quantity >= price.MinimumQuantity )
                {
                    break;
                }
            }

            return fee;
        }

        /// <summary>
        /// Searches matching customer in collection.
        /// </summary>
        /// 
        public Customer FindCustomer( Predicate<Customer> match )
        {
            if ( match == null )
            {
                return null; // nothing to match
            }

            foreach ( Customer customer in Customers )
            {
                if ( match( customer ) )
                {
                    return customer;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes a first matching customer from outlet's customer database.
        /// </summary>
        /// 
        public void RemoveCustomer( Predicate<Customer> match )
        {
            Customers.Remove( FindCustomer( match ) );
        }

        /// <summary>
        /// Searches for movies in library.
        /// </summary>
        /// 
        public Movie FindMovie( Predicate<Movie> match )
        {
            if ( match == null )
            {
                return null; // nothing to add
            }

            foreach ( Movie movie in Movies )
            {
                if ( match( movie ) )
                {
                    return movie;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes a first matching movie from library
        /// </summary>
        /// 
        public void RemoveMovie( Predicate<Movie> match )
        {
            Movies.Remove( FindMovie( match ) );
        }

        /// <summary>
        /// Searches for movie exemplars in library.
        /// </summary>
        /// 
        public MovieExemplar FindMovieExemplar( Predicate<MovieExemplar> match )
        {
            if ( match == null )
            {
                return null; // nothing to add
            }

            foreach ( Movie movie in Movies )
            {
                foreach ( MovieExemplar exemplar in movie.MovieExemplars )
                {
                    if ( match( exemplar ) )
                    {
                        return exemplar;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Removes a first matching movie exemplar from library
        /// </summary>
        /// 
        public void RemoveMovieExemplar( Predicate<MovieExemplar> match )
        {
            MovieExemplar exemplar = FindMovieExemplar( match );

            if ( exemplar != null )
            {
                exemplar.Movie.MovieExemplars.Remove( exemplar );
            }
        }

        /// <summary>
        /// Rents a MovieExemplar to a Customer.
        /// </summary>
        /// <param name="exemplar">instance of MovieExemplar class; must not be
        /// already rented</param>
        /// <param name="customer">instance of Customer class</param>
        /// <param name="days">an integer with number of days</param>
        /// <param name="fee">an decimal with rental fee</param>
        /// 
        public void RentItem( MovieExemplar exemplar, Customer customer, 
            int days, decimal fee )
        {
            if ( exemplar == null || customer == null )
            {
                return; // nothing to rent
            }

            if ( exemplar.IsRented )
            {
                throw new Exception( "Cannot rent item that is already rented." );
            }

            DateTime dueDate = DateTime.Now.AddDays( days );

            // Create RentedItem as assocation between MovieExemplar and Customer
            //
            RentedItem item = new RentedItem( exemplar, dueDate, fee );
            item.AddTo( customer );
        }

        /// <summary>
        /// Returns a MovieExemplar from rental.
        /// </summary>
        /// <param name="exemplar">a reference to MovieExemplar to be returned</param>
        /// 
        public void ReturnItem( MovieExemplar exemplar )
        {
            if ( exemplar == null || exemplar.RentedAsItem == null )
            {
                return; // nothing to do
            }

            exemplar.RentedAsItem.ReturnFromRental ();
        }

        /// <summary>
        /// Returns text info about all customers that have rented something
        /// </summary>
        ///
        public string QueryAllCustomersWithRentals ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.AppendLine( "===========================" );
            sb.AppendLine( "Customers with rented items" );
            sb.AppendLine( "===========================" );

            int rentedItemsCount = 0;
            int customerCount = 0;

            foreach( Customer c in this.Customers )
            {
                if ( c.RentedItems.Count > 0 )
                {
                    ++customerCount;

                    sb.AppendLine ().Append( c.ToString () )
                      .Append( ", rents " ).Append( c.RentedItems.Count )
                      .AppendLine( " items:" );
                    
                    int ic = 0;
                    foreach( RentedItem item in c.RentedItems )
                    {
                        ++rentedItemsCount;

                        sb.AppendLine ()
                          .Append( string.Format( "{0,4}. ", ++ic ) )
                          .Append( "Exemplar #" ).Append( item.Exemplar.ID )
                          .Append( ": " ).Append( item.Exemplar.Media.Verbose () )
                          .Append( ", " ).Append( item.Movie.FullTitle )
                          .AppendLine ()
                          .Append( string.Empty.PadRight( 6 ) ).Append( item )
                          .AppendLine ();
                    }
                }
            }

            sb.AppendLine ()
              .Append( "*** Customer Count = " ).Append( customerCount )
              .Append( ", Total Rented Item Count = " ).Append( rentedItemsCount )
              .AppendLine( " ***" );

            return sb.ToString ();
        }

        /// <summary>
        /// Returns text info about all movie exemplars that are rented out
        /// </summary>
        ///
        public string QueryAllRentedItems ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.AppendLine( "======================" );
            sb.AppendLine( "Currently rented items" );
            sb.AppendLine( "======================" );

            int rentedItemsCount = 0;

            foreach ( Movie movie in Movies )
            {
                foreach( MovieExemplar exemplar in movie.MovieExemplars )
                {
                    if ( exemplar.IsRented )
                    {
                        ++rentedItemsCount;

                        sb.AppendLine ().Append( exemplar.ToString () ).AppendLine ()
                          .AppendLine ().Append( exemplar.RentalInfo );
                    }
                }
            }

            sb.AppendLine ()
              .Append( "*** Rented Item Count = " ).Append( rentedItemsCount )
              .AppendLine( " ***" );

            return sb.ToString ();
        }

        /// <summary>
        /// Returns statistics info (number of records) as a string.
        /// Returned string is centered inside some width (if specified).
        /// </summary>
        ///
        public string StatisticsInfo( int width = 0 )
        {
            int padLeft = 5 + Math.Max( 0, ( width - 25 ) / 2 );

            StringBuilder sb = new StringBuilder ();

            sb.Append( Customers.Count.ToString ().PadLeft( padLeft ) )
              .AppendLine( " customers" );

            sb.Append( PriceList.Count.ToString ().PadLeft( padLeft ) )
              .AppendLine( " price specifications" );

            sb.Append( Movies.Count.ToString ().PadLeft( padLeft ) )
              .AppendLine( " movies" );

            sb.Append( MovieExemplars.Count.ToString ().PadLeft( padLeft ) )
              .AppendLine( " movie exemplars" );

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ]

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// 
        public override string ToString ()
        {
            return new StringBuilder ()
                .Append( "VRO '" ).Append( this.Name )
                .Append( "': " ).Append( Customers.Count )
                .Append( " customers, " ).Append( PriceList.Count )
                .Append( " prices, " ).Append( Movies.Count )
                .Append( " movies, " ).Append( MovieExemplars.Count )
                .Append( " exemplars" )
                .ToString ();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// with full details about the record.
        /// </summary>
        /// 
        public override string FullInfo ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append( "Video Rental Outlet ..: " ).AppendLine( this.Name )
              .Append( "VAT# .................: " ).AppendLine( this.VatNo )
              .Append( "Address ..............: " ).AppendLine( this.Address )
              .Append( "Post Code ............: " ).AppendLine( this.PostCode )
              .Append( "City .................: " ).AppendLine( this.City )
              .Append( "Country ..............: " ).AppendLine( this.Country )
              .Append( "Phone ................: " ).AppendLine( this.Phone )
              .Append( "Home Page ............: " ).AppendLine( this.HomePage )
              .Append( "E-Mail Address .......: " ).AppendLine( this.EMail );

            sb.AppendLine ()
              .Append( "Contents:".PadLeft( 16 ) )
              .Append( Customers.Count.ToString ().PadLeft( 7 ) )
              .AppendLine( " customers" );

            sb.Append( PriceList.Count.ToString ().PadLeft( 23 ) )
              .AppendLine( " price specifications" );

            sb.Append( Movies.Count.ToString ().PadLeft( 23 ) )
              .AppendLine( " movies" );

            sb.Append( MovieExemplars.Count.ToString ().PadLeft( 23 ) )
              .AppendLine( " movie exemplars" );

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Saves (serializes) database in binary form to an instance of the file stream.
        /// </summary>
        /// 
        public void Serialize( Stream fileStream )
        {
            new BinaryFormatter ().Serialize( fileStream, this );

            // Note that 'dirty' flags are not serialized so we can do *after* we
            // have saved object to file (as it will appear 'clean' when reloaded).
            // If Serialize() throws an exception, the following code won't be executed,
            // so the video store would keep its dirty status unchanged.
            //
            IsDirty                = false;
            Customers.IsDirty      = false;
            PriceList.IsDirty      = false;
            Movies.IsDirty         = false;
            MovieExemplars.IsDirty = false;
        }

        /// <summary>
        /// Returns a new instance of VideoRentalOutlet class deserialized (loaded) 
        /// from binary file stream.
        /// </summary>
        /// <returns>a new instance of VideoRentalOutlet database</returns>
        /// <remakrs>VideoRentalOutlet factory method</remakrs>
        /// 
        public static VideoRentalOutlet Deserialize( Stream fileStream ) 
        {
            BinaryFormatter formatter = new BinaryFormatter ();

            // Deserialize Video Rental Outlet database from the file and 
            // assign the reference to the local variable.
            //
            object deserializedObject = formatter.Deserialize( fileStream );
            VideoRentalOutlet store = deserializedObject as VideoRentalOutlet;

            if ( store != null && ! store.IsValidVersion )
            {
                string info = store.Version == null 
                    ? "of unkown version" : "v" + store.Version;

                throw new Exception( "Incompatible VRO database version!\r\n\r\n"
                    + "File format is " + info
                    + ". Application can handle only files in v" 
                    + VROLibVersion + " format." );
            }

            if ( store == null )
            {
                throw new Exception( "Failed to deserialize VRO database." );
            }

            return store;
        }
    }
}