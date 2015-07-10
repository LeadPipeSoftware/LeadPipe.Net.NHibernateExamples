// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Comment.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using LeadPipe.Net.Domain;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// A blog post comment.
	/// </summary>
	public class Comment : PersistableObject<Guid>, IEntity
	{
	    private string domainId;

		#region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment" /> class.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <param name="commenter">The Commenter.</param>
        /// <param name="text">The comment text.</param>
		public Comment(Post post, string commenter, string text)
        {
            this.Post = post;
			this.Commenter = commenter;
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
        /// Gets or sets a value indicating whether the comment has been approved by a moderator.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the comment has been approved by moderator; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ApprovedByModerator { get; set; }

        /// <summary>
        /// Gets a value indicating whether the comment text contains restricted language.
        /// </summary>
        /// <value>
        /// <c>true</c> if the comment contains restricted language; otherwise, <c>false</c>.
        /// </value>
	    public virtual bool ContainsRestrictedLanguage
	    {
	        get
	        {
                var r = new Regex("competitor|poopoo|republican");
                
                return r.IsMatch(this.Text);
	        }
	    }

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
                return this.Commenter + DateTime.UtcNow.ToLongDateString();
            }

            set
            {
                this.domainId = value;
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
		/// Gets or sets the commenter.
		/// </summary>
		public virtual string Commenter { get; protected set; }

		#endregion
	}
}