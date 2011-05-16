/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.CustomerDatabase
 *  File:       Enums.cs
 *  Created:    2011-04-08
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

using Mbk.Commons;

namespace VROLib.CustomerDatabase
{
    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Customer's membership status
    /// </summary>
    /// 
    [Serializable]
    public enum Membership : int
    {
        [Verbose( "Not Member"  )]  NotMember  = 0,
        [Verbose( "Member"      )]  Member     = 1,
        [Verbose( "Gold Member" )]  GoldMember = 2
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Type of the credit card (VISA, MasterCard etc)
    /// </summary>
    /// 
    [Serializable]
    public enum CreditCardType : int
    {
        [Verbose( "None"             )]  None            = 0,
        [Verbose( "VISA"             )]  VISA            = 1,
        [Verbose( "MasterCard"       )]  MasterCard      = 2,
        [Verbose( "American Express" )]  AmericanExpress = 3,
        [Verbose( "BankCard"         )]  BankCard        = 4,
        [Verbose( "Discover"         )]  Discover        = 5,
        [Verbose( "Diners Club"      )]  DinersClub      = 6,
        [Verbose( "JCB"              )]  JCB             = 7,
    }

    /////////////////////////////////////////////////////////////////////////////////////
}

namespace VROLib.ItemStore
{
    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Price class of partiticular movie exemplar (Older movie, New movie etc)
    /// </summary>
    ///
    [Serializable]
    public enum PriceClass : int
    {
        [Verbose( "Older Movie"   )]  OlderMovie   = 0,
        [Verbose( "New Movie"     )]  NewMovie     = 1,
        [Verbose( "New Hot Movie" )]  NewHotMovie  = 2,
        [Verbose( "Swedish New"   )]  SwedishNew   = 3
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Type of media for movie exemplars (VHS, DVD etc)
    /// </summary>
    /// 
    [Serializable]
    public enum Media : int
    {
        [Verbose( "VHS Tape"     )]  VHS    = 0,
        [Verbose( "Video CD"     )]  VCD    = 1,
        [Verbose( "DVD Disc"     )]  DVD    = 2,
        [Verbose( "Blu-ray Disc" )]  BluRay = 3
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Film genre (Action, Adventure etc). Film genre is an enumerated bitfield as movie 
    /// may belong to multiple genres.
    /// </summary>
    /// 
    [Serializable,Flags]
    public enum Genre : int
    {
        [Verbose( "Unclassified" )]  None            = 0x00000000,
        [Verbose( "Action"       )]  Action          = 0x00000001,
        [Verbose( "Adventure"    )]  Adventure       = 0x00000002,
        [Verbose( "Comedy"       )]  Comedy          = 0x00000004,
        [Verbose( "Crime"        )]  Crime           = 0x00000008,
        [Verbose( "Disaster"     )]  Disaster        = 0x00000010,
        [Verbose( "Documentary"  )]  Documentary     = 0x00000020,
        [Verbose( "Drama"        )]  Drama           = 0x00000040,
        [Verbose( "Epic"         )]  Epic            = 0x00000080,
        [Verbose( "Family"       )]  Family          = 0x00000100,
        [Verbose( "Fantasy"      )]  Fantasy         = 0x00000200,
        [Verbose( "Horror"       )]  Horror          = 0x00000400,
        [Verbose( "Musical"      )]  Musical         = 0x00000800,
        [Verbose( "Mystery"      )]  Mystery         = 0x00001000,
        [Verbose( "Romance"      )]  Romance         = 0x00002000,
        [Verbose( "Sci-Fi"       )]  ScienceFiction  = 0x00004000,
        [Verbose( "Sexual"       )]  Sexual          = 0x00008000,
        [Verbose( "Sport"        )]  Sport           = 0x00010000,
        [Verbose( "Thriller"     )]  Thriller        = 0x00020000,
        [Verbose( "War"          )]  War             = 0x00040000,
        [Verbose( "Western"      )]  Western         = 0x00080000,
    }

    /////////////////////////////////////////////////////////////////////////////////////
}