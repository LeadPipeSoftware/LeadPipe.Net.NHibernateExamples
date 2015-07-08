// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Post.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LeadPipe.Net.Domain;
using LeadPipe.Net.Extensions;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// A blog post.
	/// </summary>
	public class Post : PersistableObject<int>, IEntity
	{
	    private readonly IList<Comment> comments;

	    #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Post" /> class.
        /// </summary>
        /// <param name="blog">The blog.</param>
        /// <param name="title">The test property.</param>
		public Post(Blog blog, string title)
        {
            this.Blog = blog;
            this.Title = title;
            this.CommentsEnabled = true;
            this.comments = new List<Comment>();
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
        /// Gets or sets a value indicating whether comments are enabled for this post.
        /// </summary>
        /// <value>
        ///   <c>true</c> if comments are enabled for this post; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CommentsEnabled { get; set; }

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
	    /// Gets the post comments.
	    /// </summary>
	    public virtual IEnumerable<Comment> Comments
	    {
	        get { return comments; }
	    }

	    #endregion

        #region Public Methods

        /// <summary>
        /// Adds a comment.
        /// </summary>
        /// <param name="commenter">The commenter.</param>
        /// <param name="text">The comment text.</param>
	    public virtual void AddComment(string commenter, string text)
	    {
            Guard.Will.ThrowException("Comments are disabled for this post.").When(!this.CommentsEnabled);
	        
            var comment = new Comment(this, commenter, text);

            this.comments.Add(comment);
	    }

        /// <summary>
        /// Prints the comments.
        /// </summary>
	    public virtual void PrintComments()
        {
            foreach (var comment in this.Comments)
            {
                Console.WriteLine("{0} said {1}".FormattedWith(comment.Commenter, comment.Text));
            }
	    }

        #endregion
    }
}