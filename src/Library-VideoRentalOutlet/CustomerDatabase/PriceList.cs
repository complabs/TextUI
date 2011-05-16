/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.CustomerDatabase
 *  File:       PriceList.cs
 *  Created:    2011-03-29
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

namespace VROLib.CustomerDatabase
{
    using VROLib.ItemStore;

    /// <summary>
    /// Encapsulates a price specification collection as collection of associations 
    /// between <see cref="Membership"/>s, <see cref="PriceClass"/>es and minimum 
    /// quantities having some price as an attribute. (See also 
    /// <see cref="MinQuantityPrice"/> class.)
    /// </summary>
    /// 
    [Serializable]
    public class PriceList 
        : GenericObjectCollection<MinQuantityPrice>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the price list class containing a default base
        /// price specification. (See <see cref="MinQuantityprice.IsBasePrice"/>.)
        /// </summary>
        ///
        public PriceList( VideoRentalOutlet database )
            : base( database, "Price List", "Price Specification" )
        {
            ForwardChangedEventsToDatabase = true;

            // Add a default 'base' price specification.
            //
            UpdatePrice( Membership.NotMember, PriceClass.SwedishNew, 1, 100m );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Updates price for minimum quantity of rented items for specific membership
        /// and movie price class. If price does not exist, a new min-qty price 
        /// specification is added.
        /// </summary>
        /// <param name="membership">Membership category</param>
        /// <param name="priceClass">Movie price class</param>
        /// <param name="minimumQuantity">Minimum quantity of rented items</param>
        /// <param name="newPrice">New renting price</param>
        /// 
        public MinQuantityPrice UpdatePrice ( 
            Membership membership, PriceClass priceClass, 
            int minimumQuantity, decimal newPrice
            )
        {
            // Find exising min-qty price for given membership, price class and minimum
            // quantity
            //
            MinQuantityPrice price = this.Items.Find( priceSpec =>
            {
                return priceSpec.Membership      == membership
                    && priceSpec.PriceClass      == priceClass
                    && priceSpec.MinimumQuantity == minimumQuantity;
            } );

            if ( price != null )
            {
                // Update min-qty price if it exists
                //
                price.Price = newPrice;

                OnUpdated( price );

                return price;
            }

            // If min-qty price is not found, add new association between membership
            // and price class with minimum quantity and new price as attributes.
            // 
            this.Items.Add
            ( 
                price = new MinQuantityPrice( Database, membership, priceClass ) 
                { 
                    MinimumQuantity = minimumQuantity,
                    Price = newPrice
                } 
            );

            // Keep list sorted, so it can be searched properly.
            //
            Sort ();

            // Raise event that a new price specification was added to the price list
            //
            OnAddedNew( price );

            return price;
        }

        /// <summary>
        /// Sorts the price list with fees 'normally' (see remarks) in ascening order.
        /// See <see cref="MinQuantityPrice.CompareTo()"/>.
        /// </summary>
        /// 
        private void Sort ()
        {
            this.Items.Sort( ( t1, t2 ) => t1.CompareTo( t2 ) );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ]

        /// <summary>
        /// Removes price from PriceList.
        /// </summary>
        /// 
        public override bool Remove( MinQuantityPrice price )
        {
            // We can remove only existing prices
            //
            if ( price == null ) 
            {
                return false;
            }

            // It is not allowed to delete a default price ('base price') which is
            // a price specification for NotMember, SwedishNew with minimum quantity 1.
            //
            if ( price.IsBasePrice )
            {
                throw new InvalidOperationException(
                    "It is not allowed to delete a default base price!" );
            }

            return base.Remove( price );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ ToString() and FullInfo() Virtual Methods ]

        /// <summary>
        /// Gets a brief contents of price list.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            if ( this.Items.Count == 0 )
            {
                sb.Append( "Price List is empty." );
            }
            else
            {
                sb.AppendLine( "Price List:" );
                sb.Append( base.ToString () );
            }

            return sb.ToString ();
        }

        /// <summary>
        /// Gets a thorough contents of price list.
        /// </summary>
        /// 
        public override string FullInfo ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.AppendTitle( "Price List" );

            sb.Append( base.FullInfo () );

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}