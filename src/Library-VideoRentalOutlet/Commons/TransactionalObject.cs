/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib
 *  File:       TransactionalObject.cs
 *  Created:    2011-04-14
 *  Modified:   2011-04-28
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
    /// Instances of the TransactionalObject generic class are kept as immutable in 
    /// database. To change an immutable object, one must copy-construct mutable instance
    /// first, then change properties of the mutable instance and commit changes at the 
    /// end. To create new TransactionalObject object, one must call parameterless 
    /// constructor (or copy-constructor with null as source).
    /// </summary>
    /// <remarks>
    /// E.g. to add new object to database:
    /// <example>
    ///     Customer c = new Customer ();  // or c = new Customer( null );
    ///     c.Property = value;
    ///     c.AddTo( database );
    /// </example>
    /// To modify existing object:
    /// <example>
    ///     Customer c = new Customer( existingCustomer ); // where existing != null
    ///     c.Property = value;
    ///     c.Update ();
    /// </example>
    /// </remarks>
    /// 
    [Serializable] 
    public class TransactionalObject<T> : GenericObject
        where T: GenericObject
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Fields ]

        /// <summary>
        /// If this instance is created using copy-constructor, this field contains
        /// a reference to original (immutable) record from which this instance is
        /// copied from. Note that this field is not saved during serialization.
        /// </summary>
        /// 
        [NonSerialized]
        private T baseRecord;

        /// <summary>
        /// Read-only flag for the record. Properties of the immutable records can not 
        /// be changed.
        /// </summary>
        /// 
        [NonSerialized]
        private bool mutable;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets 'mutable' flag for the record. Properties of the
        /// immutable records can not be changed (one must copy-construct mutable
        /// record from immutable and later save changes using Update() method).
        /// </summary>
        /// 
        public bool Mutable 
        { 
            get { return this.mutable; }
        }

        /// <summary>
        /// Returns true if object is not copy-constructed, but created as an empty 
        /// record (with default values). 
        /// This property is valid only for mutable instances.
        /// </summary>
        /// 
        protected bool IsNew 
        { 
            get { return this.baseRecord == null; }
        }

        /// <summary>
        /// Returns base record from which this object is constructed from.
        /// This property is valid only for mutable instances.
        /// </summary>
        /// 
        protected T BaseRecord
        {
            get { return this.baseRecord; }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new (mutable) instance of the T class either as an
        /// empty (without argument) or as a copy-constructed object (from an existing 
        /// immutable instance from database).
        /// See also <see cref="AddTo"/> and <see cref="Update"/> methods.
        /// </summary>
        /// <remarks>
        /// The constructor does not insert or update database. To save changes to 
        /// database, use either <see cref="AddTo"/> method after construction without 
        /// original object, or <see cref="Update"/> method after copy-construction 
        /// from an existing original immutable object. Initially constructed objects 
        /// are all mutable until *saved to database*, after which they become immutable.
        /// To modify immutable object one must copy-construct new instance and later 
        /// call <see cref="Update"/> to commit changes. This ensures transactioncal 
        /// change of all records and proper delivery of on-changed events to event 
        /// handlers. 
        /// </remarks>
        ///
        protected TransactionalObject( T original, string className )
            : base( original == null ? null : original.Database, className )
        {
            this.baseRecord = original;
            this.mutable = true;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Methods ]

        /// <summary>
        /// Sets record properties to be read-only, i.e. record to be immutable.
        /// </summary>
        /// 
        protected void LockProperties ()
        {
            this.mutable = false;
        }

        /// <summary>
        /// Throws field access exception if instance is not mutable.
        /// </summary>
        /// 
        protected void ThrowExceptionIfNotMutable ()
        {
            if ( ! Mutable )
            {
                throw new FieldAccessException( "Record is not available for editing." );
            }
        }

        /// <summary>
        /// Verifies whether this instance may be added to database as a new.
        /// </summary>
        ///
        protected void VerifyAddTo( VideoRentalOutlet database )
        {
            ThrowExceptionIfNotMutable ();

            if ( database == null )
            {
                throw new ArgumentNullException( 
                    "Record must belong to some database." );
            }

            VerifyIntegrity ();
        }

        /// <summary>
        /// Verifies whether changes to this instance may be commited.
        /// </summary>
        ///
        protected void VerifyUpdate ()
        {
            ThrowExceptionIfNotMutable ();

            if ( IsNew )
            {
                throw new Exception( 
                    "Only objects obtained through copy constructor may be Updated." );
            }

            VerifyIntegrity ();
        }

        /// <summary>
        /// Verifies fields integrity of the record.
        /// </summary>
        /// 
        protected virtual void VerifyIntegrity ()
        {
        }

        /// <summary>
        /// Copies fields from source record. This is low-level method that should
        /// disregard if destination record is immutable.
        /// </summary>
        ///
        protected virtual void SetFieldsFrom( T source )
        {
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// Default implementation returns class name concatenated with object's 
        /// identifier and mutability flag, e.g. "Customer #3 (mutable)".
        /// </summary>
        /// 
        public override string ToString ()
        {
            return this.ClassName + " #" + this.ID 
                + ( this.Mutable ? " (mutable)" : string.Empty );
        }

        #endregion
    }
}