// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Blog.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The blog.
	/// </summary>
	public class Blog : PersistableObject<Guid>, IEntity
	{
        private readonly IList<Post> posts;
	    private string domainId;

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
			this.posts = new List<Post>();
		    this.IsActive = true;
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
        /// Gets or sets a value indicating whether this blog is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this blog is active; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsActive { get; set; }

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
		/// Gets or sets the blog name.
		/// </summary>
		public virtual string Name { get; protected set; }

	    /// <summary>
	    /// Gets the blog posts.
	    /// </summary>
	    public virtual IEnumerable<Post> Posts
	    {
	        get { return posts; }
	    }

	    #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new blog post.
        /// </summary>
        /// <param name="title">The post title.</param>
	    public virtual void AddPost(string title)
	    {
	        var post = new Post(this, title);

            this.posts.Add(post);
	    }

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