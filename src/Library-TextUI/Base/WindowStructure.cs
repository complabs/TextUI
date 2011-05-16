/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       WindowStructure.cs
 *  Created:    2011-03-24
 *  Modified:   2011-05-08
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Mbk.Commons;
using TextUI.Controls;

namespace TextUI
{
    /// <summary>
    /// WindowStructure.cs implements methods an properties dealing with
    /// window containment (child-parent relationship) and focus.
    /// </summary>
    /// 
    public partial class Window
    {
        #region [ Private Fields ]

        /// <summary>
        /// Field holding the parent container of the window. 
        /// </summary>
        /// <remarks>
        /// Sample Tree Structure:
        /// <pre>
        ///      Screen
        ///      └── RootWindow
        ///           ├── Window (0)
        ///           ├── Window (1)
        ///           │    └── Window(1.0)
        ///           └── Window (2)
        ///                └── Window (2.0)
        ///                     ├── Window (2.0.0)
        ///                     └── Window (2.0.1)    ◄─── in focus
        ///                     
        /// Line of focus (focus descent):
        /// 
        ///      Screen . RootWindow . (2) . (2.0) . (2.0.1 )
        /// </pre>
        /// Sample Screen View:
        /// <pre>
        ///     ┌─(RootWindow)─────────────────────────────────────────────────────────┐ 
        ///     │ Children = { 0, 1, 2 }                                               │
        ///     │                                  ┌(1)────────────────────┐           │
        ///     │                                  │       ┌(1.0)───────┐  │           │
        ///     │          ┌(2)──────────────────────────────────────────────────┐     │
        ///     │          │ Children = { 0 }      ¦       ¦            ¦  ¦     │     │
        ///     │          │                       ¦       ¦            ¦  ¦     │     │
        ///     │  ┌(0)────│----                   ¦        ------------   ¦     │     │
        ///     │  │       │    ¦                  ¦                       ¦     │     │
        ///     │  │       │    ¦                  ¦                       ¦     │     │
        ///     │  └───────│----                    -----------------------      │     │
        ///     │          │                                                     │     │
        ///     │          │            ┌(2.0)───────────────────────────────────│-----│
        ///     │          │            │ Children = { 0, 1 }  ┌(2.0.1)──────┐   │     │
        ///     │          │            │                      │)))))))))))))│   │     │
        ///     │          │            │             ┌(2.0.0)─│((( FOCUS (((│   │     │
        ///     │          │            │             │        │)))))))))))))│   │     │
        ///     │          │            │             │        └─────────────┘   │     │
        ///     │          │            │             │            │             │     │
        ///     │          │            │             └────────────┘             │     │
        ///     │          └─────────────────────────────────────────────────────┘     │
        ///     │                       ¦                                              │
        ///     └──────────────────────────────────────────────────────────────────────┘ 
        /// </pre>
        /// </remarks>
        ///
        private Window parent;

        /// <summary>
        /// Field holding a list of windows contained within the window.
        /// </summary>
        /// 
        private List<Window> children;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets top-level parent window.
        /// </summary>
        /// 
        public Window Root
        {
            get
            {
                Window root = this;
                while( root.Parent != null )
                {
                    root = root.Parent;
                }
                return root;
            }
        }

        /// <summary>
        /// Gets or sets the parent container of the window.
        /// </summary>
        /// 
        public Window Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if ( value != null )
                {
                    value.AddChild( this );
                }
                else if ( this.parent != null )
                {
                    this.parent.RemoveChild( this );
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a value indicating if the window has children.
        /// </summary>
        /// 
        public bool HasChildren
        {
            get
            {
                return this.children.Count > 0;
            }
        }

        /// <summary>
        /// Gets the read-only collection of windows contained within the window.
        /// </summary>
        /// 
        public ReadOnlyCollection<Window> Children
        {
            get 
            {
                return this.children.AsReadOnly ();
            }
        }

        /// <summary>
        /// Gets the read-only collection of controls contained within the window.
        /// </summary>
        /// 
        public IEnumerable<Control> Controls
        {
            get
            {
                foreach( Window child in this.children )
                {
                    Control control = child as Control;
                    if ( control != null )
                    {
                        yield return control;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all nested children that are leafs in children tree of the window.
        /// </summary>
        /// 
        private IEnumerable<Window> GetLeafChildren( bool nested )
        {
            foreach( Window child in this.children )
            {
                if ( child.HasChildren && nested )
                {
                    foreach( Window grandchild in child.GetLeafChildren( nested ) )
                    {
                        yield return grandchild;
                    }
                }
                else
                {
                    yield return child;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the active child window (child window in focus) of null if
        /// the window doesn't have children.
        /// </summary>
        /// 
        public Window ActiveChild
        {
            get 
            {
                return ! HasChildren ? null : this.children[ this.children.Count - 1 ];
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the leaf child (child of the child of the child) in focus.
        /// </summary>
        /// 
        public Window ActiveDescendant 
        {
            get
            {
                return ActiveChild == null ? this : ActiveChild.ActiveDescendant;
            }
        }

        /// <summary>
        /// Gets a value indicating if the window is in application focus (receiving
        /// key down events). The window is in application focus if it is 
        /// ActiveDescendant of the Root window.
        /// </summary>
        /// 
        public bool IsInApplicationFocus
        {
            get
            {
                Window p = this;

                while( p.Parent != null && p.Parent.ActiveChild == p )
                {
                    p = p.Parent;
                }

                return p == Application.RootWindow;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the previous child (relative to this) of the parent.
        /// </summary>
        /// 
        public Window PreviousSibling
        {
            get
            {
                if ( Parent == null || Parent.children.Count == 0 )
                {
                    return null;
                }

                int cur = Parent.children.IndexOf( this );

                if ( cur < 0 )
                {
                    return this;
                }
                else if ( cur == 0 )
                {
                    return Parent.children[ Parent.children.Count - 1 ];
                }

                return Parent.children[ cur - 1 ];
            }
        }

        /// <summary>
        /// Gets the next child (relative to this) of the parent.
        /// </summary>
        /// 
        public Window NextSibling
        {
            get
            {
                if ( Parent == null || Parent.children.Count == 0 )
                {
                    return null;
                }

                int cur = Parent.children.IndexOf( this );

                if ( cur < 0 )
                {
                    return this;
                }
                else if ( cur == Parent.children.Count - 1 )
                {
                    return Parent.children[ 0 ];
                }

                return Parent.children[ cur + 1 ];
            }
        }

        /// <summary>
        /// Gets the absolute tab index consisting of concatenated window's TabIndex
        /// and its ID.
        /// </summary>
        /// 
        private int AbsoluteTabIndex
        {
            get
            {
                return ( TabIndex << 16 ) + (ushort)ID;
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Navigation: SelectNextControl, FindChild etc. ]

        /// <summary>
        /// Finds the next child after (before) child. For arguments (i.e. search 
        /// conditions) see parameters for public method 
        /// <see cref="SelectNextControl"/>.
        /// </summary>
        /// 
        public Window FindNextChild
        ( 
            Window child, bool forward, bool tabStopOnly, bool nested, bool wrap
            )
        {
            // Find child in nested subtree list, which is built from recursive
            // enumeratation of all children at leaf positions.

            List<Window> wList = new List<Window> ( this.GetLeafChildren( nested ) );

            int childAt = wList.IndexOf( child );

            if ( childAt < 0 )
            {
                return null;
            }

            // Note that we are dealing with absolute tab indexes that are
            // 32-bit integers compound from two 16-bit TabIndex and 16-bit window ID
            //
            int foundAt = -1;
            int currentTabIndex = wList[ childAt ].AbsoluteTabIndex;

            // Find minimal (maximal) tab index for all windows
            // after (before) current child (having tab stops optionally)
            // which have tab index at least (at most) the same as tab index of 
            // the current child.
            //
            if ( forward ) // search forward increasing tab indices
            {
                while( foundAt < 0 )
                {
                    int minimalFoundTabIndex = int.MaxValue;

                    for ( int i = 1; i < wList.Count; ++i )
                    {
                        int index = ( childAt + i ) % wList.Count;
                        Window win = wList[ index ];
                        int tabIndex = wList[ index ].AbsoluteTabIndex;

                        if ( ( tabStopOnly && win.TabStop || ! tabStopOnly )
                            && tabIndex >= currentTabIndex
                            && tabIndex < minimalFoundTabIndex )
                        {
                            foundAt = index;
                            minimalFoundTabIndex = tabIndex;
                        }
                    }

                    if ( foundAt >= 0 || ! wrap )
                    {
                        break;
                    }

                    // If wrapping around, search once more, this time starting from 
                    // the lowest possible tab index this time, also turning-off 
                    // the wrap flag so next time we break the loop.
                    //
                    currentTabIndex = int.MinValue;
                    wrap = false;
                }
            }
            else // search backwards decreasing tab indices
            {
                while ( foundAt < 0 )
                {
                    int maximalFoundTabIndex = int.MinValue;

                    for ( int i = 1; i < wList.Count; ++i )
                    {
                        int index = ( childAt - i + wList.Count ) % wList.Count;
                        Window win = wList[ index ];
                        int tabIndex = wList[ index ].AbsoluteTabIndex;

                        if ( ( tabStopOnly && win.TabStop || ! tabStopOnly )
                            && tabIndex <= currentTabIndex
                            && tabIndex > maximalFoundTabIndex )
                        {
                            foundAt = index;
                            maximalFoundTabIndex = tabIndex;
                        }
                    }

                    if ( foundAt >= 0 || ! wrap )
                    {
                        break;
                    }

                    // If wrapping around, search once more, this time starting from 
                    // the lowest possible tab index this time, also turning-off 
                    // the wrap flag so next time we break the loop.
                    //
                    currentTabIndex = int.MaxValue;
                    wrap = false;
                }
            }

            return foundAt < 0 ? null : wList[ foundAt ];
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Activates the next control.
        /// </summary>
        /// <param name="child">The Control at which to start the search.</param>
        /// <param name="forward">true to move forward in the tab order; false to move 
        /// backward in the tab order.</param>
        /// <param name="tabStopOnly">true to ignore the controls with the TabStop 
        /// property set to false; otherwise, false.</param>
        /// <param name="nested">true to include nested (children of child controls) 
        /// child controls; otherwise, false.</param>
        /// <param name="wrap">true to continue searching from the first control in the
        /// tab order after the last control has been reached; otherwise, false</param>
        /// <returns>true if a control was activated; otherwise, false.</returns>
        /// 
        public bool SelectNextControl
        (
            Window child, bool forward = true,
            bool tabStopOnly = true, bool nested = true, bool wrap = true
            )
        {
            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                Debug.TraceLine( "{0}.SelectNextControl ()", Name );
            } );

            Window next = FindNextChild( child, forward, tabStopOnly, nested, wrap );

            return next == null ? false : next.Focus ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Finds a child matching condition.
        /// </summary>
        /// 
        public Window FindChild( Predicate<Window> match )
        {
            Window found = null;

            foreach( Window child in this.children )
            {
                if ( match( child ) )
                {
                    found = child;
                    break;
                }
            }

            return found;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Children Collection: Adding and removing children ]

        /// <summary>
        /// Adds some window before this window in parent's child collection.
        /// </summary>
        /// 
        public bool AddBefore( Window sibling )
        {
            if ( sibling == null || sibling.Parent == null )
            {
                return false;
            }
            else if ( sibling == this )
            {
                return true;
            }

            Window commonParent = sibling.Parent;

            // Extract from current parent
            //
            if ( ! Unload () )
            {
                return false;
            }

            // Find index of the sibling and insert at its position
            //
            int siblingIndex = commonParent.children.IndexOf( sibling );

            if ( siblingIndex >= 0 )
            {
                this.parent = commonParent;
                commonParent.children.Insert( siblingIndex, this );

                Invalidate ();

                OnLoad ();
            }

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds a new child to the window.
        /// </summary>
        /// 
        internal bool AddChild( Window child, bool validateFocusChange = true )
        {
            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                Debug.TraceLine( "{0}.Add( Child := {1} )", 
                    Name, child == null ? "(null)" : child.Name );
            } );

            if ( child == null )
            {
                return true;
            }

            // If already in parent's children, just change the focus and quit.
            //
            if ( this.children.Contains( child ) )
            {
                child.parent = this;
                return child.Focus( validateFocusChange );
            }

            // The child is is not a child of this window, but it has some parent.
            // Make it parentless (if validation does not fail).
            //
            if ( child.Parent != null )
            {
                if ( ! child.Parent.RemoveChild( child, validateFocusChange ) )
                {
                    return false;
                }
            }

            // Insert child at as the 'oldest' and change its focus to be
            // the 'youngest' (this will force OnValidating of current windows in focus).
            //
            child.parent = this;
            this.children.Insert( 0, child );

            // Request repaint and raise WindowLoaded event.
            //
            child.Invalidate ();

            child.OnLoad ();

            // Pop-up the window to be the youngest (the last) child
            //
            return child.Focus( validateFocusChange );
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Removes existing child from the children.
        /// </summary>
        /// 
        internal bool RemoveChild( Window child, bool validateFocusChange = true )
        {
            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                Debug.TraceLine( "{0}.Remove( Child := {1} )", 
                    Name, child == null ? "(null)" : child.Name );
            } );

            if ( child == null || ! this.children.Contains( child ) )
            {
                return true;
            }

            // Check, when child window is in focus-path, if it is valid
            // and can be removed from focus (also raising lost focus events).
            //
            bool isInFocusPath = false;

            for ( Window p = Application.RootWindow; p != null; p = p.ActiveChild )
            {
                if ( p == child )
                {
                    isInFocusPath = true;
                    break;
                }
            }

            LinkedList<Window> pathFromFocus = child.GetPathFromFocus ();

            if ( validateFocusChange && isInFocusPath )
            {
                if ( ! ValidateLostFocus( pathFromFocus ) )
                {
                    return false;
                }
            }

            // Valid and may be removed. Unlink child from this window and mark this 
            // window to be repainted.
            //
            child.parent = null;
            this.children.Remove( child );

            if ( child.Visible )
            {
                this.Invalidate ();
            }

            child.OnUnload ();

            // If child was in focus-path, find a new child in focus 
            // (if none, this window receives focus) and raise on got focus to its
            // children in focus-path.
            //
            if ( isInFocusPath )
            {
                Window nextInFocus = ActiveChild;

                if ( nextInFocus == null )
                {
                    nextInFocus = this;
                }

                while( nextInFocus != null )
                {
                    nextInFocus.OnGotFocus ();

                    nextInFocus = nextInFocus.ActiveChild;
                }

                // Raise lost focus events for all window in lost focus path.
                //
                RaiseLostFocus( pathFromFocus );
            }

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Unloads the window, i.e. removes the window from it's parent's child
        /// collection.
        /// </summary>
        /// 
        public bool Unload ()
        {
            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                Debug.TraceLine( "{0}.Unload ()", Name );
            } );

            if ( Parent != null )
            {
                return Parent.RemoveChild( this );
            }

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Unloads the window's parent from parent's parent child collection.
        /// </summary>
        /// 
        public bool UnloadParent ()
        {
            if ( Parent != null && Parent.Parent != null )
            {
                Debug.IfTracing( TraceFlag.Focus, delegate
                {
                    Debug.TraceLine( "{0}.UnloadParent ()", Name );
                } );

                return Parent.Parent.RemoveChild( Parent );
            }

            return true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Focus() Methods ]

        /// <summary>
        /// Sets input focus to the window.
        /// </summary>
        /// <remarks>
        /// Notes for ValidateLostFocus(), Focus() and Remove() methods. 
        /// <pre>
        ///          Path From Root                          Path From Focus
        ///   ================================      ================================
        ///   Filled in reverse order starting      Filled in reverse order starting
        ///   at this window (5). List starts       from common
        ///   with root (window 1).
        ///   ================================      ================================
        ///   
        ///            _█_1   ◄─── Root Window           _O_1
        ///           / | \                             / | \            Window which
        ///          /  |  \ 2                        . . .  \  2        children should
        ///         O   O  _█_   ◄────────────────────────   [*]  ◄───── bre rearanged
        ///          _____/ | \_____                   _____/ | \_____
        ///         /       |3      \                 /       |       \ C
        ///       _O_       █       _O_             _O_       O       _█_
        ///      / | \      |      / | \           / | \      |      / | \
        ///     /  |  \     |4    /  |  \         /  |  \     |     /  |  \ B
        ///    O   O   O   _█_   O   O  _O_      O   O   O   _O_   O   O  _█_     Current
        ///               / | \        / | \                / | \        / | \     focus
        ///  'this'    5 /  |  \      /  |  \              /  |  \      /  |  \ A    │
        ///  window ─► _█_  O   O    O   O   O           _O_  O   O    O   O   █  ◄──┘
        ///           / | \                             / | \  
        ///          /  |  \                           /  |  \ 
        ///         O   O   O                         O   O   O
        ///
        ///   pathFromRoot := { 1, 2, 3, 4, 5 }     pathFromFocus := { A, B, C, 2, 1 }
        ///   
        ///  Adjusted (final) pathFromRoot and pathFromFocus lists contain only windows
        ///  that are not common for both lists. From the following example:
        ///    
        ///   pathFromRoot := { 2, 3, 4, 5 }        pathFromFocus := { A, B, C }
        ///  
        ///  For windows in the pathFromFocus list are raised Validating events and,
        ///  after successfull validation, LostFocus events.
        ///  
        ///  Windows in pathFromRoot are popped-up (rearanged), if already not at  the
        ///  last (focus) position. At the same tame they will receive GotFocus events.
        ///  
        ///  The final structure, after Focus() on window (5), will be:
        /// 
        ///            _█_1          ───────────►          _█_1
        ///           / | \          win5.Focus()         / | \
        ///          /  |  \ 2                           /  |  \ 2
        ///         O   O  _█_                          O   O  _█_
        ///          _____/ | \_____                   _______/ | \___________
        ///       a /       |3      \ b             a /        b|             \ 3
        ///       _O_       █       _O_             _O_        _O_             █         
        ///      / | \      |      / | \           / | \      / | \            |         
        ///     /  |  \     |4    /  |  \         /  |  \    /  |  \           |4        
        ///    O   O   O   _█_   O   O  _O_      O   O   O  O   O  _O_        _█_        
        ///               / | \        / | \                      / | \      / | \  
        ///         win5 /  |c \ d    /  |  \                    /  |  \  c / d|  \ 5
        ///            _█_  O   O    O   O   O                  O   O   O  O   O  _█_
        ///           / | \                                                      / | \      
        ///          /  |  \ f                                                  /  |  \ f   
        ///         O   O   O                                                  O   O   O    
        /// 
        /// </pre>
        /// </remarks>
        /// 
        public bool Focus ()
        {
            return Focus( /*validateFocusChange*/ true );
        }

        /// <summary>
        /// Sets input focus to the window, optionally suppressing validating events.
        /// </summary>
        /// <remarks>
        /// This method may be used only internally.
        /// </remarks>
        /// 
        internal bool Focus( bool validateFocusChange )
        {
            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                if ( Application.RootWindow != null )
                {
                    Debug.TraceLine( "Current Window Structure:" );
                    Debug.Trace( Application.RootWindow.StructureToString () );
                }

                Debug.TraceTimeStamp ();
                Debug.Trace( "{0}.Focus (), root first", Name );
            } );

            // Fill path from root list with the list of predecessors to this window.
            // Final list will start with RootWindow and end with this window. Note that
            // the list will be always non-empty and contain at list this window.
            //
            LinkedList<Window> pathFromRoot = new LinkedList<Window> ();

            for ( Window p = this; p != null; p = p.Parent )
            {
                pathFromRoot.AddFirst( p );
            }

            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                foreach( Window w in pathFromRoot )
                {
                    Debug.Trace( " {0}", w.Name );
                }
            } );

            // Remove from the list all windows that will stay in focus.
            // The first element in the resulting list will be a window which children
            // should be rearanged (but the window itself will stay in focus).
            // This window will be further called Common Focus Holder.
            //
            for ( LinkedListNode<Window> top = pathFromRoot.First; 
                 top.Next != null && top.Value.ActiveChild == top.Next.Value;
                 top = pathFromRoot.First
                 )
            {
                pathFromRoot.RemoveFirst ();
            }

            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                Debug.Trace( " --> " );
                foreach( Window w in pathFromRoot )
                {
                    Debug.Trace( " {0}", w.Name );
                }
                Debug.TraceLine ();
            } );

            // Quit if Common Focus Holder has no children to rearange.
            //
            if ( pathFromRoot.First.Next == null )
            {
                return true;
            }

            Window firstChildInFocusPath = pathFromRoot.First.Value.ActiveChild;

            // Note that at this point firstChildInFocusPath is always not null
            // as the Common Focus Holder children list was not empty 
            // (i.e. pathFromRoot.First.Next was not null)!

            // Popuplate the 'path from focus': a path starting at window in application
            // focus backwards to Common Focus Holder child in focus.
            // 
            LinkedList<Window> pathFromFocus = firstChildInFocusPath.GetPathFromFocus ();

            // Raise OnValidating events on windows in focus-path.
            //
            if ( validateFocusChange )
            {
                if ( ! ValidateLostFocus( pathFromFocus ) )
                {
                    return false;
                }
            }

            // Pop-up children (that needs to be popup) and raise GotFocus events
            //
            for ( LinkedListNode<Window> node = pathFromRoot.First; 
                node.Next != null;
                node = node.Next
                )
            {
                if ( node.Next.Value != node.Value.ActiveChild )
                {
                    node.Value.ActiveChild.Invalidate ();

                    // Get list of windows before 'this' window that should be pop-up
                    //
                    List<Window> popUp = new List<Window> ();

                    Window win = null;
                    do
                    {
                        win = node.Value.children[ 0 ];
                        popUp.Add( win );
                        win.Invalidate ();

                        node.Value.children.Remove( win );
                    }
                    while( win != node.Next.Value );

                    node.Value.children.AddRange( popUp );
                }
            }

            // Raise got focus events for all window in path from root, as well
            // children of this window.
            //
            for ( LinkedListNode<Window> node = pathFromRoot.First; 
                node.Next != null;
                node = node.Next
                )
            {
                node.Next.Value.OnGotFocus ();
            }

            for ( Window child = this.ActiveChild; 
                child != null; 
                child = child.ActiveChild )
            {
                child.OnGotFocus ();
            }

            // Raise lost focus events for all window in lost focus path.
            //
            RaiseLostFocus( pathFromFocus );

            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                if ( Application.RootWindow != null )
                {
                    Debug.TraceLine( "Final Window Structure:" );
                    Debug.Trace( Application.RootWindow.StructureToString () );
                }
            } );

            return true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Private Methods used by Focus() methods ]

        /// <summary>
        /// Populates list with children in focus-path. At the end, the list will 
        /// contain all windows in focus-path starting from the youngest (in focus) 
        /// to the eldest (this window).
        /// </summary>
        /// <remarks>
        /// WARNING: The window *must be* in the focus-path before calling this method.
        /// </remarks>
        /// 
        private LinkedList<Window> GetPathFromFocus ()
        {

            LinkedList<Window> pathFromFocus = new LinkedList<Window> ();

            for ( Window w = this; w != null; w = w.ActiveChild )
            {
                pathFromFocus.AddFirst( w );
            }

            Debug.IfTracing( TraceFlag.Focus, delegate
            {
                Debug.TraceTimeStamp ();
                Debug.Trace( "{0}.GetPathFromFocus (), focus first ", Name );
                foreach( Window w in pathFromFocus )
                {
                    Debug.Trace( " {0}", w.Name );
                }
                Debug.TraceLine ();
            } );

            return pathFromFocus;
        }

        /// <summary>
        /// Raises Validating and Validated events for windows in the path.
        /// </summary>
        /// 
        private static bool ValidateLostFocus( LinkedList<Window> pathFromFocus )
        {
            // Check validation on vanishing list and quit with error if failed.
            //
            foreach( Window w in pathFromFocus )
            {
                if ( ! w.OnValidating () )
                {
                    return false;
                }
            }

            // Raise Validated events
            //
            foreach( Window w in pathFromFocus )
            {
                w.OnValidated ();
            }

            return true;
        }

        /// <summary>
        /// Raises lost focus event for windows in the path.
        /// </summary>
        /// 
        private static void RaiseLostFocus( LinkedList<Window> pathFromFocus )
        {
            // Raise LostFocus events
            //
            foreach( Window w in pathFromFocus )
            {
                w.OnLostFocus ();
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ IfValidateOk() -- Forced Validation  ]

        /// <summary>
        /// Forces validation (by losing focus) of the window and executes delegate if 
        /// validation was successfull.
        /// </summary>
        /// <param name="method">Delegate to be executed if validation was ok.</param>
        /// <returns>True if validation was successfull.</returns>
        ///
        public bool IfValidateOk( Action method = null )
        {
            // All focus concealer windows has the same ID.
            //
            const string focusConcealerWindowID = "<[focus]>";

            // If this is the Root Window or some window out of the window-tree, 
            // assume that it is always valid and that it  always may loose its focus.
            //
            if ( Parent == null )
            {
                if ( method != null )
                {
                    method ();
                }

                return true;
            }

            // If sibling in focus of this windows is a focus concealer, then assume
            // that we are inside of existing IfValidateOk() and invoke delagate.
            //
            Window siblingInFocus = Parent.ActiveChild;

            if ( siblingInFocus != null && siblingInFocus.Name == focusConcealerWindowID )
            {
                if ( method != null )
                {
                    method ();
                }

                return true;
            }

            // Create focus concealer (a window that will take focus temporarily,
            // if it can). Note that createion is wihout color theme to speed-up
            // a process.
            //
            Window focusConcealer = new Window( /*colorTheme*/ null )
            { 
                Name = focusConcealerWindowID, Visible = false
            };

            // Add focus concealer as the last child to parent. This will
            // take focus from this window if validation was successfull.
            //
            bool focusMovedToConcealer = Parent.AddChild( focusConcealer );

            // If validation was ok (focus was moved to concealer), then
            // execute delegate. However, don't forgett to unload focus concealer
            // if delegate raises an exception.
            //
            try
            {
                if ( focusMovedToConcealer && method != null )
                {
                    method ();
                }
            }
            finally
            {
                string savedErrorMessage = Application.ErrorMessage;

                // Remove our concealer from the list
                //
                focusConcealer.Unload ();

                if ( Application.ErrorMessage == null )
                {
                    Application.ErrorMessage = savedErrorMessage;
                }
            }

            // Return if validation was successfull. This might be used to test
            // if focus can be moved (if one passes null as delegate and looks only
            // at the result).
            //
            return focusMovedToConcealer;
        }

        #endregion

    }
}