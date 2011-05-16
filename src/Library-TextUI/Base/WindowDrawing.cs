/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       WindowDrawing.cs
 *  Created:    2011-03-24
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Text;

using Mbk.Commons;

namespace TextUI
{
    using TextUI.Drawing;

    /// <remarks>
    /// WindowDrawing.cs contains methods used to redraw window contents on the screen.
    /// </remarks>
    /// 
    public partial class Window
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Window Contents Invalidation ]

        /// <summary>
        /// Invalidates the entire surface of the window and causes the window to be
        /// redrawn. 
        /// </summary>
        /// 
        public void Invalidate ()
        {
            Invalidated = true;
        }

        /// <summary>
        /// Conditionally invalidates the entire surface of the window and causes 
        /// the window to be redrawn. 
        /// </summary>
        ///
        public void InvalidateIf( bool condition )
        {
            if ( condition )
            {
                Invalidated = true;
            }
        }

        /// <summary>
        /// Conditionally invalidates the entire surface of the parent window (if
        /// it exists) and causes both the parent window and this window to be redrawn.
        /// </summary>
        ///
        public void InvalidateParentIf( bool condition )
        {
            if ( condition && Parent != null )
            {
                Parent.Invalidated = true;
            }
            else if ( condition )
            {
                Invalidated = true;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Window Redrawing ]

        /// <summary>
        /// Forces the window to invalidate its client area and immediately redraw 
        /// itself and any child windows. 
        /// </summary>
        /// 
        public void Refresh ()
        {
            Invalidate ();
            Application.Screen.UpdateScreen ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Virtual member called before redraw of the window starts, used to
        /// recalculate window size for auto-size windows.
        /// </summary>
        ///
        protected virtual void OnCalculateSize( bool hasFocus )
        {
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Repaints window contents on the screen.
        /// </summary>
        ///
        internal bool Repaint( Screen screen, bool parentHasFocus = true )
        {
            if ( screen == null ) // FIXME: screen null OR INVISIBLE
            {
                return false;
            }

            if ( ! Visible )
            {
                Invalidated = false;
                return false;
            }

            bool hasFocus = Parent == null /* root window always has focus */
                         || ( parentHasFocus && Parent.ActiveChild == this );

            if ( Invalidated )
            {
                OnCalculateSize( hasFocus );
            }

            // Setup clipping region and drawing offset
            //
            ClipContext clipContext = new ClipContext( screen, Left, Top );
            clipContext.Clip( ClientWidth, ClientHeight );

            bool repainted = false;
            bool drawBorder = false;

            if ( Invalidated )
            {
                // Setup default cursor position and drawing colors first, then
                // erase background and draw contents
                //
                screen.SetCursorPosition( 0, 0 );

                if ( hasFocus )
                {
                    screen.BackColor = BackColor;
                    screen.ForeColor = ForeColor;
                }
                else
                {
                    screen.BackColor = BackColorInact;
                    screen.ForeColor = ForeColorInact;
                }

                ColorContext savedColor = new ColorContext( screen );

                OnEraseBackground( screen );

                savedColor.Restore ();

                OnDrawContents( screen, hasFocus );

                savedColor.Restore ();

                drawBorder = true;
                repainted = true;
                Invalidated = false;
            }

            // Repaint children.
            // Algorithm: At the beginning, 'repainted' flag is usually false.
            // First child, which returns that it has repainted itself, turns-on
            // 'repainted' flag further on so the all consecutive siblings are
            // invalidated (and repainted).
            //
            foreach( Window subWindow in children )
            {
                if ( repainted )
                {
                    subWindow.Invalidate (); // Force child to be Repaint()-ed
                }

                if ( subWindow.Repaint( screen, hasFocus ) )
                {
                    repainted = true;
                }
            }

            // Draw window edges (with parent's offset but without clipping!)
            //
            if ( drawBorder )
            {
                clipContext.RestoreClipRegion ();

                if ( hasFocus )
                {
                    screen.BackColor = BorderBackColor;
                    screen.ForeColor = BorderForeColor;
                }
                else
                {
                    screen.BackColor = BorderBackColorInact;
                    screen.ForeColor = BorderForeColorInact;
                }

                OnDrawBorder( screen, hasFocus );
            }

            // Restore clipping region and drawing offset
            //
            clipContext.Restore ();

            return repainted;
        }

        /// <summary>
        /// Repaints cursor on the screen.
        /// </summary>
        ///
        internal void RepaintCursor( Screen screen )
        {
            if ( screen == null )
            {
                return;
            }

            // When RepaintCurosr() is called for root window of the screen
            // (having no parent), we need to setup a default cursor (which is an 
            // invisible cursor at position ( 0, 0 ).
            //
            if ( Parent == null )
            {
                screen.SetCursorPosition( 0, 0 );
                screen.CursorVisible = false;
            }

            ClipContext clipContext = new ClipContext( screen, Left, Top );

            clipContext.Clip( ClientWidth, ClientHeight );

            if ( children.Count > 0 )
            {
                ActiveChild.RepaintCursor( screen );
            }
            else
            {
                if ( CursorVisible && screen.IsVisible( CursorLeft, CursorTop ) )
                {
                    screen.SetCursorPosition( 
                        screen.Offset.X + CursorLeft, screen.Offset.Y + CursorTop );
                    screen.CursorVisible = true;
                }
            }

            clipContext.Restore ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Draws default border, caption and scroll bar for the window.
        /// </summary>
        /// <remarks>
        /// Border is drawn depending on Border, Caption and VerticalScrollBar flags:
        /// <pre>
        ///              No Border           With Border           With Border      
        ///                                                        and Caption      
        ///                                                                       
        ///          Without Scrollbar:                           ┌──────────────┐  
        ///                                                       │██████████████│  
        ///                                 ┌──────────────┐      ├──────────────┤  
        ///              ▒▒▒▒▒▒▒▒▒▒▒▒▒▒     │▒▒▒▒▒▒▒▒▒▒▒▒▒▒│      │▒▒▒▒▒▒▒▒▒▒▒▒▒▒│  
        ///              ▒▒▒▒▒▒▒▒▒▒▒▒▒▒     │▒▒▒▒▒▒▒▒▒▒▒▒▒▒│      │▒▒▒▒▒▒▒▒▒▒▒▒▒▒│  
        ///              ▒▒▒▒▒▒▒▒▒▒▒▒▒▒     │▒▒▒▒▒▒▒▒▒▒▒▒▒▒│      │▒▒▒▒▒▒▒▒▒▒▒▒▒▒│  
        ///                                 └──────────────┘      └──────────────┘  
        ///                                                                       
        ///          With Scrollbar:                              ┌──────────────┐
        ///                                                       │██████████████│
        ///                                 ┌────────────┬─┐      ├────────────┬─┤
        ///              ▒▒▒▒▒▒▒▒▒▒▒▒│▲     │▒▒▒▒▒▒▒▒▒▒▒▒│▲│      │▒▒▒▒▒▒▒▒▒▒▒▒│▲│
        ///              ▒▒▒▒▒▒▒▒▒▒▒▒│█     │▒▒▒▒▒▒▒▒▒▒▒▒│█│      │▒▒▒▒▒▒▒▒▒▒▒▒│█│
        ///              ▒▒▒▒▒▒▒▒▒▒▒▒│▼     │▒▒▒▒▒▒▒▒▒▒▒▒│▼│      │▒▒▒▒▒▒▒▒▒▒▒▒│▼│
        ///                                 └────────────┴─┘      └────────────┴─┘
        /// </pre>                                                              
        /// 
        /// 3D-shadow of the border might be drawn using:
        /// <code>
        /// 
        /// screen.SetColors( Left + 1, Top + Height + 1, Width + 2, 1,  
        ///     Color.Black, Color.Black );
        ///     
        /// screen.SetColors( Left + Width + 1, Top + 1, 2, Height, 
        ///     Color.Black, Color.Black );
        ///     
        /// </code>
        /// 
        /// </remarks>                                                            
        ///                                                                       
        public void DefaultDrawBorder( Screen screen, bool hasFocus )
        {
            ColorContext savedColors = new ColorContext( screen );

            bool hasScrollbar = VerticalScrollBar && Height >= 2 && Width >= 3;

            if ( Border && ! CaptionVisible )
            {
                // Draw normal window border without caption but with optional
                // border margin
                //
                screen.DrawRectangle( -1, -1, Width + 2, Height + 2 );
            }
            else if ( Border )
            {
                // Draw expanded window border for caption
                //
                screen.DrawRectangle( -1, -3, Width + 2, Height + 4 ); 

                // Draw separator between window contents and caption
                //
                screen.Put( -1, -1, Box._UDsR );
                screen.Put( Width, -1, Box._UDLs );
                screen.DrawRectangle( 0, -1, Width, 1 );
            }

            // Draw vertical scrollbar with optional frame
            //
            if ( hasScrollbar )
            {
                int reducedWidth = Width - 2;

                // Draw separator between window contents and scroll bar
                //
                if ( Border )
                {
                    screen.DrawRectangle( reducedWidth, 0, 1, Height );
                    screen.Put( reducedWidth, -1, Box._sDLR );
                    screen.Put( reducedWidth, Height, Box._UsLR );
                }
                else
                {
                    screen.DrawRectangle( reducedWidth, 0, 1, Height );
                    screen.DrawRectangle( reducedWidth + 2, 0, 1, Height );
                }

                // Draw vertical scroll bar

                if ( hasFocus )
                {
                    screen.ForeColor = ScrollBarForeColor;
                }
                else
                {
                    screen.ForeColor = ScrollBarForeColorInact;
                }

                screen.DrawVerticalScrollBar( reducedWidth + 1, 0, Height,
                    VerticalScrollBarFirstItem, VerticalScrollBarLastItem,
                    VerticalScrollBarItemCount );
            }

            // Write caption
            //
            if ( Border & CaptionVisible & Caption != null )
            {
                if ( hasFocus )
                {
                    screen.BackColor = CaptionBackColor;
                    screen.ForeColor = CaptionForeColor;
                }
                else
                {
                    screen.BackColor = CaptionBackColorInact;
                    screen.ForeColor = CaptionForeColorInact;
                }

                string text = TaggedText.AlignedText( Caption, Width, 
                    CaptionTextAlign, CaptionIndent, CaptionIndent );

                screen.CursorTop  = -2;
                screen.CursorLeft = 0;
                screen.Write( text );
            }

            savedColors.Restore ();
        }

        #endregion
    }
}