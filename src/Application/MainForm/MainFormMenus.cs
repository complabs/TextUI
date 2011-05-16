/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MainFormMenus.cs
 *  Created:    2011-04-26
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using Mbk.Commons;
using VROLib;

#if TEXTUI
    using TextUI;
    using VideoRentalOutlet_TUI.A_TextUI_Specific;
#else
    using System.Drawing;
    using System.Windows.Forms;
    using VideoRentalOutlet_GUI.Properties;
#endif

/////////////////////////////////////////////////////////////////////////////////////////

public partial class MainForm : Form
{
    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes application main menu.
    /// </summary>
    /// 
    private void InitializeMainMenu ()
    {
        InitializeMenu_File      ();
        InitializeMenu_Customers ();
        InitializeMenu_Movies    ();
        InitializeMenu_Exemplars ();
        InitializeMenu_Test      ();
        InitializeMenu_Font      ();
        InitializeMenu_Window    ();
        InitializeMenu_Help      ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes File menu item in the main menu.
    /// </summary>
    /// 
    private void InitializeMenu_File ()
    {
        Menu parent = this.Menu;

        MenuItem miFile = new MenuItem( "&File" );
        SetToolTip( miFile, "Manage price list, company info and database files..." );

        parent.MenuItems.Add( miFile );

        //-------------------------------------------------------------------------------
        MenuItem miCompanyInfo = new MenuItem( "&Company details..." )
        {
            Shortcut = Shortcut.F4,
        };
        SetToolTip( miCompanyInfo, "Manage Video Rental Outlet company information..." );

        miFile.MenuItems.Add( miCompanyInfo );

        miCompanyInfo.Click += delegate
        {
            Open<CompanyDetailsForm>( OpenMode.Browse );
        };

        //-------------------------------------------------------------------------------
        InitializeMenu_PriceList( miFile );

        //-------------------------------------------------------------------------------
        miFile.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        InitializeMenu_Database( miFile );

        //-------------------------------------------------------------------------------
        InitializeMenu_Backup( miFile );

        //-------------------------------------------------------------------------------
        miFile.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miFileSave = new MenuItem( "&Save" )
        {
            Shortcut = Shortcut.CtrlS
        };
        SetToolTip( miFileSave, "Save database" );

        miFile.MenuItems.Add( miFileSave );

        miFileSave.Click += delegate
        {
            MdiClient.IfValidateOk( () =>
            {
                SaveDatabase( this.databaseFilename, "Saving Database", /*silent*/true );
            } );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFileExit = new MenuItem( "Save and E&xit" )
        {
            Shortcut = Shortcut.AltF4
        };
        SetToolTip( miFileExit, "Save database and exit application ..." );

        miFile.MenuItems.Add( miFileExit );

        miFileExit.Click += delegate
        {
            MdiClient.IfValidateOk( () =>
            {
                if ( VideoStore.IsDirty )
                {
                    SaveDatabase( this.databaseFilename, 
                        "Saving Database", /*silent*/true );
                }

                Application.Exit ();
            } );
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Database menu sub-item.
    /// </summary>
    /// 
    private void InitializeMenu_Database( MenuItem parentItem )
    {
        MenuItem miDatabase = new MenuItem( "&Database" );
        SetToolTip( miDatabase, "Load, save and initialize database..." );

        parentItem.MenuItems.Add( miDatabase );

        //-------------------------------------------------------------------------------
        MenuItem miDatabaseDump = new MenuItem( "&Dump Database..." );
        SetToolTip( miDatabaseDump, "Dump database contents to file 'VRO-dump.txt'..." );

        miDatabase.MenuItems.Add( miDatabaseDump );

        miDatabaseDump.Click += delegate 
        {
            DumpDatabase ();
        };

        //-------------------------------------------------------------------------------
        miDatabase.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miDatabaseLoad = new MenuItem( "Re&load" );
        SetToolTip( miDatabaseLoad, "Reload database from file..." );

        miDatabase.MenuItems.Add( miDatabaseLoad );
        miDatabaseLoad.Click += delegate
        {
            DialogResult answer = MessageBox.Show( 
                "You are about to reload database from file.\r\n\r\n"
                    + "All changes to data will be lost!", 
                "Loading Database", 
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning );

            if ( answer != DialogResult.OK )
            {
                return;
            }

            LoadDatabase( this.databaseFilename, "Video Rental Outlet Database" );
        };

        //-------------------------------------------------------------------------------
        MenuItem miDatabaseSave = new MenuItem( "&Save" );
        SetToolTip( miDatabaseSave, "Save current database into file..." );

        miDatabase.MenuItems.Add( miDatabaseSave );

        miDatabaseSave.Click += delegate
        {
            SaveDatabase( this.databaseFilename, "Video Rental Outlet Database" );
        };

        //-------------------------------------------------------------------------------
        miDatabase.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miDatabaseClear = new MenuItem( "&Clear All Data" );
        SetToolTip( miDatabaseClear, "Initialize an empty database..." );

        miDatabase.MenuItems.Add( miDatabaseClear );

        miDatabaseClear.Click += delegate
        {
            DialogResult answer = MessageBox.Show( 
                "You are about to delete all current data!\r\n\r\n"
                    + "Are you sure?", 
                "Clear Database", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button2 );

            if ( answer != DialogResult.Yes )
            {
                return;
            }

            UnloadAllMdiChildForms ();

            VideoStore = new VideoRentalOutlet( "No Name", "(No VAT)" );
            VideoStore.Changed += VideoStore_Changed;

            UpdateTitle ();
        };

        //-------------------------------------------------------------------------------
        MenuItem miDatabaseInitialize = new MenuItem( "&Initialize Sample Database" );
        SetToolTip( miDatabaseInitialize, "Initialize database with sample data..." );

        miDatabase.MenuItems.Add( miDatabaseInitialize );

        miDatabaseInitialize.Click += delegate 
        {
            try
            {
                InitializeTestDatabase ();
            }
            catch( Exception ex )
            {
                // As InitializeTestDatabase method uses external DLL, we cannot catch
                // "could not load dll error" inside the method, so we do it here.
                // Note that the method handles itself its own exceptions.
                //
                MessageBox.Show( ex.Message, "Error Loading DLL", 
                    MessageBoxButtons.OK, MessageBoxIcon.Hand );
            }
        };

        //-------------------------------------------------------------------------------
        miDatabase.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miFileTestAddRandomData = new MenuItem( "Add &Random Data" );
        SetToolTip( miFileTestAddRandomData, "Add random data to current database..." );

        miDatabase.MenuItems.Add( miFileTestAddRandomData );

        miFileTestAddRandomData.Click += delegate 
        {
            try
            {
                AddTestRecordsToDatabase ();
            }
            catch( Exception ex )
            {
                // As AddTestRecordsToDatabase method uses external DLL, we cannot catch
                // "could not load dll error" inside the method, so we do it here
                // Note that the method handles itself its own exceptions.
                //
                MessageBox.Show( ex.Message, "Error Loading DLL", 
                    MessageBoxButtons.OK, MessageBoxIcon.Hand );
            }
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Backup menu sub-item.
    /// </summary>
    /// 
    private void InitializeMenu_Backup( MenuItem parentItem )
    {
        MenuItem miFileBackup = new MenuItem( "&Backup" );
        SetToolTip( miFileBackup, "Load/save database from/to backup file..." );

        parentItem.MenuItems.Add( miFileBackup );

        //-------------------------------------------------------------------------------
        MenuItem miDatabaseLoadBackup = new MenuItem( "&Load Backup" );
        SetToolTip( miDatabaseLoadBackup, "Load database from backup file..." );

        miFileBackup.MenuItems.Add( miDatabaseLoadBackup );

        miDatabaseLoadBackup.Click += delegate
        {
            DialogResult answer = MessageBox.Show( 
                "You are about to load database from backup file.\r\n\r\n"
                    + "Make sure that you have saved current database!", 
                "Loading Backup", 
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button2 );

            if ( answer != DialogResult.OK )
            {
                return;
            }

            LoadDatabase( this.backupFilename, "Loading Backup" );
        };

        //-------------------------------------------------------------------------------

        MenuItem miDatabaseSaveBackup = new MenuItem( "&Save Backup" );
        SetToolTip( miDatabaseSaveBackup, "Save current database into backup file..." );
        miFileBackup.MenuItems.Add( miDatabaseSaveBackup );

        miDatabaseSaveBackup.Click += delegate
        {
            SaveDatabase( this.backupFilename, "Saving Backup" );
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Price List menu sub-item.
    /// </summary>
    /// 
    private void InitializeMenu_PriceList( MenuItem parentItem )
    {
        MenuItem miPriceList = new MenuItem( "&Price list" );
        SetToolTip( miPriceList, "Manage outlet's price list..." );

        parentItem.MenuItems.Add( miPriceList );

        //-------------------------------------------------------------------------------
        MenuItem miManage = new MenuItem( "&Manage Price List" )
        {
            Shortcut = Shortcut.F3
        };
        SetToolTip( miManage, "Manage Price List" );

        miPriceList.MenuItems.Add( miManage );

        miManage.Click += delegate 
        {
            Open<PriceListForm>( OpenMode.Edit );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFind = new MenuItem( "&Find..." );
        SetToolTip( miFind, "Find price specification in price list..." );

        miPriceList.MenuItems.Add( miFind );

        miFind.Click += delegate 
        {
            Open<PriceListForm>( OpenMode.Search );
        };

        //-------------------------------------------------------------------------------
        miPriceList.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miDelete = new MenuItem( "&Delete" );
        SetToolTip( miDelete, "Delete current price list..." );

        miPriceList.MenuItems.Add( miDelete );

        miDelete.Click += delegate
        {
            ExecuteFor<PriceListForm>( f => f.DeleteCurrentRecord () );
        };

        //-------------------------------------------------------------------------------
        MenuItem miAddNew = new MenuItem( "&Add New..." );
        SetToolTip( miAddNew, "Add new price specification to price list..." );

        miPriceList.MenuItems.Add( miAddNew );

        miAddNew.Click += delegate 
        {
            if ( ! OpenAddNew<PriceListForm> () )
            {
                Open<PriceDetailsForm>( OpenMode.AddNew );
            }
        };

        //-------------------------------------------------------------------------------

        miPriceList.Popup += delegate
        {
            miDelete.Enabled = Exist<PriceListForm>( f => f.EnabledDeleteButton );
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Customers menu item in the main menu.
    /// </summary>
    /// 
    private void InitializeMenu_Customers ()
    {
        Menu parent = this.Menu;

        MenuItem miCustomers = new MenuItem( "&Customers" );
        SetToolTip( miCustomers, "Manage customer database..." );

        parent.MenuItems.Add( miCustomers );

        //-------------------------------------------------------------------------------
        MenuItem miBrowse = new MenuItem( "&Browse" )
        {
            Shortcut = Shortcut.F5
        };
        SetToolTip( miBrowse, "Browse customer database..." );

        miCustomers.MenuItems.Add( miBrowse );

        miBrowse.Click += delegate 
        {
            Open<CustomerListForm>( OpenMode.Browse );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFind = new MenuItem( "&Find..." )
        {
            Shortcut = Shortcut.ShiftF5
        };
        SetToolTip( miFind, "Find customer in database..." );

        miCustomers.MenuItems.Add( miFind );

        miFind.Click += delegate 
        {
            Open<CustomerListForm>( OpenMode.Search );
        };

        //-------------------------------------------------------------------------------
        MenuItem miManage = new MenuItem( "&Manage Customers" )
        {
            Shortcut = Shortcut.CtrlF5
        };
        SetToolTip( miManage, "Manage customer database..." );

        miCustomers.MenuItems.Add( miManage );

        miManage.Click += delegate 
        {
            Open<CustomerListForm>( OpenMode.Edit );
        };
        
        //-------------------------------------------------------------------------------
        miCustomers.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miDelete = new MenuItem( "&Delete" );
        SetToolTip( miDelete, "Delete current customer..." );

        miCustomers.MenuItems.Add( miDelete );

        miDelete.Click += delegate
        {
            ExecuteFor<CustomerListForm>( f => f.DeleteCurrentRecord () );
        };

        //-------------------------------------------------------------------------------
        MenuItem miAddNew = new MenuItem( "&Add New Customer..." );
        SetToolTip( miAddNew, "Add new customer to customer database..." );

        miCustomers.MenuItems.Add( miAddNew );

        miAddNew.Click += delegate 
        {
            if ( ! OpenAddNew<CustomerListForm> () )
            {
                Open<CustomerDetailsForm>( OpenMode.AddNew );
            }
        };
        
        //-------------------------------------------------------------------------------
        miCustomers.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miRentals = new MenuItem( "&Rented Items" )
        {
            Shortcut = Shortcut.F9
        };
        SetToolTip( miRentals, "Manage rented items for selected customer..." );

        miCustomers.MenuItems.Add( miRentals );

        miRentals.Click += delegate
        {
            ExecuteFor<CustomerListForm>( f => f.OpenSubItemsForm( OpenMode.Edit ) );
        };
        
        //-------------------------------------------------------------------------------
        MenuItem miAddRental = new MenuItem( "Rent New &Item..." )
        {
            Shortcut = Shortcut.ShiftF9
        };
        SetToolTip( miAddRental, "Rent new movie exemplar to selected customer..." );

        miCustomers.MenuItems.Add( miAddRental );

        miAddRental.Click += ( sender, e ) => AddNewRentedItem ();
        
        //-------------------------------------------------------------------------------
        miCustomers.Popup += delegate
        {
            miDelete.Enabled  = Exist<CustomerListForm>( f => f.EnabledDeleteButton );
            miRentals.Enabled = Exist<CustomerListForm>( f => f.EnabledLinkButton );
            //miAddRental.Enabled = Exist<CustomerListForm>( f => f.IsActive )
                //|| Exist<RentedItemListForm>( f => f.IsActive );
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Movies menu item in the main menu.
    /// </summary>
    /// 
    private void InitializeMenu_Movies ()
    {
        Menu parent = this.Menu;

        MenuItem miMovies = new MenuItem( "&Movies" );
        SetToolTip( miMovies, "Manage movie database..." );

        parent.MenuItems.Add( miMovies );

        //-------------------------------------------------------------------------------
        MenuItem miBrowse = new MenuItem( "&Browse" )
        {
            Shortcut = Shortcut.F6
        };
        SetToolTip( miBrowse, "Browse movie database..." );

        miMovies.MenuItems.Add( miBrowse );

        miBrowse.Click += delegate 
        {
            Open<MovieListForm>( OpenMode.Browse );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFind = new MenuItem( "&Find..." )
        {
            Shortcut = Shortcut.ShiftF6
        };
        SetToolTip( miFind, "Find movie in database..." );

        miMovies.MenuItems.Add( miFind );

        miFind.Click += delegate 
        {
            Open<MovieListForm>( OpenMode.Search );
        };

        //-------------------------------------------------------------------------------
        MenuItem miManage = new MenuItem( "&Manage Movies" )
        {
            Shortcut = Shortcut.CtrlF6
        };
        SetToolTip( miManage, "Manage movie database..." );

        miMovies.MenuItems.Add( miManage );

        miManage.Click += delegate 
        {
            Open<MovieListForm>( OpenMode.Edit );
        };

        //-------------------------------------------------------------------------------
        miMovies.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miDelete = new MenuItem( "&Delete" );
        SetToolTip( miDelete, "Delete current movie..." );

        miMovies.MenuItems.Add( miDelete );

        miDelete.Click += delegate
        {
            ExecuteFor<MovieListForm>( f => f.DeleteCurrentRecord () );
        };

        //-------------------------------------------------------------------------------
        MenuItem miAddNew = new MenuItem( "&Add New Movie..." );
        SetToolTip( miAddNew, "Add new movie to movie database..." );

        miMovies.MenuItems.Add( miAddNew );

        miAddNew.Click += delegate 
        {
            if ( ! OpenAddNew<MovieListForm> () )
            {
                Open<MovieDetailsForm>( OpenMode.AddNew );
            }
        };

        //-------------------------------------------------------------------------------
        miMovies.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miExemplars = new MenuItem( "Movie &Exemplars" )
        {
            Shortcut = Shortcut.F8
        };
        SetToolTip( miExemplars, "Manage exemplars for selected movie..." );

        miMovies.MenuItems.Add( miExemplars );

        miExemplars.Click += delegate
        {
            ExecuteFor<MovieListForm> ( f => f.OpenSubItemsForm( OpenMode.Edit ) );
        };
        
        //-------------------------------------------------------------------------------
        MenuItem miAddExemplar = new MenuItem( "Add New E&xemplar..." );
        SetToolTip( miAddExemplar, "Add new exemplar for selected movie..." );

        miMovies.MenuItems.Add( miAddExemplar );

        miAddExemplar.Click += ( sender, e ) => AddNewMovieExemplar ();
        
        //-------------------------------------------------------------------------------
        miMovies.Popup += delegate
        {
            miDelete.Enabled = Exist<MovieListForm>( f => f.EnabledDeleteButton );
            miExemplars.Enabled = Exist<MovieListForm>( f => f.EnabledLinkButton );
            miAddExemplar.Enabled = Exist<MovieListForm>( f => f.IsActive );
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Exemplars menu item in the main menu.
    /// </summary>
    /// 
    private void InitializeMenu_Exemplars ()
    {
        Menu parent = this.Menu;

        MenuItem miMovieExemplars = new MenuItem( "&Exemplars" );
        SetToolTip( miMovieExemplars, "Manage movie exemplar collection..." );

        parent.MenuItems.Add( miMovieExemplars );

        //-------------------------------------------------------------------------------
        MenuItem miRented = new MenuItem( "Browse &Rented Items" )
        {
            Shortcut = Shortcut.F7
        };
        SetToolTip( miRented, "Browse only rented movie exemplars in database..." );

        miMovieExemplars.MenuItems.Add( miRented );

        miRented.Click += delegate 
        {
            OpenMovieExemplarList( OpenMode.Browse, exemplar => exemplar.IsRented );
        };

        //-------------------------------------------------------------------------------
        miMovieExemplars.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miFind = new MenuItem( "&Find..." )
        {
            Shortcut = Shortcut.ShiftF7
        };
        SetToolTip( miFind, "Search exemplars in database..." );

        miMovieExemplars.MenuItems.Add( miFind );

        miFind.Click += delegate 
        {
            OpenMovieExemplarList( OpenMode.Search, QueryAllExemplars );
        };

        //-------------------------------------------------------------------------------
        MenuItem miManage = new MenuItem( "&Manage Exemplars" )
        {
            Shortcut = Shortcut.CtrlF7
        };
        SetToolTip( miManage, "Manage movie exemplar collection..." );

        miMovieExemplars.MenuItems.Add( miManage );

        miManage.Click += delegate 
        {
            OpenMovieExemplarList( OpenMode.Edit, QueryAllExemplars );
        };

        //-------------------------------------------------------------------------------
        miMovieExemplars.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miDelete = new MenuItem( "&Delete" );
        SetToolTip( miDelete, "Delete current movie exemplar..." );

        miMovieExemplars.MenuItems.Add( miDelete );

        miDelete.Click += delegate
        {
            ExecuteFor<MovieExemplarListForm>( f => f.DeleteCurrentRecord () );
        };

        //-------------------------------------------------------------------------------
        MenuItem miAddNew = new MenuItem( "&Add New Exemplar..." )
        {
            Shortcut = Shortcut.ShiftF8
        };
        SetToolTip( miAddNew, "Add new copy to movie exemplar collection..." );

        miMovieExemplars.MenuItems.Add( miAddNew );

        miAddNew.Click += ( sender, e ) => AddNewMovieExemplar ();

        //-------------------------------------------------------------------------------

        miMovieExemplars.Popup += delegate
        {
            miDelete.Enabled = Exist<MovieExemplarListForm>( f => f.EnabledDeleteButton );
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Font menu item in the main menu (only in GUI mode).
    /// </summary>
    /// 
    private void InitializeMenu_Font ()
    {
        Menu parent = this.Menu;

#if ! TEXTUI
        //-------------------------------------------------------------------------------
        MenuItem miFonts = new MenuItem( "F&ont" );
        SetToolTip( miFonts, "Setup default font..." );

        parent.MenuItems.Add( miFonts );

        //-------------------------------------------------------------------------------
        MenuItem miFontVerdana = new MenuItem( "&Verdana" );
        SetToolTip( miFontVerdana, "Setup Verdana as default font..." );

        miFonts.MenuItems.Add( miFontVerdana );

        miFontVerdana.Click += delegate 
        {
            SetupBaseFont( "Verdana", 9f );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFontSansSerif = new MenuItem( "&MS Sans Serif" );
        SetToolTip( miFontSansSerif, "Setup Microsoft Sans Serif as default font..." );

        miFonts.MenuItems.Add( miFontSansSerif );

        miFontSansSerif.Click += delegate 
        {
            SetupBaseFont( "Microsoft Sans Serif", 10.5f );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFontArial = new MenuItem( "&Arial" );
        SetToolTip( miFontArial, "Setup Arial as default font..." );

        miFonts.MenuItems.Add( miFontArial );

        miFontArial.Click += delegate 
        {
            SetupBaseFont( "Arial", 10.5f );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFontTimes = new MenuItem( "&Times New Roman" );
        SetToolTip( miFontTimes, "Setup Times New Roman as default font..." );

        miFonts.MenuItems.Add( miFontTimes );

        miFontTimes.Click += delegate 
        {
            SetupBaseFont( "Times New Roman", 11f );
        };

        //-------------------------------------------------------------------------------
        MenuItem miFontConsolas = new MenuItem( "&Consolas" );
        SetToolTip( miFontConsolas, "Setup Consolas (alt Courier New as default font)" );

        miFonts.MenuItems.Add( miFontConsolas );

        miFontConsolas.Click += delegate 
        {
            // Dimensions are not properly scaled when setting fixed type font
            // so we need to unload all details forms first.
            //
            ExecuteFor<FormBase>( form => 
            {
                if ( ! form.IsListForm )
                {
                    form.QuitForm ();
                }
            } );

            SetupBaseFont( "Consolas", 10f );
            if ( this.Font.Name.ToLower () != "consolas" )
            {
                SetupBaseFont( "Courier New", 10f );
            }
        };

        //-------------------------------------------------------------------------------
        miFonts.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miFontLarger = new MenuItem( "&Larger" ) 
        { 
            Shortcut = Shortcut.CtrlShift2
        };
        SetToolTip( miFontLarger, "Increase font size 0.5 pt..." );

        miFonts.MenuItems.Add( miFontLarger );

        miFontLarger.Click += delegate 
        {
            if ( this.Font.Size < 12f )
            {
                SetupBaseFont( this.Font.Name, this.Font.Size + 0.5f );
            }
        };

        //-------------------------------------------------------------------------------
        MenuItem miFontSmaller = new MenuItem( "&Smaller" )
        { 
            Shortcut = Shortcut.CtrlShift1
        };
        SetToolTip( miFontSmaller, "Increase font size 0.5 pt..." );

        miFonts.MenuItems.Add( miFontSmaller );

        miFontSmaller.Click += delegate 
        {
            if ( this.Font.Size > 8f )
            {
                SetupBaseFont( this.Font.Name, this.Font.Size - 0.5f );
            }
        };

        //-------------------------------------------------------------------------------
        miFonts.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miFontSettings = new MenuItem( "&Settings..." );
        SetToolTip( miFontSettings, "Select application font..." );

        miFonts.MenuItems.Add( miFontSettings );

        miFontSettings.Click += delegate 
        {
            FontDialog dialog = new FontDialog ()
            {
                FontMustExist = true, Font = this.Font,
            };

            if ( DialogResult.OK == dialog.ShowDialog () )
            {
                SetupBaseFont( dialog.Font.Name, dialog.Font.Size );
            }
        };

        //-------------------------------------------------------------------------------
        miFonts.Popup += delegate
        {
            miFontVerdana  .Checked = this.Font.Name.ToLower() == "verdana";
            miFontSansSerif.Checked = this.Font.Name.ToLower() == "microsoft sans serif";
            miFontArial    .Checked = this.Font.Name.ToLower() == "arial";
            miFontTimes    .Checked = this.Font.Name.ToLower() == "times new roman";

            miFontConsolas .Checked = this.Font.Name.ToLower() == "consolas"
                                   || this.Font.Name.ToLower() == "courier new";

            miFontSmaller.Enabled = this.Font.Size > 8f;
            miFontLarger.Enabled = this.Font.Size < 12f;
        };
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Test menu item in the main menu.
    /// </summary>
    /// 
    private void InitializeMenu_Test ()
    {
        Menu parent = this.Menu;

        //-------------------------------------------------------------------------------
        MenuItem miTest = new MenuItem( "&Tests" );
        SetToolTip( miTest, "Miscellaneous testing and demo stuff..." );

        parent.MenuItems.Add( miTest );

        //-------------------------------------------------------------------------------
        MenuItem miFileSudoku = new MenuItem( "&Sudoku" )
        {
            Shortcut = Shortcut.F12
        };
        SetToolTip( miFileSudoku, "Play a Sudoku puzzle..." );

        miTest.MenuItems.Add( miFileSudoku );

        miFileSudoku.Click += delegate 
        {
            OpenSudoku ();
        };

        //-------------------------------------------------------------------------------
        miTest.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miDumpDatabaseInfo = new MenuItem( "Database &Info" )
        {
            Shortcut = Shortcut.F11
        };
        SetToolTip( miDumpDatabaseInfo, "Dump real-time database info ..." );

        miTest.MenuItems.Add( miDumpDatabaseInfo );

        miDumpDatabaseInfo.Click += delegate
        {
            DumpDatabaseShortInfo ();
        };

#if DEBUG
        MenuItem miTrace = new MenuItem( "T&race" )
        {
            Shortcut = Shortcut.CtrlF11
        };
        SetToolTip( miTrace, "Toggle tracing ..." );

        miTest.MenuItems.Add( miTrace );

        miTrace.Click += delegate 
        {
            Debug.ToggleTraceOnOff ();

            #if TEXTUI
                DrawMdiClientWindow ();
            #endif

            InfoMessage = Debug.IsTraceOpen 
                ? "Tracing to " + System.IO.Path.GetFileName( Debug.TraceFile.Name )
                : "Stopped tracing";
        };

        //-------------------------------------------------------------------------------
        MenuItem miThrowException = new MenuItem( "Throw Dummy &Exception" );
        SetToolTip( miThrowException, "Throw exception to test exception handling..." );

        miTest.MenuItems.Add( miThrowException );

        miThrowException.Click += delegate 
        {
           throw new Exception( "Dummy exception. Tests exception handling..." );
        };

        //-------------------------------------------------------------------------------
        miTest.Popup += delegate
        {
            miTrace.Checked = Debug.IsTraceOpen;
        };
#endif

#if TEXTUI
        //-------------------------------------------------------------------------------
        miTest.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miFileTestTUI = new MenuItem( "Test &TUI" );
        SetToolTip( miFileTestTUI, 
            "Open test client for Text User Interface (TUI) Library ..." );

        miTest.MenuItems.Add( miFileTestTUI );

        miFileTestTUI.Click += delegate
        {
            DialogResult answer = MessageBox.Show( 
                "\r\n\r\nYou are about to leave Video Rental Outlet application\r\n"
                    + "and start test client for Text User Interface (TUI).\r\n\r\n"
                    + "Make sure that you have saved current database!", 
                "Start Text User Interface Test Suite",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Severe );

            if ( answer != DialogResult.OK )
            {
                return;
            }

            Application.NewRootWindow( new VRO_TestSuite.TestClient_TUI () );
        };
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Window menu item in the main menu.
    /// </summary>
    /// 
    private void InitializeMenu_Window ()
    {
        Menu parent = this.Menu;

        //-------------------------------------------------------------------------------
        MenuItem miWindow = new MenuItem( "&Window" );
        SetToolTip( miWindow, "Arrange windows..." );

        parent.MenuItems.Add( miWindow );

        //-------------------------------------------------------------------------------
        MenuItem miCascade = new MenuItem( "&Cascade Windows" );
        SetToolTip( miCascade, "Cascade..." );

        miWindow.MenuItems.Add( miCascade );

        miCascade.Click += delegate 
        {
            LayoutMdi( MdiLayout.Cascade );
            if ( ActiveMdiChild != null )
            {
                ActiveMdiChild.Focus ();
            }
        };

        //-------------------------------------------------------------------------------
        MenuItem miCloseAll = new MenuItem( "Clean &Desktop" );
        SetToolTip( miCloseAll, "Close all open forms..." );

        miWindow.MenuItems.Add( miCloseAll );

        miCloseAll.Click += delegate 
        {
            UnloadAllMdiChildForms ();
        };

#if TEXTUI

        //-------------------------------------------------------------------------------
        MenuItem miRefresh = new MenuItem( "&Refresh Screen" )
        {
            Shortcut = Shortcut.CtrlL
        };
        SetToolTip( miRefresh, "Refresh (full repaint) screen..." );

        miWindow.MenuItems.Add( miRefresh );

        miRefresh.Click += delegate 
        {
            Application.Screen.FullRepaint ();
        };
#else
        //-------------------------------------------------------------------------------
        MenuItem miSnapshot = new MenuItem( "Sna&pshot to file..." )
        {
            Shortcut = Shortcut.CtrlP
        };
        SetToolTip( miSnapshot, "Snapshot current MDI window to PNG file..." );

        miWindow.MenuItems.Add( miSnapshot );

        miSnapshot.Click += delegate 
        {
            SnapshotAsPngImage( ActiveMdiChild == null ? this : ActiveMdiChild );
        };

        //-------------------------------------------------------------------------------
        miWindow.MdiList = true;

        miWindow.Popup += delegate 
        {
            miCascade.Enabled = MdiChildren.Length > 1;
            miCloseAll.Enabled = MdiChildren.Length > 0;
        };
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes Help menu item in the main menu.
    /// </summary>
    /// 
    private void InitializeMenu_Help ()
    {
        Menu parent = this.Menu;

        //-------------------------------------------------------------------------------
        MenuItem miHelp = new MenuItem( "&Help" );
        SetToolTip( miHelp, "Help..." );

        parent.MenuItems.Add( miHelp );

        //-------------------------------------------------------------------------------
        MenuItem miViewHelp = new MenuItem( "View &Help" )
        {
            Shortcut = Shortcut.F1
        };
        SetToolTip( miViewHelp, "View Help..." );

        miHelp.MenuItems.Add( miViewHelp );

        miViewHelp.Click += delegate 
        {
            if ( ActiveMdiChild is SudokuForm && Em.IsGUI )
            {
                MyReportBox.ShowReport( "Sudoku - Help", Resources.SudokuHelp,
                    ActiveMdiChild.Width, ActiveMdiChild.Height - 50 );
            }
            else
            {
                ShowReport( "Video Rental Outlet - Help", Resources.ShortHelp );
            }
        };

        //-------------------------------------------------------------------------------
        miHelp.MenuItems.Add( new MenuItem( "-" ) );

        //-------------------------------------------------------------------------------
        MenuItem miAbout = new MenuItem( "&About Video Rental Outlet" );
        SetToolTip( miAbout, "Show application details..." );

        miHelp.MenuItems.Add( miAbout );

        miAbout.Click += delegate
        {
            MessageBox.Show( 
                "\n".PadLeft( Em.IsGUI ? 80 : 0 )
                + GetInfoAboutApplication (),
                "About Video Rental Outlet Application...",
                MessageBoxButtons.OK, MessageBoxIcon.Information );
        };
    }
}