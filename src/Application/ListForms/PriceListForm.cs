/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       PriceListForm.cs
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
#endif

using VROLib;
using VROLib.CustomerDatabase;

/// <summary>
/// Represents the list form displaying Video Rental Outlet's Price List.
/// </summary>
/// 
internal sealed class PriceListForm 
    : ListForm<PriceList,MinQuantityPrice>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the PriceListForm class.
    /// </summary>
    /// 
    public PriceListForm ()
        : base( "Price List", "Price", 64, 27 )
    {
        Columns.Add( new TextField( "ID",            5, ""   ) );
        Columns.Add( new TextField( "Membership",  -12, "  " ) );
        Columns.Add( new TextField( "Price Class", -14, "  " ) );
        Columns.Add( new TextField( "Min.Qty",       7, ""   ) );
        Columns.Add( new TextField( "Price",        12, " "  ) );

        InitializeComponents ();

        // As the price list is kept sorted, we need to refresh its view after every 
        // new record added to collection
        //
        ReloadOnAddNewRecord = true;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Subscribes to VideoStore.PriceList Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.PriceList, FormatRow );
    }

    /// <summary>
    /// Gets the values to be displayed in columns for a specified MinQuantityPrice.
    /// </summary>
    /// 
    private string[] FormatRow( MinQuantityPrice price )
    {
        return new string[]
        {
            price.ID.ToString (),
            price.Membership.Verbose (),
            price.PriceClass.Verbose (),
            price.MinimumQuantity.ToString (),
            price.Price.ToString( "0.00" )
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the PriceDetailsForm class.
    /// </summary>
    ///
    public override DetailsForm<PriceList,MinQuantityPrice> CreateDetailsForm ()
    {
        return new PriceDetailsForm ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns true if a specified MinQuantityPrice can be removed.
    /// </summary>
    /// 
    public override bool CanRemoveRecord( MinQuantityPrice price )
    {
        // It is not allowed to delete a default price ('base price') which is
        // a price specification for NotMember, SwedishNew with minimum quantity 1.
        //
        if ( price.IsBasePrice )
        {
            MdiForm.ErrorMessage = "The price for single copy of swedish new "
                + "for non-member must not be deleted.";

            MessageBox.Show( "It is not allowed to delete the base price!",
                "Price Removal Error", MessageBoxButtons.OK, MessageBoxIcon.Hand
                 );

            MdiForm.ErrorMessage = null;

            return false;
        }

        string info = "Remove the following price specification?\n\n"
            + price.ToString ();

        DialogResult rc = MessageBox.Show( info, 
            Em.IsGUI ? "Video Rental Outlet: Remove Price Spec" : null,
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning );
        
        return rc == DialogResult.Yes;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}