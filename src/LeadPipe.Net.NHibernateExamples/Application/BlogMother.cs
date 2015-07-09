// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlogMother.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using LeadPipe.Net.NHibernateExamples.Domain;

namespace LeadPipe.Net.NHibernateExamples.Application
{
	/// <summary>
	/// Provides Blog objects for unit testing.
	/// </summary>
	public class BlogMother
	{
        /// <summary>
        /// Creates a test blog with posts and comments.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="numberOfPosts">The number of posts.</param>
        /// <param name="numberOfCommentsPerPost">The number of comments per post.</param>
        /// <returns>The blog.</returns>
		public static Blog CreateBlogWithPostsAndComments(string name, int numberOfPosts = 5, int numberOfCommentsPerPost = 10)
		{
            var blog = new Blog(name);

		    for (var i = 0; i < numberOfPosts; i++)
		    {
                blog.AddPost(RandomValueProvider.RandomString(25, true));
		    }

		    foreach (var post in blog.Posts)
		    {
		        for (var i = 0; i < numberOfCommentsPerPost; i++)
		        {
                    post.AddComment(RandomValueProvider.RandomString(10, true), RandomValueProvider.LoremIpsum(20));
		        }
		    }

		    return blog;

		}

        /// <summary>
        /// Creates multiple blogs with posts and comments.
        /// </summary>
        /// <param name="numberOfBlogs">The number of blogs.</param>
        /// <param name="numberOfPosts">The number of posts.</param>
        /// <param name="numberOfCommentsPerPost">The number of comments per post.</param>
        /// <returns>Lots of blogs.</returns>
	    public static IList<Blog> CreateBlogsWithPostsAndComments(int numberOfBlogs = 10, int numberOfPosts = 5, int numberOfCommentsPerPost = 10)
	    {
	        var blogs = new List<Blog>();

	        for (var i = 0; i < numberOfBlogs; i++)
	        {
	            blogs.Add(CreateBlogWithPostsAndComments(RandomValueProvider.RandomString(25, true), numberOfPosts, numberOfCommentsPerPost));
	        }

	        return blogs;
	    }
	}
}