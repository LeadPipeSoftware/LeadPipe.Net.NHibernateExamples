// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LargeNumberOfIndividualWrites.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.Data;
using LeadPipe.Net.Data.NHibernate;
using NUnit.Framework;
using StructureMap;

namespace LeadPipe.Net.NHibernateExamples.Application
{
	/// <summary>
	/// Demonstrates multiple inserts and how to deal with them.
	/// </summary>
	[TestFixture]
	public class LargeNumberOfIndividualWrites
	{
        /*
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
		/// Demonstrates multiple inserts.
		/// </summary>
		[Test]
		public void MultipleInserts()
		{
            /*
             * This example demonstrates what happens when batching is disabled.
             */

            this.blogName = RandomValueProvider.RandomString(25, true);
            
            var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                this.dataCommandProvider.Session.SetBatchSize(0);

                var blog = BlogMother.CreateBlogWithPostsAndComments(blogName);

                this.dataCommandProvider.Save(blog);

                unitOfWork.Commit();
            }
		}
	}
}