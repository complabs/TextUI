/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MovieExemplarDetailsForm.cs
 *  Created:    2011-04-08
 *  Modified:   2011-05-04
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
using VROLib.ItemStore;

/// <summary>
/// Represents the form displaying Movie Exemplar details.
/// </summary>
/// 
internal sealed class MovieExemplarDetailsForm 
    : DetailsForm<MovieExemplarCollection,MovieExemplar>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the Movie to which movie exemplars listed in this form belongs to.
    /// (May be null, meaning "all movies in database".)
    /// </summary>
    /// 
    public Movie Movie { get; private set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ UI Components ]

    private MyLabel    movieInfo;

    private MyComboBox media;
    private MyComboBox priceClass;
    private MyTextBox  isan;
    private MyTextBox  released;
    private MyTextBox  subtitles;
    private MyTextBox  imageFormat;
    private MyTextBox  features;
    private MyLabel    rentalInfo;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Creates a new instance of the MovieExemplarDetailsForm class.
    /// </summary>
    /// 
    public MovieExemplarDetailsForm ()
        : base( "Movie Exemplar", 80, 23 )
    {
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

        /////////////////////////////////////////////////////////////////////////////////

        Button_Link.Text = "&Movie";

        LinkClick += delegate
        {
            if ( this.Movie != null )
            {
                MainForm.Open<MovieDetailsForm>( 
                    ReadOnly ? OpenMode.Browse : OpenMode.Edit,
                    form => form.Record = this.Movie );
            }
        };

        Button_Info.Text = "&Rental";

        InfoClick += new EventHandler( EH_InfoClick );
    }

    /// <summary>
    /// Subscribes to VideoStore.MovieExemplars Changed events.
    /// </summary>
    ///
    protected override void ConnectToCollection ()
    {
        ConnectTo( MainForm.VideoStore.MovieExemplars );
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
        MovieExemplar exemplar = new MovieExemplar( Record );

        Button_Info.Enabled = ! IsAddNew && exemplar.IsRented;

        this.Movie = exemplar.Movie;

        this.movieInfo.Text = Movie != null ? exemplar.Movie.ToString()
            : "Select a movie to which this exemplar belongs first!";

        if ( this.Movie == null && MasterRecord != null )
        {
            Movie masterMovie = MasterRecord as Movie;
            if ( masterMovie != null )
            {
                this.Movie = masterMovie;
                this.movieInfo.Text = masterMovie.ToString ();
            }
        }

        this.media.SelectItem( exemplar.Media );
        this.priceClass.SelectItem( exemplar.PriceClass );

        this.isan.InitText        = exemplar.ISAN;
        this.subtitles.InitText   = exemplar.Subtitles;
        this.imageFormat.InitText = exemplar.ImageFormat;
        this.features.InitText    = exemplar.Features;

        this.released.InitText = exemplar.Released == DateTime.MinValue ? null
            : exemplar.Released.ToString( "yyyy-MM-dd" );

        if ( exemplar.IsRented )
        {
            this.rentalInfo.Text = "Rented to " 
                + exemplar.RentedAsItem.RentedTo.ToString ();
        }
        else
        {
            this.rentalInfo.Text = "Status: Available";
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
        if ( this.Movie == null )
        {
            return false;
        }

        return this.media      .ContentsChanged
            || this.priceClass .ContentsChanged
            || this.isan       .ContentsChanged
            || this.released   .ContentsChanged
            || this.subtitles  .ContentsChanged
            || this.imageFormat.ContentsChanged
            || this.features   .ContentsChanged;
    }

    /// <summary>
    /// Saves contents of the UI fields into the <see cref="Record"/>.
    /// </summary>
    /// 
    protected override void OnSaveData ()
    {
        if ( IsAddNew && this.Movie == null )
        {
            throw new Exception( 
                "You must select a movie to which this exemplar will be added." );
        }

        MovieExemplar exemplar = new MovieExemplar( Record )
        {
            ISAN         = this.isan       .TrimmedText,
            Subtitles    = this.subtitles  .TrimmedText,
            ImageFormat  = this.imageFormat.TrimmedText,
            Features     = this.features   .TrimmedText,
        };

        /////////////////////////////////////////////////////////////////////////////////
        // Parse media and price class

        Media? media = this.media.Current.Tag as Media?;
        if ( ! media.HasValue )
        {
            throw new Exception( "Media must not be null." );
        }

        exemplar.Media = media.Value;

        PriceClass? priceClass = this.priceClass.Current.Tag as PriceClass?;
        if ( ! priceClass.HasValue )
        {
            throw new Exception( "Price class must not be null." );
        }

        exemplar.PriceClass = priceClass.Value;

        /////////////////////////////////////////////////////////////////////////////////
        // Parse First Release

        try
        {
            string value = this.released.TrimmedText;

            if ( ! string.IsNullOrEmpty( value ) )
            {
                exemplar.Released = DateTime.ParseExact( value, "yyyy-M-d",
                    System.Globalization.CultureInfo.InvariantCulture );
            }
            else
            {
                exemplar.Released = DateTime.MinValue;
            }
        }
        catch( Exception )
        {
            throw new ArgumentException( "Released date must be in valid date format" );
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
                Record = exemplar;
                exemplar.AddTo( this.Movie );
            }
            catch // in case of error, rethrow exception and clear reference to new rec
            {
                Record = null;
                throw;
            }
        }
        else
        {
            exemplar.Update ();
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

        float[] col = { 3, 18, 47, 65, 79 };
        float[] row = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };

        float maxLen = 26; // Maximum text length

        /////////////////////////////////////////////////////////////////////////////////
        // Static text

        int r = 1, c = 0;

        NewStaticText( col[c], row[r++], "Media:"        );
        NewStaticText( col[c], row[r++], "Price Class:"  );
        NewStaticText( col[c], row[r++], "ISAN:"         );
        NewStaticText( col[c], row[r++], "Released:"     );
        NewStaticText( col[c], row[r++], "Subtitles:"    );
        NewStaticText( col[c], row[r++], "Image Format:" );

        r = 1; c = 2;

        NewStaticText( col[c], row[r++] + ( Em.IsGUI ? 0.4f : 0 ), "Features:" );

        /////////////////////////////////////////////////////////////////////////////////
        // Movie info and rental labels

        this.movieInfo = NewLabel( col[0], row[0] - ( Em.IsGUI ? 0.2f : 1f ),
            (float)MdiForm.Width / Em.Width - ( col[0] * 2 + 2 ) 
            );

        this.rentalInfo = NewLabel( col[0], row[7] + ( Em.IsGUI ? 0.4f : 1f ),
            (float)MdiForm.Width / Em.Width - ( col[0] * 2 + 2 )
            );
        this.rentalInfo.Height = Em.Height;

        /////////////////////////////////////////////////////////////////////////////////
        // TextBox fields

        r = 1; c = 1;

        this.media       = NewEnumField( col[c], row[r++], maxLen, typeof( Media ) );
        this.priceClass  = NewEnumField( col[c], row[r++], maxLen, typeof( PriceClass ) );
        this.isan        = NewTextField( col[c], row[r++], maxLen );
        this.released    = NewTextField( col[c], row[r++], maxLen );
        this.subtitles   = NewTextField( col[c], row[r++], maxLen );
        this.imageFormat = NewTextField( col[c], row[r++], maxLen );

        r = 2; c = 2;

        this.features = NewTextField( col[c], row[r++], 30 );
        this.features.Multiline = true;
        this.features.AutoScrollBar = true;
        this.features.Height = 9 * Em.Height + ( Em.IsGUI ? 2 : 0 );

        /////////////////////////////////////////////////////////////////////////////////
        // Field validation event handlers

        this.released.Validating += ( sender, e ) =>
        {
            MdiForm.ErrorMessage = null;

            if ( ReadOnly || ! this.released.ContentsChanged )
            {
                return;
            }

            ValidateDate( "Release date (if specified)", this.released.Text, "yyyy-M-d", 
                " in ISO format (yyyy-mm-dd).", e );
        };

        /////////////////////////////////////////////////////////////////////////////////
        // Movie info GUI/TUI specific setup
        //
#if TEXTUI
        MdiForm.GotFocus += ( sender, e ) =>
        {
            this.movieInfo.ForeColorInact = Color.White;
            this.movieInfo.ForeColor = Color.White;

            this.rentalInfo.ForeColorInact = Color.Magenta;
            this.rentalInfo.ForeColor = Color.Magenta;
        };
#else
        this.movieInfo.Font = MainForm.HeadingFont;
        this.movieInfo.Height = Em.Height * 15/10;
        this.movieInfo.ForeColor = Color.DarkBlue;
        this.movieInfo.AutoSize = false;
        this.movieInfo.AutoEllipsis = true;

        this.rentalInfo.ForeColor = Color.Blue;

        this.MdiForm.FontChanged += ( sender, e ) =>
        {
            this.movieInfo.Font = MainForm.HeadingFont;
            this.movieInfo.Height = Em.Height * 15/10;
        };
#endif
        /////////////////////////////////////////////////////////////////////////////////

        base.InitializeComponents ();
    }

    /// <summary>
    /// Sets the ReadOnly property of the form.
    /// </summary>
    /// <remarks>
    /// Configures whether locker-button is visible.
    /// </remarks>
    /// 
    protected override void SetReadOnly( bool readOnly )
    {
        readOnly = readOnly || this.Movie == null;

        /////////////////////////////////////////////////////////////////////////////////

        base.SetReadOnly( readOnly );

        /////////////////////////////////////////////////////////////////////////////////

        FirstField = this.media;

        if ( MdiForm.ActiveChild == null || ! MdiForm.ActiveChild.TabStop )
        {
            FirstField.Focus ();
        }

        if ( this.Movie == null && IsAddNew )
        {
            Button_Locker.TabStop = false;
            Button_Locker.Enabled = false;
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Event Handlers ]

    /// <summary>
    /// Handles info-button Click event.
    /// </summary>
    /// <remarks>
    /// Opens RentedItemDetailsForm for the current record.
    /// </remarks>
    /// 
    private void EH_InfoClick ( object sender, EventArgs e )
    {
        if ( this.Record != null && this.Record.RentedAsItem != null )
        {
            MainForm.Open<RentedItemDetailsForm>( OpenMode.Edit,
                form => 
                {
                    form.MasterRecord = this.Record.RentedAsItem.RentedTo;
                    form.Record = this.Record.RentedAsItem;

                    form.DeleteClick += delegate
                    {
                        try
                        {
                            MainForm.VideoStore.ReturnItem( form.Record.Exemplar );
                        }
                        catch {}
                    };
                } );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

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