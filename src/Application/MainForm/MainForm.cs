/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       MainForm.cs
 *  Created:    2011-04-26
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Reflection;

using Mbk.Commons;
using VROLib;

#if TEXTUI
    using TextUI;
#else
    using System.Windows.Forms;
#endif

/////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Represents the main form of the Multiple-Document Interface (MDI)
/// Video Rental Outlet Application, common both for TextUI and GUI mode.
/// </summary>
/// 
[System.ComponentModel.DesignerCategory("Code")]
public partial class MainForm : Form
{
    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new main form of the Video Rentl Outlet application.
    /// </summary>
    /// 
    public MainForm () 
        : base ()
    {
        FormBase.MainForm = this;

        /////////////////////////////////////////////////////////////////////////////////

        InitializeComponents ();

        InitializeMainMenu ();

        /////////////////////////////////////////////////////////////////////////////////

        VideoStore = new VideoRentalOutlet( "No Name", "(No VAT)" );
        VideoStore.Changed += VideoStore_Changed;

        UpdateTitle ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Application Title and version ]

    /// <summary>
    /// Gets the application title consisting of application name and video store name.
    /// </summary>
    /// 
    public string ApplicationTitle { get; private set; }

    public void UpdateTitle ()
    {
        this.Text = ApplicationTitle = "Video Rental Outlet: " + VideoStore.Name;
    }

    /// <summary>
    /// Gets assembly version info in human-readable form.
    /// </summary>
    ///
    private string GetVerboseVersionInfo ()
    {
        Assembly assembly = Assembly.GetAssembly( this.GetType() );
        AssemblyName assemblyName = assembly.GetName ();
        Version version = assemblyName.Version;

        string info = "KTH ID132V Laboration 4\n"
                    + "Video Rental Outlet Application\n"
                    + "Version " + version.Major + "." + version.Minor
                    + ", Build " + version.Build;
        return info;
    }

    /// <summary>
    /// Get the full information (name, version, license etc) about the application.
    /// </summary>
    ///
    private string GetInfoAboutApplication ()
    {
        Assembly assembly = Assembly.GetAssembly( this.GetType() );
        AssemblyName assemblyName = assembly.GetName();
        Version version = assemblyName.Version;

        string info = "KTH ID132V Laboration 4\n\n"
            + "Video Rental Outlet Application\n\n"
            + "    Version: " + version.Major + "." + version.Minor + "\n"
            + "    Build: " + version.Build + "\n\n"
            + "Author:\n\n"
            + "    Mikica B Kocic\n\n"
            + "License:\n\n"
            + "    GNU General Public License\n"
            + "    http://creativecommons.org/licenses/GPL/2.0/\n";

        return info;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Common Utilities ]

    /// <summary>
    /// Unload all MDI child forms (derived from FormBase).
    /// </summary>
    /// 
    private void UnloadAllMdiChildForms ()
    {
        MdiClient.IfValidateOk( () =>
        {
            ExecuteFor<FormBase>( f => f.QuitForm () );
        } );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Displays the report box with a specified contents and caption. 
    /// </summary>
    /// 
    private void ShowReport( string caption, string report, bool modal = false )
    {
        MdiClient.IfValidateOk( () =>
        {
            MyReportBox.ShowReport( caption, report, 700, this.Height - 50 );
        } );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////
}