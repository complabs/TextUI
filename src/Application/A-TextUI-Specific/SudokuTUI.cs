/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 *  (TextUI Part Only)
 * --------------------------------------------------------------------------------------
 *  File:       SudokuTUI.cs
 *  Created:    2011-03-15
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if TEXTUI // <------ !!!

using System;
using System.Diagnostics;

using Mbk.Commons;

using TextUI;
using TextUI.Controls;
using TextUI.Drawing;

/// <summary>
/// Represents the visual part of the Sudoku puzzle.
/// </summary>
/// 
internal class SudokuForm : Control
{
    /////////////////////////////////////////////////////////////////////////////////

    #region [ Fields and Private Properties ]

    // Instance of the Sudoku puzzle.
    //
    private Sudoku Matrix { get; set; }

    // Number of rows and columns in the puzzle
    //
    private int RowCount { get; set; }
    private int ColumnCount { get; set; }

    /// <summary>
    /// Locked (read-only) cells.
    /// </summary>
    /// 
    private bool[,] Locked { get; set; }

    // Grid of controls that corresponds cells in the puzzle
    //
    private ButtonBase[,] Grid { get; set; }

    // Top-left position of the ButtonBase grid in the window.
    // 
    private int GridTop { get; set; }
    private int GridLeft { get; set; }

    // Cell dimensions (in characters)
    //
    private int CellWidth { get; set; }
    private int CellHeight { get; set; }

    /////////////////////////////////////////////////////////////////////////////////

    // Current row and column (where the cursor is)
    private int    currentRow;
    private int    currentColumn;

    // Temporary message shown to the user (shown 'til next key down event occurs)
    private string tempMessage;

    // The last elapsed time (needed to solve the puzzle)
    //
    private string elapsedTime;

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Public Properties ]

    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the current row.
    /// </summary>
    /// 
    public int CurrentRow
    {
        get
        {
            return this.currentRow;
        }
        set
        {
            InvalidateIf( value != this.currentRow );
            this.currentRow = value;
        }
    }

    /// <summary>
    /// Gets or sets the current column.
    /// </summary>
    /// 
    public int CurrentColumn
    {
        get
        {
            return this.currentColumn;
        }
        set
        {
            InvalidateIf( value != this.currentColumn );
            this.currentColumn = value;
        }
    }

    /// <summary>
    /// Gets or sets the temporary message (shown until first key down event occurs).
    /// </summary>
    /// 
    public string TempMessage
    {
        get
        {
            return this.tempMessage;
        }
        set
        {
            InvalidateIf( value != this.tempMessage );
            this.tempMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets verbose elapsed time needed to solve the last puzzle.
    /// </summary>
    /// 
    public string ElapsedTime
    {
        get
        {
            return this.elapsedTime;
        }
        set
        {
            InvalidateIf( value != this.elapsedTime );
            this.elapsedTime = value;
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the SudokuForm class.
    /// </summary>
    /// 
    public SudokuForm( int cellWidth = 6, int cellHeight = 4 )
    {
        this.ForwadKeysToParent = true;

        this.Matrix = new Sudoku();
        SetupLockedCells ();

        this.CellWidth = cellWidth;
        this.CellHeight = cellHeight;

        this.RowCount = this.Matrix.RowCount;
        this.ColumnCount = this.Matrix.ColumnCount;

        this.CurrentRow = this.RowCount / 2;
        this.CurrentColumn = this.ColumnCount / 2;

        this.GridLeft = 1;
        this.GridTop = 2;

        this.FileName = null;
        this.TempMessage = null;
        this.ElapsedTime = null;

        this.Grid = new ButtonBase[ this.RowCount, this.ColumnCount ];

        this.Name = "Sudoku";
        this.Width = this.ColumnCount * this.CellWidth + 3;
        this.Height = this.RowCount * this.CellHeight + 3;

        this.DrawContents += new DrawEventHandler( SudokuTUI_DrawContents );

        for ( int i = 0; i < RowCount; ++i )
        {
            for ( int j = 0; j < ColumnCount; ++j )
            {
                ButtonBase cell = this.Grid[ i, j ] = new ButtonBase()
                {
                    Name = "Sudoku-R" + (i + 1) + "C" + (j + 1),
                    Parent = this, ForwadKeysToParent = true,
                    Left = this.GridLeft + 1 + j * cellWidth, 
                    Top  = this.GridTop + 1 + i * cellHeight,
                    Width = cellWidth - 1, Height = cellHeight - 1,
                    Border = false, TabStop = false,
                    CursorVisible = true,
                    ToolTipText =  "Row " + (i + 1) + ", Column " + (j + 1),
                };

                SetupCell( i, j );
            }
        }

        this.KeyDown += new KeyEventHandler( SudokuTUI_KeyDown );

        this.Grid[ this.CurrentRow, this.CurrentColumn ].Focus ();

        UpdateCells ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /// <summary>
    /// Updates locked (read-only) cells based on Matrix contents.
    /// </summary>
    /// <remarks>
    /// Locked cells become cells that are currently not empty.
    /// </remarks>
    /// 
    private void SetupLockedCells ()
    {
        Locked = new bool[ Matrix.RowCount, Matrix.ColumnCount ];

        for ( int i = 0; i < Matrix.RowCount; ++i )
        {
            for ( int j = 0; j < Matrix.ColumnCount; ++j )
            {
                Locked[ i, j ] = Matrix[ i, j ] != 0;
            }
        }
    }

    /// <summary>
    /// Updates visual styles of the cells depending on whether cell value is in
    /// conflict with other cells or not.
    /// </summary>
    /// 
    private void UpdateCells ()
    {
        for ( int i = 0; i < RowCount; ++i )
        {
            for ( int j = 0; j < ColumnCount; ++j )
            {
                SetupCell( i, j );
            }
        }
        Invalidate ();
    }

    /// <summary>
    /// Updates visual styles of the specified cell depending on whether the cell 
    /// value is in conflict with other cells or not.
    /// </summary>
    /// 
    private void SetupCell( int row, int col )
    {
        Window cell = Grid[ row, col ];
        int value = Matrix[ row, col ]; 

        cell.Text = value == 0 ? null : "\r\n".PadRight( 1 + CellWidth / 2 ) + value;
        cell.CursorTop = 1;
        cell.CursorLeft = value == 0 ? 2 : 3;

        Matrix[ row, col ] = 0;
        bool noConflict = value == 0 || Matrix.CanPlaceValueInCell( row, col, value );
        Matrix[ row, col ] = value;

        cell.CursorVisible = ! Locked[ row, col ];

        if ( Locked[ row, col ] && noConflict )
        {
            cell.BackColor = Color.DarkBlue;
            cell.ForeColor = Color.Cyan;
            cell.BackColorInact = Color.DarkBlue;
            cell.ForeColorInact = Color.Cyan;
        }
        else if ( Locked[ row, col ] )
        {
            cell.BackColor = Color.DarkRed;
            cell.ForeColor = Color.Cyan;
            cell.BackColorInact = Color.DarkRed;
            cell.ForeColorInact = Color.Cyan;
        }
        else if ( noConflict )
        {
            cell.BackColor = Color.Black;
            cell.ForeColor = Color.Gray;
            cell.BackColorInact = Color.DarkBlue;
            cell.ForeColorInact = Color.Gray;
        }
        else
        {
            cell.BackColor = Color.DarkRed;
            cell.ForeColor = Color.White;
            cell.BackColorInact = Color.DarkRed;
            cell.ForeColorInact = Color.Gray;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// DrawContents event handler that displays background of the form (the grid) and
    /// current status message. 
    /// </summary>
    /// 
    private void SudokuTUI_DrawContents ( object sender, DrawEventArgs e )
    {
        Screen screen = e.Screen;

        int w = ColumnCount * CellWidth + 1;
        int h = RowCount * CellHeight + 1;

        string status = string.Empty;

        if ( ! Matrix.Consistent )
        {
            screen.ForeColor = Color.Red;
            status = "Inconsistent grid...";
        }
        else if ( Matrix.Solved )
        {
            screen.ForeColor = Color.Green;
            status = ElapsedTime == null 
                    ? "Solved!" 
                    : "Solved in " + ElapsedTime + "...";
        }
        else if ( TempMessage != null )
        {
            screen.ForeColor = Color.Red;
            status = TempMessage;
        }
        else
        {
            screen.ForeColor = Color.DarkGray;
            status = "Press F1 or H for help...";
        }

        screen.SetCursorPosition( 3 + ( Width - 6 - status.Length ) / 2, 1 );
        screen.Write( status );

        screen.ForeColor = Color.DarkCyan;

        screen.DrawRectangle( 2, 0, Width - 4, 3, BoxLines.NotJoined );
        screen.DrawRectangle( GridLeft, GridTop, w, h, BoxLines.NotJoined );

        // Draw lines dividing rows
        //
        for ( int i = 1; i < RowCount; ++i )
        {
            screen.DrawRectangle( GridLeft, GridTop + i * CellHeight, w, 1, BoxLines.Joined );
        }

        // Draw lines dividing columns
        //
        for ( int j = 1; j < ColumnCount; ++j )
        {
            screen.DrawRectangle( GridLeft + j * CellWidth, GridTop, 1, h, BoxLines.Joined );
        }

        // Draw highlighed lines dividing rows

        screen.ForeColor = Color.Gray;

        for ( int i = 3; i < RowCount; i += 3 )
        {
            screen.DrawRectangle( GridLeft, GridTop + i * CellHeight, w, 1, BoxLines.Joined );
        }

        // Draw lines dividing columns
        //
        for ( int j = 3; j < ColumnCount; j += 3 )
        {
            screen.DrawRectangle( GridLeft + j * CellWidth, GridTop, 1, h, BoxLines.Joined );
        }

        screen.DrawRectangle( GridLeft, GridTop, w, h, BoxLines.Joined );

        screen.BackColor = Color.DarkBlue;
        screen.ForeColor = Matrix.Consistent ? Color.Green : Color.DarkRed;

        screen.DrawRectangle( GridLeft + CurrentColumn * CellWidth,
            GridTop + CurrentRow * CellHeight,
            CellWidth + 1, CellHeight + 1, BoxLines.NotJoined );
    }

    /// <summary>
    /// Handles the KeyDown event for the SudokuForm.
    /// </summary>
    /// 
    private void SudokuTUI_KeyDown ( object sender, KeyEventArgs e )
    {
        if ( e.Handled )
        {
            return;
        }

        if ( Parent == null )
        {
            return;
        }

        if ( char.IsDigit( e.Character ) )
        {
            ElapsedTime = null;
            if ( ! Locked[ CurrentRow, CurrentColumn ] )
            {
                Matrix[ CurrentRow, CurrentColumn ] = (int)( e.Character - '0' );
                UpdateCells ();
            }
            e.StopHandling ();
            return;
        }

        // Reset 'solve failed' flag on each key press. This ensures
        // that potential (temporary) 'solve failed' message disappears.
        //
        TempMessage = null; 

        // Move window on shift + arrows
        //
        if ( e.Shift )
        {
            switch( e.KeyCode )
            {
                case Keys.Left:
                    if ( Left > -5 )
                    {
                        --Left;
                    }
                    e.StopHandling ();
                    return;
                case Keys.Right:
                    if ( Left < Parent.Width - Width + 5 )
                    {
                        ++Left;
                    }
                    e.StopHandling ();
                    return;
                case Keys.Up:
                    if ( Top > -5 )
                    {
                        --Top;
                    }
                    e.StopHandling ();
                    return;
                case Keys.Down:
                    if ( Top < Parent.Height - Height + 5 )
                    {
                        ++Top;
                    }
                    e.StopHandling ();
                    return;
            }
        }

        // Display hint
        //
        if ( e.KeyCode == Keys.Applications )
        {
            string hint = string.Empty;

            int oldValue = Matrix[ CurrentRow, CurrentColumn ];
            Matrix[ CurrentRow, CurrentColumn ] = 0;

            for ( int i = 1; i <= 9; ++i )
            {
                if ( Matrix.CanPlaceValueInCell( CurrentRow, CurrentColumn, i ) )
                {
                    hint += i.ToString ();
                }
            }

            Matrix[ CurrentRow, CurrentColumn ] = oldValue;

            TempMessage = "Hint: " + ( hint == string.Empty ? "Impossible" : hint );

            e.StopHandling ();
            return;
        }

        // Proceed to parse 'pure' keys (pressed without any modifier)
        //
        if ( e.Modifiers != 0 )
        {
            return;
        }
                
        switch( e.KeyCode )
        {
            case Keys.Down:
                if ( CurrentRow < Matrix.RowCount - 1 )
                {
                    ++CurrentRow;
                }
                Grid[ CurrentRow, CurrentColumn ].Focus ();
                e.StopHandling ();
                break;

            case Keys.Up:
                if ( CurrentRow > 0 )
                {
                    --CurrentRow;
                }
                Grid[ CurrentRow, CurrentColumn ].Focus ();
                e.StopHandling ();
                break;

            case Keys.Right:
                if ( CurrentColumn < Matrix.ColumnCount - 1 )
                {
                    ++CurrentColumn;
                }
                Grid[ CurrentRow, CurrentColumn ].Focus ();
                e.StopHandling ();
                break;

            case Keys.Left:
                if ( CurrentColumn > 0 )
                {
                    --CurrentColumn;
                }
                Grid[ CurrentRow, CurrentColumn ].Focus ();
                e.StopHandling ();
                break;

            case Keys.PageUp:
            case Keys.Add:
            case Keys.OemPlus:
                //
                // Increment current cell value
                //
                ElapsedTime = null;
                if ( ! Locked[ CurrentRow, CurrentColumn ] )
                {
                    Matrix[ CurrentRow, CurrentColumn ] =
                        ( Matrix[ CurrentRow, CurrentColumn ] + 1 ) % 9;
                    UpdateCells ();
                }
                e.StopHandling ();
                break;

            case Keys.PageDown:
            case Keys.Subtract:
            case Keys.OemMinus:
                //
                // Decrement current cell value
                //
                ElapsedTime = null;
                if ( ! Locked[ CurrentRow, CurrentColumn ] )
                {
                    Matrix[ CurrentRow, CurrentColumn ] =
                        ( Matrix[ CurrentRow, CurrentColumn ] + 8 ) % 9;
                    UpdateCells ();
                }
                e.StopHandling ();
                break;

            case Keys.X:
                //
                // Lock/Unlock cell
                //
                Locked[ CurrentRow, CurrentColumn ] = 
                    ! Locked[ CurrentRow, CurrentColumn ];
                UpdateCells ();
                e.Handled = true;
                break;

            case Keys.Q:
            case Keys.Escape:
                //
                // Hide the puzzle
                //
                Parent =  null;
                e.StopHandling ();
                break;

            case Keys.Delete:
            case Keys.Spacebar:
            case Keys.Back:
                //
                // Clear current cell
                //
                ElapsedTime = null;
                if ( ! Locked[ CurrentRow, CurrentColumn ] )
                {
                    Matrix[ CurrentRow, CurrentColumn ] = 0;
                    UpdateCells ();
                }
                e.StopHandling ();
                break;

            case Keys.C:
                //
                // Clear the puzzle
                //
                ElapsedTime = null;
                TempMessage = "Contents cleared...";
                Matrix = new Sudoku ();
                SetupLockedCells ();
                UpdateCells ();
                e.StopHandling ();
                break;

            case Keys.I:
                //
                // Initialize the puzzle with AI Escargot
                //
                ElapsedTime = null;
                LoadAiEscargot ();
                e.StopHandling ();
                break;

            case Keys.L:
                //
                // Load puzzle from file
                //
                ElapsedTime = null;
                if ( this.FileName != null )
                {
                    Load( this.FileName );
                }
                e.StopHandling ();
                break;

            case Keys.H:
            case Keys.F1:
                //
                // Display Help
                //
                {
                    Window help = new Window ()
                    {
                        Name = "SudokuHelp", Parent =  this,
                        Border = true, BorderForeColor = Color.Green,
                        ForeColor = Color.DarkGreen
                    };

                    help.Maximize ();

                    help.KeyDown += new KeyEventHandler( EH_QuitOnEscape );
                    help.DrawContents += new DrawEventHandler( Help_DrawContents );
                }
                e.StopHandling ();
                break;

            case Keys.S:
            case Keys.F5:
                //
                // Try to solve the puzzle
                //
                ElapsedTime = null;
                if ( Matrix.Consistent ) 
                {
                    Stopwatch sw = new Stopwatch ();
                    sw.Start ();

                    if ( ! Matrix.Solve () )
                    {
                        TempMessage = "Sudoku does not have solution...";
                    }

                    sw.Stop ();

                    double ms = (double)sw.ElapsedTicks * 1e3 
                                / (double)Stopwatch.Frequency;

                    if ( ms < 1 ) // very fast
                    {
                        ElapsedTime = string.Format( "{0:N0} ns", ms * 1e3 );
                    }
                    else
                    {
                        ElapsedTime = string.Format( "{0:N0} ms", ms );
                    }
                }
                UpdateCells ();
                e.StopHandling ();
                break;
        }
    }

    /// <summary>
    /// Handles the Enter or Escape KeyDown events and closes the sender window.
    /// </summary>
    /// 
    private void EH_QuitOnEscape ( object sender, KeyEventArgs e )
    {
        switch ( e.KeyCode )
        {
            case Keys.Enter:
            case Keys.Escape:
                if ( sender is Window )
                {
                    Window w = (Window)sender;
                    w.Parent =  null;
                    e.StopHandling ();
                }
                break;
        }
    }

    /// <summary>
    /// Draws contents of the Help window.
    /// </summary>
    /// 
    void Help_DrawContents ( object sender, DrawEventArgs e )
    {
        Screen screen = e.Screen;

        string title = "SUDOKU -- Short Help";

        screen.ForeColor = Color.Green;
        screen.DrawRectangle( 0, 3, Width, 1, BoxLines.NotJoined );
        screen.SetCursorPosition( ( Width - title.Length ) / 2, 1 );
        screen.WriteLine( title );

        screen.ForeColor = Color.Gray;
        screen.SetCursorPosition( 0, 4 );
        screen.WriteLine( VideoRentalOutlet_TUI.A_TextUI_Specific.Resources.SudokuHelp );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Public Methods ]

    /// <summary>
    /// Loads Sudoku puzzle from file. For the file format
    /// see <see cref="Sudoku.LoadMatrixFromFile"/>.
    /// </summary>
    /// 
    public void Load( string filename )
    {
        this.FileName = filename;

        Focus ();

        bool successfullyLoaded = Matrix.LoadMatrixFromFile( filename );

        SetupLockedCells ();
        UpdateCells ();
        Invalidate ();

        if ( successfullyLoaded )
        {
            TempMessage = "Loaded " + filename;
        }
        else if ( Matrix.Errors.Length > 0 && Matrix.Warnings.Length == 0 )
        {
            MessageBox.Show( 
                "Failed to load Sudoku from file '" + filename 
                    + "'...\n\n" + Matrix.Errors,
                "Sudoku Error", MessageBoxButtons.OK, MessageBoxIcon.Hand );
        }
        else
        {
            DialogResult rc = MessageBox.Show( 
                "There were some warnings while loading file.\n\n"
                    + "Do you want to see report?", 
                "Sudoku Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand );

            if ( rc == DialogResult.OK )
            {
                TextBox messageBox = new TextBox ()
                {
                    Name = "SudokuError", Parent =  Application.Screen.RootWindow, 
                    Caption = "Sodoku Loading Report",
                    Multiline = true, VerticalScrollBar = true, Border = true,
                    ReadOnly = true, Top = 3, Left = 1,
                    Width = Application.Screen.RootWindow.Width - 4, 
                    Height = Application.Screen.RootWindow.Height - 5,
                    BorderForeColor = Color.DarkRed, BackColor = Color.DarkRed, 
                    ForeColor = Color.Gray, ScrollBarForeColor = Color.Gray,
                    CaptionForeColor = Color.Yellow,
                    Text = Matrix.Errors + Matrix.Warnings,
                    ToolTipText = "Press Escape to quit browsing the report... ",
                    CurrentRow = int.MaxValue/2, CurrentColumn = int.MaxValue/2
                };

                messageBox.KeyDown += new KeyEventHandler( EH_QuitOnEscape );
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes AI Escargot Sudoku puzzle.
    /// </summary>
    /// 
    public void LoadAiEscargot ()
    {
        TempMessage = "Loaded 'AI Escargot' Sudoku puzzle...";

        Matrix.Matrix = new int[ 9, 9 ] {
            { 1 , 0 , 0 , 0 , 0 , 7 , 0 , 9 , 0 },
            { 0 , 3 , 0 , 0 , 2 , 0 , 0 , 0 , 8 },
            { 0 , 0 , 9 , 6 , 0 , 0 , 5 , 0 , 0 },
            { 0 , 0 , 5 , 3 , 0 , 0 , 9 , 0 , 0 },
            { 0 , 1 , 0 , 0 , 8 , 0 , 0 , 0 , 2 },
            { 6 , 0 , 0 , 0 , 0 , 4 , 0 , 0 , 0 },
            { 3 , 0 , 0 , 0 , 0 , 0 , 0 , 1 , 0 },
            { 0 , 4 , 0 , 0 , 0 , 0 , 0 , 0 , 7 },
            { 0 , 0 , 7 , 0 , 0 , 0 , 3 , 0 , 0 },
        };

        SetupLockedCells ();
        UpdateCells ();
        Invalidate ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////
}

#endif