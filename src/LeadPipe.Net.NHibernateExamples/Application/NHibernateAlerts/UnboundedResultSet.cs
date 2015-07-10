// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnboundedResultSet.cs" company="Lead Pipe Software">
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
	/// Demonstrates the Unbounded Result Set alert and shows how to deal with them.
	/// </summary>
	[TestFixture]
	public class UnboundedResultSet
	{
        /*
         * http://hibernatingrhinos.com/Products/NHProf/learn/Alert/UnboundedResultSet
         * 
         * An unbounded result set is where a query is performed and does not explicitly limit the
         * number of returned results using SetMaxResults() with NHibernate, or TOP or LIMIT
         * clauses in the SQL. Usually, this means that the application assumes that a query will
         * always return only a few records. That works well in development and in testing, but it
         * is a time bomb waiting to explode in production.
         * 
         * The query may suddenly start returning thousands upon thousands of rows, and in some
         * cases, it may return millions of rows. This leads to more load on the database server,
         * the application server, and the network. In many cases, it can grind the entire system
         * to a halt, usually ending with the application servers crashing with out of memory
         * errors.
         */

        private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public UnboundedResultSet()
	    {
            Bootstrapper.Start();

            this.dataCommandProvider = ObjectFactory.GetInstance<DataCommandProvider>();
            this.unitOfWorkFactory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();
	    }
       
        /// <summary>
        /// Demonstrates an unbounded result set.
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
        /// Demonstrates using Take to create a bounded result set.
        /// </summary>
        [Test]
        public void UsingTake()
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