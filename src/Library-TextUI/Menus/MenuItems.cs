/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Text User Interface (TextUI / TUI) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  TextUI
 *  File:       MenuItems.cs
 *  Created:    2011-03-25
 *  Modified:   2011-04-26
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;

namespace TextUI
{
    /// <summary>
    /// Represents a collection of MenuItem objects.
    /// </summary>
    /// 
    public class MenuItems : IList<MenuItem>
    {
        #region [ Fields ]

        // Owner of the collection
        //
        private Menu owner;

        // Internal collection of the items.
        //
        private List<MenuItem> items;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether menu item structure has been changed.
        /// </summary>
        /// 
        public bool StructureChanged { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the Menu.MenuItemCollection class belonging
        /// to specified menu.
        /// </summary>
        ///
        public MenuItems( Menu owner )
            : base ()
        {
            this.owner = owner;
            this.items = new List<MenuItem> ();
            StructureChanged = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Finds the items matching predicate.
        /// </summary>
        /// 
        public MenuItem Find( Predicate<MenuItem> match )
        {
            foreach( MenuItem mi in this.items )
            {
                if ( match( mi ) )
                {
                    return mi;
                }
            }

            return null;
        }

        /// <summary>
        /// Resets StructureChanged to false.
        /// </summary>
        /// 
        public void AcknolwedgeStructureChange ()
        {
            StructureChanged = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ IList<MenuItem> Imlementation ]

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        /// 
        public int Count
        {
            get { return this.items.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only. Returns false.
        /// </summary>
        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        ///
        public MenuItem this[ int index ]
        {
            get
            {
                return this.items[ index ];
            }
            set
            {
                StructureChanged = true;
                this.items[ index ] = value;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        ///
        public int IndexOf( MenuItem item )
        {
            return this.items.IndexOf( item );
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index. 
        /// </summary>
        ///
        public void Insert( int index, MenuItem item )
        {
            StructureChanged = true;
            this.items.Insert( index, item );
        }

        /// <summary>
        /// Removes the menu item at the specified index. 
        /// </summary>
        ///
        public void RemoveAt( int index )
        {
            StructureChanged = true;
            this.items.RemoveAt( index );
        }

        /// <summary>
        /// Adds an menu item to the collection.
        /// </summary>
        ///
        public void Add( MenuItem item )
        {
            StructureChanged = true;
            this.items.Add( item );
        }

        /// <summary>
        /// Removes all menu items from the collection.
        /// </summary>
        /// 
        public void Clear ()
        {
            StructureChanged = true;
            this.items.Clear ();
        }

        /// <summary>
        /// Determines whether the collection contains a specific menu item. 
        /// </summary>
        ///
        public bool Contains( MenuItem item )
        {
            return this.items.Contains( item );
        }

        /// <summary>
        /// Copies the elements of the collection to an Array, starting at a 
        /// particular Array index.
        /// </summary>
        ///
        public void CopyTo( MenuItem[] array, int arrayIndex )
        {
            StructureChanged = true;
            this.items.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Removes the first occurrence of a specific menu item from the collection.
        /// </summary>
        ///
        public bool Remove( MenuItem item )
        {
            StructureChanged = true;
            return this.items.Remove( item );
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        ///
        public IEnumerator<MenuItem> GetEnumerator ()
        {
            return this.items.GetEnumerator ();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        ///
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.items.GetEnumerator ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
   }
}