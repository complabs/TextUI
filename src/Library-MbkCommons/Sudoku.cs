/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       Sudoku.cs
 *  Created:    2011-02-14
 *  Modified:   2011-04-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.IO;
using System.Text;

namespace Mbk.Commons
{
    /// <summary>
    /// Represents the matrix of a Sudoku puzzle.
    /// </summary>
    /// <remarks>
    /// In more general case, the Sudoku class may be used to instantiate any 
    /// Latin Square matrix. The rules of logic consistency are checked in public 
    /// virtual method CanPlaceValueInCell(), which may be overriden in an inherited
    /// class, thus enabling creation of most general Latin Square based games.
    /// </remarks>
    /// <see cref="Sudoku.CanPlaceValueInCell"/>
    /// 
    public class Sudoku
    {
        #region [ Private Fields ]

        /// <summary>
        /// Matrix containing sudoku or latin square numbers aranged in rows and 
        /// columns where empty cells are set to 0.
        /// </summary>
        /// 
        private int[,] matrix = new int[ 9, 9 ];

        /// <summary>
        /// File loading errors
        /// </summary>
        /// 
        private StringBuilder errors;

        /// <summary>
        /// File loading warnings
        /// </summary>
        /// 
        private StringBuilder warnings;

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region [ Public Properties ]

        /// <summary>
        /// Returns verbose type of the matrix (either Sudoku or Latin Square)
        /// </summary>
        /// 
        public string Name
        {
            get 
            {
                return matrix == null ? "Unknown"
                    : matrix.GetLength( 0 ) == 9 ? "Sudoku"
                    : "Latin Square";
            }
        }

        /// <summary>
        /// Returns true if sudoku is solved. Sudoku is solved if it is
        /// fully populated and logicaly consistent.
        /// </summary>
        /// 
        public bool Solved
        {
            get 
            { 
                return FullyPopulated && Consistent;
            }
        }

        /// <summary>
        /// Returns true if there are no empty elements in the matrix.
        /// </summary>
        /// 
        public bool FullyPopulated
        {
            get 
            { 
                foreach( int element in matrix )
                {
                    if ( element == 0 ) {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Returns true if all matrix elements are empty.
        /// </summary>
        /// 
        public bool FullyEmpty
        {
            get 
            { 
                foreach( int element in matrix )
                {
                    if ( element != 0 ) {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Returns true if matrix is logically consistent i.e. having
        /// all numbers in cells not clashing with each other per row, column
        /// or subsquare.
        /// </summary>
        /// <remakrs>
        /// Algorithm: 
        /// <pre>
        ///  Foreach cell in matrix:
        ///    If cell is populated with number
        ///       1) Make cell empty
        ///       2) Check if we violate consistency if we put the same
        ///          number in the cell
        ///       3) Put back original number in the cell
        ///    Inconsistency of any cell makes complete matrix inconsistent.
        /// </pre>
        /// </remakrs>
        /// 
        public bool Consistent
        {
            get 
            {
                bool ok = true;

                for ( int row = 0; ok && row < matrix.GetLength( 0 ); ++row )
                {
                    for ( int col = 0; ok && col < matrix.GetLength( 1 ); ++col )
                    {
                        int n = matrix[ row, col ];

                        if ( n != 0 ) // check only populated elements
                        {
                            matrix[ row, col ] = 0;
                            ok = CanPlaceValueInCell( row, col, n );
                            matrix[ row, col ] = n;
                        }
                    }
                }

                return ok;
            }
        }

        /// <summary>
        /// Gets number of rows in matrix
        /// </summary>
        ///
        public int RowCount { get { return matrix.GetLength( 0 ); } }

        /// <summary>
        /// Gets number of columns in matrix
        /// </summary>
        /// 
        public int ColumnCount { get { return matrix.GetLength( 1 ); } }

        /// <summary>
        /// Returns matrix element
        /// </summary>
        ///
        public int this [ int row, int col ] 
        {
            get { return matrix[ row, col ]; }
            set {  matrix[ row, col ] = Math.Max( 0, Math.Min( RowCount, value ) ); }
        }

        /// <summary>
        /// Gets ors sets matrix for the puzzle
        /// </summary>
        ///
        public int[,] Matrix 
        { 
            get { return this.matrix; } 
            set { this.matrix = value; } 
        }

        /// <summary>
        /// Gets errors reported in the last loading matrix from file
        /// </summary>
        /// 
        public string Errors
        {
            get { return this.errors.ToString (); }
        }

        /// <summary>
        /// Gets warnings reported in the last loading matrix from file
        /// </summary>
        /// 
        public string Warnings
        {
            get { return this.warnings.ToString (); }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Creates an instance of Sudoku class by loading initial 9x9 matrix from 
        /// semicolon separated file.
        /// </summary>
        /// <param name="sourceFile">a string with filename containing sudoku</param>
        /// 
        public Sudoku ()
        {
            // Note: all private fields are instantiated in LoadMatrixFromFile()
            // This enables private methods to reinitialize instance when needed.
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region [ Load Matrix From File ]

        /// <summary>
        /// Loads contents of the sudoku from DSV (delimiter-separated values) file.
        /// </summary>
        /// <remarks>
        /// Sudoku DSV file format is:
        /// 
        /// <pre>
        ///     file  ::= { line }
        ///     line  ::= [ { [ entry ] DELIMITER } [ entry ] ] NEWLINE
        ///     entry ::= NUMBER
        /// </pre>
        /// 
        /// where NEWLINE is end-of-line delimiter, DELIMITER is column separator
        /// (like comma, tab or semicolon) and NUMBER is an integer 0..'dimension'
        /// 
        /// Column fields containing whitespaces or null strings are considered
        /// to be zeros (i.e. empty cells).
        /// 
        /// LoadMatrixFromFile reads at max 'dimension' (usually 9) rows and
        /// columns. Superfluous rows and columns are ignored.
        /// 
        /// If file contains rows or columns less than 'dimension', missing
        /// cells are considered to be empty.
        /// </remarks>
        /// <param name="fileName">a string with sudoku DSV filename</param>
        /// <param name="dimension">an integer with sudoku dimension (default
        /// is 9)</param>
        /// <param name="delimiter">a character that separates values (default
        /// is semicolon i.e. ';'</param>
        /// 
        public bool LoadMatrixFromFile( 
            string fileName, int dimension = 9, char delimiter = ';'
            )
        {
            this.matrix = new int[ dimension, dimension ];

            int warningCount = 0;

            this.warnings = new StringBuilder ();
            this.errors = new StringBuilder ();

            try
            {
                // Create an instance of StreamReader to read from a file.
                //
                // NOTE: The using statement also closes the StreamReader,
                // so we actually do not need any 'finally' code for that job.
                //
                using ( StreamReader sr = new StreamReader( fileName ) )
                {
                    // Read lines from the file until the end of the file is reached.
                    //
                    String line;
                    for ( int row = 0; ( line = sr.ReadLine() ) != null; ++row )
                    {
                        // Ignore rows above maximum row count
                        //
                        if ( row >= matrix.GetLength( 0 ) )
                        {
                            break;
                        }

                        // We expect rows with columns separated by delimiter
                        //
                        string[] cols = line.Split( delimiter );

                        // We accept maximum sudoku.GetLength(1) elements per row
                        // (ignoring superfluous columns).
                        //
                        int colCount = Math.Min( cols.Length, matrix.GetLength( 1 ) );

                        // Parse column elements in row
                        //
                        for( int col = 0; col < colCount; ++col )
                        {
                            string cellValue = cols[ col ].Trim ();

                            if ( string.IsNullOrEmpty( cellValue ) )
                            {
                                matrix[ row, col ] = 0;
                            }
                            else try
                            {
                                int n = int.Parse( cellValue );

                                if ( n >= 0 && n <= dimension )
                                {
                                    matrix[ row, col ] = n;
                                }
                                else
                                {
                                    ++warningCount;
                                    this.warnings.AppendFormat(
                                        "Line {0}: {1}\n", row + 1, line );
                                    this.warnings.AppendFormat( 
                                        "Column {0}: Value = {1}\n", 
                                        col + 1, cols[ col ] );
                                    this.warnings.AppendFormat( 
                                        "Warning: Value is not between 0 and {0}.\n\n",
                                        dimension );
                                }
                            }
                            catch( FormatException ) // not in the correct format.
                            {
                                ++warningCount;
                                this.warnings.AppendFormat(
                                    "Line {0}: {1}\n", row + 1, line );
                                this.warnings.AppendFormat( 
                                    "Column {0}: Value = {1}\n", 
                                    col + 1, cols[ col ] );
                                this.warnings.AppendFormat( 
                                    "Warning: Value is not integer.\n\n" );
                            }
                            catch( OverflowException )
                            {
                                // less than int.MinValue or greater 
                                // than int.MaxValue.
                                //
                                ++warningCount;
                                this.warnings.AppendFormat(
                                    "Line {0}: {1}\n", row + 1, line );
                                this.warnings.AppendFormat( 
                                    "Column {0}: Value = {1}\n", 
                                    col + 1, cols[ col ] );
                                this.warnings.AppendFormat( 
                                    "Warning: Value is not between 0 and {0}.\n\n",
                                    dimension );
                            }
                        }

                    }
                }
            }
            catch( FileNotFoundException )
            {
                this.errors.AppendFormat( "Error: File '{0}' not found.\n\n", fileName );
                this.errors.AppendFormat( "Current Directory: {0}\n", 
                    Directory.GetCurrentDirectory () );
            }
            catch( Exception e )
            {
                // Catch *ALL* other errors and let the user know what went wrong.
                //
                this.errors.AppendFormat( "Error: The file could not be read:\n" );
                this.errors.AppendLine( e.Message );
            }
            finally
            {
                // We are using "using" statement to close stream. 
                // See comment above at the beginning of this try-statement.
            }

            if ( warningCount > 0 )
            {
                this.warnings.AppendFormat( "Loading completed with {0} warnings.\n", 
                    warningCount );
            }

            // Note that although sudoku was successfully parse and loaded from file
            // it does not mean that it is also logically correct i.e. we might have 
            // loaded syntaxically perfect but semantically completelly wrong data!
            //
            if ( ! Consistent )
            {
                this.warnings.AppendFormat( 
                    "Warning: Loaded {0} matrix contains inconsitent data.\n", Name );
            }

            return this.errors.Length == 0 && this.warnings.Length == 0;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods ]

        /// <summary>
        /// Try to solve sudoku by putting all possible numbers (i.e. 1,2...)
        /// at position given by linear index. Process is recursive and
        /// TryToSolve( index ) calls TryToSolve( index + 1 ) until all
        /// possible numbers are exhausted at all levels. Maximum recursion
        /// depth is matrix.Length (81 in our case).
        /// </summary>
        /// <remarks>
        /// Linear index is index of flattened (vectorized) matrix where
        /// all rows are concatenated in a single array. For example, in
        /// matrix 9 x 9 linear index goes from 0 to 81. Mathematically
        /// linear index is row * columnCount + column.
        /// </remarks>
        /// <param name="index">an integer containing linear index</param>
        /// <returns>true if sudoku solution has been found</returns>
        /// 
        private bool TryToSolve( int index )
        {
            if ( index >= matrix.Length )
            {
                return true;
            }

            int row = index / matrix.GetLength( 0 );
            int col = index % matrix.GetLength( 0 );

            if ( matrix[ row, col ] != 0 ) // if cell is not empty
            {
                if ( TryToSolve( index + 1 ) ) // recurse with next cell
                {
                    return true;
                }
            }
            else // matrix[ row, col ] == 0 i.e. it is empty cell
            {
                for ( int n = 1; n <= matrix.GetLength( 0 ); ++n )
                {
                    if ( CanPlaceValueInCell( row, col, n ) )
                    {
                        matrix[ row, col ] = n; // try this number in cell

                        if ( TryToSolve( index + 1 ) ) // recurse with next cell
                        {
                            return true;
                        }

                        matrix[ row, col ] = 0; // not succeded, make cell empty
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Solve sudoku.
        /// </summary>
        /// <returns>true if sudoku was successfully solved; false otherwise.</returns>
        /// 
        public bool Solve ()
        {
            if ( Consistent )
            {
                TryToSolve( 0 );
            }

            return Solved;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region [ Public Virtual Methods ]

        /// <summary>
        /// Determines if some value may be put in some cell preserving sudoku
        /// logically consistent.
        /// </summary>
        /// <param name="row">an integer containing row index</param>
        /// <param name="col">an integer containing column index</param>
        /// <param name="value">an integer with value to be tested</param>
        /// <returns>true if putting a value keeps sudoku consistent</returns>
        /// 
        public virtual bool CanPlaceValueInCell( int row, int col, int value )
        {
            // NOTE: method checks first for the most general Latin Square consistency
            // and later for specific Sudoku sub-square consistency aplied only
            // to matrices with dimensions 9x9.

            // Latin Square: Check row for conflicts
            //
            for ( int j = 0; j < matrix.GetLength( 1 ); ++j )
            {
                if ( matrix[ row, j ] == value )
                {
                    return false;
                }
            }

            // Latin Square: Check column for conflicts
            //
            for ( int i = 0; i < matrix.GetLength( 0 ); ++i )
            {
                if ( matrix[ i, col ] == value )
                {
                    return false;
                }
            }

            // Sudoku: Check sub-square (that contains element at row, col) for conflicts.
            //
            // Square dimensions are 3x3 only in case of sudoku 9x9, otherwise 1
            // (falling back to plain Latin Square).
            //
            int squareDim = matrix.GetLength(0) == 9 ? 3 : 1;

            int top  = row - ( row % squareDim ); // top position of sub-square
            int left = col - ( col % squareDim ); // left position of sub-square

            for ( int i = 0; i < squareDim; ++i )
            {
                for ( int j = 0; j < squareDim; ++j )
                {
                    if ( matrix[ top + i, left + j ] == value )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods ]

        /// <summary>
        /// Returns equivalent string representation for sudoku or latin square
        /// (works even for multi-digit rows and columns > 9).
        /// </summary>
        /// <remarks>
        /// Format of the string representation is:
        /// <pre>
        /// |---|---|---     - row divider 
        /// | 1 | 2 | ...    - row 0
        /// |---|---|---     - row divider 
        /// | 4 | 5 | ...    - row 1
        /// ...
        /// |---|---|---     - row divider 
        /// </pre>
        /// </remarks>
        /// <returns>
        /// a string containing sudoku matrix in the format:
        /// </returns>
        /// 
        public override string ToString () 
        {
            StringBuilder sb = new StringBuilder ();

            if ( ! Consistent )
            {
                sb.Append( "\n*** " ).Append( Name )
                  .Append( " is logically incosistent!\n" );
            }
            else if ( Solved )
            {
                sb.Append( "\n*** " ).Append( Name )
                    .Append( " is solved!\n" );
            }

            return sb.ToString ();
        }

        #endregion

    }
}