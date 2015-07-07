// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstExample.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
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
	/// The first set of examples.
	/// </summary>
	[TestFixture]
	public class FirstExample
	{
	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public FirstExample()
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
		/// Demonstrates an N+1 problem.
		/// </summary>
		[Test]
		public void NPlusOne()
		{
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();           

			// Assert
			using (unitOfWork.Start())
			{
			    var blog = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .FirstOrDefault(x => x.Name == this.blogName);

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
        /// Demonstrates using FetchMany.
        /// </summary>
        [Test]
        public void FetchMany()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            // Assert
            using (unitOfWork.Start())
            {
                var blog = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(b => b.Name == this.blogName)
                    .FetchMany(b => b.Posts)
                    .First();

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
        /// Demonstrates using ThenFetchMany.
        /// </summary>
        [Test]
        public void ThenFetchMany()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            // Assert
            using (unitOfWork.Start())
            {
                var blog = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(b => b.Name == this.blogName)
                    .FetchMany(b => b.Posts)
                    .ThenFetchMany(p => p.Comments)
                    .First();

                /*
                 * As of v4, NHibernate doesn't let you use ThenFetchMany when you have multiple
                 * bag maps with a fetch type of JOIN. Cue the sad trombone noise.
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
        /// Demonstrates using ToFuture.
        /// </summary>
        [Test]
        public void ToFuture()
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            // Assert
            using (unitOfWork.Start())
            {
                /*
                 * Calling .Future will return an IEnumerable<T> that will not be populated until
                 * one of the results is needed. NHibernate will build up queries using .Future or
                 * .FutureValue until one of the results are accessed.
                 * 
                 * Oh, and .FutureValue is essentially a deferred version of .SingleOrDefault.
                 */

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
	}
}