/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.CustomerDatabase
 *  File:       Customer.cs
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
using System.Collections.ObjectModel;
using System.Text;
using Mbk.Commons;

namespace VROLib.CustomerDatabase
{
    using ItemStore;

    /// <summary>
    /// Video Rental Outlet's Customer database record.
    /// </summary>
    /// 
    [Serializable]
    public class Customer : TransactionalObject<Customer>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Mandatory Properties ]

        public string PersonID
        {
            get { return this.persID; }
            set { ThrowExceptionIfNotMutable (); this.persID = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string FirstName
        {
            get { return this.firstName; }
            set { ThrowExceptionIfNotMutable (); this.firstName = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string LastName
        {
            get { return this.lastName; }
            set { ThrowExceptionIfNotMutable (); this.lastName = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Optional Properties ]

        public string Address
        {
            get { return this.address; }
            set {  ThrowExceptionIfNotMutable (); this.address = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string PostCode
        {
            get { return this.postCode;}
            set { ThrowExceptionIfNotMutable (); this.postCode = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string City
        {
            get { return this.city; }
            set { ThrowExceptionIfNotMutable (); this.city = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string Country
        {
            get { return this.country; }
            set { ThrowExceptionIfNotMutable (); this.country = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string Phone
        {
            get { return this.phone; }
            set { ThrowExceptionIfNotMutable (); this.phone = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string CellPhone
        {
            get { return this.cellPhone; }
            set { ThrowExceptionIfNotMutable (); this.cellPhone = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string EMail
        {
            get { return this.eMail; }
            set 
            { 
                ThrowExceptionIfNotMutable ();
                this.eMail = value;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public Membership Membership
        {
            get { return this.membership; }
            set { ThrowExceptionIfNotMutable (); this.membership = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public CreditCard CreditCard
        {
            get { return this.creditCard; }
            set { ThrowExceptionIfNotMutable (); this.creditCard = value; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Calculated Properties ]

        /// <summary>
        /// Gets customer's full name as first name followed by last name.
        /// </summary>
        /// 
        public string FullName
        {
            get
            {
                return new StringBuilder ()
                    .Append( this.FirstName ).Append( " " ).Append( this.LastName )
                    .ToString ();
            }
        }

        /// <summary>
        /// Gets number of rented items.
        /// </summary>
        /// 
        public int RentedItemsCount
        {
            get { return RentedItems != null ? RentedItems.Count : 0; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        /// <summary>
        /// Contains aggregated list of rented items. This field is exposed as 
        /// read-only collection property RentedItems.
        /// </summary>
        ///
        public RentedItems RentedItems { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Fields ]

        // Private fields behind public properties 
        //
        private string     persID     ;
        private string     firstName  ;
        private string     lastName   ;
        private string     address    ;
        private string     postCode   ;
        private string     city       ;
        private string     country    ;
        private string     phone      ;
        private string     cellPhone  ;
        private string     eMail      ;
        private Membership membership ;
        private CreditCard creditCard ;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new (mutable) instance of the Customer class either as an empty
        /// (without argument) or as a copy-constructed object from an existing 
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
        public Customer( Customer original = null )
            : base( original, "Customer" )
        {
            if ( ! IsNew ) // get fields from original
            {
                this.ID = original.ID;

                this.SetFieldsFrom( BaseRecord );

                this.RentedItems = BaseRecord.RentedItems;
            }
            else // initialize default fields
            {
                this.Membership = Membership.NotMember;

                // Note: we should not instantiate this.RentedItems collection
                // as we are still not connected to database. Instantiation is delayed
                // until AddTo() is called.
                //
                this.RentedItems = null;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ]

        /// <summary>
        /// Executed when record is about to be removed from database.
        /// Removes also all all rented items by this customer.
        /// </summary>
        /// 
        internal override void OnRemove ()
        {
            if ( RentedItems != null )
            {
                RentedItems.Clear ();
            }
        }

        /// <summary>
        /// Copies fields from source record. This is low-level method that
        /// disregards if destination record (i.e. this record) is immutable.
        /// </summary>
        ///
        protected override void SetFieldsFrom( Customer source )
        {
            this.firstName  = source.firstName  ;
            this.lastName   = source.lastName   ;
            this.persID     = source.persID     ;
            this.address    = source.address    ;
            this.address    = source.address    ;
            this.postCode   = source.postCode   ;
            this.city       = source.city       ;
            this.country    = source.country    ;
            this.phone      = source.phone      ;
            this.cellPhone  = source.cellPhone  ;
            this.eMail      = source.eMail      ;
            this.membership = source.membership ;
            this.creditCard = source.creditCard ;
        }

        /// <summary>
        /// Verifies fields integrity of the record.
        /// </summary>
        /// 
        protected override void VerifyIntegrity ()
        {
            if ( string.IsNullOrEmpty( PersonID ) )
            {
                throw new ArgumentException(
                    "Customer's Person-ID must not be null" );
            }

            string notValidInfo = ValidatePNR( PersonID );

            if ( notValidInfo != null )
            {
                throw new ArgumentException( notValidInfo );
            }

            if ( ! IsMarkedAsForeign( PersonID ) ) // Remove spaces from domestic PID
            {
                PersonID = PersonID.Replace( " ", "" );
            }

            if ( string.IsNullOrEmpty( FirstName ) )
            {
                throw new ArgumentException( 
                    "Customer's first name must not be null" );
            }

            if ( string.IsNullOrEmpty( LastName ) )
            {
                throw new ArgumentException( 
                    "Customer's last name must not be null" );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Inserts newly created instance into database.
        /// </summary>
        /// 
        public Customer AddTo( VideoRentalOutlet database )
        {
            VerifyAddTo( database );

            Database = database;

            Customer customerWithPID = Database.Customers.FindByPersID( PersonID );

            if ( customerWithPID != null && customerWithPID != this )
            {
                throw new ArgumentException( "Customer with the same Person-ID "
                    + "already exists in database." );
            }

            ID = Database.NextCustomerID;

            this.RentedItems = new RentedItems( this );

            LockProperties (); // make record immutable

            Database.Customers.Add( this );

            return this;
        }

        /// <summary>
        /// Updates an instance obtained through copy constructor into database.
        /// </summary>
        /// 
        public Customer Update ()
        {
            VerifyUpdate ();

            Customer customerWithPID = Database.Customers.FindByPersID( PersonID );

            if ( customerWithPID != null && customerWithPID != BaseRecord )
            {
                throw new ArgumentException( "Customer #" + customerWithPID.ID
                    + " (" + customerWithPID.FullName + ") already has PID " + PersonID );
            }

            BaseRecord.SetFieldsFrom( this );

            Database.Customers.OnUpdated( BaseRecord );

            return BaseRecord;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Person-ID Validation ]

        /// <summary>
        /// Returns true if person ID starts with letter, which indicates a foreign PID.
        /// </summary>
        ///
        private bool IsMarkedAsForeign( string pnr )
        {
            pnr = pnr.Trim ();
            return pnr.Length > 0 && ! char.IsDigit( pnr[ 0 ] );
        }

        /// <summary>
        /// Validates Swedish PID. Returns null if OK, otherwise a string containing 
        /// an error message.
        /// </summary>
        ///
        public static string ValidatePNR( string pnr )
        {
            if ( string.IsNullOrEmpty( pnr ) )
            {
                return null;
            }

            pnr = pnr.Replace( " ", "" ).Replace( "-", "" ).Replace( "+", "" );

            // Foreign person ID start with letter
            //
            if ( pnr.Length > 0 && ! char.IsDigit( pnr[ 0 ] ) )
            {
                return null;
            }

            if ( pnr.Length != 10 && pnr.Length != 12 )
            {
                return "Swedish PID must have 12 or 16 digits. "
                    + "Begin PID with letters to specify a foreign PID";
            }

            for ( int i = 0; i < pnr.Length; ++i )
            {
                if ( ! char.IsDigit( pnr[ i ] ) )
                {
                    return "Swedish PID consists of digits and +/- sign. "
                        + "Begin PID with letters to specify a foreign PID";
                }
            }

            string yearDigits  = pnr.Substring( 0, pnr.Length == 10 ? 2 : 4 );
            string monthDigits = pnr.Substring( pnr.Length == 10 ? 2 : 4, 2 );
            string dayDigits   = pnr.Substring( pnr.Length == 10 ? 4 : 6, 2 );
            // string seqNoDigits = pnr.Substring( pnr.Length == 10 ? 6 : 8, 4 );

            if ( yearDigits.Length == 2 )
            {
                yearDigits = "20" + yearDigits;
            }

            try
            {
                DateTime dateOfBirth = DateTime.ParseExact( 
                    yearDigits + "-" + monthDigits + "-" + dayDigits,
                    "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture );
            }
            catch
            {
                return "Date of birth part of PID is not valid date.";
            }

            // Here normally comes a checksum verification (for algorithm
            // see http://sv.wikipedia.org/wiki/Personnummer_i_Sverige).
            //
            // However, not to be very restrictive, just return null (which means OK,
            // as it is expected to return an error).
            //
            return null;
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

            sb.Append( "Customer #" ).Append( this.ID )
              .Append( ": " ).Append( this.FullName )
              .Append( ", PID " ).Append( this.PersonID );

            if ( this.RentedItems != null )
            {
                if ( this.RentedItems.Count == 0 )
                {
                    sb.Append( ", No Rentals" );
                }
                else
                {
                    sb.Append( ", Rentals " ).Append( this.RentedItems.Count );
                }
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

            sb.Append( "Customer ID ..........: " )
              .Append( this.ID )
              .AppendLine ();

            sb.Append( "First Name ...........: " ).AppendLine( this.FirstName )
              .Append( "Last Name ............: " ).AppendLine( this.LastName )
              .Append( "Person-ID ............: " ).AppendLine( this.PersonID );

            sb.Append( "Rentals ..............: " )
              .Append( this.RentedItems.Count )
              .AppendLine ();

            sb.Append( "Address ..............: " ).AppendLine( this.Address )
              .Append( "Post Code ............: " ).AppendLine( this.PostCode )
              .Append( "City .................: " ).AppendLine( this.City )
              .Append( "Country ..............: " ).AppendLine( this.Country )
              .Append( "Phone ................: " ).AppendLine( this.Phone )
              .Append( "Cell Phone ...........: " ).AppendLine( this.CellPhone )
              .Append( "E-Mail Address .......: " ).AppendLine( this.EMail );

            sb.Append( "Membership ...........: " )
              .Append( this.Membership.Verbose () )
              .AppendLine ();

            if ( this.CreditCard.Type != CreditCardType.None )
            {
                sb.Append( "Credit Card ..........: " )
                  .Append( this.CreditCard )
                  .AppendLine ();
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}