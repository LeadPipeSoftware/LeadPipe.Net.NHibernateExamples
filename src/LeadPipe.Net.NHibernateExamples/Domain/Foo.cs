// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Foo.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The Foo.
	/// </summary>
	public class Foo : PersistableObject<int>, IEntity
	{
	    #region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Foo"/> class.
		/// </summary>
		/// <param name="name">
		/// The name.
		/// </param>
		public Foo(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Foo"/> class.
		/// </summary>
		protected Foo()
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