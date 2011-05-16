/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 *  (GUI Part Only)
 * --------------------------------------------------------------------------------------
 *  Project:    Unknown
 *  Namespace:  nothing
 *  File:       GuiExtensionMethods.cs
 *  Created:    2011-03-31
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if ! TEXTUI // <------ !!!

using System;
using System.Collections.Generic;
using System.Windows.Forms;

/// <summary>
/// Static class holding common extension methods for the application in GUI
/// mode (e.g. for compatibility with existing methods found in TextUI library).
/// </summary>
/// 
internal static class GuiExtensionMethods
{
    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Forces validation of the window and executes delegate if 
    /// validation was successfull.
    /// </summary>
    /// <param name="method">Delegate to be executed if validation was ok.</param>
    /// <returns>True if validation was successfull.</returns>
    ///
    public static bool IfValidateOk( this MdiClient control, Action method = null )
    {
        Form parent = control.Parent as Form;

        if ( parent != null )
        {
            MyMdiForm mdiForm = parent.ActiveMdiChild as MyMdiForm;

            if ( mdiForm != null )
            {
                return mdiForm.IfValidateOk( method );
            }
        }

        if ( method != null )
        {
            method ();
        }

        return true;
    }
    
    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Shows tab indexes for all contained (and nested) controls.
    /// </summary>
    /// 
    [System.Diagnostics.Conditional("DEBUG")]
    public static void DumpTabIndexes( this Control parent, int level = 0 )
    {
        for ( int i = 0; i < parent.Controls.Count; ++i )
        {
            Control child = parent.Controls[i];

            System.Diagnostics.Trace.TraceInformation( 
                "{0} TabIndex {1}: {2}", 
                "".PadRight( level * 4 ), child.TabIndex, child );

            child.DumpTabIndexes( level + 1 );
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
}

#endif