// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Comment.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// A blog post comment.
	/// </summary>
	public class Comment : PersistableObject<int>, IEntity
	{
		#region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment" /> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="text">The comment text.</param>
		public Comment(string user, string text)
		{
			this.User = user;
            this.Text = text;
		}

	    /// <summary>
		/// Initializes a new instance of the <see cref="Comment"/> class.
		/// </summary>
		protected Comment()
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
				return this.User + DateTime.UtcNow.ToLongDateString();
			}
		}

        /// <summary>
        /// Gets or sets the comment's post.
        /// </summary>
        /// <value>
        /// The post.
        /// </value>
        public virtual Post Post { get; protected set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public virtual string Text { get; protected set; }

		/// <summary>
		/// Gets or sets user.
		/// </summary>
		public virtual string User { get; protected set; }

		#endregion
	}
}