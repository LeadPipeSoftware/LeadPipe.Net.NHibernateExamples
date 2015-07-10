// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bar.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The Bar.
	/// </summary>
	public class Bar : PersistableObject<Guid>, IEntity
	{
	    private string domainId;

	    #region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Bar"/> class.
		/// </summary>
		/// <param name="name">
		/// The name.
		/// </param>
		public Bar(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Bar"/> class.
		/// </summary>
		protected Bar()
		{
		}

		#endregion

		#region Public Properties

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

	    #endregion
    }
}