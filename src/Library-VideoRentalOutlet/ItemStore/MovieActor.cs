/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.ItemStore
 *  File:       MovieActor.cs
 *  Created:    2011-04-05
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Text;
using Mbk.Commons;

namespace VROLib.ItemStore
{
    /// <summary>
    /// MovieActor is an immutable structure that encapsulates actor's real name together
    /// with its role name.
    /// </summary>
    /// 
    [Serializable]
    public struct MovieActor
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Properties ]

        /// <summary>
        /// Actor's (real) name.
        /// </summary>
        /// 
        public string RealName { get; private set; }

        /// <summary>
        /// Actor's role in the movie (optional i.e. may be null).
        /// </summary>
        /// 
        public string RoleName { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        public MovieActor( string actorsRealName, string actorsRole )
            : this ()
        {
            // Trim strings also removing additional inner white-spaces from text
            //
            actorsRealName = actorsRealName.TrimmedName ();
            actorsRole = actorsRole.TrimmedName ();

            // Mandatory properties:
            //
            if ( string.IsNullOrEmpty( actorsRealName ) )
            {
                throw new ArgumentException( "Actor's real name must not be null" );
            }

            this.RealName = actorsRealName;
            this.RoleName = actorsRole;

        }

        /// <summary>
        /// Creates a new instance of MovieActor class based on text in compound
        /// "actor as role" format. Actor and role are separated with "as" which 
        /// is parsed as case-insensitive. Actor is mandatory while "as" and role 
        /// are optional. 
        /// </summary>
        /// <param name="compoundNames">a string with compound actor's real name and 
        /// his/her role in "actor as role" format. 
        /// E.g. "Douglas Rain aS HAL 9000 (voice)" is parsed as actor = "Douglas Rain"
        /// and Role = "HAL 9000 (voice)".
        /// </param>
        /// 
        public MovieActor( string compoundNames )
            : this ()
        {
            compoundNames = compoundNames.TrimmedName ();

            if ( string.IsNullOrEmpty( compoundNames ) )
            {
                throw new ArgumentException( "Actor's real name must not be null" );
            }

            // Findout delimiters position, case-invariant.
            //
            string lowercaseNames = compoundNames.ToLowerInvariant ();
            if ( lowercaseNames.StartsWith( NameDelimiterParsed + " " ) )
            {
                throw new ArgumentException( "Actor's real name must not be null" );
            }

            int pos = lowercaseNames.IndexOf( " " + NameDelimiterParsed + " " );

            if ( pos < 0  )
            {
                if ( lowercaseNames.EndsWith( " " + NameDelimiterParsed ) )
                {
                    pos = compoundNames.Length - 3;
                    this.RealName = compoundNames.Substring( 0, pos );
                }
                else
                {
                    this.RealName = compoundNames;
                }
                this.RoleName = null;
            }
            else
            {
                this.RealName = compoundNames.Substring( 0, pos ).TrimToNull ();

                if ( pos + 4 < compoundNames.Length )
                {
                    this.RoleName = compoundNames.Substring( pos + 4 ).TrimToNull ();
                }
                else
                {
                    this.RoleName = null;
                }
            }

            // Check mandatory values once more (to be sure that we don't have empty 
            // strings after splitting).
            //
            if ( string.IsNullOrEmpty( this.RealName ) )
            {
                throw new ArgumentException( "Actor's real name must not be null" );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Method ToString() ]

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            sb.Append( this.RealName );

            if ( ! string.IsNullOrEmpty( this.RoleName ) )
            {
                sb.Append( " " + NameDelimiterEmitted + " " ).Append( this.RoleName );
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constants and Parameters ]

        /// <summary>
        /// Name delimiter (between actor's real name and its role) that is used when 
        /// converting MovieActor to string.
        /// </summary>
        /// <example>
        /// Actor's real name:      Keir Dullea
        /// Actor's role:           Dr Dave Bowman
        /// ToString() may return:  Keir Dullea As Dr Dave Bowman
        /// Compound name may be:   Keir Dullea as Dr Dave Bowman
        /// </example>
        /// 
        private static readonly string NameDelimiterEmitted = "as";

        /// <summary>
        /// Name delimiter that is used when parsing compound name.
        /// </summary>
        /// <remarks>
        /// Must be lowercase.
        /// </remarks>
        /// 
        private static readonly string NameDelimiterParsed = "as";

        #endregion
    }
}