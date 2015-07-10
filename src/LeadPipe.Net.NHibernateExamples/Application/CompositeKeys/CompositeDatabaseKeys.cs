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

namespace LeadPipe.Net.NHibernateExamples.Application.CompositeKeys
{
	/// <summary>
	/// Demonstrates the issue with composite database keys and the first level id map.
	/// </summary>
	[TestFixture]
	public class CompositeDatabaseKeys
	{
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
                fooBars.ForEach(fb => this.dataCommandProvider.Save(fb));

                var fooIdPartA = foos.First().IdPartA;
                var fooIdPartB = foos.First().IdPartB;
                var barSid = bars.First().Sid;

                var fooBar = this.dataCommandProvider.Session
                    .Query<FooBar>()
                    .Where(fb => fb.Foo.IdPartA.Equals(fooIdPartA) && fb.Foo.IdPartB.Equals(fooIdPartB) && fb.Bar.Sid.Equals(barSid))
                    .FirstOrDefault();

                Console.WriteLine("The FooBar name is {0}.".FormattedWith(fooBar.Name));

                this.ParentMethod(fooIdPartA, fooIdPartB, barSid);

                unitOfWork.Commit();
			}
		}

        /// <summary>
        /// Parent test method that calls the next method inside the UoW scope.
        /// </summary>
        /// <param name="fooIdPartA">The foo identifier part a.</param>
        /// <param name="fooIdPartB">The foo identifier part b.</param>
        /// <param name="barSid">The bar sid.</param>
        private void ParentMethod(int fooIdPartA, int fooIdPartB, Guid barSid)
        {
            var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            using (unitOfWork.Start())
            {
                var fooBar = this.dataCommandProvider.Session
                    .Query<FooBar>()
                    .Where(fb => fb.Foo.IdPartA.Equals(fooIdPartA) && fb.Foo.IdPartB.Equals(fooIdPartB) && fb.Bar.Sid.Equals(barSid))
                    .FirstOrDefault();

                fooBar.MutableProperty = !fooBar.MutableProperty;

                this.dataCommandProvider.Save(fooBar);

                unitOfWork.Commit();
            }
        }
	}
}