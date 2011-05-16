/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Application
 *  (GUI Part Only)
 * --------------------------------------------------------------------------------------
 *  File:       SudokuGUI.cs
 *  Created:    2011-02-15
 *  Modified:   2011-05-02
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

#if ! TEXTUI // <------ !!!

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Mbk.Commons;

/// <summary>
/// Represents a Sudoku puzzle MDI form.
/// </summary>
/// 
[System.ComponentModel.DesignerCategory("Code")]
internal class SudokuForm : MyMdiForm
{
    /////////////////////////////////////////////////////////////////////////////////

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the instance of Sudoku puzzle.
    /// </summary>
    /// 
    public Sudoku Matrix { get; private set; }

    /// <summary>
    /// Gets or sets the current row.
    /// </summary>
    /// 
    public int CurrentRow { get; private set; }

    /// <summary>
    /// Gets or sets the current column.
    /// </summary>
    /// 
    public int CurrentColumn { get; private set; }

    /// <summary>
    /// Locked (read-only) cells.
    /// </summary>
    /// 
    private bool[,] Locked { get; set; }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Constructor ]

    /// <summary>
    /// Initializes a new instance of the SudokuForm with an empty puzzle.
    /// </summary>
    /// 
    public SudokuForm ()
        : base ()
    {
        this.CurrentRow = 0;
        this.CurrentColumn = 0;

        this.Matrix = new Sudoku ();
        SetupLockedCells ();

        InitializeComponents ();
    }

    /// <summary>
    /// Initializes default visual components.
    /// </summary>
    /// 
    private void InitializeComponents ()
    {
        this.Name = "Sudoku";

        SetupIcon( this.Name );

        this.Text = this.Name;

        // Setup status strip
        //
        this.StatusStrip.Items.Remove( this.StatusLocker );
        this.Controls.Add( this.StatusStrip );

        // Setup dimensions
        //
        this.ClientSize = new Size( 450, 450 + this.StatusStrip.Height );
        this.MinimumSize = this.Size;
        this.StartPosition = FormStartPosition.CenterScreen;

        // Create key focus holder. We need some control that will receive key events,
        // and in our case it will be read-only text box that is put outside
        // of form client area (note that it must be visible, otherwise it won't
        // receive key events).
        //
        TextBox keyFocusHolder = new TextBox ()
        {
            Parent = this, ReadOnly = true, Top = -30
        };

        // Event Handlers
        //
        this.Resize += ( sender, e ) => Invalidate ();
        this.Paint += new PaintEventHandler( EH_Paint );
        this.MouseDown += new MouseEventHandler( EH_MouseDown );
        this.MouseWheel += new MouseEventHandler( EH_MouseWheel );
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Initializes AI Escargot Sudoku puzzle.
    /// </summary>
    /// 
    public void LoadAiEscargot ()
    {
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

        UpdateStatus( "Loaded 'AI Escargot' puzzle..." );
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Private Methods ]

    /////////////////////////////////////////////////////////////////////////////////

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
    /// Updates status bar depending on the Sudoku puzzle status and contents of the
    /// temporary message
    /// </summary>
    /// 
    private void UpdateStatus( string tempMessage = null )
    {
        Invalidate ();

        ErrorMessage = null;

        if ( ! Matrix.Consistent )
        {
            ErrorMessage = "Inconsistent grid...";
            this.Text = this.Name + ": Inconsistent grid!";
        }
        else if ( tempMessage != null )
        {
            InfoMessage = tempMessage;
            this.Text = this.Name + ": " + tempMessage;
        }
        else if ( Matrix.Solved )
        {
            InfoMessage = "Solved!";
            this.Text = this.Name + ": Solved!";
        }
        else
        {
            InfoMessage = "Press S to solve, I to reload, C to clear all, Esc to quit";
        }
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    #region [ Event Handlers ]

    /// <summary>
    /// Redraws the Sudoku puzzle (grid and cells).
    /// </summary>
    /// 
    private void EH_Paint ( object sender, PaintEventArgs e )
    {
        // Create a local version of the graphics object for the PictureBox.
        Graphics g = e.Graphics;

        g.FillRectangle( Brushes.White, ClientRectangle );

        int rowCount = Matrix.RowCount;
        int colCount = Matrix.ColumnCount;

        Size clientSize = ClientSize - new Size( 0, this.StatusStrip.Height );

        Size cellSize = clientSize;
        cellSize.Height /= rowCount;
        cellSize.Width /= colCount;

        Point topLeft = new Point ();
        topLeft.X = ( clientSize.Width - colCount * cellSize.Width ) / 2;
        topLeft.Y = ( clientSize.Height - rowCount * cellSize.Height ) / 2;

        // Draw lines dividing rows
        //
        for ( int i = 0; i <= rowCount; ++i )
        {
            Pen pen = ( i > 0 && i % 3 == 0 ) 
                    ? Pens.Gray 
                    : Pens.LightGray;

            g.DrawLine( pen, 
                topLeft.X,  topLeft.Y + i * cellSize.Height, 
                topLeft.X + colCount * cellSize.Width, topLeft.Y + i * cellSize.Height );
        }

        // Draw lines dividing columns
        //
        for ( int j = 0; j <= colCount; ++j )
        {
            Pen pen = ( j > 0 && j % 3 == 0 ) 
                    ? Pens.Gray 
                    : Pens.LightGray;

            g.DrawLine( pen, 
                topLeft.X + j * cellSize.Width, topLeft.Y, 
                topLeft.X + j * cellSize.Width, topLeft.Y + rowCount * cellSize.Height );
        }

        using ( Font drawFont = new Font( "Arial", 27 ) )
        using ( StringFormat drawFormat = new StringFormat () )
        {
            drawFormat.Alignment     = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;
            drawFormat.Trimming      = StringTrimming.Character;

            // Draw cell contents
            //
            for ( int i = 0; i < rowCount; ++i )
            {
                for ( int j = 0; j < colCount; ++j )
                {
                    Rectangle cellRectangle = new Rectangle( 
                        topLeft.X + j * cellSize.Width, topLeft.Y + i * cellSize.Height, 
                        cellSize.Width, cellSize.Height );

                    // Determine if the cell is in conflict with other cells.
                    //
                    int value = Matrix[ i, j ]; Matrix[ i, j ] = 0;

                    bool inConflict = value != 0 
                                      && ! Matrix.CanPlaceValueInCell( i, j, value );

                    Matrix[ i, j ] = value;

                    // Paint the cell background
                    //
                    if ( i == CurrentRow && j == CurrentColumn )
                    {
                        cellRectangle.Inflate( -3, -3 );
                        if ( ! Locked[ i, j ] )
                        {
                            g.FillRectangle( Brushes.LightGoldenrodYellow, cellRectangle );
                        }
                        g.DrawRectangle( Pens.DarkGoldenrod, cellRectangle );
                    }
                    else if ( inConflict )
                    {
                        cellRectangle.Inflate( -2, -2 );
                        g.FillRectangle( Brushes.LightPink, cellRectangle );
                    }

                    // Draw the number
                    //
                    Brush brush = Locked[ i, j ] ? Brushes.Black
                        : inConflict ? Brushes.Red : Brushes.Blue;

                    cellRectangle.Offset( 0, 2 );

                    string drawString = String.Format( "{0}", 
                        Matrix[ i, j ] == 0 ? "" : Matrix[ i, j ].ToString () );

                    g.DrawString( drawString, drawFont, brush, cellRectangle, drawFormat);
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the MouseWheel event. Increases/decreases current cell value.
    /// </summary>
    /// 
    void EH_MouseWheel( object sender, MouseEventArgs e )
    {
        if ( e.Delta > 0 )
        {
            --Matrix[ CurrentRow, CurrentColumn ];
            UpdateStatus ();
        }
        else if ( e.Delta < 0 )
        {
            ++Matrix[ CurrentRow, CurrentColumn ];
            UpdateStatus ();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the KeyDown event.
    /// </summary>
    /// <param name="e"></param>
    /// 
    protected override void OnKeyDown ( KeyEventArgs e )
    {
        string tempMessage = null;

        switch( e.KeyCode )
        {
            case Keys.Down:
                ++CurrentRow;
                e.Handled = true;
                break;

            case Keys.Up:
                --CurrentRow;
                e.Handled = true;
                break;

            case Keys.Right:
                ++CurrentColumn;
                e.Handled = true;
                break;

            case Keys.Left:
                --CurrentColumn;
                e.Handled = true;
                break;

            case Keys.Delete:
            case Keys.Space:
            case Keys.Back:
                //
                // Clear cell
                //
                if ( ! Locked[ CurrentRow, CurrentColumn ] )
                {
                    Matrix[ CurrentRow, CurrentColumn ] = 0;
                }
                e.Handled = true;
                break;

            case Keys.X:
                //
                // Lock/Unlock cell
                //
                Locked[ CurrentRow, CurrentColumn ] = 
                    ! Locked[ CurrentRow, CurrentColumn ];
                e.Handled = true;
                break;

            case Keys.Q:
            case Keys.Escape:
                //
                // Quit form
                //
                Close ();
                e.Handled = true;
                break;

            case Keys.C:
                //
                // Clear the puzzle
                //
                tempMessage = "Contents cleared...";
                Matrix = new Sudoku ();
                SetupLockedCells ();
                e.Handled = true;
                break;
            
            case Keys.I:
                //
                // Initialize the puzzle with AI Escargot
                //
                LoadAiEscargot ();
                tempMessage = InfoMessage;
                e.Handled = true;
                break;

            case Keys.D0:
            case Keys.D1:
            case Keys.D2:
            case Keys.D3:
            case Keys.D4:
            case Keys.D5:
            case Keys.D6:
            case Keys.D7:
            case Keys.D8:
            case Keys.D9:
                //
                // Change cell contents
                //
                if ( ! Locked[ CurrentRow, CurrentColumn ] )
                {
                    Matrix[ CurrentRow, CurrentColumn ] = e.KeyCode - Keys.D0;
                }
                e.Handled = true;
                break;

            case Keys.S:
                //
                // Try to solve the puzzle
                //
                if ( Matrix.Consistent ) 
                {
                    Stopwatch sw = new Stopwatch ();
                    sw.Start ();

                    bool solved = Matrix.Solve ();

                    sw.Stop ();

                    if ( ! solved )
                    {
                        tempMessage = "Puzzle does not have a solution...";
                        MessageBox.Show( "Puzzle does not have a solution!", this.Name, 
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                    }
                    else
                    {
                        double ms = (double)sw.ElapsedTicks * 1e3 
                                    / (double)Stopwatch.Frequency;

                        if ( ms < 1 ) // very fast
                        {
                            tempMessage = string.Format( "Solved in {0:N0} ns", ms * 1e3 );
                        }
                        else
                        {
                            tempMessage = string.Format( "Solved in {0:N0} ms", ms );
                        }
                    }
                }
                e.Handled = true;
                break;
        }

        // Keep row/column in valid bounds
        //
        CurrentColumn = Math.Max( 0, Math.Min( Matrix.ColumnCount - 1, CurrentColumn ) );
        CurrentRow    = Math.Max( 0, Math.Min( Matrix.RowCount - 1,    CurrentRow    ) );

        UpdateStatus( tempMessage );

        base.OnKeyDown( e );
    }

    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Handles the MouseDown event. Select current cell depending on the area
    /// where the mouse was clicked.
    /// </summary>
    /// 
    private void EH_MouseDown( object sender, MouseEventArgs e )
    {
        // Get cell size (size of the client area reduced for status strip divided
        // by number of rows/columns).
        //
        Size cellSize = ClientSize - new Size( 0, this.StatusStrip.Height );
        cellSize.Height /= Matrix.RowCount;
        cellSize.Width  /= Matrix.ColumnCount;

        // Translate mouse xy-position to cell indices
        //
        CurrentColumn = e.X / cellSize.Width;
        CurrentRow    = e.Y / cellSize.Height;

        // Keep row/column in valid bounds
        //
        CurrentColumn = Math.Max( 0, Math.Min( Matrix.ColumnCount - 1, CurrentColumn ) );
        CurrentRow    = Math.Max( 0, Math.Min( Matrix.RowCount - 1,    CurrentRow    ) );

        UpdateStatus ();
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////
}

#endif