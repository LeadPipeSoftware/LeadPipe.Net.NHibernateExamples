// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DealingWithDuplicatedBusinessRules.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using LeadPipe.Net.Data;
using LeadPipe.Net.Data.NHibernate;
using LeadPipe.Net.Domain;
using LeadPipe.Net.NHibernateExamples.Domain;
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
        /// Demonstrates how to use a Query Object.
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
                 * that have posts with comments. This is different than the other examples.
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