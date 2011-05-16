/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MovieDetailsForm.cs
 *  Created:    2011-04-06
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Windows.Forms;
#endif

using Mbk.Commons;

using VROLib.ItemStore;

/// <summary>
/// Represents the form displaying Movie details.
/// </summary>
/// 
internal sealed class MovieDetailsForm 
    : DetailsForm<MovieCollection,Movie>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Components ]

    private MyTextBox movieTitle;
    private MyTextBox release;
    private MyTextBox duration;
    private MyTextBox country;

    private MyTextBox language;
    private MyTextBox imdb;
    private MyTextBox pgRate;

    private MyTextBox directors;
    private MyTextBox writers;
    private MyTextBox actors;

    private MyLabel labelTitle;
    private MyLabel labelDirectors;
    private MyLabel labelWriters;
    private MyLabel labelActors;

    private MyCheckBoxCollection genre;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the MovieDetailsForm class.
    /// </summary>
    /// 
    public MovieDetailsForm ()
        : base( "Movie", 80, 29 )
    {
        InitializeComponents ();

        /////////////////////////////////////////////////////////////////////////////////

        Button_Link.Text = "&Exemplars";

        LinkClick += delegate
        {
            if ( Record != null )
            {
                MainForm.OpenMovieExemplarList( Record, OpenMode.Edit );
            }
        };
    }

    /// <summary>
    /// Subscribes to VideoStore.Movies Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.Movies );
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
        Movie movie = new Movie( Record );

        this.movieTitle.InitText = movie.Title ;
        this.country   .InitText = movie.Country   ;
        this.language  .InitText = movie.Language  ;
        this.imdb      .InitText = movie.IMDB      ;
        this.pgRate    .InitText = movie.PgRate    ;

        this.release.InitText = movie.FirstRelease == DateTime.MinValue ? null
            : movie.FirstRelease.ToString( "yyyy-MM-dd" );

        this.duration.InitText = movie.Duration == TimeSpan.MinValue ? null
            : movie.Duration.TotalMinutes.ToString ();

        this.directors .SetText( movie.Directors );
        this.writers   .SetText( movie.Writers   );

        this.actors.InitText = movie.VerboseActorsList;

        this.genre.SetValue( movie.Genre );

        base.OnLoadData ();
    }

    /// <summary>
    /// Returns a value indicating whether contents of the UI fields has been changed
    /// since they were loaded in <see cref="OnLoadData"/>.
    /// </summary>
    ///
    public override bool IsDirty ()
    {
        return this.movieTitle.ContentsChanged
            || this.release   .ContentsChanged
            || this.duration  .ContentsChanged
            || this.country   .ContentsChanged
            || this.language  .ContentsChanged
            || this.imdb      .ContentsChanged
            || this.pgRate    .ContentsChanged
            || this.directors .ContentsChanged
            || this.writers   .ContentsChanged
            || this.actors    .ContentsChanged
            || this.genre     .ContentsChanged;
    }

    /// <summary>
    /// Saves contents of the UI fields into the <see cref="Record"/>.
    /// </summary>
    /// 
    protected override void OnSaveData ()
    {
        Movie movie = new Movie( Record )
        {
            Title        = this.movieTitle .TrimmedText,
            Country      = this.country    .TrimmedText,
            Language     = this.language   .TrimmedText,
            IMDB         = this.imdb       .TrimmedText,
            PgRate       = this.pgRate     .TrimmedText,
            Directors    = this.directors  .Lines.TrimmedNameCollection (),
            Writers      = this.writers    .Lines.TrimmedNameCollection (),
            Genre        = (Genre)this.genre.GetValue ()
        };

        /////////////////////////////////////////////////////////////////////////////////
        // Parse First Release

        try
        {
            string value = this.release.TrimmedText;

            if ( ! string.IsNullOrEmpty( value ) )
            {
                movie.FirstRelease = DateTime.ParseExact( value, "yyyy-M-d",
                    System.Globalization.CultureInfo.InvariantCulture );
            }
            else
            {
                movie.FirstRelease = DateTime.MinValue;
            }
        }
        catch( Exception )
        {
            throw new ArgumentException( "First release must be a valid date" );
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Parse Duration

        try
        {
            string value = this.duration.TrimmedText;

            if ( ! string.IsNullOrEmpty( value ) )
            {
                movie.Duration = new TimeSpan( 0, int.Parse( value ), 0 );
            }
            else
            {
                movie.Duration = TimeSpan.MinValue;
            }
        }
        catch( Exception )
        {
            throw new ArgumentException( "Duration (in minutes) must be an integer" );
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Parse actor

        movie.Actors = new List<MovieActor> ();

        List<string> compoundNames = this.actors.Lines.TrimmedNameCollection ();
        foreach ( string compoundName in compoundNames )
        {
            movie.Actors.Add( new MovieActor( compoundName ) );
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
                Record = movie;
                movie.AddTo( MainForm.VideoStore );
            }
            catch // in case of error, rethrow exception and clear reference to new rec
            {
                Record = null;
                throw;
            }
        }
        else
        {
            movie.Update ();
        }

        base.OnSaveData ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Overriden Base Methods ]

    /// <summary>
    /// Initializes UI components of the MdiForm.
    /// </summary>
    /// 
    protected override void InitializeComponents ()
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Table layout grid definition, with rows and columns in Em units.

        float[] col = { 3, 18, 47, 63, 79 };
        float[] row = { 2, 4, 6, 8, 10, 12, 14, 17, 19, 21, 22 };

        float maxLen = 24; // Maximum text length

        /////////////////////////////////////////////////////////////////////////////////
        // Static text

        int r = 0, c = 0;

        NewStaticText( col[c] - 2, row[r++], "*" );

        NewStaticText( col[c], row[r++], "First Release:" );
        NewStaticText( col[c], row[r++], "Duration:" );
        NewStaticText( col[c], row[r++], "Country:"       );
        NewStaticText( col[c], row[r++], "Language:"      );
        NewStaticText( col[c], row[r++], "PG-Rate:"       );
        NewStaticText( col[c], row[r++], "IMDB:"          );

        NewStaticText( col[c] + 21, row[2], "(min)" );

        /////////////////////////////////////////////////////////////////////////////////
        // TextBox fields

        r = 0; c = 1;

        this.labelTitle = NewLabel( col[0], row[0], 10 );
        this.labelTitle.UseMnemonic = true;
        this.labelTitle.Text = "&Title:";

        // Always move focus from title label to movie title
        //
        this.labelTitle.GotFocus += ( sender, e ) => this.movieTitle.Focus ();

        this.movieTitle = NewTextField( col[c], row[r++], maxLen );
        this.release    = NewTextField( col[c], row[r++], maxLen );
        this.duration   = NewTextField( col[c], row[r++], 5 );
        this.country    = NewTextField( col[c], row[r++], maxLen );
        this.language   = NewTextField( col[c], row[r++], maxLen );
        this.pgRate     = NewTextField( col[c], row[r++], maxLen );
        this.imdb       = NewTextField( col[c], row[r++], maxLen );

        /////////////////////////////////////////////////////////////////////////////////
        // ChecBoxes for Genre

        MyGroupBox genreBox = NewGroupBox( "&Genre", 
            col[2] - 3, row[0] - ( Em.IsGUI ? 0.5f : 0f ), 34, 14 );

        this.genre = new MyCheckBoxCollection( typeof( Genre ) );

        r = 0; c = 0;
        foreach( CheckBox cb in this.genre )
        {
            cb.Parent = genreBox;
            cb.TabIndex = NextTabIndex;
            cb.Left   = (int)( ( 2f + c * 16f ) * Em.Width );
            cb.Top    = (int)( Em.IsGUI ? 1.4f * Em.Height : 2f * Em.Height );
            #if TEXTUI
                cb.Height = Em.Height;
                cb.Width  = 15 * Em.Width;
            #else
                cb.AutoSize = true;
            #endif
            cb.Top += ( r++ ) * ( Em.IsGUI ? Em.Height + 3 : Em.Height );
            if ( r >= 10 )
            {
                r = 0; if ( ++c > 3 ) break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Multiline TextBox: Directors

        r = 7; c = 1;

        if ( Em.IsGUI ) { row[8] -= 0.4f; }

        this.labelDirectors = NewLabel( col[0], row[r], 18 );
        this.labelDirectors.UseMnemonic = true;
        this.labelDirectors.Text = "Di&rectors";
        this.labelDirectors.Height = Em.Height;

        this.directors = NewTextField( col[0], row[r+1], 18 );
        this.directors.Multiline = true;
        this.directors.Height = 5 * Em.Height + ( Em.IsGUI ? 4 : 0 );
        this.directors.AutoScrollBar = true;

        // Always move focus from directors label to directors field
        //
        this.labelDirectors.GotFocus += ( source, e ) => this.directors.Focus ();

        /////////////////////////////////////////////////////////////////////////////////
        // Multiline TextBox: Writers

        this.labelWriters = NewLabel( col[0] + 20, row[r], 18 );
        this.labelWriters.UseMnemonic = true;
        this.labelWriters.Text = "&Writers";
        this.labelWriters.Height = Em.Height;

        this.writers = NewTextField( col[0] + 20, row[r+1], 18 );
        this.writers.Multiline = true;
        this.writers.Height = 5 * Em.Height + ( Em.IsGUI ? 4 : 0 );
        this.writers.AutoScrollBar = true;

        // Always move focus from writers label to writers field
        //
        this.labelWriters.GotFocus += ( source, e ) => this.writers.Focus ();

        /////////////////////////////////////////////////////////////////////////////////
        // Multiline TextBox: Actors

        this.labelActors = NewLabel( col[0] + 40, row[r], 18 );
        this.labelActors.UseMnemonic = true;
        this.labelActors.Text = "&Actors";
        this.labelActors.Height = Em.Height;

        this.actors = NewTextField( col[0] + 40, row[r+1], 34 );
        this.actors.Multiline = true;
        this.actors.Height = 5 * Em.Height + ( Em.IsGUI ? 4 : 0 );
        this.actors.AutoScrollBar = true;

        // Always move focus from actors label to actors field
        //
        this.labelActors.GotFocus += ( source, e ) => this.actors.Focus ();

        /////////////////////////////////////////////////////////////////////////////////
        // Field validation event handlers

        this.movieTitle.Validating  += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.movieTitle.ContentsChanged )
            {
                return;
            }

            ValidateNotNull( "Movie title", this.movieTitle.Text, e );
        };

        this.release.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.release.ContentsChanged )
            {
                return;
            }

            ValidateDate( "First release (if specified)", this.release.Text, "yyyy-M-d", 
                " in ISO format (yyyy-mm-dd).", e );
        };

        this.duration.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.duration.ContentsChanged )
            {
                return;
            }

            string fieldName = "Duration in minutes, if specified,";

            int fieldValue = 1; // default value to satisfy validation if null

            ValidateInteger( fieldName, this.duration.Text, e, ref fieldValue );

            // Duration must be either null or an integer >= 1
            //
            if ( ! e.Cancel && fieldValue < 1 )
            {
                MdiForm.ErrorMessage = fieldName + " must be greater than zero.";
                MdiForm.Beep ();
                e.Cancel = true;
            }
            else if ( ! e.Cancel && fieldValue > 24 * 60 )
            {
                MdiForm.ErrorMessage = fieldName + " must be less than 24 hours.";
                MdiForm.Beep ();
                e.Cancel = true;
            }
        };

        /////////////////////////////////////////////////////////////////////////////////

        base.InitializeComponents ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}