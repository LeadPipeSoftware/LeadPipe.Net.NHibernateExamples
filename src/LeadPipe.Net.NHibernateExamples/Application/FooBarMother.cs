// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FooBarMother.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using LeadPipe.Net.NHibernateExamples.Domain;

namespace LeadPipe.Net.NHibernateExamples.Application
{
	/// <summary>
	/// Provides FooBar objects for unit testing.
	/// </summary>
	public class FooBarMother
	{
        /// <summary>
        /// Creates the foos.
        /// </summary>
        /// <param name="numberOfFoos">The number of foos.</param>
        /// <returns>A slew of Foo.</returns>
        public static IList<Foo> CreateFoos(int numberOfFoos = 10)
        {
            var foos = new List<Foo>();

            for (var i = 0; i < numberOfFoos; i++)
            {
                var foo = new Foo(RandomValueProvider.RandomString(10, false));

                foos.Add(foo);
            }

            return foos;
        }

        /// <summary>
        /// Creates the bars.
        /// </summary>
        /// <param name="numberOfBars">The number of bars.</param>
        /// <returns>A batallion of Bar.</returns>
        public static IList<Bar> CreateBars(int numberOfBars = 10)
        {
            var bars = new List<Bar>();

            for (var i = 0; i < numberOfBars; i++)
            {
                var bar = new Bar(RandomValueProvider.RandomString(10, false));

                bars.Add(bar);
            }

            return bars;
        }

        /// <summary>
        /// Creates the foo bars.
        /// </summary>
        /// <param name="foos">The foos.</param>
        /// <param name="bars">The bars.</param>
        /// <returns></returns>
	    public static IList<FooBar> CreateFooBars(IList<Foo> foos, IList<Bar> bars)
	    {
	        var trimmedBars = bars;

            if (bars.Count > foos.Count)
	        {
	            trimmedBars = bars.Take(foos.Count).ToList();
	        }

	        var fooBarDictionary = Enumerable.Range(0, foos.Count).ToDictionary(i => foos[i], i => trimmedBars[i]);

	        return (from pair in fooBarDictionary let foo = pair.Key let bar = pair.Value select new FooBar(foo, bar, RandomValueProvider.RandomString(10, false))).ToList();
	    }
	}
}