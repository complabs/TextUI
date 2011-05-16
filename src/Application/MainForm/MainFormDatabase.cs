/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MainFormDatabase.cs
 *  Created:    2011-04-26
 *  Modified:   2011-05-11
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.IO;
using System.Text;
using System.Reflection;

#if TEXTUI
    using TextUI;
#else
    using System.Windows.Forms;
#endif

using Mbk.Commons;
using VROLib;

/////////////////////////////////////////////////////////////////////////////////////////
// Database handling (loading, saving etc) methods

public partial class MainForm : Form
{
    #region [ Constants ]

    private readonly string databaseFilename = "VRO.vroData";
    private readonly string backupFilename   = "VRO-backup.vroData";

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Database Instance ]

    /// <summary>
    /// Gets the VideoRentalOutlet database for this application.
    /// </summary>
    /// 
    public VideoRentalOutlet VideoStore { get; private set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Load/Save Database Methods ]

    /// <summary>
    /// Loads database from file.
    /// </summary>
    /// 
    private void LoadDatabase( string filename, string caption, bool silent = false )
    {
        string progressInfo = "Loading database from '" + filename + "'...";

        // Remove children from client area (probably referring to the contents
        // of the current database)
        //
        UnloadAllMdiChildForms ();

        // Create progress bar
        //
        MyProgressBar progress = new MyProgressBar( this, progressInfo );

        Exception lastException = null;
        VideoRentalOutlet loadedStore = null;
        string elapsedTime = string.Empty;

        // Deserialize database using our file stream with progress info.
        //
        try 
        {
            FileInfo fileInfo = new FileInfo( filename );
            long fileLength = fileInfo.Exists ? fileInfo.Length : 0;

            using( FileStreamWithProgressBar fs = new FileStreamWithProgressBar( 
                filename, FileMode.Open, progress, fileLength )
                )
            {
                loadedStore = VideoRentalOutlet.Deserialize( fs );

                fs.StopTimer ();
                elapsedTime = " in " + fs.VerboseElapsedTime;
            }
        }
        catch( Exception ex )
        {
            lastException = ex;
        }

        progress.Quit ();

        if ( loadedStore != null )
        {
            VideoStore = loadedStore;
            VideoStore.Changed += VideoStore_Changed;

            string info = VideoStore.TotalRecordsCount.ToString () 
                + " records loaded from '" + filename + "'"  + elapsedTime;

            if ( silent )
            {
                this.InfoMessage = info;
            }
            else
            {
                info += "\n\n\n" + VideoStore.StatisticsInfo( info.Length ) + "\n";

                MessageBox.Show( info, caption, 
                    MessageBoxButtons.OK, MessageBoxIcon.Information );
            }

            UpdateTitle ();
        }
        else if ( lastException != null )
        {
            MessageBox.Show( 
                "There was an error while loading database.\n\n" 
                    + "Filename: " + filename + "\n\n"
                    + "System Message:\n\n"
                    + lastException.Message, 
                caption, MessageBoxButtons.OK, MessageBoxIcon.Hand );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Serializes database to file.
    /// </summary>
    /// 
    private void SaveDatabase( string filename, string caption, bool silent = false )
    {
        // Create progress bar

        string progressInfo = "Saving database to '" + filename + "'...";

        // Create progress bar
        //
        MyProgressBar progress = new MyProgressBar( this, progressInfo );

        Exception lastException = null;
        bool succeded = false;
        string elapsedTime = string.Empty;

        // Serialize database using our file stream with progress info.
        //
        try 
        {
            FileInfo fileInfo = new FileInfo( filename );
            long fileLength = fileInfo.Exists ? fileInfo.Length : 0;

            // Anticipate file length based on records count (if file length is 0
            // or there was significant increase in database)
            //
            fileLength = Math.Max( VideoStore.TotalRecordsCount * 160, fileLength );

            using( FileStreamWithProgressBar fs = new FileStreamWithProgressBar( 
                filename, FileMode.Create, progress, fileLength )
                )
            {
                VideoStore.Serialize( fs );

                succeded = true;
                fs.StopTimer ();
                elapsedTime = " in " + fs.VerboseElapsedTime;
            }
        }
        catch( Exception ex )
        {
            lastException = ex;
        }

        progress.Quit ();

        // Report status
        //
        if ( succeded ) 
        {
            string info = VideoStore.TotalRecordsCount.ToString () 
                + " records saved to '" + filename + "'"  + elapsedTime;

            if ( silent )
            {
                this.InfoMessage = info;
            }
            else
            {
                MessageBox.Show( 
                    info + "\n\n\n" + VideoStore.StatisticsInfo( info.Length ) + "\n", 
                    caption, MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
        }
        else  if ( lastException != null )
        {
            MessageBox.Show( 
                "There was an error while saving database.\n\n" 
                + "Filename: " + filename + "\n\n"
                + "System Message:\n" + lastException.Message, 
                caption, MessageBoxButtons.OK, MessageBoxIcon.Hand );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Initialize Test Database and Add Test Records to Database Methods ]

    /// <summary>
    /// Initializes sample database using VRO_TestSuite (from ID132V Lab3a).
    /// </summary>
    /// 
    private void InitializeTestDatabase ()
    {
        DialogResult answer = MessageBox.Show(
            "You are about to delete all current data!\n\nAre you sure?", 
            "Initialize Sample Database",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 
            MessageBoxDefaultButton.Button2 );

        if ( answer != DialogResult.Yes )
        {
            return;
        }

        UnloadAllMdiChildForms ();

        // Create progress bar
        //
        MyProgressBar progress = new MyProgressBar( this, "Initializing database..." );
        progress.Minimum  = 0;
        progress.Maximum = 100;

        string report = null;

        try
        {
            VRO_TestSuite.StringWriter sb = new VRO_TestSuite.StringWriter ();

            VRO_TestSuite.TestClient_VRO test = new VRO_TestSuite.TestClient_VRO( sb );

            sb.WriteLine ();
            sb.WriteLine( "Initializing Sample Database..." );

            test.CreateSampleDatabase ();

            progress.Value = 100;
            progress.Refresh ();

            report = sb.ToString ();

            if ( test.VideoStore != null )
            {
                VideoStore = test.VideoStore;
                VideoStore.Changed += VideoStore_Changed;

                UpdateTitle ();
            }
        }
        catch( Exception ex )
        {
            report = ex.ToString ();
        }

        progress.Quit ();

        try 
        { 
            System.IO.File.WriteAllText( "VRO-test.txt", report ); 
            report += "Report saved to to 'VRO-Test.txt'";
        } 
        catch {}

        ShowReport( "Video Rental Outlet - Test Suite Report", report );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Adds number of test records to existing database.
    /// </summary>
    ///
    private void AddTestRecordsToDatabase( int extraRecords = 100 )
    {
        DialogResult answer = MessageBox.Show(
            "You are about to add random data to database!\n\nAre you sure?", 
            "Add Test Records",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 
            MessageBoxDefaultButton.Button2 );

        if ( answer != DialogResult.Yes )
        {
            return;
        }

        string progressInfo = "Adding random records to existing database...";

        // Remove children from client area (probably referring to the contents
        // of the current database)
        //
        UnloadAllMdiChildForms ();

        // Create progress bar
        //
        MyProgressBar progress = new MyProgressBar( this, progressInfo );

        // Add records using VRO_TestSuite
        //
        try
        {
            // Create VRO test suite that will be used to add records to existing databse
            //
            VRO_TestSuite.StringWriter sb = new VRO_TestSuite.StringWriter ();
            VRO_TestSuite.TestClient_VRO test = new VRO_TestSuite.TestClient_VRO( sb );
            test.VideoStore = this.VideoStore;

            // Add records and maintain progress bar every 5th added record
            //
            for ( int i = 1; i <= extraRecords; ++i )
            {
                if ( i % 5 == 4 )
                {
                    progress.Value = i;
                    progress.Refresh ();

                    Application.DoEvents (); // increase responsiveness
                }

                try
                {
                    test.AddTestRecords ();
                }
                catch
                {
                    // Ignore any database violations as we try to insert random data
                }
            }

            this.InfoMessage = "Database now contains " + VideoStore.TotalRecordsCount
                + " records";
        }
        catch( Exception ex )
        {
            MessageBox.Show( ex.ToString (), "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Hand );
        }

        progress.Quit ();

        #if TEXTUI
        if ( MdiClient.ActiveChild != null )
        {
            MdiClient.Focus ();
        }
        #endif
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Methods Displaying Database Info and Contents ]

    /// <summary>
    /// Dumps full contents of database to file and shows the report in report box.
    /// </summary>
    /// 
    private void DumpDatabase ()
    {
        string progressInfo = "Dumping database contents as text...";

        // Create progress bar
        //
        MyProgressBar progress = new MyProgressBar( this, progressInfo );

        progress.Minimum = 0;
        progress.Maximum = VideoStore.TotalRecordsCount;

        StringBuilder sb = new StringBuilder ();

        sb.AppendLine ();
        sb.AppendLine( "VROLib Database Format v" + VideoStore.Version );
        sb.AppendLine ();

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( VideoStore.FullInfo () );

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( VideoStore.Customers.FullInfo () );

        progress.Value += VideoStore.Customers.Count;
        progress.Refresh ();

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( VideoStore.PriceList.FullInfo () );

        progress.Value += VideoStore.PriceList.Count;
        progress.Refresh ();

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( VideoStore.Movies.FullInfo () );

        progress.Value += VideoStore.Movies.Count;
        progress.Refresh ();

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( VideoStore.MovieExemplars.FullInfo () );

        progress.Value += VideoStore.MovieExemplars.Count;
        progress.Refresh ();

        /////////////////////////////////////////////////////////////////////////////////

        progress.Quit ();

        string report = sb.ToString ();

        try 
        { 
            System.IO.File.WriteAllText( "VRO-dump.txt", report ); 
            sb.AppendLine( "Dump saved to 'VRO-dump.txt'" );
            report = sb.ToString ();
        } 
        catch {}

        ShowReport( "Video Rental Outlet - Database Text Dump", report );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Displays short information about the VRO database in report box.
    /// </summary>
    /// 
    private void DumpDatabaseShortInfo ()
    {
        StringBuilder sb = new StringBuilder ();

        sb.AppendLine ();
        sb.AppendLine( "VROLib Database Format v" + VideoStore.Version );
        sb.AppendLine ();

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( VideoStore.FullInfo () );

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( "Event Subscribers:" )
          .AppendLine ();

        sb.Append( "    Database .........: " )
          .Append( VideoStore.ChangedListenerCount )
          .AppendLine ();

        sb.Append( "    PriceList ........: " )
          .Append( VideoStore.PriceList.ChangedListenerCount )
          .AppendLine ();

        sb.Append( "    Customers ........: " )
          .Append( VideoStore.Customers.ChangedListenerCount )
          .AppendLine ();

        sb.Append( "    Movies ...........: " )
          .Append( VideoStore.Movies.ChangedListenerCount )
          .AppendLine ();

        sb.Append( "    MovieExemplars ...: " )
          .Append( VideoStore.MovieExemplars.ChangedListenerCount )
          .AppendLine ()
          .AppendLine ();

        /////////////////////////////////////////////////////////////////////////////////

        sb.AppendLine( "Loaded Assemblies:" );

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies (); 
        foreach ( var assembly in loadedAssemblies )
        {
            AssemblyName name = assembly.GetName ();

            sb.AppendLine ()
              .Append( name.Name )
              .AppendLine ();

            sb.Append( "    Version: " )
              .Append( name.Version )
              .AppendLine ();

            sb.Append( "    " )
              .Append( name.CodeBase )
              .AppendLine ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        string report = sb.ToString ();

        ShowReport( "Video Rental Outlet - Database Info", report );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Traces changes made to database (if tracing is enabled).
    /// </summary>
    /// 
    private void VideoStore_Changed( GenericObject item, ChangeType how, string reason )
    {
        Debug.TraceLine ();

        Debug.TraceLine( ">>>> {0} {1}{2}", how, item.ToString (),
            reason != null ? ", Reason: " + reason : "" );

        Debug.TraceLine ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}