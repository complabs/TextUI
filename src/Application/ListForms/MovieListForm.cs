/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MovieListForm.cs
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

using VROLib.ItemStore;

/// <summary>
/// Represents the list form displaying Video Rental Outlet's Movies Database.
/// </summary>
/// 
internal sealed class MovieListForm 
    : ListForm<MovieCollection,Movie>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the MovieListForm class.
    /// </summary>
    /// 
    public MovieListForm () 
        : base( "Movies", "Movie", 80, 27 )
    {
        Columns.Add( new TextField( "ID",        5, ""   ) );
        Columns.Add( new TextField( "Title",   -30, "  " ) );
        Columns.Add( new TextField( "Ex",        2, " "  ) );
        Columns.Add( new TextField( "Year",      4, "  " ) );
        Columns.Add( new TextField( "Length",    7, " "  ) );
        Columns.Add( new TextField( "Director",  0, "  " ) );

        /////////////////////////////////////////////////////////////////////////////////
        
        IntializeContextMenu ();
        InitializeToolStrip ();
        InitializeComponents ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Subscribes to VideoStore.Movies Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.Movies, FormatRow );
    }

    /// <summary>
    /// Gets the values to be displayed in columns for a specified Movie.
    /// </summary>
    /// 
    private string[] FormatRow( Movie movie )
    {
        return new string[]
        {
            movie.ID.ToString (), 
            movie.Title,
            movie.MovieExemplars.Count.ToString (),
            movie.VerboseFirstRelease,
            movie.VerboseDuration,
            movie.Directors.Count > 0 ? movie.Directors[ 0 ] : string.Empty
        };
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the MovieDetailsForm class.
    /// </summary>
    ///
    public override DetailsForm<MovieCollection,Movie> CreateDetailsForm ()
    {
        return new MovieDetailsForm ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    protected override void OpenSubItemsForm( Movie record, OpenMode mode )
    {
        MainForm.OpenMovieExemplarList( record, OpenMode.Edit );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns true if a specified Movie can be removed.
    /// </summary>
    /// 
    public override bool CanRemoveRecord( Movie movie )
    {
        string info = "Remove movie #" + movie.ID.ToString () + "?";

        info += "\n\nMovie Title: " + movie.FullTitle;

        bool warning = movie.MovieExemplars.Count > 0;

        if ( warning )
        {
            info += "\n\nRemoving the movie will also remove all its exemplars!"
                  + "\n\nNumber of exemplars: " + movie.MovieExemplars.Count;
            
            if ( movie.MovieExemplars.RentedCount > 0 )
            {
                info += "\nNumber of rented exemplars: " 
                        + movie.MovieExemplars.RentedCount + "\n"; 
            }
        }
        
        DialogResult rc = MessageBox.Show
        (
            info, 
            Em.IsGUI ? "Video Rental Outlet: Remove Movie" : null,
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
    /// Additonal items are: Manage Exemplars and Add New Exemplar.
    /// </remarks>
    /// 
    private void IntializeContextMenu ()
    {
#if ! TEXTUI
        if ( ListView == null || ListView.ContextMenuStrip == null )
        {
            return;
        }

        ContextMenuStrip contextMenu = ListView.ContextMenuStrip;

        //---------------------------------------------------------------------------
        ToolStripMenuItem miSubitems = new ToolStripMenuItem()
        {
            Text = "Manage E&xemplars",
            Image = Resources.Movies16
        };

        ToolStripMenuItem miAddSubitem = new ToolStripMenuItem()
        {
            Text = "Add Ne&w Exemplar...",
            Image = Resources.AddRental16
        };

        //---------------------------------------------------------------------------
        miSubitems.Click += ( sender, e ) => OpenSubItemsForm( OpenMode.Edit );
        miAddSubitem.Click += ( sender,e ) => MainForm.AddNewMovieExemplar ();

        contextMenu.Opening += delegate
        {
            miSubitems.Enabled = CurrentRecord != null;
        };

        //---------------------------------------------------------------------------
        int pos = contextMenu.Items.Count - 1;
        contextMenu.Items.Insert( pos, miSubitems );
        contextMenu.Items.Insert( pos+1, miAddSubitem );
        contextMenu.Items.Insert( pos+2, new ToolStripSeparator () );
#endif
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Appends the tool strip with additional menu items.
    /// </summary>
    /// <remarks>
    /// Additonal items are: Exemplars and New Exemplar.
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

        //---------------------------------------------------------------------------
        ToolStripMenuItem miSubitems = new ToolStripMenuItem()
        {
            Text = "E&xemplars",
            Image = Resources.Movies16, 
            ToolTipText = "Manage Movie Exemplars...",
            Enabled = false
        };

        ToolStripMenuItem miAddSubitem = new ToolStripMenuItem()
        {
            Text = "Ne&w Exemplar",
            Image = Resources.AddRental16,
            ToolTipText = "Add new exemplar to the Movie...",
            Enabled = false
        };

        //---------------------------------------------------------------------------
        miSubitems.Click += ( sender, e ) => OpenSubItemsForm( OpenMode.Edit );
        miAddSubitem.Click += ( sender,e ) => MainForm.AddNewMovieExemplar ();

        ListView.ItemSelectionChanged += ( sender, e ) => OnUiStateChanged ();

        //---------------------------------------------------------------------------
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