/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MovieExemplarListForm.cs
 *  Created:    2011-03-29
 *  Modified:   2011-04-30
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
using VROLib.ItemStore;

/// <summary>
/// Represents the list form displaying Video Rental Outlet's Movie Exemplar Collection.
/// </summary>
/// 
internal sealed class MovieExemplarListForm 
    : ListForm<MovieExemplarCollection,MovieExemplar>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the Movie to which movie exemplars listed in this form belongs to.
    /// (May be null, meaning "all movies in database".)
    /// </summary>
    /// 
    public Movie Movie { get; set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the MovieExemplarListForm class.
    /// </summary>
    /// 
    public MovieExemplarListForm ()
        : base( "Movie Exemplars", "Movie Exemplar", 82, 28 )
    {
        Movie = null;

        Columns.Add( new TextField( "ID",            5, ""   ) );
        Columns.Add( new TextField( "Media",        -6, "  " ) );
        Columns.Add( new TextField( "Rel.",          4, " "  ) );
        Columns.Add( new TextField( "N\u00BA",       3, ""  ) );
        Columns.Add( new TextField( "Movie Title", -42, "  " ) );
        Columns.Add( new TextField( "Due Date",      0, "  " ) );

        InitializeComponents ();

        /////////////////////////////////////////////////////////////////////////////////

        // Subscribe to the Changed event for Movies collection
        //
        MainForm.VideoStore.Movies.Changed += Movies_Changed;

        MdiForm.FormClosed += delegate
        {
            // Unsubscribe the Changed event for Movies collection
            //
            MainForm.VideoStore.Movies.Changed -= Movies_Changed;
        };
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Subscribes to VideoStore.MovieExemplars Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.MovieExemplars, FormatRow );
    }

    /// <summary>
    /// Gets the values to be displayed in columns for a specified MovieExemplar.
    /// </summary>
    /// 
    private string[] FormatRow( MovieExemplar exemplar )
    {
        return new string[]
        {
            exemplar.ID.ToString (), 
            VerboseMedia[ (int)exemplar.Media ],
            exemplar.VerboseReleased,
            exemplar.CopyNumber.ToString (),
            exemplar.Movie.FullTitle,
            exemplar.VerboseDueDate
        };
    }

    // This is much faster than converting enum to string using ToString (according 
    // to profiling analysis, enum ToString() uses reflection which is very slow). 
    // However, this trick requires that enum starts at 0 and is wihtout any gap.
    //
    private string[] VerboseMedia = Enum.GetNames( typeof( Media ) );

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates a new instance of the MovieExemplarDetailsForm class.
    /// </summary>
    ///
    public override DetailsForm<MovieExemplarCollection,MovieExemplar> CreateDetailsForm ()
    {
        return new MovieExemplarDetailsForm ();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    protected override void OnBeforeOpenDetailsForm(
        DetailsForm<MovieExemplarCollection,MovieExemplar> form )
    {
        // Setup a movie of the current movie exemplar as preselected master record
        // for details form, when adding new movie exemplars.
        //
        if ( form != null && form.Record == null )
        {
            MovieExemplar current = CurrentRecord;

            if ( current != null )
            {
                form.MasterRecord = current.Movie;
            }
            else
            {
                form.MasterRecord = Movie;
            }
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
            if ( Movie == null )
            {
                return base.FormTitle;
            }

            return ReadOnly 
                ? "Exemplars of the Movie #" + Movie.ID + ": " + Movie.FullTitle
                : "Manage Exemplars of the Movie #" + Movie.ID + ": " + Movie.FullTitle;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns true if a specified MovieExemplar can be removed.
    /// </summary>
    /// 
    public override bool CanRemoveRecord( MovieExemplar exemplar )
    {
        // Ask for removal confirmation; build a question first...
        //
        StringBuilder info = new StringBuilder ();

        info.Append( "Remove movie exemplar #" ).Append( exemplar.ID.ToString () )
            .Append( "?" );

        if ( exemplar.RentedAsItem != null )
        {
            info.Append( "\n\nRented to " );
            info.Append( exemplar.RentedAsItem.RentedTo.ToString () );
        }

        info.Append( "\n\nMovie: " ).Append( exemplar.Movie.FullTitle )
            .Append( "\n\nCopy #" )
            .Append( exemplar.Movie.MovieExemplars.IndexOf( exemplar ) + 1 )
            .Append( " on " ).Append( exemplar.Media.Verbose () );

        if ( exemplar.Released != DateTime.MinValue )
        {
            info.Append( " released " ).Append( exemplar.VerboseReleased );
        }

        // Ask user to confirm removal, especially if exemplar is rented out
        // in which case the the question will be highlighted.
        //
        DialogResult rc = MessageBox.Show
        (
            info.ToString (), 
            Em.IsGUI ? "Video Rental Outlet: Remove Exemplar" : null,
            MessageBoxButtons.YesNo, 
            exemplar.IsRented ? MessageBoxIcon.Error : MessageBoxIcon.Warning 
            );

        return rc == DialogResult.Yes;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Event Handlers ]

    /// <summary>
    /// Monitors video store's movies collection. If the movie is deleted
    /// from collection, the form is closed.
    /// </summary>
    /// 
    private void Movies_Changed( GenericObject item, ChangeType how, string reason )
    {
        Debug.TraceLine( "{0} >>>> {1} {2}{3}", TraceID, how, item.GetType().Name,
            reason != null ? ", Reason: " + reason : "" );

        Movie record = item as Movie;

        if ( record != null && this.Movie == record )
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