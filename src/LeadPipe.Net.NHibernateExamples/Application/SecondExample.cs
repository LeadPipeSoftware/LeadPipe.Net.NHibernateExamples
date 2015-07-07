// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecondExample.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using LeadPipe.Net.Data;
using LeadPipe.Net.Data.NHibernate;
using LeadPipe.Net.Domain;
using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Linq;
using NUnit.Framework;
using StructureMap;

namespace LeadPipe.Net.NHibernateExamples.Application
{
	/// <summary>
	/// The second set of examples.
	/// </summary>
	[TestFixture]
	public class SecondExample
	{
	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public SecondExample()
	    {
            Bootstrapper.Start();

            this.dataCommandProvider = ObjectFactory.GetInstance<DataCommandProvider>();
            this.unitOfWorkFactory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();            
	    }

	    [TestFixtureSetUp]
	    public void TestFixtureSetup()
	    {
            this.blogName = RandomValueProvider.RandomString(25, true);

	        var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork();

	        using (unitOfWork.Start())
            {
                var blog = BlogMother.CreateBlogWithPostsAndComments(blogName);

                //this.dataCommandProvider.Session.SetBatchSize(blog.Posts.Count);

                this.dataCommandProvider.Save(blog);

                //this.dataCommandProvider.Session.SetBatchSize(0);

                unitOfWork.Commit();
            }
	    }

        /// <summary>
        /// Demonstrates stolen behavior.
        /// </summary>
        [Test]
        public void StolenBehavior()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            // Assert
            using (unitOfWork.Start())
            {
                var query = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(b => b.Name == this.blogName)
                    .FetchMany(b => b.Posts)
                    .ToFuture();

                this.dataCommandProvider.Session.Query<Post>()
                    .Where(p => p.Blog.Name == this.blogName)
                    .FetchMany(p => p.Comments)
                    .ToFuture();

                var blog = query.FirstOrDefault();

                /*
                 * The nested foreach represents stolen behavior. In other words, the domain could
                 * be doing this work.
                 */

                foreach (var post in blog.Posts)
                {
                    foreach (var comment in post.Comments)
                    {
                        Console.WriteLine(comment.Text);
                    }
                }

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Demonstrates pushing behavior back into the domain.
        /// </summary>
        [Test]
        public void ImprovedDomain()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            // Assert
            using (unitOfWork.Start())
            {
                var query = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(b => b.Name == this.blogName)
                    .FetchMany(b => b.Posts)
                    .ToFuture();

                this.dataCommandProvider.Session.Query<Post>()
                    .Where(p => p.Blog.Name == this.blogName)
                    .FetchMany(p => p.Comments)
                    .ToFuture();

                var blog = query.FirstOrDefault();

                blog.PrintPostComments();

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// A demonstration of Query Objects.
        /// </summary>
        [Test]
        public void UsingQueryObjects()
        {
            var queryRunner = ObjectFactory.GetInstance<IQueryRunner<Blog>>();

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            // Assert
            using (unitOfWork.Start())
            {
                var blog = queryRunner.GetQueryResult(new BlogByName(this.blogName, this.dataCommandProvider));

                blog.PrintPostComments();

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Another demonstration of Query Objects.
        /// </summary>
        [Test]
        public void MoreQueryObjects()
        {
            var queryRunner = ObjectFactory.GetInstance<IQueryRunner<IEnumerable<Blog>>>();

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            // Assert
            using (unitOfWork.Start())
            {
                /*
                 * Note that in this example I'm using a query object that will return ALL blogs
                 * that have posts with comments.
                 */

                var blogs = queryRunner.GetQueryResult(new BlogsWithPostsThatHaveComments(this.dataCommandProvider));
                
                foreach (var blog in blogs)
                {
                    blog.PrintPostComments();
                }

                unitOfWork.Commit();
            }
        }
	}
}