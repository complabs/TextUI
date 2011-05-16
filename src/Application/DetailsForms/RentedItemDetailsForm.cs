/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       RentedItemDetailsForm.cs
 *  Created:    2011-04-10
 *  Modified:   2011-04-34
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.ComponentModel;

using Mbk.Commons;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
#endif

using VROLib;
using VROLib.CustomerDatabase;
using VROLib.ItemStore;

/// <summary>
/// Represents the form displaying Customer's Rented Item details.
/// </summary>
/// 
internal sealed class RentedItemDetailsForm 
    : DetailsForm<RentedItems,RentedItem>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the Customer to which rented items listed in this form belongs to.
    /// </summary>
    /// 
    public Customer Customer { get; private set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Components ]

    private MyLabel customerInfo;

    private MovieExemplar Exemplar;
    private MyLabel exemplarInfo;
    private MyButton exemplarSelect;
    private bool exemplarChanged;

    private MyTextBox dueDate;
    private MyTextBox rentalFee;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the RentedItemDetailsForm class.
    /// </summary>
    /// 
    public RentedItemDetailsForm ()
        : base( "Rented Item", 78, 16 )
    {
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

        /////////////////////////////////////////////////////////////////////////////////

        Button_Link.Text = "&Customer";

        LinkClick += delegate
        {
            // Open customer details form for the current customer 
            //
            if ( this.Customer != null )
            {
                MainForm.Open<CustomerDetailsForm>( 
                    ReadOnly ? OpenMode.Browse : OpenMode.Edit,
                    form => form.Record = this.Customer );
            }
        };

    }

    /// <summary>
    /// Subscribes to customer's RentedItems Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        this.Customer = MasterRecord as Customer;

        if ( Customer != null )
        {
            ConnectTo( Customer.RentedItems );
        }
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
        RentedItem rentedItem = Record; //new RentedItem( Record );

        this.customerInfo.Text = Customer != null ? Customer.ToString()
            : "(Select a customer to which this exemplar belongs first)";

        if ( this.Customer == null && MasterRecord != null )
        {
            Customer masterCustomer = MasterRecord as Customer;
            if ( masterCustomer != null )
            {
                this.Customer = masterCustomer;
                this.customerInfo.Text = masterCustomer.ToString ();
            }
        }

        if ( rentedItem == null )
        {
            this.Exemplar = null;
            this.exemplarInfo.Text = "Select a movie exemplar to be rented...";

            this.rentalFee.InitText = "0.00";
            this.dueDate.InitText   = DateTime.Now.AddDays( 3 ).ToString( "yyyy-MM-dd" );
        }
        else
        {
            this.Exemplar = rentedItem.Exemplar;
            this.exemplarInfo.Text = rentedItem.Exemplar.ToString ();

            this.rentalFee.InitText = rentedItem.RentalFee.ToString( "0.00" );
            this.dueDate.InitText   = rentedItem.VerboseDueDate;
        }

        this.exemplarChanged = false;

        base.OnLoadData ();
    }

    /// <summary>
    /// Returns a value indicating whether contents of the UI fields has been changed
    /// since they were loaded in <see cref="OnLoadData"/>.
    /// </summary>
    ///
    public override bool IsDirty ()
    {
        if ( this.Customer == null )
        {
            return false;
        }

        return this.exemplarChanged
            || this.dueDate.ContentsChanged
            || this.rentalFee.ContentsChanged;
    }

    /// <summary>
    /// Saves contents of the UI fields into the <see cref="Record"/>.
    /// </summary>
    /// 
    protected override void OnSaveData ()
    {
        if ( IsAddNew && this.Customer == null )
        {
            throw new Exception( 
                "You must select a customer to which this rental will be added." );
        }

        if ( this.Exemplar == null )
        {
            throw new Exception( 
                "You must select a movie exemplar which will be rented." );
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Parse Due Date

        DateTime newDueDate = DateTime.MinValue;

        try
        {
            string value = this.dueDate.TrimmedText;

            if ( ! string.IsNullOrEmpty( value ) )
            {
                newDueDate = DateTime.ParseExact( value, "yyyy-M-d",
                    System.Globalization.CultureInfo.InvariantCulture );
            }
        }
        catch( Exception )
        {
            throw new ArgumentException( "Due Date must be a valid date." );
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Parse Rental Fee

        decimal newFee = 0;

        try
        {
            string value = this.rentalFee.TrimmedText;

            if ( ! string.IsNullOrEmpty( value ) )
            {
                newFee = decimal.Parse( value );
            }
        }
        catch( Exception )
        {
            throw new ArgumentException( "Rental Fee must be a decimal number." );
        }

        // Note that insert/update of the record also invokes OnLoadData() through
        // an on-changed database event that is catched by ListForm and 
        // propagated to its DetailsForm, which is actually this form.
        // This is important as it cleans ContentsChanged property for all fields.
        // See ListForm.Collection_Changed() event handler.

        if ( IsAddNew )
        {
            try
            {
                Record = new RentedItem( Exemplar, newDueDate, newFee );
                Record.AddTo( Customer );
            }
            catch // in case of error, rethrow exception and clear reference to new rec
            {
                Record = null;
                throw;
            }
        }
        else
        {
            Record.SetNewConditions( newDueDate, newFee );
        }

        base.OnSaveData ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Gets a dynamic caption for the form.
    /// </summary>
    /// 
    protected override string FormTitle 
    { 
        get
        {
            return IsAddNew ? "Add New Rental to " + this.customerInfo.Text
                 : ReadOnly ? "Rental Details" : "Edit Rental Details";
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

        float[] col = { 3, 18, 40, 50, 79 };
        float[] row = { 2, 4, 7, 9, 11, 12, 14, 16, 18, 21, 22 };

        float maxLen = 15; // Maximum text length

        /////////////////////////////////////////////////////////////////////////////////
        // Static text

        int r = 2, c = 0;

        NewStaticText( col[c], row[r++], "Due Date:"   );
        NewStaticText( col[c], row[r++], "Rental Fee:" );

        /////////////////////////////////////////////////////////////////////////////////
        // Customer info label

        this.customerInfo = NewLabel( col[0], row[0] - ( Em.IsGUI ? 0.2f : 0f ),
            (float)MdiForm.Width / Em.Width - col[0] * 2
            );

        this.exemplarInfo = NewLabel( col[0], row[1] + ( Em.IsGUI ? 0.2f : 0f ),
            (float)MdiForm.Width / Em.Width - col[0] * 2
            );

        this.exemplarSelect = new MyButton ()
        {
            Left = (int)( col[2] * Em.Width ), 
            Top  = (int)( ( row[2] - ( Em.IsGUI ? 0.2f : 0f ) ) * Em.Height ),
            Text  = "&Select Movie Exemplar", AutoSize = true,
            TabStop = true, TabIndex = NextTabIndex,
            Parent = MdiForm, 
        };

        /////////////////////////////////////////////////////////////////////////////////
        // TextBox fields

        r = 2; c = 1;

        this.dueDate   = NewTextField( col[c], row[r++], maxLen );
        this.rentalFee = NewTextField( col[c], row[r++], maxLen );

        /////////////////////////////////////////////////////////////////////////////////
        // Field validation event handlers

        this.dueDate.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.dueDate.ContentsChanged )
            {
                return;
            }

            ValidateNotNull( "Due date", this.dueDate.Text, e );

            ValidateDate( "Due date", this.dueDate.Text, "yyyy-M-d", 
                " in ISO format (yyyy-mm-dd).", e );
        };

        this.rentalFee.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.rentalFee.ContentsChanged )
            {
                return;
            }

            string fieldName = "Rental Fee";

            ValidateNotNull( fieldName, this.dueDate.Text, e );

            decimal fieldValue = 0; // default value to satisfy validation if null

            ValidateDecimal( fieldName, this.rentalFee.Text, e, ref fieldValue );

            // Rental fee must be either null or an decimal >= 0
            //
            if ( ! e.Cancel && fieldValue <= 0 )
            {
                MdiForm.ErrorMessage = fieldName + " must not be a positive number.";
                MdiForm.Beep ();
                e.Cancel = true;
            }
        };

        /////////////////////////////////////////////////////////////////////////////////
        // Movie Exemplar Selection as modal MovieExemplarListForm (i.e. dialog)

        this.exemplarSelect.Click += ( sender, e ) =>
        {
            this.ExecuteWithoutFormValidation( () =>
            {
                MovieExemplarListForm form = new MovieExemplarListForm ();

                form.CommonFilter = ex => ! ex.IsRented;

                form.OpenModal( this.Exemplar, selectedExemplar =>
                {
                    this.Exemplar = selectedExemplar;
                    this.exemplarInfo.Text = selectedExemplar.ToString ();
                    this.exemplarChanged = true;

                    if ( this.Customer != null )
                    {
                        decimal fee = MainForm.VideoStore.GetPrice(
                            this.Customer.Membership, this.Exemplar.PriceClass,
                            this.Customer.RentedItemsCount + 1 );

                        this.rentalFee.Text = fee.ToString( "0.00" );
                    }

                    form.QuitForm ();
                    this.dueDate.Select ();
                } );
            } );
        };

        /////////////////////////////////////////////////////////////////////////////////
        // Customer info GUI/TUI specific setup
        //
#if TEXTUI
        MdiForm.GotFocus += ( sender, e ) =>
        {
            this.customerInfo.ForeColorInact = Color.White;
            this.customerInfo.ForeColor = Color.White;

            this.exemplarInfo.ForeColorInact = Color.Cyan;
            this.exemplarInfo.ForeColor = Color.Cyan;
        };
#else
        this.customerInfo.Font = MainForm.HeadingFont;
        this.customerInfo.Height = Em.Height * 15/10;
        this.customerInfo.ForeColor = Color.DarkBlue;
        this.customerInfo.AutoSize = false;
        this.customerInfo.AutoEllipsis = true;

        this.exemplarInfo.Height = Em.Height + ( Em.IsGUI ? 2 : 0 );
        this.exemplarInfo.ForeColor = Color.DarkRed;
        this.exemplarInfo.AutoSize = false;
        this.exemplarInfo.AutoEllipsis = true;

        this.MdiForm.FontChanged += ( sender, e ) =>
        {
            this.customerInfo.Font = MainForm.HeadingFont;
            this.customerInfo.Height = Em.Height * 15/10;
            this.exemplarInfo.Height = Em.Height + ( Em.IsGUI ? 2 : 0 );
        };
#endif
        /////////////////////////////////////////////////////////////////////////////////

        base.InitializeComponents ();
    }

    /// <summary>
    /// Sets the ReadOnly property of the form.
    /// </summary>
    /// <remarks>
    /// Configures buttons' text and whether exemplarSelect field is enabled.
    /// </remarks>
    /// 
    protected override void SetReadOnly( bool readOnly )
    {
        base.SetReadOnly( readOnly );

        /////////////////////////////////////////////////////////////////////////////////

        Button_OK.Text = ReadOnly ? "Cl&ose" : IsAddNew ? "&Rent Item" : "&Update";
        Button_Delete.Text = "&Return Item";

        LayoutButtonPanel ();

        /////////////////////////////////////////////////////////////////////////////////

        this.exemplarSelect.TabStop = ! ReadOnly && IsAddNew;
        this.exemplarSelect.Enabled = ! ReadOnly && IsAddNew;
        this.exemplarSelect.Visible = this.exemplarSelect.Enabled;

        FirstField = IsAddNew ? this.exemplarSelect : (Control)this.dueDate;

        if ( MdiForm.ActiveChild != null && ! MdiForm.ActiveChild.TabStop )
        {
            FirstField.Focus ();
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Event Handlers ]

    /// <summary>
    /// Monitors video store's customers collection. If the customer is deleted
    /// from collection, the form is closed.
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