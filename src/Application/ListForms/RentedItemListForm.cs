/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       RentedItemListForm.cs
 *  Created:    2011-04-10
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

#if TEXTUI
    using TextUI;
#else
    using System.Windows.Forms;
#endif

using VROLib;
using VROLib.CustomerDatabase;
using VROLib.ItemStore;

/// <summary>
/// Represents the list form displaying Rented Items to VRO Customer.
/// </summary>
/// 
internal sealed class RentedItemListForm 
    : ListForm<RentedItems,RentedItem>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the Customer to which rented items listed in this form belongs to.
    /// </summary>
    /// 
    public Customer Customer { get; set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the RentedItemListForm class.
    /// </summary>
    /// 
    public RentedItemListForm ()
        : base( "Rented Items", "Rented Item", 80, 20 )
    {
        Customer = null;

        Columns.Add( new TextField( "Due Date",       10, " "  ) );
        Columns.Add( new TextField( "Fee",             8, " "  ) );
        Columns.Add( new TextField( "Movie Exemplar",  0, "  " ) );

        InitializeComponents ();

        /////////////////////////////////////////////////////////////////////////////////

        // Subscribe to the Changed event for Customers collection
        //
        MainForm.VideoStore.Customers.Changed += Customers_Changed;

        MdiForm.FormClosed += delegate
        {
            // Unsubscribe the Changed event for Customers collection
            //
            MainForm.VideoStore.Customers.Changed -= Customers_Changed;
        };
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Subscribes to customer's RentedItems Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( Customer.RentedItems, FormatRow );
    }

    /// <summary>
    /// Gets the values to be displayed in columns for a specified RentedItem.
    /// </summary>
    /// 
    private string[] FormatRow( RentedItem item )
    {
        return new string[]
        {
            item.VerboseDueDate,
            item.RentalFee.ToString( "0.00" ),
            item.ExemplarInfo
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the PriceDetailsForm class.
    /// </summary>
    ///
    public override DetailsForm<RentedItems,RentedItem> CreateDetailsForm ()
    {
        return new RentedItemDetailsForm ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    protected override void OnBeforeOpenDetailsForm( 
        DetailsForm<RentedItems,RentedItem> form )
    {
        if ( form != null )
        {
            form.MasterRecord = this.Customer;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the caption for the form displaying form's read-only status,
    /// modality and master record to which this form belongs.
    /// </summary>
    /// 
    protected override string FormTitle 
    { 
        get
        {
            string customer = Customer.FullName + ", Customer #" + Customer.ID;

            return ReadOnly 
                ? "Items Rented by " + customer
                : "Manage Rented Items for " + customer;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns true if a specified RentedItem can be removed.
    /// </summary>
    /// 
    public override bool CanRemoveRecord( RentedItem item )
    {
        // Ask for removal confirmation; build a question first...
        //
        StringBuilder info = new StringBuilder ();

        info.Append( "Return from rental " ).Append( item.ExemplarInfo )
            .Append( "?" );

        DialogResult rc = MessageBox.Show
        ( 
            info.ToString (), 
            Em.IsGUI ? "Video Rental Outlet: Return Item" : null,
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning
            );

        return rc == DialogResult.Yes;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Event Handlers ]

    /// <summary>
    /// Monitors video store's customers collection. If customer is deleted
    /// from collection, form is closed.
    /// </summary>
    /// 
    private void Customers_Changed( GenericObject item, ChangeType how, string reason )
    {
        Debug.TraceLine( "{0} >>>> {1} {2}{3}", TraceID, how, item.GetType().Name,
            reason != null ? ", Reason: " + reason : "" );

        Customer record = item as Customer;

        if ( record != null && this.Customer == record )
        {
            if ( how == ChangeType.Removed )
            {
                QuitForm ();
            }
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}