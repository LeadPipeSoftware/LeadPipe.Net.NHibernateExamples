// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Blog.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The blog.
	/// </summary>
	public class Blog : PersistableObject<int>, IEntity
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Blog"/> class.
		/// </summary>
		/// <param name="name">
		/// The blog name.
		/// </param>
		public Blog(string name)
		{
			this.Name = name;
			this.Posts = new List<Post>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Blog"/> class.
		/// </summary>
		protected Blog()
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
		/// Gets or sets the blog name.
		/// </summary>
		public virtual string Name { get; protected set; }

		/// <summary>
		/// Gets or sets the posts.
		/// </summary>
		public virtual IList<Post> Posts { get; protected set; }

		#endregion

        #region Public Methods

        /// <summary>
        /// Prints the post comments.
        /// </summary>
	    public virtual void PrintPostComments()
	    {
	        foreach (var post in this.Posts)
	        {
	            post.PrintComments();
	        }
	    }

        #endregion
    }
}