/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       PriceDetailsForm.cs
 *  Created:    2011-03-31
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.ComponentModel;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Windows.Forms;
#endif

using VROLib;
using VROLib.CustomerDatabase;
using VROLib.ItemStore;

/// <summary>
/// Represents the form displaying Price Specification details.
/// </summary>
/// 
internal sealed class PriceDetailsForm 
    : DetailsForm<PriceList,MinQuantityPrice>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Components ]

    private MyComboBox   membership;
    private MyComboBox   priceClass;
    private MyTextBox    textMinQuantity;
    private MyTextBox    textRentalFee;

    // Parsed and validated field values

    private int        minimumQuantity;
    private decimal    rentalFee;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the PriceDetailsForm class.
    /// </summary>
    /// 
    public PriceDetailsForm ()
        : base( "Price", 60, 18 )
    {
        InitializeComponents ();
    }

    /// <summary>
    /// Subscribes to VideoStore.PriceList Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.PriceList );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Database Record Synchronization ]

    /// <summary>
    /// Loads contents of the UI components from the <see cref="Record"/>.
    /// </summary>
    /// 
    protected override void OnLoadData ()
    {
        if ( IsAddNew )
        {
            this.membership.SelectItem( Membership.NotMember );
            this.priceClass.SelectItem( PriceClass.OlderMovie );

            minimumQuantity = 1;
            rentalFee       = 10;
        }
        else
        {
            this.membership.SelectItem( Record.Membership );
            this.priceClass.SelectItem( Record.PriceClass );

            minimumQuantity = Record.MinimumQuantity;
            rentalFee       = Record.Price;
        }


        this.textMinQuantity.InitText = minimumQuantity.ToString();
        this.textRentalFee.InitText = rentalFee.ToString( "0.00" );

        base.OnLoadData ();
    }

    /// <summary>
    /// Returns a value indicating whether contents of the UI fields has been changed
    /// since they were loaded in <see cref="OnLoadData"/>.
    /// </summary>
    ///
    public override bool IsDirty ()
    {
        return this.membership     .ContentsChanged
            || this.priceClass     .ContentsChanged
            || this.textMinQuantity.ContentsChanged
            || this.textRentalFee  .ContentsChanged;
    }

    /// <summary>
    /// Saves contents of the UI fields into the <see cref="Record"/>.
    /// </summary>
    /// 
    protected override void OnSaveData ()
    {
        Membership? membership = this.membership.Current.Tag as Membership?;
        if ( ! membership.HasValue )
        {
            throw new Exception( "Membership must not be null." );
        }

        PriceClass? priceClass = this.priceClass.Current.Tag as PriceClass?;
        if ( ! priceClass.HasValue )
        {
            throw new Exception( "Price Class must not be null." );
        }

        MinQuantityPrice currentRecord = Record;

        Record = MainForm.VideoStore.PriceList.UpdatePrice( 
            membership.Value, priceClass.Value,
            minimumQuantity, rentalFee );

        base.OnSaveData ();

        if ( currentRecord != Record ) // Saved to different record
        {
            // Reload saved values. This cleans ContentsChanged for fields.
            //
            OnLoadData ();
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Gets the caption for the form.
    /// </summary>
    /// 
    protected override string FormTitle 
    { 
        get
        {
            return IsAddNew ? "Add New Price Specification"
                 : ReadOnly ? "Price Details" : "Edit Price Specification";
        }
    }

    /// <summary>
    /// Initializes UI components of the MdiForm.
    /// </summary>
    /// 
    protected override void InitializeComponents ()
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Table layout grid definition, with rows and columns in Em units.

        float[] col = { 4, 23 };
        float[] row = { 2, 5, 8, 11 };

        float maxLen = 24; // Maximum text length

        /////////////////////////////////////////////////////////////////////////////////
        // Static text

        int r = 0, c = 0;

        NewStaticText( col[c], row[r++], "Membership:      " );
        NewStaticText( col[c], row[r++], "Price Class:     " );
        NewStaticText( col[c], row[r++], "Minimum Quantity:" );
        NewStaticText( col[c], row[r++], "Rental Fee:      " );

        /////////////////////////////////////////////////////////////////////////////////
        // TextBox and ComboBox Fields

        r = 0; c = 1;

        this.membership = NewEnumField( col[c], row[r++], maxLen, typeof( Membership ) );
        this.priceClass = NewEnumField( col[c], row[r++], maxLen, typeof( PriceClass ) );

        this.textMinQuantity = NewTextField( col[c], row[r++], maxLen );
        this.textRentalFee   = NewTextField( col[c], row[r++], maxLen );

        /////////////////////////////////////////////////////////////////////////////////
        // Field validation event handlers

        this.textMinQuantity.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.textMinQuantity.ContentsChanged )
            {
                return;
            }

            string fieldName = "Minimum Quantity";

            // Verify (and parse) field as not null integer number
            //
            ValidateNotNull( fieldName, this.textMinQuantity.Text, e );

            int fieldValue = 0;
            ValidateInteger( fieldName, this.textMinQuantity.Text, e, ref fieldValue );

            // Minimum Quantity must be an integer >= 1
            //
            if ( ! e.Cancel && fieldValue > 0 )
            {
                minimumQuantity = fieldValue;
            }
            else if ( ! e.Cancel )
            {
                MdiForm.ErrorMessage = fieldName + " must be greater or equal one.";
                MdiForm.Beep ();
                e.Cancel = true;
            }
        };

        this.textRentalFee.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.textRentalFee.ContentsChanged )
            {
                return;
            }

            // Verify (and parse) field as not null decimal number
            //
            string fieldName = "Rental Fee";
            ValidateNotNull( fieldName, this.textRentalFee.Text, e );

            decimal fieldValue = 0;
            ValidateDecimal( fieldName, this.textRentalFee.Text, e, ref fieldValue );

            // Rental fee must be a positive decimal.
            //
            if ( ! e.Cancel && fieldValue > 0 )
            {
                rentalFee = fieldValue;
            }
            else if ( ! e.Cancel )
            {
                MdiForm.ErrorMessage = fieldName + " must be greater than zero.";
                MdiForm.Beep ();
                e.Cancel = true;
            }
        };

        /////////////////////////////////////////////////////////////////////////////////

        this.membership.TabStop = false;
        this.membership.ReadOnly = true;

        this.priceClass.TabStop = false;
        this.priceClass.ReadOnly = true;

        this.textMinQuantity.TabStop = false;
        this.textMinQuantity.ReadOnly = true;

        base.InitializeComponents ();
    }

    /// <summary>
    /// Sets the ReadOnly property of the form.
    /// </summary>
    /// <remarks>
    /// Configures whether membership, priceClass and minQuantity fields are
    /// enabled.
    /// </remarks>
    /// 
    protected override void SetReadOnly( bool readOnly )
    {
        base.SetReadOnly( readOnly );

        /////////////////////////////////////////////////////////////////////////////////

        this.membership.TabStop = IsAddNew;
        this.membership.ReadOnly = readOnly || ! IsAddNew;

        this.priceClass.TabStop = IsAddNew;
        this.priceClass.ReadOnly = readOnly || ! IsAddNew;

        this.textMinQuantity.TabStop = IsAddNew;
        this.textMinQuantity.ReadOnly = readOnly || ! IsAddNew;

        FirstField = IsAddNew ? (Control)this.membership : this.textRentalFee;

        if ( MdiForm.ActiveChild == null || ! MdiForm.ActiveChild.TabStop )
        {
            FirstField.Focus ();
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}