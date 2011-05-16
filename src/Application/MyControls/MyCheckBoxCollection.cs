/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MyCheckBoxCollection.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;

#if TEXTUI
    using TextUI.Controls;
#else
    using System.Windows.Forms;
#endif

using Mbk.Commons;

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents a collection of check boxes connected to individual bit-flags 
/// of particular Enum datatype.
/// </summary>
/// 
internal class MyCheckBoxCollection
    : List<MyCheckBox>
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the MyCheckBoxCollection for a specified
    /// bit-flags Enum datatype.
    /// </summary>
    ///
    public MyCheckBoxCollection( Type enumType )
    {
        foreach( Enum value in Enum.GetValues( enumType ) )
        {
            ulong intValue = Convert.ToUInt64( value );

            if ( intValue == 0 )
            {
                continue; // skip flag values without bits set
            }

            MyCheckBox cb = new MyCheckBox ()
            {
                Text = value.Verbose (), Tag = intValue
            };

            this.Add( cb );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Gets a value indicating whether contents of any check box item belonging to
    /// the collection has been changed.
    /// </summary>
    /// 
    public bool ContentsChanged
    {
        get
        {
            foreach( MyCheckBox cb in this )
            {
                if ( cb.ContentsChanged )
                {
                    return true;
                }
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets checked values for check boxes in the collection depending on specified
    /// flagged Enum value (i.e. turns on/off individual check boxes representing
    /// individual bit-flags of the enum).
    /// </summary>
    ///
    public void SetValue( Enum value )
    {
        ulong intValue = Convert.ToUInt64( value );

        foreach( MyCheckBox cb in this )
        {
            ulong flag = (ulong)cb.Tag;
            cb.Checked = intValue != 0 && ( intValue & flag ) == flag;
            cb.ContentsChanged = false;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets a combined bit-flag Enum value by or-ing individual enum bit-flag values
    /// associated to check boxes in the collection, depending whether check boxes
    /// are checked or not.
    /// </summary>
    /// 
    public ulong GetValue ()
    {
        ulong intValue = 0;

        foreach( MyCheckBox cb in this )
        {
            if ( cb.Checked )
            {
                intValue |= (ulong)cb.Tag;
            }
        }
        return intValue;
    }

    #endregion
}