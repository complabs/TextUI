/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       CustomerListForm.cs
 *  Created:    2011-03-29
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

using Mbk.Commons;

#if TEXTUI
    using TextUI;
#else
    using System.Windows.Forms;
    using VideoRentalOutlet_GUI.Properties;
#endif

using VROLib;
using VROLib.CustomerDatabase;

/// <summary>
/// Represents the list form displaying Video Rental Outlet's Customers.
/// </summary>
/// 
internal sealed class CustomerListForm 
    : ListForm<CustomerCollection,Customer>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the CustomerListForm class.
    /// </summary>
    /// 
    public CustomerListForm ()
        : base( "Customers", "Customer", 74, 27 )
    {
        Columns.Add( new TextField( "ID",           5, ""   ) );
        Columns.Add( new TextField( "Membership", -11, "  " ) );
        Columns.Add( new TextField( "Person ID",  -14, "  " ) );
        Columns.Add( Em.IsGUI 
                   ? new TextField( "Rented",       6, ""   )
                   : new TextField( "\u2302",       2, ""   ) );
        Columns.Add( new TextField( "Name",         0, "  " ) );

        /////////////////////////////////////////////////////////////////////////////////

        InitializeContextMenu ();
        InitializeToolStrip ();
        InitializeComponents ();
    }

    #endregion 

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Subscribes to VideoStore.Customers Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.Customers, RecordToStringArray );
    }

    /// <summary>
    /// Gets the values to be displayed in columns for a specified Customer.
    /// </summary>
    /// 
    private string[] RecordToStringArray( Customer customer )
    {
        return new string[]
        {
            customer.ID.ToString (), 
            customer.Membership == Membership.NotMember ? string.Empty 
                : customer.Membership.Verbose (),
            customer.PersonID,
            customer.RentedItemsCount == 0 ? string.Empty 
                : customer.RentedItemsCount.ToString (),
            customer.FullName
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the CustomerDetailsForm class.
    /// </summary>
    ///
    public override DetailsForm<CustomerCollection,Customer> CreateDetailsForm ()
    {
        return new CustomerDetailsForm ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    protected override void OpenSubItemsForm( Customer record, OpenMode mode )
    {
        MainForm.OpenRentedItemsList( record, mode );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns true if a specified Customer can be removed.
    /// </summary>
    /// 
    public override bool CanRemoveRecord( Customer customer )
    {
        // Ask for removal confirmation; build a question first...
        //
        string info = "Remove customer #" + customer.ID.ToString () 
                    + ": " + customer.FirstName + " " + customer.LastName + "?";

        bool warning  = customer.RentedItemsCount > 0;

        if ( warning )
        {
            info += "\n\nRemoving the customer will return rented items back to store!"
                  + "\n\nNumber of rented exemplars: " + customer.RentedItemsCount;
        }

        // Ask user to confirm removal, especially if the customer has rented items.
        //
        DialogResult rc = MessageBox.Show
        (
            info, 
            Em.IsGUI ? "Video Rental Outlet: Remove Customer" : null,
            MessageBoxButtons.YesNo, 
            warning ? MessageBoxIcon.Warning : MessageBoxIcon.Exclamation
            );

        return rc == DialogResult.Yes;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /// <summary>
    /// Appends the context menu with additional menu items.
    /// </summary>
    /// <remarks>
    /// Additonal items are: Customer Rentals and New Rental.
    /// </remarks>
    /// 
    private void InitializeContextMenu ()
    {
#if ! TEXTUI
        if ( ListView == null || ListView.ContextMenuStrip == null )
        {
            return;
        }

        ContextMenuStrip contextMenu = ListView.ContextMenuStrip;

        //-------------------------------------------------------------------------------
        ToolStripMenuItem miSubitems = new ToolStripMenuItem()
        {
            Text = "Customer &Rentals",
            Image = Resources.Rentals16
        };

        ToolStripMenuItem miAddSubitem = new ToolStripMenuItem()
        {
            Text = "Rent Ne&w Item...",
            Image = Resources.AddRental16
        };

        //-------------------------------------------------------------------------------
        miSubitems.Click += ( sender, e ) => OpenSubItemsForm( OpenMode.Edit );
        miAddSubitem.Click += ( sender,e ) => MainForm.AddNewRentedItem ();

        contextMenu.Opening += delegate
        {
            miSubitems.Enabled = CurrentRecord != null;
        };

        //-------------------------------------------------------------------------------
        int pos = contextMenu.Items.Count - 1;
        contextMenu.Items.Insert( pos, miSubitems );
        contextMenu.Items.Insert( pos+1, miAddSubitem );
        contextMenu.Items.Insert( pos+2, new ToolStripSeparator () );
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Appends the tool strip with additonal menu items.
    /// </summary>
    /// <remarks>
    /// Additonal items are: Rentals and New Rental.
    /// </remarks>
    /// 
    private void InitializeToolStrip ()
    {
#if ! TEXTUI
        if ( MdiForm == null || MdiForm.ToolStrip == null )
        {
            return;
        }

        ToolStrip toolStrip = MdiForm.ToolStrip;

        //-------------------------------------------------------------------------------
        ToolStripMenuItem miSubitems = new ToolStripMenuItem()
        {
            Text = "&Rentals",
            Image = Resources.Rentals16, 
            ToolTipText = "Edit Customer Rentals...",
            Enabled = false
        };

        ToolStripMenuItem miAddSubitem = new ToolStripMenuItem()
        {
            Text = "Ne&w Rental",
            Image = Resources.AddRental16,
            ToolTipText = "Rent a New Item to the customer...",
        };

        //-------------------------------------------------------------------------------
        miSubitems.Click += ( sender, e ) => OpenSubItemsForm( OpenMode.Edit );
        miAddSubitem.Click += ( sender, e ) => MainForm.AddNewRentedItem ();

        ListView.ItemSelectionChanged += ( sender, e ) => OnUiStateChanged ();

        //-------------------------------------------------------------------------------
        int pos = toolStrip.Items.Count - 1;
        toolStrip.Items.Insert( pos, new ToolStripSeparator () );
        toolStrip.Items.Insert( pos+1, miSubitems );
        toolStrip.Items.Insert( pos+2, miAddSubitem );

        //-------------------------------------------------------------------------------
        this.UiStateChanged += delegate
        {
            miSubitems.Enabled = CurrentRecord != null;
            miAddSubitem.Enabled = CurrentRecord != null;
        };
#endif
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}