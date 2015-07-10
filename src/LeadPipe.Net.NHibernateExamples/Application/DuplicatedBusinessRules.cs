// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplicatedBusinessRules.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using LeadPipe.Net.Data;
using LeadPipe.Net.Data.NHibernate;
using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Linq;
using NUnit.Framework;
using StructureMap;

namespace LeadPipe.Net.NHibernateExamples.Application
{
	/// <summary>
	/// Demonstrates dealing with duplicated business rules with specifications.
	/// </summary>
	[TestFixture]
	public class DuplicatedBusinessRules
	{
        /*
         * Let's start with a common pattern. Our business has said that only active blogs will
         * have printable comments. In other words, if the blog is not active then no comments
         * will ever be displayed. We're going to query for all the blogs and then enumerate
         * them looking for the ones that meet that business rule.
         */

	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public DuplicatedBusinessRules()
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
        /// Demonstrates domains without specifications.
        /// </summary>
        [Test]
        public void Problem()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blogs = dataCommandProvider.Session
                    .Query<Blog>()
                    .ToList();

                foreach (var blog in blogs)
                {
                    if (blog.IsActive && blog.Posts.Any())
                    {
                        blog.PrintPostComments();
                    }
                }

                unitOfWork.Commit();
            }
        }
        
        /// <summary>
        /// Demonstrates using specifications to query.
        /// </summary>
        [Test]
        public void UsingSpecificationsToQuery()
        {
            /*
             * It doesn't take much for simple business rules to be implemented all over our code
             * and, as a result, it's easy to get in trouble if the business rule changes. To help
             * fix this problem, we're going to encapsulate the business logic in a specification
             * and then use that to execute our query.
             */

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blogs = dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(BlogSpecifications.IsActiveAndHasPosts().SatisfiedBy())
                    .ToList();

                foreach (var blog in blogs)
                {
                    blog.PrintPostComments();
                }

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Demonstrates using specifications to query and using ToFuture.
        /// </summary>
        [Test]
        public void UsingSpecificationsToQueryWithFetchManyAndToFuture()
        {
            /*
             * A quick look at the profiler shows us that both of our previous examples have an N+1
             * problem. We can solve that with ToFuture.
             */

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var query = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(BlogSpecifications.IsActiveAndHasPosts().SatisfiedBy())
                    .FetchMany(b => b.Posts)
                    .ToFuture();

                this.dataCommandProvider.Session.Query<Post>()
                    .FetchMany(p => p.Comments)
                    .ToFuture();

                var blogs = query.ToList();

                foreach (var blog in blogs)
                {
                    blog.PrintPostComments();
                }

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Demonstrates using specifications to query and using ToFuture with a non-persisted property.
        /// </summary>
        [Test]
        [ExpectedException]
        public void UsingSpecificationsToQueryAndFetchManyWithNonPersistedProperty()
        {
            /*
             * It seems very logical that we could optimize our queries even further by taking
             * advantage of the HasPrintableComments specification. Unfortunately, that
             * specification uses a property that isn't persisted so we'll get an exception from
             * NHibernate indicating that the property could not be resolved.
             */
            
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var query = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(BlogSpecifications.IsActiveAndHasPosts().SatisfiedBy())
                    .FetchMany(b => b.Posts)
                    .ToFuture();

                this.dataCommandProvider.Session.Query<Post>()
                    .Where(PostSpecifications.HasPrintableComments().SatisfiedBy())
                    .FetchMany(p => p.Comments)
                    .ToFuture();
                
                var blogs = query.ToList();
                
                foreach (var blog in blogs)
                {
                    blog.PrintPostComments();
                }

                unitOfWork.Commit();
            }
        }
	}
}