/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib
 *  File:       MonitoredObject.cs
 *  Created:    2011-04-08
 *  Modified:   2011-05-07
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace VROLib
{
    /////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// MonitoredObject Changed event handler used to track changes done to an instance
    /// of <see cref="MonitoredObject"/> class.
    /// </summary>
    /// <param name="item">an object that was changed</param>
    /// <param name="how">type of change</param>
    /// 
    public delegate void ChangedEventHandler( GenericObject item, 
        ChangeType how, string reason );
    
    /////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Types of changes tracked for <see cref="MonitoredObject"/>.
    /// </summary>
    /// 
    public enum ChangeType
    {
        Added   = 0,
        Removed = 1,
        Updated = 2,
    }

    /////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Generic class providing changed-event distribution, so the user could track
    /// object changes using event handlers. Collection classes in VROLib are normally 
    /// derived from this class.
    /// </summary>
    /// 
    [Serializable]
    public class MonitoredObject 
        : GenericObject
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// True if the object was modified since the last serialization.
        /// </summary>
        /// 
        public virtual bool IsDirty
        { 
            get { return this.dirty; }
            set { this.dirty = value; }
        }

        [NonSerialized]
        private bool dirty = false;

        /// <summary>
        /// Gets number of current subscribers to Changed event.
        /// </summary>
        /// 
        public int ChangedListenerCount
        {
            get { return this.changedListeners; }
        }

        [NonSerialized]
        private int changedListeners = 0;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Event Handler ]

        /// <summary>
        /// Occurs when the object is changed (added, removed, updated).
        /// </summary>
        /// 
        public event ChangedEventHandler Changed
        {
            add { ++this.changedListeners; this.changed += value;  }
            remove { this.changed -= value; this.changedListeners--; }
        }

        /// <summary>
        /// List of event handlers receiving on-change events.
        /// </summary>
        /// 
        [field:NonSerialized]
        private event ChangedEventHandler changed = null;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Constructs a new instance of MonitoredObject class.
        /// </summary>
        ///
        public MonitoredObject( VideoRentalOutlet database, string name )
            : base( database, name )
        {
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Virtual Methods ]

        /// <summary>
        /// Executed when a new object is added to database.
        /// </summary>
        ///
        public virtual void OnAddedNew( GenericObject item, string reason )
        {
            IsDirty = true;

            if ( this.changed != null )
            {
                this.changed( item, ChangeType.Added, reason );
            }
        }

        /// <summary>
        /// Executed when existing object is removed from database.
        /// </summary>
        ///
        public virtual void OnRemoved( GenericObject item, string reason )
        {
            IsDirty = true;

            if ( this.changed != null )
            {
                this.changed( item, ChangeType.Removed, reason );
            }
        }

        /// <summary>
        /// Executed when properites (or relationships) of an existing object in
        /// database are updated.
        /// </summary>
        ///
        public virtual void OnUpdated( GenericObject item, string reason )
        {
            IsDirty = true;

            if ( this.changed != null )
            {
                this.changed( item, ChangeType.Updated, reason );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}