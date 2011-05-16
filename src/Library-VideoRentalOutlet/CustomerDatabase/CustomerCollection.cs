/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.CustomerDatabase
 *  File:       CustomerCollection.cs
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
    /// <summary>
    /// Encapsulates Video Rental Outlet's customer database.
    /// </summary>
    /// 
    [Serializable]
    public class CustomerCollection 
        : GenericObjectCollection<Customer>
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the CustomerDB class with an empty database.
        /// </summary>
        ///
        public CustomerCollection( VideoRentalOutlet database )
            : base( database, "Customers", "Customer" )
        {
            ForwardChangedEventsToDatabase = true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Internal Methods ]

        /// <summary>
        /// Find customer by PID
        /// </summary>
        /// 
        internal Customer FindByPersID( string persID )
        {
            return this.Items.Find( customer => customer.PersonID == persID );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ ToString() and FullInfo() Virtual Methods ]

        /// <summary>
        /// Gets a brief contents of customer database.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            if ( this.Items.Count == 0 )
            {
                sb.Append( "Customer database is empty." );
            }
            else
            {
                sb.AppendLine( "Customers:" );
                sb.Append( base.ToString () );
            }

            return sb.ToString ();
        }

        /// <summary>
        /// Gets a thorough contents of customer database.
        /// </summary>
        /// 
        public override string FullInfo ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.AppendTitle( "Customer Database" );

            sb.Append( base.FullInfo () );

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}