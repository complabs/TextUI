/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.CustomerDatabase
 *  File:       MinQuantityPrice.cs
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

namespace VROLib.CustomerDatabase
{
    using ItemStore;

    /// <summary>
    /// Association class between <see cref="Membership"/> (non-member, member, etc) and 
    /// <see cref="PriceClass"/> (old-movie, new-hot-movie etc) with particular price 
    /// valid for  minimum quantity of total rented items.
    /// </summary>
    ///
    [Serializable]
    public class MinQuantityPrice 
        : GenericObject, IComparable<MinQuantityPrice>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Minimum quantity for which this price specification is valid.
        /// </summary>
        /// 
        public int MinimumQuantity { get; internal set; }

        /// <summary>
        /// A rental fee for this price specification i.e. combination of membership, 
        /// price class and minimum quantity.
        /// </summary>
        /// 
        public decimal Price { get; internal set; }

        /// <summary>
        /// Returns true if price specification is a default base price
        /// (price for <see cref="Membership.NotMember"/> for 
        /// <see cref="PriceClass.SwedishNew"/> with minimum quantity 1).
        /// </summary>
        /// 
        public bool IsBasePrice
        {
            get
            {
                return Membership == Membership.NotMember
                    && PriceClass == PriceClass.SwedishNew
                    && MinimumQuantity == 1;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Relationships ]

        // Note that MinQuantityPrice is an association class between Membership
        // and PriceClass.

        public Membership Membership { get; private set; }
        public PriceClass PriceClass { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of price specification. Note that constructor is
        /// marked as internal, so the user cannot directly create instances --
        /// it must use <see cref="PriceList.UpdatePrice"/> method instead.
        /// </summary>
        /// 
        internal MinQuantityPrice( VideoRentalOutlet database, 
                Membership membership, PriceClass priceClass )
            : base( database, "Price Specification" )
        {
            // Mandatory values
            //
            this.ID = database.NextPriceSpecID;
            this.Membership = membership;
            this.PriceClass = priceClass;

            // Default properties:
            //
            this.MinimumQuantity = 0;
            this.Price = 0;
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
            return new StringBuilder ()
                .Append( "Price for " )
                .Append( this.Membership.Verbose () )
                .Append( " and " )
                .Append( this.PriceClass.Verbose () )
                .Append( ", Min.Qty: " )
                .Append( this.MinimumQuantity.ToString () )
                .Append( ", Fee: " )
                .Append( this.Price.ToString( "0.00" ) )
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

            sb.Append( "Membership ...........: " )
              .Append( this.Membership.Verbose () )
              .AppendLine ();

            sb.Append( "Price Class ..........: " )
              .Append( this.PriceClass.Verbose () )
              .AppendLine ();

            sb.Append( "Minimum Quantity .....: " )
              .Append( this.MinimumQuantity.ToString () )
              .AppendLine ();

            sb.Append( "Price ................: " )
              .Append( this.Price.ToString( "0.00" ) )
              .AppendLine ();

            return sb.ToString ();
        }

        /// <summary>
        /// Determines whether this instance and a specified object are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if obj type is the same and they have same memberships,
        /// price classes and minimum quantities; otherwise, false.</returns>
        /// 
        public override bool Equals( object obj )
        {
            // Return false if obj is either null or if it is not of
            // the same type as our instance.
            //
            MinQuantityPrice otherPrice = obj as MinQuantityPrice;
            if ( obj == null )
            {
                return false;
            }

            // Two price specifications are same if have the same memberships,
            // price classes and minimum quantities.
            //
            return this.Membership      == otherPrice.Membership
                && this.PriceClass      == otherPrice.PriceClass
                && this.MinimumQuantity == otherPrice.MinimumQuantity;
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

        #region [ IComparable Interface Implementation  ]

        /// <summary>
        /// Compares the current instance with another price specification object and 
        /// returns an integer that indicates whether the current instance precedes, 
        /// follows, or occurs in the same position in the sort order as the other
        /// price specification.
        /// </summary>
        /// <remarks>
        /// Membership is sorted in descending order, as fees should rise with decreased
        /// membership status (i.e. fall with higher status).
        /// Price class is sorted in ascending order, as fees should rise with increased
        /// price class (i.e. fall with lower price class).
        /// Minimum quantity is sorted in descending order, as fees should rise with
        /// decreased minimum quantity (i.e. fall with increased quantity).
        /// However, it is *up to user* to specify sensible fees!
        /// </remarks>
        /// <param name="other">an instance of another price specification to be 
        /// compared with</param>
        /// <returns>
        /// <list type="table">
        ///   <listheader>
        ///     <term>Value</term>
        ///     <description>Meaning</description>
        ///   </listheader>
        ///   <item>
        ///     <term>Less than zero</term>
        ///     <description>This instance precedes obj in the sort order.</description>
        ///   </item>
        ///   <item>
        ///     <term>Zero</term>
        ///     <description>This instance occurs in the same position in the sort 
        ///     order as obj.</description>
        ///   </item>
        ///   <item>
        ///     <term>Greater than zero</term>
        ///     <description>This instance follows obj in the sort order.</description>
        ///   </item>
        /// </list>
        /// </returns>
        /// 
        public int CompareTo( MinQuantityPrice other )
        {
            if ( this.Membership > other.Membership )
            {
                return -1;
            }
            else if ( this.Membership < other.Membership )
            {
                return 1;
            }
            else if ( this.PriceClass < other.PriceClass )
            {
                return -1;
            }
            else if ( this.PriceClass > other.PriceClass )
            {
                return 1;
            }
            else if ( this.MinimumQuantity > other.MinimumQuantity )
            {
                return -1;
            }
            else if ( this.MinimumQuantity < other.MinimumQuantity )
            {
                return 1;
            }

            return 0;
        }

        #endregion
    }
}