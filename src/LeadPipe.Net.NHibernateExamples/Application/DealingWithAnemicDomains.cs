// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DealingWithAnemicDomains.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using LeadPipe.Net.Data;
using LeadPipe.Net.Data.NHibernate;
using LeadPipe.Net.Extensions;
using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Linq;
using NUnit.Framework;
using StructureMap;

namespace LeadPipe.Net.NHibernateExamples.Application
{
	/// <summary>
	/// Demonstrates anemic domains and how to deal with them.
	/// </summary>
	[TestFixture]
	public class DealingWithAnemicDomains
	{
	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public DealingWithAnemicDomains()
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
                        Console.WriteLine("{0} said {1}".FormattedWith(comment.Commenter, comment.Text));
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
	}
}