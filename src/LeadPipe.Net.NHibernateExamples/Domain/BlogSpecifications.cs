// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostSpecifications.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using LeadPipe.Net.Specifications;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The blog specifications.
	/// </summary>
	public static class BlogSpecifications
	{
		/// <summary>
		/// The blog has posts.
		/// </summary>
		/// <returns>
		/// True if the blog has posts.
		/// </returns>
		public static ISpecification<Blog> HasPosts()
		{
			return new AdHocSpecification<Blog>(blog => blog.Posts.Any());
		}

        /// <summary>
        /// Determines whether the blog has posts with comments enabled.
        /// </summary>
        /// <returns>True if the blog has posts with comments enabled.</returns>
        public static ISpecification<Blog> HasPostsWithCommentsEnabled()
        {
            return new AdHocSpecification<Blog>(blog => blog.Posts.Any(post => post.CommentsEnabled));
        }

        /// <summary>
        /// Determines whether the blog is active.
        /// </summary>
        /// <returns>True if the blog is active.</returns>
        public static ISpecification<Blog> IsActive()
        {
            return new AdHocSpecification<Blog>(blog => blog.IsActive);
        }

        /// <summary>
        /// Determines whether the blog is active and has posts.
        /// </summary>
        /// <returns>True if the blog is active and has posts.</returns>
	    public static ISpecification<Blog> IsActiveAndHasPosts()
	    {
	        return new AndSpecification<Blog>(HasPosts(), IsActive());
	    }

        /// <summary>
        /// Determines whether the blog is active or has posts.
        /// </summary>
        /// <returns>True if the blog is active or has posts.</returns>
        public static ISpecification<Blog> IsActiveOrHasPosts()
        {
            return new OrSpecification<Blog>(HasPosts(), IsActive());
        }

        /// <summary>
        /// Determines whether the blog is not active.
        /// </summary>
        /// <returns>True if the blog is not active.</returns>
        public static ISpecification<Blog> IsNotActive()
        {
            return new NotSpecification<Blog>(IsActive());
        }
	}
}