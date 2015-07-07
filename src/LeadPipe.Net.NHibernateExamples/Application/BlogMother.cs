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
                var postWithComments = CreatePostWithComments(numberOfCommentsPerPost);

                blog.Posts.Add(postWithComments);
		    }

		    return blog;

		}

        /// <summary>
        /// Creates a post with comments.
        /// </summary>
        /// <returns>A new post with comments.</returns>
	    public static Post CreatePostWithComments(int numberOfComments = 15)
	    {
            var post = new Post(RandomValueProvider.RandomString(25, true));

	        for (var i = 0; i < numberOfComments; i++)
	        {
                var comment = new Comment(RandomValueProvider.RandomString(10, false), RandomValueProvider.LoremIpsum(10));

                post.Comments.Add(comment);
	        }
            
            return post;
	    }
	}
}