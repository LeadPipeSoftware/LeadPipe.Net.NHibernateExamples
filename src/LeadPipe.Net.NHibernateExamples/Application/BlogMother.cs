// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlogMother.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
		public static Blog CreateBlogWithPostsAndComments(string name, int numberOfPosts = 5, int numberOfCommentsPerPost = 10)
		{
            var blog = new Blog(name);

		    for (var i = 0; i < numberOfPosts; i++)
		    {
                blog.AddPost(RandomValueProvider.RandomString(25, true));
		    }

		    foreach (var post in blog.Posts)
		    {
		        post.AddComment(RandomValueProvider.RandomString(10, true), RandomValueProvider.LoremIpsum(20));
		    }

		    return blog;

		}
	}
}