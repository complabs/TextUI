/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       ExtensionMethods.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Mbk.Commons
{
    /// <summary>
    /// Static class holding common extension methods in Mbk.Commons namespace.
    /// </summary>
    /// 
    public static class MbkCommonExtensionMethods
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Extends: string ]

        /// <summary>
        /// Trims a string to a null reference, if the string is empty.
        /// </summary>
        ///
        public static string TrimToNull( this string text )
        {
            string t = text.Trim ();
            return string.IsNullOrEmpty( t ) ? null : t;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Trims string from white-spaces also removing all additional inner 
        /// white-spaces from text. This method works also on null strings.
        /// </summary>
        /// <remakrs>
        /// Main purpose of the method is to parse some name (personal or other kind).
        /// </remakrs>
        /// <param name="name">a string that will be trimmed from spaces</param>
        /// <returns>a resulting string having no spaces at the beginning or
        /// end and multiple spaces between words</returns>
        /// 
        public static string TrimmedName( this string name )
        {
            if ( name != null )
            {
                name = Regex.Replace( name.Trim (), @"\s+", " " );

                if ( string.IsNullOrEmpty( name ) )
                {
                    name = null;
                }
            }

            return name;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Extends: string[] ]

        /// <summary>
        /// Trims string array of names (<see cref="TrimmedName"/>).
        /// </summary>
        /// 
        public static List<string> TrimmedNameCollection( this string[] lines )
        {
            List<string> array = new List<string> ();

            foreach ( string t in lines )
            {
                string name = t.TrimmedName ();

                // Do not include empty lines
                //
                if ( ! string.IsNullOrEmpty( name ) )
                {
                    array.Add( name );
                }
            }

            return array;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Extends: Enum ]

        /// <summary>
        /// Returns <see cref="VerboseAttribute"/> associated to an enum, if any; 
        /// otherwise, returns enum's ToString (). In real life, Verbose() should get
        /// values from culture-specific UI application resources instead. However, this 
        /// is educational code and this is good oportunity to show how to deal with C#
        /// attributes and reflections.
        /// </summary>
        /// 
        public static string Verbose( this Enum m )
        {
            // The default value is (specially for non-enums) a result of ToString ()
            //
            if ( ! m.GetType().IsEnum )
            {
                return m.ToString ();
            }

            string defaultValue = m.ToString ();

            // If we have an Enum, and if we can get VerboseAttribute, return
            // the first attribute value.
            //
            FieldInfo fi = m.GetType().GetField( m.ToString () );

            if ( fi != null )
            {
                VerboseAttribute[] attrs = 
                    fi.GetCustomAttributes( typeof(VerboseAttribute), false )
                    as VerboseAttribute[];

                return attrs != null && attrs.Length > 0 ? attrs[0].Value : defaultValue;
            }

            // Probably failure to get field info was caused by multiple flags.
            // Check if Enum is marked as 'Flags', if not then just return 
            // the default value (which may be a number?).
            //
            FlagsAttribute[] flags = 
                m.GetType().GetCustomAttributes( typeof(FlagsAttribute), false )
                as FlagsAttribute[];

            if ( flags == null || flags.Length == 0 )
            {
                return defaultValue;
            }

            // We have Enum marked with flags. Scan all attributes and return
            // flagged values (Verbose() of course). If that fails, we will
            // return hexadecimal representation of enum flag.
            //
            StringBuilder sb = new StringBuilder ();

            ulong intValue = Convert.ToUInt64( m );

            foreach( object value in Enum.GetValues( m.GetType () ) )
            {
                var flag = Convert.ToUInt64( value );

                if ( flag != 0 && ( intValue & flag ) == flag )
                {
                    if ( sb.Length != 0 ) sb.Append( " | " );
                    sb.Append( ( (Enum)value ).Verbose () );

                    intValue &= ~flag;
                }
            }

            if ( intValue != 0 )
            {
                sb.Append( " | " ).Append( intValue.ToString( "X8" ) );
            }

            return sb.Length == 0 ? m.ToString( "x" ) : sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Extends: StringBuilder ]

        /// <summary>
        /// Appends framed (boxed) title using marker character.
        /// </summary>
        /// <param name="sb">a 'this' instance of string builder</param>
        /// <param name="title">a string with title to be boxed</param>
        /// <param name="width">an integer with box width; default is 70</param>
        /// <param name="marker">a box marker; default is '='</param>
        /// <returns>a 'this' instance of string builder</returns>
        ///
        public static StringBuilder AppendTitle( this StringBuilder sb, string title,
            int width = 70, char marker = '=' )
        {
            const int boxThickness = 4;

            int padding = Math.Max( 2, width - title.Length - 2 * boxThickness );
            int leftPadding = padding / 2;
            int rightPadding = padding - leftPadding;

            width = boxThickness * 2 + leftPadding + title.Length + rightPadding;

            sb.Append( marker, width ).AppendLine ();

            sb.Append( marker, boxThickness )
              .Append( ' ', leftPadding )
              .Append( title )
              .Append( ' ', rightPadding )
              .Append( marker, boxThickness )
              .AppendLine ();

            sb.Append( marker, width ).AppendLine ();

            return sb;
        }

        #endregion
    }
}