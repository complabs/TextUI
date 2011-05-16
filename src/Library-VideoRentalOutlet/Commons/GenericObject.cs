/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib
 *  File:       GenericObject.cs
 *  Created:    2011-04-08
 *  Modified:   2011-04-30
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;

namespace VROLib
{
    /// <summary>
    /// Generic VideoRentalOutlet object. All classes having objects in VRO database
    /// should be derived from this class. This is very important so database objects
    /// could be matched against owning database.
    /// </summary>
    /// 
    [Serializable] 
    public class GenericObject
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Reference to database that owns this object.
        /// </summary>
        /// 
        public VideoRentalOutlet Database { get; protected set; }

        /// <summary>
        /// A unique identifier of this object in database.
        /// </summary>
        /// 
        public virtual int ID { get; protected set; }

        /// <summary>
        /// Generic name of the class, like "Customer", "Movie", "Movie Collection" etc.,
        /// which is ususally used by ToString(), FullInfo() or error reporting methods.
        /// </summary>
        /// 
        public string ClassName { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the generic VideoRentalOutlet database object.
        /// </summary>
        /// 
        public GenericObject( VideoRentalOutlet database, string className )
        {
            this.Database  = database;
            this.ClassName = className;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Methods ]

        /// <summary>
        /// Executed when record is about to be removed from database.
        /// </summary>
        /// 
        internal virtual void OnRemove ()
        {
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation
        /// with full details about the record. 
        /// </summary>
        /// <remarks>By default equals to <see cref="ToString"/></remarks>
        /// 
        public virtual string FullInfo ()
        {
            return this.ToString ();
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// Default implementation returns class name concatenated with object's 
        /// identifier, e.g. "Customer #3".
        /// </summary>
        /// 
        public override string ToString ()
        {
            return this.ClassName + " #" + this.ID;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Methods Equals() and GetHashCode() ]

        /// <summary>
        /// Determines whether this instance and a specified object are the same.
        /// Objects are the same if they have both type and neigher of them is null
        /// and they have same ID.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if obj type is the same type and they have same IDs; 
        /// otherwise, false.</returns>
        /// 
        public override bool Equals( object obj )
        {
            // Return false if obj is either null or if it is not of
            // the same type as our instance.
            //
            GenericObject otherObject = obj as GenericObject;
            if ( obj == null )
            {
                return false;
            }

            // Two GenericObject are same if have the same ID
            //
            return this.ID == otherObject.ID;
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// 
        public override int GetHashCode ()
        {
            return this.ID;
        }

        #endregion
    }
}