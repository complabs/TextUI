/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       CustomerDetailsForm.cs
 *  Created:    2011-03-29
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

/// <summary>
/// Represents the form displaying Customer details.
/// </summary>
/// 
internal sealed class CustomerDetailsForm 
    : DetailsForm<CustomerCollection,Customer>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Components ]

    private MyTextBox    firstName        ;
    private MyTextBox    lastName         ;
    private MyTextBox    personID         ;
    private MyTextBox    address          ;
    private MyTextBox    postCode         ;
    private MyTextBox    city             ;
    private MyTextBox    country          ;
    private MyTextBox    phone            ;
    private MyTextBox    cellPhone        ;
    private MyTextBox    email            ;
    private MyComboBox   membership       ;
    private MyComboBox   creditCard       ;
    private MyTextBox    creditCardNumber ;
    private MyTextBox    creditCardValid  ;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the CustomerDetailsForm class.
    /// </summary>
    /// 
    public CustomerDetailsForm ()
        : base( "Customer", 83, 23 )
    {
        InitializeComponents ();

        /////////////////////////////////////////////////////////////////////////////////

        Button_Link.Text = "&Rentals";

        LinkClick += delegate
        {
            if ( Record != null )
            {
                MainForm.OpenRentedItemsList( Record, OpenMode.Edit );
            }
        };

        Button_Info.Text = Em.IsGUI ? "&New Rental" : "Rent &New";

        InfoClick += delegate
        {
            if ( Record != null )
            {
                MainForm.AddNewRentedItem ();
            }
        };
    }

    /// <summary>
    /// Subscribes to VideoStore.Customers Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.Customers );
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
        Customer customer = new Customer( Record );

        this.firstName.InitText = customer.FirstName ;
        this.lastName .InitText = customer.LastName  ;
        this.personID .InitText = customer.PersonID  ;
        this.address  .InitText = customer.Address   ;
        this.postCode .InitText = customer.PostCode  ;
        this.city     .InitText = customer.City      ;
        this.country  .InitText = customer.Country   ;
        this.phone    .InitText = customer.Phone     ;
        this.cellPhone.InitText = customer.CellPhone ;
        this.email    .InitText = customer.EMail     ;

        this.membership.SelectItem( customer.Membership );

        this.creditCard.SelectItem( customer.CreditCard.Type );

        if ( customer.CreditCard.Type != CreditCardType.None )
        {
            this.creditCardNumber.InitText = customer.CreditCard.Number;
            this.creditCardNumber.TabStop = true;
            this.creditCardNumber.Enabled = true;
            this.creditCardValid.InitText = customer.CreditCard.VerboseValidThru;
            this.creditCardValid.TabStop = true;
            this.creditCardValid.Enabled = true;
        }
        else
        {
            this.creditCardNumber.InitText = null;
            this.creditCardNumber.TabStop = false;
            this.creditCardNumber.Enabled = false;
            this.creditCardValid.InitText = null;
            this.creditCardValid.TabStop = false;
            this.creditCardValid.Enabled = false;
        }

        base.OnLoadData ();
    }

    /// <summary>
    /// Returns a value indicating whether contents of the UI fields has been changed
    /// since they were loaded in <see cref="OnLoadData"/>.
    /// </summary>
    ///
    public override bool IsDirty ()
    {
        return this.firstName .ContentsChanged
            || this.lastName  .ContentsChanged
            || this.personID  .ContentsChanged
            || this.address   .ContentsChanged
            || this.postCode  .ContentsChanged
            || this.city      .ContentsChanged
            || this.country   .ContentsChanged
            || this.phone     .ContentsChanged
            || this.cellPhone .ContentsChanged
            || this.email     .ContentsChanged
            || this.membership.ContentsChanged
            || this.creditCard.ContentsChanged
            || this.creditCardNumber.ContentsChanged
            || this.creditCardValid.ContentsChanged;
    }

    /// <summary>
    /// Saves contents of the UI fields into the <see cref="Record"/>.
    /// </summary>
    /// 
    protected override void OnSaveData ()
    {
        Customer customer = new Customer( Record )
        {
            PersonID  = this.personID  .TrimmedText,
            FirstName = this.firstName .TrimmedText,
            LastName  = this.lastName  .TrimmedText,
            Address   = this.address   .TrimmedText,
            PostCode  = this.postCode  .TrimmedText,
            City      = this.city      .TrimmedText,
            Country   = this.country   .TrimmedText,
            Phone     = this.phone     .TrimmedText,
            CellPhone = this.cellPhone .TrimmedText,
            EMail     = this.email     .TrimmedText,
        };

        Membership? newMembership = this.membership.Current.Tag as Membership?;

        if ( newMembership.HasValue )
        {
            customer.Membership = newMembership.Value;
        }

        CreditCardType? creditCardType = this.creditCard.Current.Tag as CreditCardType?;

        if ( creditCardType.HasValue && creditCardType.Value != CreditCardType.None )
        {
            customer.CreditCard = new CreditCard( creditCardType.Value,
                this.creditCardNumber.TrimmedText, 
                this.creditCardValid.TrimmedText );
        }
        else
        {
            customer.CreditCard = new CreditCard ();
        };

        // Note that insert/update of the record also invokes OnLoadData() through
        // an on-changed database event that is catched by ListForm and 
        // propagated to its DetailsForm, which is actually this form.
        // This is important as it cleans ContentsChanged property for all fields.
        // See ListForm.Collection_Changed() event handler.

        if ( IsAddNew )
        {
            try
            {
                Record = customer;
                customer.AddTo( MainForm.VideoStore );
            }
            catch // in case of error, rethrow exception and clear reference to new rec
            {
                Record = null;
                throw;
            }
        }
        else
        {
            customer.Update ();
        }

        base.OnSaveData ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    protected override void InitializeComponents ()
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Table layout grid definition, with rows and columns in Em units.

        float[] col = { 3, 17, 44, 57 };
        float[] row = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22 };

        float colWidth = 24; // default column width

        /////////////////////////////////////////////////////////////////////////////////
        // Static text

        int r = 0, c = 0;

        NewStaticText( col[c], row[r++], "First Name:" );
        NewStaticText( col[c], row[r++], "Last Name:"  );
        NewStaticText( col[c], row[r++], "Person ID:"  );

        NewStaticText( col[c], row[r++], "Membership:" );

        NewStaticText( col[c], row[r++], "Address:"    );
        NewStaticText( col[c], row[r++], "Post Code:"  );
        NewStaticText( col[c], row[r++], "City:"       );
        NewStaticText( col[c], row[r++], "Country:"    );

        // Mandatory fields marker:
        NewStaticText( col[c]-2, row[0], "*" );
        NewStaticText( col[c]-2, row[1], "*" );
        NewStaticText( col[c]-2, row[2], "*" );

        r = 0; c = 2;

        NewStaticText( col[c], row[r++], "Phone:"      );
        NewStaticText( col[c], row[r++], "Cell Phone:" );
        NewStaticText( col[c], row[r++], "E-Mail:"     );

        /////////////////////////////////////////////////////////////////////////////////
        // TextBox fields

        r = 0; c = 1;

        this.firstName = NewTextField( col[c], row[r++], colWidth );
        this.lastName  = NewTextField( col[c], row[r++], colWidth );
        this.personID  = NewTextField( col[c], row[r++], colWidth );

        this.membership = NewEnumField( col[c], row[r++], 
            Em.IsGUI ? colWidth : 0, typeof( Membership ) );

        this.address   = NewTextField( col[c], row[r++], colWidth );
        this.postCode  = NewTextField( col[c], row[r++], colWidth );
        this.city      = NewTextField( col[c], row[r++], colWidth );
        this.country   = NewTextField( col[c], row[r++], colWidth );

        r = 0; c = 3;

        this.phone     = NewTextField( col[c], row[r++], colWidth );
        this.cellPhone = NewTextField( col[c], row[r++], colWidth );
        this.email     = NewTextField( col[c], row[r++], colWidth );

        /////////////////////////////////////////////////////////////////////////////////
        // Credit Card Group Box

        MyGroupBox ccBox = NewGroupBox( "Credit Card", 
            col[2] - ( Em.IsGUI ? 1f : 2f ), 
            row[r] + ( Em.IsGUI ? 0.6f : 1f ), 38, ( Em.IsGUI ? 8.6f : 9f )
            );

        NewLabel( ccBox, 2, 2, "Type:"       );
        NewLabel( ccBox, 2, 4, "Number:"     );
        NewLabel( ccBox, 2, 6, "Valid Thru:" );

        int offset = Em.IsGUI ? 14 : 15;

        this.creditCard = NewEnumField( ccBox, offset, 2, 
            Em.IsGUI ? 22 : 0, typeof( CreditCardType ) );

        this.creditCardNumber = NewTextField( ccBox, offset, 4, Em.IsGUI ? 22 : 19 );
        this.creditCardValid  = NewTextField( ccBox, offset, 6, Em.IsGUI ? 10 : 8 );

        /////////////////////////////////////////////////////////////////////////////////
        // Field validation event handlers

        this.firstName.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.firstName.ContentsChanged )
            {
                return;
            }

            ValidateNotNull( "Customer's first name", this.firstName.Text, e );
        };

        this.lastName.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.lastName.ContentsChanged )
            {
                return;
            }

            ValidateNotNull( "Customer's last name", this.lastName.Text, e );
        };

        this.personID.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.personID.ContentsChanged )
            {
                return;
            }

            ValidateNotNull( "Customer's Person-ID", this.personID.Text, e );

            if ( ! ReadOnly && ! e.Cancel )
            {
                string notValidInfo = Customer.ValidatePNR( this.personID.TrimmedText );

                if ( notValidInfo != null )
                {
                    MdiForm.ErrorMessage = notValidInfo;
                    MdiForm.Beep ();
                    e.Cancel = true;
                }
            }
        };

        this.email.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.email.ContentsChanged )
            {
                return;
            }

            ValidateEMailAddress( "E-Mail", this.email.Text, e );
        };

        this.creditCard.SelectedIndexChanged += ( sender, e ) =>
        {
            CreditCardType? ccType = this.creditCard.Current.Tag as CreditCardType?;

            if ( ccType.HasValue && ccType.Value != CreditCardType.None )
            {
                this.creditCardNumber.TabStop = true;
                this.creditCardNumber.Enabled = true;
                this.creditCardValid.TabStop = true;
                this.creditCardValid.Enabled = true;
            }
            else
            {
                // this.creditCardNumber.Text = null;
                this.creditCardNumber.TabStop = false;
                this.creditCardNumber.Enabled = false;
                // this.creditCardValid.Text = null;
                this.creditCardValid.TabStop = false;
                this.creditCardValid.Enabled = false;
            }
        };

        this.creditCardNumber.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.creditCardNumber.ContentsChanged )
            {
                return;
            }

            CreditCardType? ccType = this.creditCard.Current.Tag as CreditCardType?;

            if ( ccType.HasValue && ccType.Value != CreditCardType.None )
            {
                string ccNumber = this.creditCardNumber.TrimmedText;

                // ValidateNotNull( "Credit Card Number", ccNumber, e );

                // Credit card number valiation will be performed when record
                // is updated.
            }
        };

        this.creditCardValid.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.creditCardValid.ContentsChanged )
            {
                return;
            }

            CreditCardType? ccType = this.creditCard.Current.Tag as CreditCardType?;

            if ( ccType.HasValue && ccType.Value != CreditCardType.None )
            {
                // ValidateNotNull( "Valid Thru", this.creditCardValid.Text, e );

                ValidateDate( "Valid Thru", this.creditCardValid.Text, 
                    "yyyy-M", " in 'yyyy-mm' format.", e );
            }
        };

        /////////////////////////////////////////////////////////////////////////////////

        base.InitializeComponents ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}