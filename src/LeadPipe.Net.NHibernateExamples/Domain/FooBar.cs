// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FooBar.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The FooBar.
	/// </summary>
	public class FooBar : PersistableObject<Guid>, IEntity
	{
	    private string domainId;

	    #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FooBar" /> class.
        /// </summary>
        /// <param name="foo">The foo.</param>
        /// <param name="bar">The bar.</param>
        /// <param name="name">The name.</param>
		public FooBar(Foo foo, Bar bar, string name)
        {
            this.Foo = foo;
            this.Bar = bar;
			this.Name = name;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FooBar"/> class.
        /// </summary>
        protected FooBar()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the bar.
        /// </summary>
        /// <value>
        /// The bar.
        /// </value>
	    public virtual Bar Bar { get; set; }

        /// <summary>
        /// Gets or sets the foo.
        /// </summary>
        /// <value>
        /// The foo.
        /// </value>
	    public virtual Foo Foo { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public virtual string Key
        {
            get
            {
                return this.Name;
            }

            set
            {
                this.domainId = value;
            }
        }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public virtual string Name { get; protected set; }

        /// <summary>
        /// Gets or sets a mutable property.
        /// </summary>
        /// <value>
        ///   <c>true</c> if true; otherwise, <c>false</c>.
        /// </value>
        public virtual bool MutableProperty { get; set; }

	    #endregion

        #region Public Methods

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Equals(FooBar other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;
            
            return Equals(Foo, other.Foo) && Equals(Bar, other.Bar);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != this.GetType()) return false;

            return Equals((FooBar)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Foo != null ? Foo.GetHashCode() : 0);

                hashCode = (hashCode * 397) ^ Bar.GetHashCode();

                return hashCode;
            }
        }

        #endregion
    }
}