// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionDisposedWithoutExplicitRollbackCommit.cs" company="Lead Pipe Software">
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
    /// Demonstrates the Transaction Disposed Without Explicit Rollback/Commit alert.
    /// </summary>
    [TestFixture]
    public class TransactionDisposedWithoutExplicitRollbackCommit
    {
        /*
         * http://hibernatingrhinos.com/Products/NHProf/learn/Alert/AvoidImplicitRollback
         * 
         * NHibernate Profiler has detected a transaction that was disposed without calling either
         * Commit() or Rollback(). Forgetting to call transaction.Commit() before disposing the
         * session is pretty common and results in an implicit rollback. 
         * 
         * Note that the LeadPipe.Net.Data.NHibernate.UnitOfWork implementation provides a
         * transaction during the call to .Start by default. As such, there's no reason for us
         * to concern ourselves with it here. However, if you're using a raw ISession then you
         * absolutely SHOULD create a transaction even if you're only reading and not altering
         * data in any way. For more information see:
         * 
         * http://ayende.com/blog/3775/nh-prof-alerts-use-of-implicit-transactions-is-discouraged
         */

        private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public TransactionDisposedWithoutExplicitRollbackCommit()
	    {
            Bootstrapper.Start();

            this.dataCommandProvider = ObjectFactory.GetInstance<DataCommandProvider>();
            this.unitOfWorkFactory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();
	    }

        /// <summary>
        /// Demonstrates disposing the transaction without an explicit commit.
        /// </summary>
        [Test]
        public void Problem()
        {
            this.blogName = RandomValueProvider.RandomString(25, true);

            this.unitOfWorkFactory.UnitOfWorkBatchMode = UnitOfWorkBatchMode.Nested;

            var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blog = BlogMother.CreateBlogWithPostsAndComments(blogName);

                this.dataCommandProvider.Save(blog);

                unitOfWork.Commit();

                this.ParentMethod();
            }
        }

        /// <summary>
        /// Parent test method that calls the next method inside the UoW scope.
        /// </summary>
        private void ParentMethod()
        {
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

                blog.IsActive = false;

                this.dataCommandProvider.Save(blog);

                unitOfWork.Commit();

                this.ChildMethod();
            }
        }
        
        /// <summary>
        /// The child method.
        /// </summary>
        private void ChildMethod()
        {
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

                Console.WriteLine("The blog has {0} posts.".FormattedWith(blog.Posts.Count()));

                //unitOfWork.Commit();
            }
        }
    }
}