/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.ItemStore
 *  File:       RentedItem.cs
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
    using CustomerDatabase;

    /// <summary>
    /// RentedItem is an association class between <see cref="Customer"/> and several 
    /// <see cref="MovieExemplar"/>s. Customer class contains collection of instances 
    /// of RentedItems while MovieExemplar contains reference to single RentedItem 
    /// (if rented) or null (if exemplar is not rented).
    /// </summary>
    ///
    [Serializable]
    public class RentedItem : GenericObject
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets the last date when the item should be returned.
        /// </summary>
        ///
        public DateTime DueDate { get; private set; }

        /// <summary>
        /// Gets calculated rental fee for the item.
        /// </summary>
        ///
        public decimal RentalFee { get; private set; }

        /// <summary>
        /// Gets current overdue in days (if any); otherwise, 0.
        /// </summary>
        /// 
        public int OverdueDays
        {
            get
            {
                TimeSpan ts = DateTime.Now.Subtract( this.DueDate );
                return ts.TotalDays <= 0 ? 0 : (int)Math.Ceiling( ts.TotalDays );
            }
        }

        /// <summary>
        /// Gets due date as a string in ISO format.
        /// </summary>
        /// 
        public string VerboseDueDate
        {
            get { return DueDate.ToString( "yyyy-MM-dd" ); }
        }

        /// <summary>
        /// Gets rental conditions (i.e. due date and fee) as string info
        /// </summary>
        /// 
        public string VerboseConditions
        {
            get 
            {
                StringBuilder sb = new StringBuilder ();

                sb.Append( "Due Date: " )
                  .Append( this.VerboseDueDate )
                  .Append( ", Rental Fee: " )
                  .Append( this.RentalFee.ToString( "0.00" ) );

                return sb.ToString ();
            }
        }

        /// <summary>
        /// Gets movie of the movie exemplar.
        /// </summary>
        /// 
        public Movie Movie
        {
            get
            {
                return Exemplar != null ? Exemplar.Movie : null;
            }
        }

        /// <summary>
        /// Gets textual info about rented exemplar like movie title, exemplar media
        /// and exemplar ID.
        /// </summary>
        /// 
        public string ExemplarInfo
        {
            get
            {
                return Exemplar.Movie.FullTitle + ", " + Exemplar.Media.Verbose() 
                    + " Ex #" + Exemplar.ID.ToString ();
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        public MovieExemplar Exemplar { get; private set; }

        public Customer RentedTo { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the RentedItem as association between
        /// MovieExemplar and Customer with dueDate and rentalFee as attributes.
        /// </summary>
        /// 
        public RentedItem( MovieExemplar exemplar, DateTime dueDate, decimal rentalFee )
            : base( exemplar.Database, "Rented Item" )
        {
            this.RentedTo  = null;
            this.Exemplar  = exemplar;

            this.DueDate   = dueDate;
            this.RentalFee = rentalFee;

            VerifyIntegrity ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Inserts newly created instance into database.
        /// </summary>
        /// 
        public void AddTo( Customer customer )
        {
            VerifyIntegrity ();

            if ( this.Exemplar.IsRented )
            {
                throw new ArgumentException( "Movie exemplar is already rented." );
            }

            // Relationships
            //
            this.RentedTo = customer;
            this.Database = customer.Database;

            // Add reference to RentedItem in MovieExemplar and add RentedItem to 
            // Customer's collection of rented items.
            //
            this.Exemplar.RentedAsItem = this;
            this.RentedTo.RentedItems.Add( this );
        }

        /// <summary>
        /// Updates new conditions of existing instance.
        /// </summary>
        /// 
        public void SetNewConditions( DateTime dueDate, decimal rentalFee )
        {
            this.DueDate   = dueDate;
            this.RentalFee = rentalFee;

            VerifyIntegrity ();

            RentedTo.RentedItems.OnUpdated( this, "New Conditions" );

            Exemplar.Movie.MovieExemplars.OnUpdated( Exemplar, "New Conditions" );
        }

        /// <summary>
        /// Verifies fields integrity of the record.
        /// </summary>
        /// 
        private void VerifyIntegrity ()
        {
            if ( this.Exemplar == null )
            {
                throw new ArgumentException( "Movie exemplar must not be null." );
            }

            if ( this.DueDate == DateTime.MinValue )
            {
                throw new ArgumentException( "Date time must not be null." );
            }

            if ( this.RentalFee <= 0 )
            {
                throw new ArgumentException( "Rental fee must be a postive number." );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Internal Methods ]

        /// <summary>
        /// Unlink relationship between MovieExemplar and Customer.
        /// </summary>
        /// 
        internal void ReturnFromRental ()
        {
            if ( RentedTo != null )
            {
                RentedTo.RentedItems.Remove( this );
                RentedTo = null;
            }
        }

        /// <summary>
        /// Trigger called when removal of the customer is requested.
        /// Removes all customer's rented items.
        /// </summary>
        /// 
        internal override void OnRemove ()
        {
            Exemplar.RentedAsItem = null;
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
            StringBuilder sb = new StringBuilder ();

            sb
              .Append( "MovieEx #" ).Append( Exemplar.ID )
              .Append( ", Rented by Customer #" ).Append( RentedTo.ID )
              .Append( ", " ).Append( this.VerboseConditions );

            return sb.ToString ();
        }

        /// <summary>
        /// Determines whether this instance and a specified object are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if obj type is the same and they have same IDs; 
        /// otherwise, false.</returns>
        /// 
        public override bool Equals( object obj )
        {
            // Return false if obj is either null or if it is not of
            // the same type as our instance.
            //
            RentedItem otherPrice = obj as RentedItem;
            if ( obj == null )
            {
                return false;
            }

            // Two rented items are same if they belongs to the same customer
            // and points to the same movie exemplar.
            //
            return this.RentedTo == otherPrice.RentedTo
                && this.Exemplar == otherPrice.Exemplar;
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// 
        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}