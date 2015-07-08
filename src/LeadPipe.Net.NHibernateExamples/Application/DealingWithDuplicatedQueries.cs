// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DealingWithDuplicatedBusinessRules.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
	/// Demonstrates dealing with duplicated queries using query objects.
	/// </summary>
	[TestFixture]
	public class DealingWithDuplicatedQueries
	{
	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public DealingWithDuplicatedQueries()
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

                this.dataCommandProvider.Save(blog);

                unitOfWork.Commit();
            }
	    }

        /// <summary>
        /// Demonstrates commonly duplicated queries.
        /// </summary>
        [Test]
        public void DuplicatedQueries()
        {
            /*
             * All too often we see the same query issued over and over again and, unfortunately,
             * this duplication leads to subtle differences. Not only does that present some risk,
             * but it also makes it a real pain to make changes. For example, when an optimization
             * is implemented how do you find each place in your code to change?
             */

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

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
        /// Demonstrates how to use a Query Object.
        /// </summary>
        [Test]
        public void UsingQueryObjects()
        {
            /*
             * A simple solution to this problem is to encapsulate the query into a reusable object
             * that not only clarifies the code by making it more intention revealing, but also
             * makes it easy to implement changes that will benefit all the callers.
             */

            var queryRunner = ObjectFactory.GetInstance<IQueryRunner<Blog>>();

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

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
            /*
             * Note that in this example I'm using a query object that will return ALL blogs that
             * have posts with comments. This is different than what we've seen so far.
             */
            
            var queryRunner = ObjectFactory.GetInstance<IQueryRunner<IEnumerable<Blog>>>();

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
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