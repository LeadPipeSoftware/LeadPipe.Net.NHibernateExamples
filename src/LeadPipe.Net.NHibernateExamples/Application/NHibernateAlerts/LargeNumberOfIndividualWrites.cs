// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LargeNumberOfIndividualWrites.cs" company="Lead Pipe Software">
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

namespace LeadPipe.Net.NHibernateExamples.Application.NHibernateAlerts
{
	/// <summary>
	/// Demonstrates the Large Number of Individual Writes alert.
	/// </summary>
	[TestFixture]
	public class LargeNumberOfIndividualWrites
	{
        /*
         * http://hibernatingrhinos.com/Products/NHProf/learn/Alert/LargeNumberOfWrites
         * 
         * This warning is raised when the profiler detects that you are writing a lot of data to
         * the database. Similar to the warning about too many calls to the database, the main
         * issue here is the number of remote calls and the time they take.
         * 
         * We can batch together several queries using NHibernate's support for MultiQuery and
         * MultiCriteria, but a relatively unknown feature for NHibernate is the ability to batch a
         * set of write statements into a single database call.
         * 
         * This is controlled using the adonet.batch_size setting in the configuration. If you set
         * it to a number larger than zero, you can immediately start benefiting from reduced
         * number of database calls. You can even set this value at runtime, using
         * session.SetBatchSize().
         */

        private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public LargeNumberOfIndividualWrites()
	    {
            Bootstrapper.Start();

            this.dataCommandProvider = ObjectFactory.GetInstance<DataCommandProvider>();
            this.unitOfWorkFactory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();
	    }
       
        /// <summary>
        /// Demonstrates a large number of individual writes.
        /// </summary>
        [Test]
        public void Problem()
        {
            var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blogs = BlogMother.CreateBlogsWithPostsAndComments(50);

                foreach (var blog in blogs)
                {
                    this.dataCommandProvider.Save(blog);
                }

                unitOfWork.Commit();
            }

            unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                this.dataCommandProvider.Session.Clear();
                
                var query = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .FetchMany(b => b.Posts)
                    .ToFuture();

                this.dataCommandProvider.Session.Query<Post>()
                    .FetchMany(p => p.Comments)
                    .ToFuture();

                var blogs = query.ToList();

                Console.WriteLine(blogs.Count);

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Demonstrates using batches to reduce the number of individual writes.
        /// </summary>
        [Test]
        public void UsingBatchSize()
        {
            var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {                
                var blogs = BlogMother.CreateBlogsWithPostsAndComments(50);

                var batchSize = blogs.Count;

                foreach (var blog in blogs)
                {
                    batchSize += blog.Posts.Sum(post => post.Comments.Count());

                    batchSize += blog.Posts.Count();
                }

                this.dataCommandProvider.Session.SetBatchSize(batchSize);

                foreach (var blog in blogs)
                {
                    this.dataCommandProvider.Save(blog);
                }

                unitOfWork.Commit();
            }

            unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                this.dataCommandProvider.Session.Clear();

                var query = this.dataCommandProvider.Session
                    .Query<Blog>()
                    .FetchMany(b => b.Posts)
                    .Take(25)
                    .ToFuture();

                this.dataCommandProvider.Session.Query<Post>()
                    .FetchMany(p => p.Comments)
                    .ToFuture();

                var blogs = query.ToList();

                Console.WriteLine(blogs.Count);

                unitOfWork.Commit();
            }
        }
	}
}