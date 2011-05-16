/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.ItemStore
 *  File:       RentedItems.cs
 *  Created:    2011-04-10
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
    using VROLib.CustomerDatabase;

    /// <summary>
    /// Encapsulates <see cref="Customer"/>'s collection of <see cref="RentedItem"/>s.
    /// </summary>
    /// 
    [Serializable]
    public class RentedItems 
        : GenericObjectCollection<RentedItem>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        /// <summary>
        /// Instance of the <see cref="Customer"/> class that owns this collection.
        /// </summary>
        /// 
        public Customer Customer { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RentedItems"/> class that is
        /// owned by a <see cref="Customer"/>.
        /// </summary>
        ///
        public RentedItems( Customer customer )
            : base( customer.Database, "Rented Items", "Rented Item" )
        {
            this.Customer = customer;

            ForwardChangedEventsToDatabase = true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Virtual Methods ]

        /// <summary>
        /// Executed when a new object is added to database.
        /// </summary>
        ///
        public override void OnAddedNew( GenericObject item, string reason = null )
        {
            base.OnAddedNew( item, reason );

            if ( Customer != null )
            {
                // Changes made to customer's rented items are considered
                // as updates of the customer itself and propagated to the customer.
                //
                Database.Customers.OnUpdated( Customer, "Addedd Rented Item" );
            }

            RentedItem ri = item as RentedItem;

            if ( ri != null && ri.Movie != null )
            {
                // Changes made to rented item are considered as updates of the movie
                // exemplar connected to this rented item.
                //
                ri.Movie.MovieExemplars.OnUpdated( ri.Exemplar, "Addedd Rented Item" );
            }
        }

        /// <summary>
        /// Executed when existing object is removed from database.
        /// </summary>
        ///
        public override void OnRemoved( GenericObject item, string reason = null )
        {
            base.OnRemoved( item, reason );

            if ( Customer != null )
            {
                // Changes made to customer's rented items are considered
                // as updates of the customer itself and propagated to the customer.
                //
                Database.Customers.OnUpdated( Customer, "Removed Rented Item" );
            }

            RentedItem ri = item as RentedItem;

            if ( ri != null && ri.Movie != null )
            {
                // Changes made to rented item are considered as updates of the movie
                // exemplar connected to this rented item.
                //
                ri.Movie.MovieExemplars.OnUpdated( ri.Exemplar, "Removed Rented Item" );
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
                sb.AppendLine( "Rented Items collection is empty." );
            }
            else
            {
                sb.AppendLine( "Rented Items:" );
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

            sb.AppendTitle( "Rented Items" );

            sb.Append( base.FullInfo () );

            if ( this.Items.Count != 0 )
            {
                sb.AppendLine ()
                  .Append( "Count: " ).Append( this.Items.Count )
                  .AppendLine ();
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}