// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DealingWithDuplicatedQueries.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
	/// Demonstrates dealing with duplicated business rules with specifications.
	/// </summary>
	[TestFixture]
	public class DealingWithDuplicatedBusinessRules
	{
	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public DealingWithDuplicatedBusinessRules()
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
        public void WithoutSpecifications()
        {
            var queryRunner = ObjectFactory.GetInstance<IQueryRunner<Blog>>();

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blog = queryRunner.GetQueryResult(new BlogByName(this.blogName, this.dataCommandProvider));

                /*
                 * Only print the post comments if the blog is active and has posts.
                 */

                if (blog.IsActive && blog.Posts.Any())
                {
                    blog.PrintPostComments();
                }

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Demonstrates using specifications outside of the domain.
        /// </summary>
        [Test]
        public void UsingSpecificationsOutsideOfTheDomain()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blogs = dataCommandProvider.Session.Query<Blog>().Where(BlogSpecifications.IsActiveAndHasPosts().SatisfiedBy()).ToList();

                foreach (var blog in blogs)
                {
                    blog.PrintPostComments();
                }

                unitOfWork.Commit();
            }
        }
	}
}