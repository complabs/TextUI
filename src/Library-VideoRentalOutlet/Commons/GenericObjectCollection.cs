/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib
 *  File:       GenericObjectCollection.cs
 *  Created:    2011-04-08
 *  Modified:   2011-05-04
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VROLib
{
    /// <summary>
    /// Base collection class from which all collections having objects in 
    /// VideoRentalOutlet database are (normally) derived from. GenericObjectCollection
    /// class provides on-change notifications i.e. events, when its contents changes 
    /// (see <see cref="MonitoredObject"/> class).
    /// </summary>
    /// 
    [Serializable]
    public class GenericObjectCollection<T> : MonitoredObject, IList<T>, IList
        where T: GenericObject
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// List of <see cref="GenericObject"/>s belonging to this collection.
        /// </summary>
        /// 
        protected List<T> Items { get; private set; }

        /// <summary>
        /// Gets a name that describes a single item in collection (e.g. "Customer"
        /// in Customer collection).
        /// </summary>
        /// 
        public string ItemName { get; private set; }

        /// <summary>
        /// Gets or sets a flag that, if set true, enables forwarding events to a common 
        /// on-change event handlers hooked to database instance of 
        /// <see cref="VideoRentalOutlet"/> class (as <see cref="VideoRentalOutlet"/> is 
        /// also derived from <see cref="MonitoredObject"/> class).
        /// By default, this flag is set to false and must be enabled manually.
        /// </summary>
        /// 
        public bool ForwardChangedEventsToDatabase { get; protected set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance with an empty collection without forwarding
        /// events to owning database.
        /// </summary>
        ///
        public GenericObjectCollection( VideoRentalOutlet database, 
                string className, string itemName )
            : base( database, className )
        {
            this.Items = new List<T> ();

            this.ItemName = itemName;

            this.ForwardChangedEventsToDatabase = false;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Removes item from item collection.
        /// </summary>
        /// 
        public virtual bool Remove( T item )
        {
            // We can remove only existing customers
            //
            if ( item == null || ! this.Items.Contains( item ) ) 
            {
                return false;
            }

            item.OnRemove ();

            bool rc = this.Items.Remove( item );

            if ( rc )
            {
                OnRemoved( item );
            }

            return rc;
        }

        /// <summary>
        /// Adds item to collection.
        /// </summary>
        /// 
        internal virtual void Add( T item )
        {
            if ( item == null )
            {
                return; // nothing to add
            }

            if ( this.Items.Contains( item ) )
            {
                throw new ArgumentException( "Can't duplicate items in " + ClassName );
            }

            this.Items.Add( item );

            OnAddedNew( item );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handlers ]

        /// <summary>
        /// Executed when a new object is added to database. Raises Changed event.
        /// </summary>
        ///
        public override void OnAddedNew( GenericObject item, string reason = null )
        {
            if ( ForwardChangedEventsToDatabase )
            {
                Database.OnAddedNew( item, reason );
            }

            base.OnAddedNew( item, reason );
        }

        /// <summary>
        /// Executed when existing object is removed from database. Raises Changed event.
        /// </summary>
        ///
        public override void OnRemoved( GenericObject item, string reason = null )
        {
            if ( ForwardChangedEventsToDatabase )
            {
                Database.OnRemoved( item, reason );
            }

            base.OnRemoved( item, reason );
        }

        /// <summary>
        /// Executed when existing object is updated. Raises Changed event.
        /// </summary>
        ///
        public override void OnUpdated( GenericObject item, string reason = null )
        {
            if ( ForwardChangedEventsToDatabase )
            {
                Database.OnUpdated( item, reason );
            }

            base.OnUpdated( item, reason );
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ IList<T> Interface Implementation ]
        
        /// <summary>
        /// Returns number of items in collection.
        /// </summary>
        /// 
        public int Count
        {
            get { return this.Items.Count; }
        }

        /// <summary>
        /// Returns always false as collections are never read-only.
        /// </summary>
        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets item at position given by index. Throws exception if user
        /// tries to change item at some position directly (as replacing items in
        /// database is not allowed).
        /// </summary>
        /// <remarks>Throws InvalidOperationException on set.</remarks>
        /// 
        public T this[ int index ]
        {
            get { return this.Items[ index ]; }
            set { throw new InvalidOperationException( 
                    "Replacing items in " + ClassName + " is not allowed." ); }
        }

        /// <summary>
        /// Gets index of item. Return -1 if item is not found.
        /// </summary>
        /// 
        public int IndexOf( T item )
        {
            return this.Items.IndexOf( item );
        }

        /// <summary>
        /// Returns true if collection contains item.
        /// </summary>
        /// 
        public bool Contains( T item )
        {
            return this.Items.Contains( item );
        }

        /// <summary>
        /// Removes item from collection at position given by index.
        /// </summary>
        /// 
        public void RemoveAt( int index )
        {
            T item = this.Items[ index ];

            if ( item != null )
            {
                Remove( item );
            }
        }

        /// <summary>
        /// Removes all items from collection.
        /// </summary>
        /// 
        public void Clear ()
        {
            while( this.Items.Count > 0 )
            {
                RemoveAt( 0 );
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the GenericObjectCollection<T>.
        /// </summary>
        ///
        public IEnumerator<T> GetEnumerator ()
        {
            return this.Items.GetEnumerator ();
        }

        #region [ Explicitly Implemented IList Methods (not used normally) ]

        /// <summary>
        /// Adds item to collection.
        /// </summary>
        ///
        void ICollection<T>.Add( T item )
        {
            Add( item );
        }

        /// <summary>
        /// Removes item from collection.
        /// </summary>
        ///
        bool ICollection<T>.Remove( T item )
        {
            return Remove( item );
        }

        /// <summary>
        /// Inserts item at position. Throws InvalidOperationException as
        /// inserting items is prohibited.
        /// </summary>
        /// 
        void IList<T>.Insert( int index, T item )
        {
            throw new InvalidOperationException(
                "Freeely inserting items in " + ClassName + " is not allowed." );
        }

        /// <summary>
        /// Copies the entire GenericObjectCollection<T> to a compatible one-dimensional 
        /// array, starting at the specified index of the target array. 
        /// </summary>
        /// 
        void ICollection<T>.CopyTo( T[] array, int arrayIndex )
        {
            this.Items.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Returns an enumerator that iterates through the GenericObjectCollection<T>.
        /// </summary>
        ///
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.Items.GetEnumerator ();
        }

        #endregion

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ IList Interface Implementation ]

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size. 
        /// </summary>
        /// 
        public bool IsFixedSize
        {
            get { return this.IsFixedSize; }
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// 
        public bool Contains( object value )
        {
            return value is T && this.Contains( value as T );
        }

        /// <summary>
        /// Determines the index of a specific item in the collection. 
        /// </summary>
        /// 
        public int IndexOf( object value )
        {
            return value is T ? this.IndexOf( value ) : -1;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <remarks>Throws NotImplementedException on set.</remarks>
        /// 
        object IList.this[ int index ]
        {
            get { return this.Items[ index ]; }
            set { throw new InvalidOperationException( 
                    "Replacing items in " + ClassName + " is not allowed." ); }
        }

        #region [ Not Implemeneted IList Methods ]

        /// <summary>
        /// Adds an item to the IList. 
        /// </summary>
        /// 
        public int Add( object value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index. 
        /// </summary>
        /// <remarks>Throws NotImplementedException.</remarks>
        /// 
        public void Insert( int index, object value )
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection. 
        /// </summary>
        /// <remarks>Throws NotImplementedException.</remarks>
        ///
        public void Remove( object value )
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular 
        /// Array index.
        /// </summary>
        /// <remarks>Throws NotImplementedException.</remarks>
        /// 
        public void CopyTo( Array array, int index )
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized
        /// (thread safe). 
        /// </summary>
        /// <remarks>Throws NotImplementedException.</remarks>
        /// 
        public bool IsSynchronized
        {
            get { throw new NotImplementedException (); }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        /// <remarks>Throws NotImplementedException.</remarks>
        /// 
        public object SyncRoot
        {
            get { throw new NotImplementedException (); }
        }

        #endregion

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ ToString() and FullInfo() Virtual Methods ]

        /// <summary>
        /// Gets brief contents of this collection with one item per line.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            foreach( T item in this.Items )
            {
                if ( sb.Length != 0 )
                {
                    sb.AppendLine ();
                }

                sb.Append( item.ToString () );
            }

            return sb.ToString ();
        }

        /// <summary>
        /// Gets a thorough contents of this collection with full info about items.
        /// </summary>
        /// 
        public override string FullInfo ()
        {
            StringBuilder sb = new StringBuilder ();

            if ( this.Items.Count == 0 )
            {
                sb.AppendLine().AppendLine( "(Empty)" );
            }

            foreach( T item in this.Items )
            {
                sb.AppendLine()
                  .Append( "_______________________ " )
                  .AppendLine( ( ItemName + " " ).PadRight( 46, '_' ) )
                  .Append( item.FullInfo () );
            }

            return sb.ToString ();
        }

        #endregion
    }
}