﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FooBar.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The FooBar.
	/// </summary>
	public class FooBar : PersistableObject<int>, IEntity
	{
	    #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FooBar" /> class.
        /// </summary>
        /// <param name="foo">The foo.</param>
        /// <param name="bar">The bar.</param>
        /// <param name="name">The name.</param>
		public FooBar(Foo foo, Bar bar, string name)
        {
            this.FooId = foo.Sid;
            this.BarId = bar.Sid;
            
            this.Foo = foo;
            this.Bar = bar;
			this.Name = name;
		}

        /// <summary>
        /// Gets or sets the bar.
        /// </summary>
        /// <value>
        /// The bar.
        /// </value>
	    public virtual Bar Bar { get; set; }

        /// <summary>
        /// Gets or sets the bar identifier.
        /// </summary>
        /// <value>
        /// The bar identifier.
        /// </value>
        public virtual int BarId { get; protected set; }

        /// <summary>
        /// Gets or sets the foo.
        /// </summary>
        /// <value>
        /// The foo.
        /// </value>
	    public virtual Foo Foo { get; set; }

        /// <summary>
        /// Gets or sets the foo identifier.
        /// </summary>
        /// <value>
        /// The foo identifier.
        /// </value>
        public virtual int FooId { get; protected set; }

	    /// <summary>
		/// Initializes a new instance of the <see cref="FooBar"/> class.
		/// </summary>
        protected FooBar()
		{
		}

		#endregion

		#region Public Properties

        /// <summary>
		/// Gets the entity key.
		/// </summary>
		public virtual string Key
		{
			get
			{
				return this.Name;
			}
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public virtual string Name { get; protected set; }

	    #endregion
    }
}