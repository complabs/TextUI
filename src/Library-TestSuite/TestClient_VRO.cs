/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  VRO Test Suite Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VRO_TestSuite
 *  File:       TestClient_VRO.cs
 *  Created:    2011-04-07
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.IO;

using Mbk.Commons;

using VROLib;
using VROLib.CustomerDatabase;
using VROLib.ItemStore;

namespace VRO_TestSuite
{
    /// <summary>
    /// VideoRentalOutlet Library test suite.
    /// </summary>
    /// <see cref="CreateSampleDatabase"/>
    /// <see cref="AddTestRecords"/>
    /// 
    public class TestClient_VRO
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Instance of the VideoRentalOutlet class that is used for tests.
        /// It might be either internally created (<see cref="CreateSampleDatabase"/>
        /// or <see cref="LoadFromFile"/>) or provided by the user and later used
        /// (<see cref="AddTestRecords"/>).
        /// </summary>
        /// 
        public VideoRentalOutlet VideoStore { get; set; }

        /// <summary>
        /// Test results are written to this instance of ITestClientWriter interface,
        /// VROLib contains two classes that implement ITestclientWriter interface, 
        /// which user migh use: <seealso cref="ConsoleWriter"/> and <seealso cref=
        /// "StringWriter"/>.
        /// </summary>
        /// 
        public ITestClientWriter Out { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handlers ]

        /// <summary>
        /// Creates an instance of the test suite with specified output interface,
        /// where the results will be written to.
        /// </summary>
        ///
        public TestClient_VRO( ITestClientWriter outputInterface )
        {
            Out = outputInterface;
            VideoStore = null;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Event handler hooked to VideoStore Changed event that displays track of
        /// all changes made to database.
        /// </summary>
        /// 
        void VideoStore_Changed( GenericObject item, ChangeType how, string reason )
        {
            Out.WriteLine( string.Format( ">>> {0} {1}", how, item ) );
            Out.WriteLine ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Test Suite ]

        /// <summary>
        /// Creates a sample VRO database and runs various tests according ID132V
        /// Lab3a specification.
        /// </summary>
        /// <remarks>
        /// Final contents of the sample database:
        /// <pre>
        ///    3 customers
        ///    3 price specifications
        ///    4 movies
        ///    9 movie exemplars
        ///    3 rented items
        /// </pre>
        /// </remarks>
        /// 
        public void CreateSampleDatabase ()
        {
            /////////////////////////////////////////////////////////////////////////////
            // Create default instance of VideoRentalOutlet class and hook
            // our event handler that will trace changes.

            VideoStore = new VideoRentalOutlet( "Blurry Bluray", "SE554078214101" )
            {
                Address  = "Tulegatan 50",
                PostCode = "112 54",
                City     = "Solna",
                Country  = "Sweden",
                Phone    = "+46(8)90510",
                EMail    = "info@blurrybluray.com",
                HomePage = "http://www.blurrybluray.com"
            };

            VideoStore.Changed += VideoStore_Changed;

            Out.Begin ();

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Dumping contents of initial empty store..." );

            Out.WriteLine( VideoStore.FullInfo () );
            Out.WriteLine( VideoStore.PriceList.FullInfo () );
            Out.WriteLine( VideoStore.Customers.FullInfo () );
            Out.WriteLine( VideoStore.MovieExemplars.FullInfo () );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Changing price list..." );

            VideoStore.PriceList.UpdatePrice( Membership.Member, 
                PriceClass.OlderMovie, 1, 10m );

            VideoStore.PriceList.UpdatePrice( Membership.GoldMember, 
                PriceClass.OlderMovie, 1, 5m );

            VideoStore.PriceList.UpdatePrice( Membership.GoldMember, 
                PriceClass.NewHotMovie, 1, 10m );

            VideoStore.PriceList.UpdatePrice( Membership.NotMember, 
                PriceClass.OlderMovie, 3, 15m );

            VideoStore.PriceList.UpdatePrice( Membership.NotMember, 
                PriceClass.SwedishNew, 1, 20m );

            Out.WriteLine( VideoStore.PriceList.FullInfo () );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Getting fees for misc Membership/PriceClass combinations..." );

            Enum[] membershipsDesc = 
            { 
                Membership.GoldMember, 
                Membership.Member, 
                Membership.NotMember
            };

            Enum[] priceClassesAsc = 
            { 
                PriceClass.OlderMovie,
                PriceClass.NewMovie,
                PriceClass.NewHotMovie,
                PriceClass.SwedishNew, 
            };

            int[] quantitiesDesc = 
            { 
                5, 1
            };

            foreach( Enum membership in membershipsDesc )
            {
                foreach( Enum priceClass in priceClassesAsc )
                {
                    foreach ( int quantity in quantitiesDesc )
                    {
                        decimal fee = VideoStore.GetPrice( 
                            (Membership)membership, (PriceClass)priceClass, quantity );

                        Out.WriteLine( string.Format( "{0,-12} {1,-15} {2,3} {3,8:0.00}",
                            membership.Verbose (), priceClass.Verbose (), 
                            quantity, fee ) );
                    }
                }
            }

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding Andrew Wiles as a customer..." );

            Customer customer = new Customer ()
            { 
                PersonID    = "US 1953-05-14",
                FirstName   = "Andrew",
                LastName    = "Wiles",
                Address     = "Princeton University",
                PostCode    = "08544",
                City        = "Princeton, NJ",
                Country     = "USA",
                Phone       = "+1(609)789-44-44",
                CreditCard  = new CreditCard( CreditCardType.VISA, 
                                              "4000-0012-3456-7899", 14, 12 ),
                Membership  = Membership.Member
            };

            customer.AddTo( VideoStore );

            Out.WriteLine( VideoStore.Customers );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding John Poe as a customer..." );

            customer = new Customer ()
            {
                PersonID    = "820315-1035",
                FirstName   = "John",
                LastName    = "Poe",
                Address     = "aaa",
                PostCode    = "ppp",
                City        = "ccc",
                EMail       = "john.doe@mail.se",
                Membership  = Membership.GoldMember
            };

            customer.AddTo( VideoStore );

            Out.WriteLine( VideoStore.Customers );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding Linnéa Andersson as a customer..." );

            customer = new Customer ()
            { 
                PersonID    = "830415-2034", 
                FirstName   = "Linnéa", 
                LastName    = "Andersson",
                Address     = "Ostmästargatan 12",
                PostCode    = "128 87",
                City        = "Stockholm",
                CellPhone   = "+46(73)45342225",
                Membership  = Membership.GoldMember
            };

            customer.AddTo( VideoStore );

            Out.WriteLine( VideoStore.Customers );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding Richard Liboff as a customer..." );

            customer = new Customer ()
            { 
                PersonID    = "193405143415",
                FirstName   = "Richard",
                LastName    = "Liboff",
                Address     = "Oxvägen 44",
                PostCode    = "141 40",
                City        = "Huddinge",
                Phone       = "+46(8)7798881",
                EMail       = "richard@liboff.name"
            };

            customer.AddTo( VideoStore );

            Out.WriteLine( VideoStore.Customers );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Removing non-existing customer..." );

            VideoStore.RemoveCustomer( cust => cust.FirstName == "(*&*($@" );

            Out.WriteLine( VideoStore.Customers );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Changing customer's last name 'Poe' to 'Doe'..." );

            customer = VideoStore.FindCustomer( cust => cust.LastName == "Poe" );

            customer = new Customer( customer );
            customer.LastName = "Doe";
            customer.Update ();

            Out.WriteLine( VideoStore.Customers );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Removing customer with name 'John Doe'..." );

            VideoStore.RemoveCustomer( cust => cust.FullName == "John Doe" );

            Out.WriteLine( VideoStore.Customers );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Dumping contents of the customer database..." );

            Out.WriteLine( VideoStore.Customers.FullInfo () );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding 1 VHS and 3 BluRay copies of '2001 A Space Odyssey'..." );

            Movie movie = new Movie ()
            {
                Title        = "2001: A Space Odyssey",
                IMDB         = "http://www.imdb.com/title/tt0062622/",
                Genre        = Genre.ScienceFiction | Genre.Adventure | Genre.Mystery,
                FirstRelease = new DateTime( 1968, 4, 2 ),
                Duration     = new TimeSpan( 0, 141, 0 ),
                Country      = "UK | US",
                Language     = "English | Russian (only few words)",
                PgRate       = "11 (Sweden)",
                Directors    = new List<string> () {
                               "Stanley Kubrick"
                },
                Writers      = new List<string> () {
                               "Stanley Kubrick",
                               "Arthur C Clarke"
                },
                Actors       = new List<MovieActor> () {
                               new MovieActor( "Keir Dullea", "Dr Dave Bowman" ),
                               new MovieActor( "Gary Lockwood", "Dr Frank Poole " ),
                               new MovieActor( "William Sylves", "Dr Heywood R Floyd" ),
                               new MovieActor( "Douglas Rain", "HAL 9000 (voice)" ),
                }
            };

            movie.AddTo( VideoStore );

            MovieExemplar exemplar = new MovieExemplar ()
            {
                Media        = Media.VHS,
                Released     = new DateTime( 1999, 6, 29 ),
                ImageFormat  = "NTSC 4:3",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B00000J2KZ",
                Features     = "Turner Home Ent"
            };

            exemplar.AddTo( movie );
        
            exemplar = new MovieExemplar ()
            {
                Media        = Media.BluRay,
                Released     = new DateTime( 2007, 10, 23 ),
                ImageFormat  = "2.20:1",
                Subtitles    = "English | Spanish | French",
                PriceClass   = PriceClass.NewMovie,
                ISAN         = "ASIN:B000Q66J1M",
                Features     = "All Regions"
            };
            
            exemplar.AddTo( movie ).CreateCopies( 2 );
        
            Out.WriteLine( VideoStore.Movies );
            Out.WriteLine ();
            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding 2 DVD copies of 'Platoon'..." );

            movie = new Movie ()
            {
                Title        = "Platoon",
                IMDB         = "http://www.imdb.com/title/tt0091763/",
                Genre        = Genre.Action | Genre.Drama | Genre.War,
                FirstRelease = new DateTime( 1986, 12, 19 ),
                Duration     = new TimeSpan( 0, 120, 0 ),
                Country      = "UK | US",
                Language     = "English | Vietnamese",
                PgRate       = "15 (Sweden)",
                Directors    = new List<string> () {
                               "Oliver Stone"
                },
                Writers      = new List<string> () {
                               "Oliver Stone"
                },
                Actors       = new List<MovieActor> () {
                               new MovieActor( "Charlie Sheen", "Chris" ),
                               new MovieActor( "Tom Berenger", "Sgt. Barnes" ),
                               new MovieActor( "Willem Dafoe", "Sgt. Elias" ),
                }
            };

            movie.AddTo( VideoStore );

            exemplar = new MovieExemplar ()
            {
                Media        = Media.DVD,
                Released     = new DateTime( 2001, 6, 5 ),
                ImageFormat  = "Anamorphic, 1.85:1",
                Subtitles    = "Spanish | French",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B00005AUJQ",
                Features     = "Special Edition | MGM"
            };
            
            exemplar.AddTo( movie ).CreateCopies( 1 );
        
            Out.WriteLine( VideoStore.Movies );
            Out.WriteLine ();
            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding one exemplar of 'Sällskapsresan'..." );

            movie = new Movie ()
            {
                Title        = "Sällskapsresan",
                IMDB         = "http://www.imdb.com/title/tt0081590/",
                FirstRelease = new DateTime( 1980, 8, 22 ),
                Duration     = new TimeSpan( 0, 107, 0 ),
                Country      = "Sweden",
                Language     = "Swedish | Norwegian | Spanish",
                Directors    = new List<string> () {
                               "Lasse Åberg", 
                               "Peter Hald"
                },
                Writers      = new List<string> () {
                               "Bo Jönsson",
                               "Lasse Åberg"
                },
                Actors       = new List<MovieActor> () {
                               new MovieActor( "Lasse Åberg", "Stig-Helmer Olsson" ),
                               new MovieActor( "Lottie Ejebrant", "Maj-Britt Lindberg" ),
                               new MovieActor( "Jon Skolmen", "Ole Bramserud" )
                }
            };

            movie.AddTo( VideoStore );

            exemplar = new MovieExemplar ()
            {
                Media        = Media.BluRay,
                Released     = new DateTime( 2001, 6, 5 ),
                ImageFormat  = "Widescreen (1.78:1)",
                Subtitles    = "Swedish | Norwegian",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B003TLJY1I",
                Features     = "SF(Fox) | Region 2"
            };

            exemplar.AddTo( movie );
        
            Out.WriteLine( VideoStore.Movies );
            Out.WriteLine ();
            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Adding 2 DVD, 1 VHS and 1 BluRay copies of 'Citizen Kane'..." );

            movie = new Movie ()
            {
                Title        = "Citizen Kane",
                IMDB         = "http://www.imdb.com/title/tt0033467/",
                Genre        = Genre.Drama | Genre.Mystery,
                FirstRelease = new DateTime( 1941, 5, 1 ),
                Duration     = new TimeSpan( 0, 119, 0 ),
                Country      = "USA",
                Language     = "English",
                PgRate       = "15 (Sweden)",
                Directors    = new List<string> () {
                               "Orson Welles"
                },
                Writers      = new List<string> () {
                               "Herman J. Mankiewicz",
                               "Orson Welles"
                },
                Actors       = new List<MovieActor> () {
                               new MovieActor( "Orson Welles", "Kane" ),
                               new MovieActor( "Joseph Cotten", "Jedediah Leland" ),
                               new MovieActor( "Dorothy Comingore", "Susan A Kane" ),
                }
            };

            movie.AddTo( VideoStore );

            exemplar = new MovieExemplar ()
            {
                Media        = Media.DVD,
                Released     = new DateTime( 2001, 9, 25 ),
                ImageFormat  = "B&W, 1.33:1, NTSC",
                Subtitles    = "English | French | Portuguese | Spanish ",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B00003CX9E",
                Features     = "Turner Home Ent | 2 discs"
            };
            
            exemplar.AddTo( movie ).CreateCopies( 1 );
        
            exemplar = new MovieExemplar ()
            {
                Media        = Media.VHS,
                Released     = new DateTime( 1996, 8, 13 ),
                ImageFormat  = "B&W, 1.33:1, NTSC",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:6304119046",
                Features     = "RKO Radio Pictures | Special Edition"
            };

            exemplar.AddTo( movie );
        
            exemplar = new MovieExemplar ()
            {
                Media        = Media.BluRay,
                ImageFormat  = "B&W, 1.33:1, NTSC",
                Subtitles    = "English | French | Portuguese | Spanish ",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B001PIHH5M",
                Features     = "Warner"
            };

            exemplar.AddTo( movie );
        
            Out.WriteLine( VideoStore.Movies );
            Out.WriteLine ();
            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Removing exemplar with ID #3 from library..." );

            VideoStore.RemoveMovieExemplar( ex => ex.ID == 3 );

            Out.WriteLine( VideoStore.Movies );
            Out.WriteLine ();
            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Removing exemplar with ID #8 from library..." );

            VideoStore.RemoveMovieExemplar( ex => ex.ID == 8 );

            Out.WriteLine( VideoStore.Movies );
            Out.WriteLine ();
            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Renting out movie exemplar #4 to user with PID 830415-2034" );

            try
            {
                VideoStore.RentItem(
                    VideoStore.FindMovieExemplar( ex => ex.ID == 4 ),
                    VideoStore.FindCustomer( cust => cust.PersonID == "830415-2034" ),
                    /* rent days: */ 2, /* rental fee: */ 15
                );
            }
            catch( Exception ex )
            {
                Out.WriteLine( ex.Message );
            }

            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Renting out movie ex #6 to user with first name Andrew..." );

            try
            {
                VideoStore.RentItem(
                    VideoStore.FindMovieExemplar( ex => ex.ID == 6 ),
                    VideoStore.FindCustomer( cust => cust.FirstName == "Andrew" ),
                    /* rent days: */ 3, /* rental fee: */ 20
                );
            }
            catch( Exception ex )
            {
                Out.WriteLine( ex.Message );
            }

            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Returning exemplar #4 from renting..." );

            VideoStore.ReturnItem(
                VideoStore.FindMovieExemplar( ex => ex.ID == 4 )
                );

            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Renting out movie exemplar #2 to user with ID #1..." );

            try
            {
                VideoStore.RentItem(
                    VideoStore.FindMovieExemplar( ex => ex.ID == 2 ),
                    VideoStore.FindCustomer( cust => cust.ID == 1 ),
                    /* rent days: */ 1, /* rental fee: */ 15
                    );
            }
            catch( Exception ex )
            {
                Out.WriteLine( ex.Message );
            }

            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Renting out movie exemplar #9 to user with ID #3..." );

            try
            {
                VideoStore.RentItem(
                    VideoStore.FindMovieExemplar( ex => ex.ID == 9 ),
                    VideoStore.FindCustomer( cust => cust.ID == 3 ),
                    /* rent days: */ 1, /* rental fee: */ 15
                    );
            }
            catch( Exception ex )
            {
                Out.WriteLine( ex.Message );
            }

            Out.WriteLine( VideoStore.MovieExemplars );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "LINQ: Listing all rented items using LINQ and yield return..." );

            var rented = from p in VideoStore.RentedItems select p;
 
            Out.WriteLine( "Total {0} rented items:", rented.Count<RentedItem> () );

            foreach( var item in rented )
            {
                Out.WriteLine ();
                Out.WriteLine( item );
                Out.WriteLine ();
                Out.WriteLine( "    " + item.Exemplar );
                Out.WriteLine( "    " + item.RentedTo );
            }

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "LINQ: Querying movies having 'odys' in title..." );

            var queryMovies =
                from p in VideoStore.Movies
                where p.FullTitle.ToLower().Contains( "odys" )
                select p;
 
            Out.WriteLine( "Selected {0} movies:", queryMovies.Count<Movie> () );

            foreach( var item in queryMovies )
            {
                Out.WriteLine( item.FullTitle );
            }

           //////////////////////////////////////////////////////////////////////////////

            Out.Title( "Dumping final movie collection..." );

            Out.WriteLine( VideoStore.Movies.FullInfo () );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Dumping final movie exemplar collection..." );

            Out.WriteLine( VideoStore.MovieExemplars.FullInfo () );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Dumping all customers with rented items..." );

            Out.WriteLine( VideoStore.QueryAllCustomersWithRentals () );

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Dumping all rented items..." );

            Out.WriteLine( VideoStore.QueryAllRentedItems () );

            /////////////////////////////////////////////////////////////////////////////

            // Don't forget to remove hooked event handlers and mark end of test.
            //
            VideoStore.Changed -= VideoStore_Changed;

            Out.End ();

            /////////////////////////////////////////////////////////////////////////////
            // Our sample database is now ready to be used.
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Random Test Records ]

        /// <summary>
        /// Adds random test records to existing database. Basically, this method has
        /// the same structure as <see cref="CreateSampleDatabase"/>, but it rather
        /// generates random customer names and movie titles.
        /// </summary>
        /// 
        public void AddTestRecords ()
        {
            if ( VideoStore == null )
            {
                return;
            }

            /////////////////////////////////////////////////////////////////////////////

            Customer customer = new Customer ()
            { 
                PersonID    = RandomPID, 
                FirstName   = RndFirstName, 
                LastName    = RndLastName,
                Address     = RndLastName + "sgatan " + ( random.Next( 32 ) + 1 ),
                PostCode    = "128 87",
                City        = "Stockholm",
                Country     = "Sweden",
                CellPhone   = "+46(73)" + ( random.Next( 10000000 ) + 50000000 ),
                CreditCard  = new CreditCard( CreditCardType.MasterCard, 
                                              "5105-1051-0510-5100", 14, 12 ),
                Membership  = RndMembership
            };

            customer.AddTo( VideoStore );

            /////////////////////////////////////////////////////////////////////////////

            customer = new Customer ()
            { 
                PersonID    = RandomPID, 
                FirstName   = RndFirstName, 
                LastName    = RndLastName,
                Address     = RndLastName + "svägen " + ( random.Next( 32 ) + 1 ),
                PostCode    = "128 87",
                City        = "Stockholm",
                Country     = "Sweden",
                CellPhone   = "+46(73)" + ( random.Next( 10000000 ) + 80000000 ),
                Membership  = RndMembership
            };

            customer.AddTo( VideoStore );

            /////////////////////////////////////////////////////////////////////////////

            customer = new Customer ()
            { 
                PersonID    = RandomPID, 
                FirstName   = RndFirstName, 
                LastName    = RndLastName,
                Address     = RndLastName + "sgatan " + ( random.Next( 32 ) + 1 ),
                PostCode    = "141 40",
                City        = "Huddinge",
                Country     = "Sweden",
                CellPhone   = "+46(73)" + ( random.Next( 10000000 ) + 30000000 ),
                EMail       = RndFirstName + "@" + RndFirstName + ".name",
                Membership  = RndMembership
            };

            customer.AddTo( VideoStore );

            /////////////////////////////////////////////////////////////////////////////

            Movie movie = new Movie ()
            {
                Title        = RndMovieTitle,
                IMDB         = "Faked Movie",
                Genre        = RndGenre,
                FirstRelease = RndDateTime,
                Duration     = RndDuration,
                Country      = "UK | US",
                Language     = "English | Russian (only few words)",
                PgRate       = "11 (Sweden)",
                Directors    = new List<string> () {
                               "Stanley Kubrick"
                },
                Writers      = new List<string> () {
                               "Stanley Kubrick",
                               "Arthur C Clarke"
                },
                Actors       = new List<MovieActor> () {
                               new MovieActor( "Keir Dullea", "Dr Dave Bowman" ),
                               new MovieActor( "Gary Lockwood", "Dr Frank Poole " ),
                               new MovieActor( "William Sylves", "Dr Heywood R Floyd" ),
                               new MovieActor( "Douglas Rain", "HAL 9000 (voice)" ),
                }
            };

            movie.AddTo( VideoStore );

            MovieExemplar exemplar = new MovieExemplar ()
            {
                Media        = Media.VHS,
                Released     = new DateTime( 1999, 6, 29 ),
                ImageFormat  = "NTSC 4:3",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B00000J2KZ",
                Features     = "Turner Home Ent"
            };

            exemplar.AddTo( movie );
        
            exemplar = new MovieExemplar ()
            {
                Media        = Media.BluRay,
                Released     = new DateTime( 2007, 10, 23 ),
                ImageFormat  = "2.20:1",
                Subtitles    = "English | Spanish | French",
                PriceClass   = PriceClass.NewMovie,
                ISAN         = "ASIN:B000Q66J1M",
                Features     = "All Regions"
            };
            
            exemplar.AddTo( movie ).CreateCopies( 1 + random.Next( 2 ) );
        
            /////////////////////////////////////////////////////////////////////////////

            movie = new Movie ()
            {
                Title        = RndMovieTitle,
                IMDB         = "Faked Movie",
                Genre        = RndGenre,
                FirstRelease = RndDateTime,
                Duration     = RndDuration,
                Country      = "UK | US",
                Language     = "English | Vietnamese",
                PgRate       = "15 (Sweden)",
                Directors    = new List<string> () {
                               RndMoviePerson, 
                },
                Writers      = new List<string> () {
                               RndMoviePerson, 
                },
                Actors       = new List<MovieActor> () {
                               RndActor, RndActor, RndActor
                }
            };

            movie.AddTo( VideoStore );

            exemplar = new MovieExemplar ()
            {
                Media        = Media.DVD,
                Released     = new DateTime( 1905 + random.Next( 105 ), 3, 5 ),
                ImageFormat  = "Anamorphic, 1.85:1",
                Subtitles    = "Spanish | French",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B00005AUJQ",
                Features     = "Special Edition | MGM"
            };
            
            exemplar.AddTo( movie ).CreateCopies( 1 + random.Next( 2 ) );
        
            /////////////////////////////////////////////////////////////////////////////

            movie = new Movie ()
            {
                Title        = RndMovieTitle,
                IMDB         = "Faked Movie",
                Genre        = RndGenre,
                FirstRelease = RndDateTime,
                Duration     = RndDuration,
                Country      = "Sweden",
                Language     = "Swedish | Norwegian | Spanish",
                Directors    = new List<string> () {
                               RndMoviePerson, RndMoviePerson, 
                },
                Writers      = new List<string> () {
                               RndMoviePerson, RndMoviePerson, 
                },
                Actors       = new List<MovieActor> () {
                               RndActor, RndActor, RndActor,
                               RndActor, RndActor, RndActor
                }
            };

            movie.AddTo( VideoStore );

            exemplar = new MovieExemplar ()
            {
                Media        = Media.BluRay,
                Released     = new DateTime( 2001, 6, 5 ),
                ImageFormat  = "Widescreen (1.78:1)",
                Subtitles    = "Swedish | Norwegian",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B003TLJY1I",
                Features     = "SF(Fox) | Region 2"
            };

            exemplar.AddTo( movie );
        
            /////////////////////////////////////////////////////////////////////////////

            movie = new Movie ()
            {
                Title        = RndMovieTitle,
                IMDB         = "Faked Movie",
                Genre        = RndGenre,
                FirstRelease = RndDateTime,
                Duration     = RndDuration,
                Country      = "USA",
                Language     = "English",
                PgRate       = "15 (Sweden)",
                Directors    = new List<string> () {
                               RndMoviePerson, 
                },
                Writers      = new List<string> () {
                               RndMoviePerson, RndMoviePerson, 
                },
                Actors       = new List<MovieActor> () {
                               RndActor, RndActor, RndActor
                }
            };

            movie.AddTo( VideoStore );

            exemplar = new MovieExemplar ()
            {
                Media        = Media.DVD,
                Released     = new DateTime( 2001, 9, 25 ),
                ImageFormat  = "B&W, 1.33:1, NTSC",
                Subtitles    = "English | French | Portuguese | Spanish ",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B00003CX9E",
                Features     = "Turner Home Ent | 2 discs"
            };
            
            exemplar.AddTo( movie ).CreateCopies( 1 + random.Next( 3 ) );
        
            exemplar = new MovieExemplar ()
            {
                Media        = Media.VHS,
                Released     = new DateTime( 1996, 8, 13 ),
                ImageFormat  = "B&W, 1.33:1, NTSC",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:6304119046",
                Features     = "RKO Radio Pictures | Special Edition"
            };

            exemplar.AddTo( movie );
        
            exemplar = new MovieExemplar ()
            {
                Media        = Media.BluRay,
                ImageFormat  = "B&W, 1.33:1, NTSC",
                Subtitles    = "English | French | Portuguese | Spanish ",
                PriceClass   = PriceClass.OlderMovie,
                ISAN         = "ASIN:B001PIHH5M",
                Features     = "Warner"
            };

            exemplar.AddTo( movie );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Serialization Procedures ]

        public void SaveIntoFile( string filename )
        {
            if ( VideoStore == null )
            {
                return;
            }

            Out.Begin ();

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Serializing video store data..." );

            try 
            {
                // To serialize data we must first open a stream for writing. 
                // In this case, use a file stream.
                //
                using( FileStream fs = new FileStream( filename, FileMode.Create ) )
                {
                    VideoStore.Serialize( fs );
                }
            }
            catch( Exception ex )
            {
                Out.WriteLine( ex );
            }

            /////////////////////////////////////////////////////////////////////////////

            Out.End ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        public void LoadFromFile( string filename )
        {
            Out.Begin ();

            /////////////////////////////////////////////////////////////////////////////

            Out.Title( "Deserializing video store data..." );

            VideoStore = null;

            try 
            {
                // Open the file containing the data that we want to deserialize.
                //
                using( FileStream fs = new FileStream( filename, FileMode.Create ) )
                {
                    VideoStore = VideoRentalOutlet.Deserialize( fs );
                }
            }
            catch( Exception ex )
            {
                Out.WriteLine( ex );
            }

            if ( VideoStore == null )
            {
                return;
            }

            Out.WriteLine( VideoStore.FullInfo () );
            Out.WriteLine( VideoStore.MovieExemplars.FullInfo () );
            Out.WriteLine( VideoStore.Movies.FullInfo () );
            Out.WriteLine( VideoStore.Customers.FullInfo () );

            /////////////////////////////////////////////////////////////////////////////

            Out.End ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods that generates random data ]

        private static Random random = new Random ();

        private string RandomPID
        {
            get
            {
                return string.Format( "{0:00}{1:00}{2:00}-{3:0000}",
                    random.Next( 100 ), 1 + random.Next( 12 ), 1 + random.Next( 30 ),
                    random.Next( 10000 ) );
            }
        }

        private string RndFirstName
        {
            get { return sampleFirstNames[ random.Next( sampleFirstNames.Length ) ]; }
        }

        private string RndLastName
        {
            get { return sampleLastNames[ random.Next( sampleLastNames.Length ) ]; }
        }

        private Membership RndMembership
        {
            get { return (Membership) random.Next( 3 ); }
        }

        private string RndMovieTitle
        {
            get { return sampleMovies[ random.Next( sampleMovies.Length ) ]; }
        }

        private string RndMoviePerson
        {
            get { return RndFirstName + " " + RndLastName; }
        }

        private MovieActor RndActor
        {
            get { return new MovieActor( RndMoviePerson, RndFirstName ); }
        }

        private string RndSubID
        {
            get { return random.Next( 10000 ).ToString( "0000" ); }
        }

        private DateTime RndDateTime
        {
            get { return new DateTime( 1905 + random.Next( 105 ),
                1 + random.Next( 12 ), 1 + random.Next( 28 ) ); }
        }

        private TimeSpan RndDuration
        {
            get { return new TimeSpan( 0, 30 + random.Next( 200 ), 0 ); }
        }

        private Genre RndGenre
        {
            get
            {
                // Generate between 1 and 4 random Genre bit-flags
                int genre = 0;
                for ( int i = 1; i <= random.Next( 5 ); ++i )
                {
                    genre |= ( 1 << random.Next( 20 ) );
                }
                return (Genre)genre;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private string[] sampleFirstNames = 
        {
            "Lucas",       "Elias",       "Oscar",       "William",     "Hugo",
            "Alexander",   "Oliver",      "Viktor",      "Erik",        "Axel",
            "Filip",       "Emil",        "Isak",        "Leo",         "Liam",
            "Theo",        "Anton",       "Melvin",      "Albin",       "Gustav",
            "Max",         "Ludvig",      "Arvid",       "Edvin",       "Vincent",
            "Alvin",       "Wilmer",      "Adam",        "Noah",        "Elliot",
            "Simon",       "Jonathan",    "Kevin",       "Olle",        "Benjamin",
            "Linus",       "Carl",        "Alfred",      "Rasmus",      "Gabriel",
            "Charlie",     "Jacob",       "Theodor",     "Nils",        "Leon",
            "Felix",       "Sebastian",   "Noel",        "Casper",      "Malte",
            "Sixten",      "Ville",       "David",       "Melker",      "Joel",
            "Hampus",      "Neo",         "Adrian",      "Samuel",      "Love",
            "Milo",        "Josef",       "Jack",        "Mohammed",    "Wilhelm",
            "Alex",        "Daniel",      "Tim",         "Valter",      "Sigge",
            "Vilgot",      "Robin",       "Marcus",      "Vidar",       "August",
            "Måns",        "Milton",      "Hannes",      "Maximilian",  "Loke",
            "Harry",       "Elis",        "John",        "Aron",        "Hjalmar",
            "Otto",        "Elton",       "Mio",         "Fabian",      "Eddie",
            "Sam",         "Svante",      "Dante",       "Ali",         "Johannes",
            "Mattias",     "Jesper",      "Dennis",      "Ruben",       "Alice",
            "Maja",        "Ella",        "Emma",        "Elsa",        "Alva",
            "Julia",       "Linnea",      "Wilma",       "Ebba",        "Molly",
            "Nellie",      "Klara",       "Agnes",       "Elin",        "Olivia",
            "Emilia",      "Isabelle",    "Amanda",      "Saga",        "Moa",
            "Ellen",       "Alma",        "Matilda",     "Hanna",       "Sara",
            "Alicia",      "Felicia",     "Stella",      "Tuva",        "Isabella",
            "Meja",        "Selma",       "Thea",        "Elvira",      "Lilly",
            "Lovisa",      "Tindra",      "Lova",        "Nora",        "Sofia",
            "Ester",       "Nathalie",    "Vera",        "Tilde",       "Tyra",
            "Emelie",      "Filippa",     "Freja",       "Signe",       "Tilda",
            "Siri",        "Lea",         "Stina",       "Cornelia",    "Inez",
            "Edith",       "Linn",        "Mira",        "Jasmine",     "Liv",
            "Leia",        "Emmy",        "Frida",       "Lisa",        "Ellie",
            "Josefin",     "Svea",        "Rebecka",     "Evelina",     "Ronja",
            "Kajsa",       "Anna",        "Joline",      "Elina",       "Elise",
            "Märta",       "Lina",        "Hilda",       "Iris",        "Ingrid",
            "Melissa",     "Livia",       "Vilda",       "My",          "Sofie",
            "Malva",       "Nicole",      "Victoria",    "Fanny",       "Greta",
            "Hedda",       "Alexandra",   "Maria",       "Rut",         "Miranda",
        };

        private string[] sampleLastNames =
        {
            "Johansson",   "Andersson",   "Karlsson",    "Nilsson",     "Eriksson",
            "Larsson",     "Olsson",      "Persson",     "Svensson",    "Gustafsson",
            "Pettersson",  "Jonsson",     "Jansson",     "Hansson",     "Bengtsson",
            "Jönsson",     "Lindberg",    "Jakobsson",   "Magnusson",   "Olofsson",
            "Edström",     "Lindgren",    "Lindqvist",   "Axelsson",    "Adberg",
            "Berg",        "Bergström",   "Lundgren",    "Mattsson",    "Lundqvist",
            "Lind",        "Berglund",    "Fredriksson", "Henriksson",  "Sandberg",
            "Forsberg",    "Sjöberg",     "Danielsson",  "Håkansson",   "Wallin",
            "Engström",    "Eklund",      "Lundin",      "Gunnarsson",  "Fransson",
            "Samuelsson",  "Holm",        "Bergman",     "Björk",       "Vikström",
            "Isaksson",    "Arvidsson",   "Bergqvist",   "Holmberg",    "Nyström",
            "Claesson",    "Löfgren",     "Söderberg",   "Nyberg",      "Mårtensson",
            "Blomqvist",   "Nordström",   "Lundström",   "Pålsson",     "Eliasson",
            "Björklund",   "lund",        "Berggren",    "Nordin",      "Sandström",
            "Ström",       "Hermansson",  "Lund",        "Åberg",       "Ekström",
            "Holmgren",    "Sundberg",    "Hedlund",     "Hellström",   "Dahlberg",
            "Sjögren",     "Abrahamsson", "Martinsson",  "Andreasson",  "Öberg",
            "Månsson",     "Falk",        "Blom",        "Ek",          "Åkesson",
            "Strömberg",   "Jonasson",    "Norberg",     "Hansen",      "Sundström",
            "Åström",      "Holmqvist",   "Ivarsson"
        };

        private string[] sampleMovies =
        {
            "Die Hard",
            "Seven Samurai",
            "Raiders of the Lost Ark",
            "Spirited Away",
            "Snow White and the Seven Dwarfs",
            "Beauty and the Beast",
            "Toy Story",
            "What's Opera, Doc?",
            "The Lion King",
            "A Christmas Story",
            "It's a Wonderful Life",
            "The Nightmare Before Christmas",
            "Airplane!",
            "Dr. Strangelove or: How I Learned to Stop Worrying and Love the Bomb",
            "Monty Python and the Holy Grail",
            "Monty Python's Life of Brian",
            "National Lampoon's Animal House",
            "Some Like It Hot",
            "City Lights",
            "Persepolis",
            "The Dark Knight",
            "Spider-Man 2",
            "X2",
            "To Kill a Mockingbird",
            "The Godfather",
            "Goodfellas",
            "The Poseidon Adventure",
            "Titanic",
            "Hoop Dreams",
            "Bowling for Columbine",
            "Seven Up!",
            "Man on Wire",
            "The Thin Blue Line",
            "The Last Waltz",
            "The Battle of Algiers",
            "Lawrence of Arabia",
            "The Lord of the Rings: The Return of the King",
            "The Wizard of Oz",
            "The Shining",
            "The Exorcist",
            "Psycho",
            "King Kong",
            "Jaws",
            "The Texas Chain Saw Massacre",
            "The Silence of the Lambs",
            "A Hard Day's Night",
            "Singin' in the Rain",
            "West Side Story",
            "The Sound of Music",
            "Rear Window",
            "Vertigo",
            "Schindler's List",
            "Triumph of the Will",
            "Casablanca",
            "Romeo & Juliet",
            "The Notebook",
            "Blade Runner",
            "E.T. the Extra-Terrestrial",
            "The Empire Strikes Back",
            "Inception",
            "Star Wars",
            "Murderball",
            "Raging Bull",
            "Miracle",
            "Cinderella Man",
            "Le Mans",
            "Bull Durham",
            "Coach Carter",
            "Caddyshack",
            "Chariots of Fire",
            "Paths of Glory",
            "Saving Private Ryan",
            "The Searchers",
            "The Good, the Bad and the Ugly",
            "Once Upon a Time in the West",
            "Butch Cassidy and the Sundance Kid",
        };

        #endregion
    }
}