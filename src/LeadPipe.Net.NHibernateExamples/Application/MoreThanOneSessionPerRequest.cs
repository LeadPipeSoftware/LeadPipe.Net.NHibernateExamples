// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoreThanOneSessionPerRequest.cs" company="Lead Pipe Software">
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
    /// Demonstrates the More Than One Session Per Request alert.
    /// </summary>
    [TestFixture]
    public class MoreThanOneSessionPerRequest
    {
        /*
         * http://hibernatingrhinos.com/Products/NHProf/learn/Alert/MultipleSessionsInTheSameRequest
         * 
         * Using more than one session per request is generally a bad practice. Here's why:
         * 
         * 1. Each session has its own database connection. Using more than one session means using
         *    more than one database connection per request, resulting in additional strain on the
         *    database and slower overall performance.
         * 2. Typically we expect the session to keep track of our entities. When we have multiple
         *    sessions, each session is not aware of the entities that tracked by the other session
         *    and might have to query the database again for its current state or have to issue an
         *    unnecessary update.
         * 3. Having more than a single session also mean that we can't take advantage of
         *    NHibernate's Unit of Work and have to manually manage our entities' change tracking
         *    and we might end up with multiple instances of the same entities in the same request
         *    (which using a single session for the whole request would prevent).
         * 4. Having more than one session means that the ORM has more work to do. In most cases,
         *    this is unnecessary and should be avoided.
         * 5. You can no longer take advantage of features scoped to the session, such as the first
         *    level cache.
         */

        private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        private string blogName;

        public MoreThanOneSessionPerRequest()
	    {
            Bootstrapper.Start();

            this.dataCommandProvider = ObjectFactory.GetInstance<DataCommandProvider>();
            this.unitOfWorkFactory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();
	    }

        /// <summary>
        /// Demonstrates calling other methods that use Units of Work from outside an existing scope.
        /// </summary>
        [Test]
        public void CallingOutsideUnitOfWorkScope()
        {
            /*
             * A Unit of Work is intended to be a business transaction. Unfortunately, it's common
             * to see them fragmented by forgetting to include the next process inside the outer-
             * most Unit of Work. This code, for example, calls a method (ParentMethodOutside) from
             * outside the Unit of Work scope and, as a result, a new session is created.
             */

            this.blogName = RandomValueProvider.RandomString(25, true);

            this.unitOfWorkFactory.UnitOfWorkBatchMode = UnitOfWorkBatchMode.Nested;

            var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blog = BlogMother.CreateBlogWithPostsAndComments(blogName);

                this.dataCommandProvider.Save(blog);

                unitOfWork.Commit();
            }

            this.ParentMethodOutside();
        }

        /// <summary>
        /// Demonstrates calling other methods that use Units of Work from within an existing scope.
        /// </summary>
        [Test]
        public void CallingInsideUnitOfWorkScope()
        {
            /*
             * If you are using an ambient session provider, the solution is as simple as including
             * the next bit of code inside the scope of your outermost Unit of Work.
             */

            this.blogName = RandomValueProvider.RandomString(25, true);

            this.unitOfWorkFactory.UnitOfWorkBatchMode = UnitOfWorkBatchMode.Nested;

            var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var blog = BlogMother.CreateBlogWithPostsAndComments(blogName);

                this.dataCommandProvider.Save(blog);

                unitOfWork.Commit();

                this.ParentMethodInside();
            }
        }

        /// <summary>
        /// Parent test method that calls the next method inside the UoW scope.
        /// </summary>
        private void ParentMethodInside()
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
        /// Parent test method that calls the next method outside the UoW scope.
        /// </summary>
        private void ParentMethodOutside()
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
            }

            this.ChildMethod();
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

                unitOfWork.Commit();
            }
        }
    }
}