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
            /*
             * A common anti-pattern in Domain Driven Design is what we call an anemic domain model
             * and it's most frequently seen when behavior that could be accomplished by the domain
             * is pulled out into a service, controller, or some other type. By itself, an anemic
             * domain is problematic because it makes determining what should be going on much
             * harder and it scatters business rules all over the place. There's often a hidden
             * cost associated with anemic domains when it comes to an O/RM, however.
             * 
             * When a domain is fragmented and behavior is stolen by types that are outside of the
             * domain we'll often see lots of individual methods that start sessions, perform some
             * work, and closes the session. This completely wrecks the first level identity map
             * (sometimes called the first level cache) because the map is lost when the session is
             * disposed.
             * 
             * "Wait!", you say, "My implementation makes sure that the current session is returned
             * when a session is asked for!" Sure, that may be true, but when does the session get
             * DISPOSED of? Yeah, see that's the problem. The "give me the current session" trick
             * will only work if nobody disposes of the session. Otherwise, you'll be getting a new
             * session each and every time.
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

                var blog = query.FirstOrDefault();

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
            /*
             * Pushing behavior to the domain whenever possible not only helps you to develop and
             * maintain a strong and consistent domain model, but it will almost certainly reduce
             * the number of sessions you're forced to create. This keeps the first level id map in
             * scope as long as possible which is often a dramatic performance improvement.
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

                var blog = query.FirstOrDefault();

                blog.PrintPostComments();

                unitOfWork.Commit();
            }
        }
	}
}