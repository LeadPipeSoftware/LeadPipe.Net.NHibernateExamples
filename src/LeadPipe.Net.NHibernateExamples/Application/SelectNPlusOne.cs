// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectNPlusOne.cs" company="Lead Pipe Software">
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
	/// Demonstrates the Select N+1 alert and how to deal with them.
	/// </summary>
	[TestFixture]
	public class SelectNPlusOne
	{
        /*
         * http://hibernatingrhinos.com/Products/NHProf/learn/Alert/SelectNPlusOne
         * 
         * O/RM tools such as NHibernate are great for dealing with what we call impedance
         * mismatch between our relational database and the object graph. However, if you don't
         * know how to use your O/RM you can run into serious performance issues very fast. One
         * of the most common problems is what we call the SELECT N+1 problem.
         * 
         * Most O/RM's have a default behavior known as lazy loading. Effectively, this means
         * that the child of a parent-child relationship isn't hydrated until it's actually
         * needed. This is great from the perspective of keeping our queries as light and as
         * performant as possible. After all, why bother hydrating a bunch of other objects if
         * they don't actually get used?
         * 
         * The downside to lazy loading is when the child of a parent-child relationship is a
         * collection. If we load the parent and then proceed to enumerate the children then
         * the O/RM will begin issuing single SELECT statements to hydrate each child one at a
         * time. The O/RM simply can't know that you're going to ultimately be touching each of
         * the children and does the least amount of work that it can.
         * 
         * Solving this is really quite simple; we just tell our O/RM that it should go ahead
         * and hydrate the children. We know that we'll be needing them and the O/RM can issue
         * a nice, efficient query (usually a JOIN) to get us what we need.
         */

	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public SelectNPlusOne()
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
		/// Demonstrates an N+1 problem.
		/// </summary>
		[Test]
		public void Problem()
		{
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
			{
			    var blog = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .FirstOrDefault(x => x.Name == this.blogName);

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
        /// Demonstrates using FetchMany to reduce N+1 problems.
        /// </summary>
        [Test]
        public void UsingFetchMany()
        {
            /*
             * A simple way to tell NHibernate that we want to fetch child collections is to use
             * the FetchMany method. This will instruct NHibernate to fetch the referenced
             * collection using whatever fetch strategy you've set up in your map:
             * 
             * bag.Fetch(CollectionFetchMode.Join);
             * 
             * This by itself can make a dramatic difference. Be careful! If you use more than one
             * FetchMany calls in the same query you're effectively telling NHibernate to create a
             * cartesian product which is almost certainly NOT what you want. Cartesian products
             * are essentially every possible combination of values and will result in what you'll
             * probably consider very strange duplicates in the result set.
             */

            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

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
                        Console.WriteLine("{0} said {1}".FormattedWith(comment.Commenter, comment.Text));
                    }
                }

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Demonstrates using ThenFetchMany to reduce N+1 problems.
        /// </summary>
        [Test]
        [ExpectedException]
        public void UsingThenFetchMany()
        {
            /*
             * If one is good then more is better, right? This is where ThenFetchMany seems like it
             * would make sense to fetch the grandchildren. However, as of v4, NHibernate doesn't
             * let you use ThenFetchMany when you have multiple bag maps with a fetch type of JOIN.
             * 
             * This will cause NHibernate to throw an exception.
             */
            
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blog = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .Where(b => b.Name == this.blogName)
                    .FetchMany(b => b.Posts)
                    .ThenFetchMany(p => p.Comments)
                    .First();

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
        /// Demonstrates using ToFuture to solve N+1 problems.
        /// </summary>
        [Test]
        public void UsingToFuture()
        {
            /*
             * The good news is that we CAN get what we want, but in a somewhat non-intuitive way.
             * Calling .Future will return an IEnumerable<T> that will not be populated until one
             * of the results is needed. NHibernate will build up queries using .Future or
             * .FutureValue until one of the results are accessed.
             * 
             * Oh, and .FutureValue is essentially a deferred version of .SingleOrDefault in case
             * you were wondering.
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

                var blog = query.FirstOrDefault(); // Set a breakpoint here so you can see that no
                                                   // SQL is actually issued until this line is
                                                   // executed in the profiler.

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
	}
}