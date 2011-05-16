/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  Project:    Unknown
 *  Namespace:  VideoRentalOutlet_GUI.Properties
 *  File:       Settings.cs
 *  Created:    2011-04-29
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System.Configuration;
using System.ComponentModel;

namespace VideoRentalOutlet_GUI.Properties
{
    // This class allows you to handle specific events on the settings class:
    // The SettingChanging event is raised before a setting's value is changed.
    // The PropertyChanged event is raised after a setting's value is changed.
    // The SettingsLoaded event is raised after the setting values are loaded.
    // The SettingsSaving event is raised before the setting values are saved.
    //
    internal sealed partial class Settings
    {
        public Settings ()
        {
            // // To add event handlers for saving and changing settings, 
            // // uncomment the lines below:
            //
            // this.SettingChanging += this.EH_SettingChanging;
            //
            // this.SettingsSaving += this.EH_SettingsSaving;
            //
        }
        
        private void EH_SettingChanging( object sender, SettingChangingEventArgs e )
        {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void EH_SettingsSaving( object sender, CancelEventArgs e )
        {
            // Add code to handle the SettingsSaving event here.
        }
    }
}