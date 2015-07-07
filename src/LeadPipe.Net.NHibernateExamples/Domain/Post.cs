// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Post.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// A blog post.
	/// </summary>
	public class Post : PersistableObject<int>, IEntity
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Post"/> class.
		/// </summary>
		/// <param name="title">
		/// The test property.
		/// </param>
		public Post(string title)
		{
			this.Title = title;
            this.Comments = new List<Comment>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Post"/> class.
		/// </summary>
		protected Post()
		{
		}

		#endregion

		#region Public Properties

        /// <summary>
        /// Gets or sets the post's blog.
        /// </summary>
        /// <value>
        /// The blog.
        /// </value>
        public virtual Blog Blog { get; protected set; }

		/// <summary>
		/// Gets the entity key.
		/// </summary>
		public virtual string Key
		{
			get
			{
				return this.Title;
			}
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public virtual string Title { get; protected set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public virtual IList<Comment> Comments { get; protected set; }

		#endregion

        #region Public Methods

        /// <summary>
        /// Prints the comments.
        /// </summary>
	    public virtual void PrintComments()
	    {
            foreach (var comment in this.Comments)
            {
                Console.WriteLine(comment.Text);
            }
	    }

        #endregion
    }
}