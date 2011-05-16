/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       VerboseAttribute.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace Mbk.Commons
{
    /// <summary>
    /// Provides verbose attribute for Enum datatypes.
    /// </summary>
    /// <remarks>
    /// Attributes can be accessed using <see cref="ExtensionMethods.Verbose(Enum)"/> 
    /// as alternative to enum's <see cref="Enum.ToString()"/>. 
    /// Note that in real life Verbose extension method (that returns string) would be 
    /// implemented to get values associated to enums from culture-specific UI registry, 
    /// instead from Verbose attributes.
    /// </remarks>
    /// 
    [AttributeUsage(
        AttributeTargets.All,
        AllowMultiple=false,
        Inherited=true
    )]
    public sealed class VerboseAttribute : Attribute
    {
        public string Value { get; private set; }

        public VerboseAttribute( string value )
        {
            this.Value = value;
        }
    }
}