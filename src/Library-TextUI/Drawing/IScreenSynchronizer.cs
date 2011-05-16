/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI.Drawing
 *  File:       IScreenSynchronizer.cs
 *  Created:    2011-03-15
 *  Modified:   2011-03-19
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

namespace TextUI.Drawing
{
    /// <summary>
    /// Syncrhonization interface for the Screen class.
    /// It is used to sync contents of the Screen and actual media, where
    /// actual media may be e.g. .NET Console, an ANSI terminal via TCP connection
    /// or similar.
    /// </summary>
    /// 
    public interface IScreenSynchronizer
    {
        /// <summary>
        /// Called when sync begins.
        /// </summary>
        /// 
        void BeginUpdate ();

        /// <summary>
        /// Called when sync ends.
        /// </summary>
        /// 
        void EndUpdate ();

        /// <summary>
        /// Called to sync between screen and media.
        /// </summary>
        /// 
        void Update ();

        /// <summary>
        /// Clears media
        /// </summary>
        ///
        void Clear ();

        /// <summary>
        /// Enters full screen on media.
        /// </summary>
        /// 
        void EnterFullscreen ();

        /// <summary>
        /// Navigates to the log area on media.
        /// </summary>
        /// 
        void GotoLogArea ();
    }
}