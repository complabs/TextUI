/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 * --------------------------------------------------------------------------------------
 *  File:       FileStreamWithProgressBar.cs
 *  Created:    2011-04-16
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.IO;
using System.Diagnostics;

#if TEXTUI
    using TextUI;
    using TextUI.Controls;
#else
    using System.Drawing;
    using System.Windows.Forms;
#endif

/// <summary>
/// Represents the file stream connected to a progress bar, where the file
/// loading/saving progress is shown.
/// </summary>
/// 
public class FileStreamWithProgressBar : FileStream
{
    #region [ Fields ]

    // Used to calculate maximumTicks for updates.
    //
    private static readonly double Resolution = 32e3; // bytes

    /////////////////////////////////////////////////////////////////////////////////////

    // Associated progress bar. May be null.
    //
    private ProgressBar progressBar;

    // Predetermined file length (in case when loading file) or guessed file length
    // (when saving file).
    //
    private long fileLength;

    // Maximum number of ticks that will be trigger during updates. Depends on 
    // actual file length (as it is calculated as file length divided by the 
    // Resolution constant).
    //
    private int maximumTicks;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets elapsed time in milliseconds since object initialization.
    /// </summary>
    /// 
    public double ElapsedMilliseconds
    {
        get 
        {
            return (double)this.stopWatch.ElapsedTicks * 1e3 
                    / (double)Stopwatch.Frequency;
        }
    }

    private Stopwatch stopWatch;

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets elapsed time in human readable form, appropriatelly scaled to nano seconds, 
    /// milliseconds or seconds.
    /// </summary>
    /// 
    public string VerboseElapsedTime
    {
        get 
        {
            double ms = ElapsedMilliseconds;
            return ms <    1 ? string.Format( "{0:N1} ns" , ms * 1e3 )
                    : ms <    2 ? string.Format( "{0:N3} ms" , ms       )
                    : ms < 1000 ? string.Format( "{0:N1} ms" , ms       )
                    : ms < 2000 ? string.Format( "{0:N3} sec", ms / 1e3 )
                                : string.Format( "{0:N1} sec", ms / 1e3 );
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the FileStreamWithProgressBar class 
    /// with the specified file path and creation mode, connected
    /// to a specified progress bar with an optional (predetermined) file length. 
    /// </summary>
    /// 
    public FileStreamWithProgressBar( string filename, FileMode fileMode, 
        ProgressBar progressBar, long fileLength = 0 )
        : base( filename, fileMode )
    {
        this.stopWatch = new Stopwatch ();
        this.stopWatch.Start ();

        this.progressBar = progressBar;

        if ( fileLength != 0 )
        {
            this.fileLength = fileLength;
        }
        else
        {
            FileInfo fileInfo = new FileInfo( filename );
            this.fileLength = fileInfo.Exists ? fileInfo.Length : 0;
        }

        if ( this.fileLength != 0 && this.fileLength < Resolution )
        {
            if ( this.progressBar != null )
            {
                this.progressBar.Value = this.progressBar.Maximum;
                this.progressBar.Refresh ();
            }

            // Turn-off progress bar if file length is specified but it falls
            // under minimum resolution
            //
            this.progressBar = null;
        }
        else if ( this.progressBar != null )
        {
            this.maximumTicks = this.progressBar.Width / Em.Width;
            this.nextTick = 0;
            this.deltaNextTick = 1;
            this.nextTickPosition = 0;

            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = this.maximumTicks;

            // Adjust next tick resolution according to file length
            //
            if ( this.fileLength > 0 )
            {
                double res = this.maximumTicks * Resolution / this.fileLength;
                if ( res > 1 )
                {
                    this.deltaNextTick = (int)res;
                }
            }
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods: OnProgress() ]

    /// <summary>
    /// Triggered when file IO operation occurs to update progress bar status.
    /// </summary>
    /// 
    private void OnProgress ()
    {
        if ( this.progressBar != null 
            && this.Position >= nextTickPosition 
            && this.nextTick < this.progressBar.Maximum )
        {
            this.nextTick += this.deltaNextTick;
            this.progressBar.Value = Math.Max( this.progressBar.Minimum, 
                Math.Min( this.progressBar.Maximum, this.nextTick ) );

            #if TEXTUI
                Application.StatusBarWindow.ForeColorInact = this.progressBar.ForeColor;
                Application.StatusBarWindow.Text = "Elapsed " + VerboseElapsedTime;
            #endif

            this.progressBar.Refresh ();

            long maxPos = this.fileLength == 0 ? this.Length * 2 : this.fileLength;

            this.nextTickPosition = this.nextTick * maxPos / this.maximumTicks;
        }
    }

    #region [ Private fields used by OnProgress ]

    private int nextTick; // next tick when progress bar update will happen
    private int deltaNextTick; // nextTick increment
    private double nextTickPosition; // corresponding position in file to nextTick

    #endregion

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [ Public Members ]

    /// <summary>
    /// Stops the stopwatch timer that was started at the time this object was created.
    /// </summary>
    /// 
    public void StopTimer ()
    {
        this.stopWatch.Stop ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////

    #region [  Overriden FileStream Methods ]
    
    /// <summary>
    /// Releases the unmanaged resources used by the FileStream and optionally releases
    /// the managed resources.
    /// </summary>
    ///
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            stopWatch.Stop ();
        }

        base.Dispose( disposing );

        if ( this.progressBar != null && this.nextTick > 0 )
        {
            this.progressBar.Value = this.progressBar.Maximum;
            this.progressBar.Refresh ();

            // Ensure that the whole loading process takes at least 100 ms
            // so the user could see our fancy progress bar...
            //
            int sleep = (int)( 100 - ElapsedMilliseconds );
            if ( sleep > 0 )
            {
                System.Threading.Thread.Sleep( sleep );
            }
        }
    }

    /// <summary>
    /// Reads a block of bytes from the stream and writes the data in a given buffer.
    /// </summary>
    /// 
    public override int Read( byte [] array, int offset, int count )
    {
        int rc = base.Read( array, offset, count );
        OnProgress ();
        return rc;
    }

    /// <summary>
    /// Reads a byte from the file and advances the read position one byte.
    /// </summary>
    ///
    public override int ReadByte ()
    {
        int rc = base.ReadByte();
        OnProgress ();
        return rc;
    }

    /// <summary>
    /// Writes a block of bytes to this stream using data from a buffer.
    /// </summary>
    /// 
    public override void Write( byte[] array, int offset, int count )
    {
        base.Write( array, offset, count );
        OnProgress ();
    }

    /// <summary>
    /// Writes a byte to the current position in the file stream. 
    /// </summary>
    ///
    public override void WriteByte( byte value )
    {
        base.WriteByte( value );
        OnProgress ();
    }

    #endregion
}