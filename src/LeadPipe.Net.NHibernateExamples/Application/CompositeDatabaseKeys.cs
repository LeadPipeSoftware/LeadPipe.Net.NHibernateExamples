// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeDatabaseKeys.cs" company="Lead Pipe Software">
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
using NHibernate.Util;
using NUnit.Framework;
using StructureMap;

namespace LeadPipe.Net.NHibernateExamples.Application
{
	/// <summary>
	/// Demonstrates the issue with composite database keys and the first level id map.
	/// </summary>
	[TestFixture]
	public class CompositeDatabaseKeys
	{
        /*
         */

	    private readonly DataCommandProvider dataCommandProvider;
	    private readonly IUnitOfWorkFactory unitOfWorkFactory;
	    
        public CompositeDatabaseKeys()
	    {
            Bootstrapper.Start();

            this.dataCommandProvider = ObjectFactory.GetInstance<DataCommandProvider>();
            this.unitOfWorkFactory = ObjectFactory.GetInstance<IUnitOfWorkFactory>();
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
                var foos = FooBarMother.CreateFoos(10);
                foos.ForEach(foo => this.dataCommandProvider.Save(foo));

                var bars = FooBarMother.CreateBars(10);
                bars.ForEach(bar => this.dataCommandProvider.Save(bar));

                var fooBars = FooBarMother.CreateFooBars(foos, bars);
                fooBars.ForEach(fooBar => this.dataCommandProvider.Save(fooBar));
                
                ////var blog = this.dataCommandProvider.Session
                ////    .Query<Blog>()
                ////    .FirstOrDefault(x => x.Name == this.blogName);

                ////foreach (var post in blog.Posts)
                ////{
                ////    foreach (var comment in post.Comments)
                ////    {
                ////        Console.WriteLine("{0} said {1}".FormattedWith(comment.Commenter, comment.Text));
                ////    }
                ////}

                unitOfWork.Commit();
			}
		}
	}
}