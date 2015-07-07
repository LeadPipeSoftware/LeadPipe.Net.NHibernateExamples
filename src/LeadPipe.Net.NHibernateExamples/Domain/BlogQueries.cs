// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModelsWithTestPropertiesThatStartWithABC.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using LeadPipe.Net.Data;
using NHibernate.Linq;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
    /// <summary>
    /// A query that returns all the blogs that have posts with comments.
    /// </summary>
    public class BlogByName : Query<Blog>
    {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogByName" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        public BlogByName(string name, IDataCommandProvider dataCommandProvider)
            : base(dataCommandProvider)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns></returns>
        public override Blog GetResult()
        {
            var query = base.dataCommandProvider
                    .Query<Blog>()
                    .Where(b => b.Name == this.name)
                    .FetchMany(b => b.Posts)
                    .ToFuture();

            base.dataCommandProvider.Query<Post>()
                .FetchMany(p => p.Comments)
                .ToFuture();

            var blog = query.FirstOrDefault();

            return blog;
        }
    }

    /// <summary>
    /// A query that returns all the blogs that have posts with comments.
    /// </summary>
    public class BlogsWithPostsThatHaveComments : Query<IEnumerable<Blog>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogsWithPostsThatHaveComments"/> class.
        /// </summary>
        /// <param name="dataCommandProvider">The data command provider.</param>
        public BlogsWithPostsThatHaveComments(IDataCommandProvider dataCommandProvider)
            : base(dataCommandProvider)
        {
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Blog> GetResult()
        {
            var query = base.dataCommandProvider
                    .Query<Blog>()
                    .FetchMany(b => b.Posts)
                    .ToFuture();

            base.dataCommandProvider.Query<Post>()
                .FetchMany(p => p.Comments)
                .ToFuture();

            var blogs = query.ToList();

            var blogsWithComments = new List<Blog>();

            foreach (var blog in blogs)
            {
                blogsWithComments.AddRange(from post in blog.Posts where post.Comments.Any() select blog);
            }

            return blogsWithComments;
        }
    }
}