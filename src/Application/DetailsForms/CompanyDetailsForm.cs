/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       CompanyDetailsForm.cs
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

/// <summary>
/// Represents the details form displaying Video Rental Outlet's Company Info.
/// </summary>
/// 
internal sealed class CompanyDetailsForm 
    : DetailsForm<VideoRentalOutlet,VideoRentalOutlet>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Components ]

    private MyTextBox companyName;
    private MyTextBox vatNo;
    private MyTextBox address;
    private MyTextBox postCode;
    private MyTextBox city;
    private MyTextBox country;
    private MyTextBox phone;
    private MyTextBox homePage;
    private MyTextBox email;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the CompanyDetailsForm class.
    /// </summary>
    /// 
    public CompanyDetailsForm ()
        : base( "Video Rental Outlet", 64, 25 )
    {
        Record = MainForm.VideoStore;

        InitializeComponents ();

        /////////////////////////////////////////////////////////////////////////////////

        Button_Link.Text = "&Price List";

        LinkClick += delegate
        {
            MainForm.Open<PriceListForm>( ReadOnly ? OpenMode.Browse : OpenMode.Edit );
        };
    }

    /// <summary>
    /// Subscribes to MainForm.VideoStore object Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore );
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
        // Note that we cannot 'add new' video store, so the Record is always != null

        this.companyName.InitText = Record.Name     ;
        this.vatNo      .InitText = Record.VatNo    ;
        this.address    .InitText = Record.Address  ;
        this.postCode   .InitText = Record.PostCode ;
        this.city       .InitText = Record.City     ;
        this.country    .InitText = Record.Country  ;
        this.phone      .InitText = Record.Phone    ;
        this.homePage   .InitText = Record.HomePage ;
        this.email      .InitText = Record.EMail    ;

        base.OnLoadData ();
    }

    /// <summary>
    /// Returns a value indicating whether contents of the UI fields has been changed
    /// since they were loaded in <see cref="OnLoadData"/>.
    /// </summary>
    ///
    public override bool IsDirty ()
    {
        return this.companyName.ContentsChanged
            || this.vatNo      .ContentsChanged
            || this.address    .ContentsChanged
            || this.postCode   .ContentsChanged
            || this.city       .ContentsChanged
            || this.country    .ContentsChanged
            || this.phone      .ContentsChanged
            || this.homePage   .ContentsChanged
            || this.email      .ContentsChanged;
    }

    /// <summary>
    /// Saves contents of the UI fields into the <see cref="Record"/>.
    /// </summary>
    /// 
    protected override void OnSaveData ()
    {
        Record.SetName( this.companyName.TrimmedText );

        Record.SetVatNo( this.vatNo.TrimmedText );

        Record.Address   = this.address  .TrimmedText ;
        Record.PostCode  = this.postCode .TrimmedText ;
        Record.City      = this.city     .TrimmedText ;
        Record.Country   = this.country  .TrimmedText ;
        Record.Phone     = this.phone    .TrimmedText ;
        Record.HomePage  = this.homePage .TrimmedText ;
        Record.EMail     = this.email    .TrimmedText ;

        base.OnSaveData ();

        // Reload saved values and clean ContentsChanged for all fields
        //
        OnLoadData ();

        // Update application title
        //
        MainForm.UpdateTitle ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    protected override string FormTitle 
    { 
        get
        {
            return ReadOnly ? "Video Rental Outlet Details" 
                            : "Edit Company Details";
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

        float[] col = { 4, 22 };
        float[] row = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 21, 23 };

        // Maximum text length
        float maxLen = (float)MdiForm.Width / Em.Width - col[1] - 2f;

        /////////////////////////////////////////////////////////////////////////////////
        // Static text

        int r = 0, c = 0;

        NewStaticText( col[c], row[r++], "Company Name:"    );
        NewStaticText( col[c], row[r++], "VAT Number:"      );
        NewStaticText( col[c], row[r++], "Address:"         );
        NewStaticText( col[c], row[r++], "Post Code:"       );
        NewStaticText( col[c], row[r++], "City:"            );
        NewStaticText( col[c], row[r++], "Country:"         );
        NewStaticText( col[c], row[r++], "Phone:"           );
        NewStaticText( col[c], row[r++], "Home Page:"       );
        NewStaticText( col[c], row[r++], "E-Mail Address: " );

        // Mandatory fields marker:
        NewStaticText( col[c]-2, row[0], "*" );
        NewStaticText( col[c]-2, row[1], "*" );
        NewStaticText( col[c]-2, row[2], "*" );

        /////////////////////////////////////////////////////////////////////////////////
        // TextBox fields

        r = 0; c = 1;
        this.companyName = NewTextField( col[c], row[r++], maxLen );
        this.vatNo       = NewTextField( col[c], row[r++], maxLen );
        this.address     = NewTextField( col[c], row[r++], maxLen );
        this.postCode    = NewTextField( col[c], row[r++], maxLen );
        this.city        = NewTextField( col[c], row[r++], maxLen );
        this.country     = NewTextField( col[c], row[r++], maxLen );
        this.phone       = NewTextField( col[c], row[r++], maxLen );
        this.homePage    = NewTextField( col[c], row[r++], maxLen );
        this.email       = NewTextField( col[c], row[r++], maxLen );

        /////////////////////////////////////////////////////////////////////////////////
        // Field validation event handlers

        this.companyName.Validating  += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.companyName.ContentsChanged )
            {
                return;
            }

            ValidateNotNull( "Company Name", this.companyName.Text, e );
        };

        this.vatNo.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.vatNo.ContentsChanged )
            {
                return;
            }

            ValidateNotNull( "Company's VAT Number", this.vatNo.Text, e );
        };

        this.homePage.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.homePage.ContentsChanged )
            {
                return;
            }

            ValidateHttpURI( "Home Page", this.homePage.Text, e );
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

        /////////////////////////////////////////////////////////////////////////////////

        base.InitializeComponents ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}