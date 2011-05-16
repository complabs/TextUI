/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MainFormMdi.cs
 *  Created:    2011-04-26
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;

#if TEXTUI
    using TextUI;
#else
    using System.Windows.Forms;
#endif

using Mbk.Commons;

using VROLib.CustomerDatabase;
using VROLib.ItemStore;

/////////////////////////////////////////////////////////////////////////////////////////
// MDI handling methods (finding, opening, closing etc MDI forms derived from FormBase)

public partial class MainForm : Form
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Generic Methods ]

    // Note that in the following methods generic type T is always considered to
    // be derived from FormBase class.

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Finds the first visible MDI form of FormBase subtype T.
    /// </summary>
    /// 
    private T Find<T> () where T: FormBase
    {
        foreach( var w in MdiChildren )
        {
            if ( ! w.Visible || w.Tag == null ) continue;

            T form = w.Tag as T;

            if ( form != null )
            {
                return form;
            }
        }

        return null;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Checks whether form of FormBase subtype T exists matching a specified predicate.
    /// </summary>
    /// 
    private bool Exist<T>( Predicate<T> match ) where T: FormBase
    {
        foreach( var w in MdiChildren )
        {
            if ( ! w.Visible || w.Tag == null ) continue;

            T form = w.Tag as T;

            if ( form != null && match( form ) )
            {
                return true;
            }
        }

        return false;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens existing or creates new form of FormBase subtype T in specified
    /// mode, optionally executing some action before the form is shown.
    /// </summary>
    /// 
    internal T Open<T>( OpenMode mode = OpenMode.Browse, 
                       Action<T> actionBeforeOpen = null )
        where T: FormBase, new ()
    {
        T form = null;

        MdiClient.IfValidateOk( () =>
        {
            form = Find<T> ();

            // Reuse existing form i.e. don't allow multiple instances
            //
            if ( form == null )
            {
                form = new T ();
            }

            // User can set e.g. common filter to filter records in delegate
            //
            if ( form != null && actionBeforeOpen != null )
            {
                actionBeforeOpen( form );
            }

            // Open form finally
            //
            if ( form != null )
            {
                form.OpenForm( mode );
            }
        } );

        return form;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens new form of FormBase subtype T in AddNew mode.
    /// </summary>
    /// 
    private bool OpenAddNew<T> ()
        where T: FormBase, new ()
    {
        T form = null;

        MdiClient.IfValidateOk( () =>
        {
            form = Find<T> ();

            // Open form if exists
            //
            if ( form != null )
            {
                form.OpenForm( OpenMode.AddNew );
            }
        } );

        return form != null;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Executes a specified action for all forms of FormBase subtype T.
    /// </summary>
    /// 
    private void ExecuteFor<T>( Action<T> action ) where T: FormBase
    {
        if ( action == null )
        {
            return;
        }

        // Note that we must cache all forms because action may modify
        // MdiChildren list (e.g. delete some form from the list);
        //
        List<T> forms = new List<T> ();

        foreach( var w in MdiChildren )
        {
            if ( ! w.Visible || w.Tag == null ) continue;

            T form = w.Tag as T;

            if ( form != null )
            {
                forms.Add( form );
            }
        }

        foreach( T form in forms )
        {
            action( form );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Movie Exemplars ]

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens MovieExemplarList form for specified movie.
    /// </summary>
    /// 
    internal MovieExemplarListForm OpenMovieExemplarList( Movie movie,
        OpenMode mode )
    {
        return Open<MovieExemplarListForm>( mode, form =>
        {
            if ( form.Movie != movie )
            {
                form.Movie = movie;
                form.CommonFilter = exemplar => exemplar.Movie == movie;
            }
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens MovieExemplarList form for specified movie in AddNew mode.
    /// </summary>
    /// 
    internal MovieExemplarListForm OpenAddNewMovieExemplarList( Movie movie )
    {
        return Open<MovieExemplarListForm>( OpenMode.AddNew, form =>
        {
            if ( form.Movie != movie )
            {
                form.Movie = movie;
                form.CommonFilter = exemplar => exemplar.Movie == movie;
            }
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens MovieExemplarDetails form for specified movie in AddNew mode.
    /// </summary>
    /// 
    internal MovieExemplarDetailsForm OpenAddNewMovieExemplarDetails( Movie movie )
    {
        return Open<MovieExemplarDetailsForm>( OpenMode.AddNew, form => 
        {
            form.Record = null;
            form.MasterRecord = movie;
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens MovieExemplarList form with particular selection filter for exemplars.
    /// </summary>
    /// 
    internal MovieExemplarListForm OpenMovieExemplarList( OpenMode mode,
        Predicate<MovieExemplar> filter = null )
    {
        return Open<MovieExemplarListForm>( mode, form =>
        {
            if ( form.Movie != null || form.CommonFilter != filter )
            {
                form.Movie = null;
                form.CommonFilter = filter;
            }
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens/creates form to add new movie exemplar depending on the current
    /// active form with selected movie.
    /// </summary>
    /// 
    internal void AddNewMovieExemplar ()
    {
        MovieListForm movieList = Find<MovieListForm> ();
        MovieDetailsForm movieDetials = Find<MovieDetailsForm> ();
        MovieExemplarListForm exemplarList = Find<MovieExemplarListForm> ();

        if ( exemplarList != null && exemplarList.MdiForm == ActiveMdiChild )
        {
            OpenAddNew<MovieExemplarListForm> ();
        }
        else if ( movieDetials != null && movieDetials.Record != null 
            && movieDetials.MdiForm == ActiveMdiChild )
        {
            OpenAddNewMovieExemplarDetails( movieDetials.Record );
        }
        else if ( movieList != null && movieList.CurrentRecord != null
            && movieList.MdiForm == ActiveMdiChild )
        {
            OpenAddNewMovieExemplarDetails( movieList.CurrentRecord );
        }
        else if ( exemplarList != null 
            && ( exemplarList.Movie != null || exemplarList.CurrentRecord != null ) )
        {
            OpenAddNew<MovieExemplarListForm> ();
        }
        else if ( movieDetials != null && movieDetials.Record != null )
        {
            OpenAddNewMovieExemplarDetails( movieDetials.Record );
        }
        else if ( movieList != null && movieList.CurrentRecord != null )
        {
            OpenAddNewMovieExemplarDetails( movieList.CurrentRecord );
        }
        else // movies == null && exemplars == null
        {
            Open<MovieListForm>( OpenMode.Edit );

            MessageBox.Show( 
                "Choose a movie first!\n\n"
                    + "Then select Add New Exemplar for the movie...",
                "Add New Movie Exemplar",
                MessageBoxButtons.OK, MessageBoxIcon.Information
                );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Query filter to return all movie exemplars.
    /// </summary>
    /// 
    internal bool QueryAllExemplars( MovieExemplar exemplar )
    {
        return true;
    }

    #endregion 

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Customer Rentals ]

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens RentedItemsList form for a specified customer.
    /// </summary>
    /// 
    internal RentedItemListForm OpenRentedItemsList( Customer customer,
        OpenMode mode )
    {
        return Open<RentedItemListForm>( mode, form =>
        {
            if ( form.Customer != customer )
            {
                form.Customer = customer;
                form.CommonFilter = exemplar => exemplar.RentedTo == customer;
            }
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens RentedItemsList form for a specified customer in AddNew mode.
    /// </summary>
    /// 
    internal RentedItemListForm OpenAddNewRentedItemList( Customer customer )
    {
        return Open<RentedItemListForm>( OpenMode.AddNew, form =>
        {
            if ( form.Customer != customer )
            {
                form.Customer = customer;
                form.CommonFilter = exemplar => exemplar.RentedTo == customer;
            }
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens RentedItemDetailsForm for a specified customer in AddNew mode.
    /// </summary>
    /// 
    internal RentedItemDetailsForm OpenAddNewRentedItemDetails( Customer customer )
    {
        return Open<RentedItemDetailsForm>( OpenMode.AddNew, form => 
        {
            form.Record = null;
            form.MasterRecord = customer;
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Opens/creates form to add new rented item depending on the current
    /// active form with selected customer.
    /// </summary>
    /// 
    internal void AddNewRentedItem ()
    {
        CustomerListForm customerList = Find<CustomerListForm> ();
        CustomerDetailsForm customerDetails = Find<CustomerDetailsForm> ();
        RentedItemListForm rentalList = Find<RentedItemListForm> ();

        if ( rentalList != null && rentalList.MdiForm == ActiveMdiChild )
        {
            OpenAddNew<RentedItemListForm> ();
        }
        else if ( customerDetails != null && customerDetails.Record != null 
            && customerDetails.MdiForm == ActiveMdiChild )
        {
            OpenAddNewRentedItemDetails( customerDetails.Record );
        }
        else if ( customerList != null && customerList.CurrentRecord != null 
            && customerList.MdiForm == ActiveMdiChild )
        {
            OpenAddNewRentedItemDetails( customerList.CurrentRecord );
        }
        else if ( rentalList != null && rentalList.Customer != null )
        {
            OpenAddNew<RentedItemListForm> ();
        }
        else if ( customerDetails != null && customerDetails.Record != null )
        {
            OpenAddNewRentedItemDetails( customerDetails.Record );
        }
        else if ( customerList != null && customerList.CurrentRecord != null )
        {
            OpenAddNewRentedItemDetails( customerList.CurrentRecord );
        }
        else // customers == null && exemplars == null
        {
            Open<CustomerListForm>( OpenMode.Edit );

            MessageBox.Show( 
                "Choose a customer first!\n\n" 
                    + "Then select Rent New Item for the customer...",
                "Rent Item to Customer",
                MessageBoxButtons.OK, MessageBoxIcon.Information
                );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}