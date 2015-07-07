// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlogSpecifications.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.Specifications;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The blog specifications.
	/// </summary>
	public static class BlogSpecifications
	{
		#region Public Methods and Operators

		/// <summary>
		/// The test property starts with abc.
		/// </summary>
		/// <returns>
		/// Test Models that start with ABC.
		/// </returns>
		public static ISpecification<Blog> TestPropertyStartsWithABC()
		{
			return new AdHocSpecification<Blog>(x => x.Name.StartsWith("ABC"));
		}

        /// <summary>
        /// The test property ends with xyz.
        /// </summary>
        /// <returns>
        /// Test Models that end with XYZ.
        /// </returns>
	    public static ISpecification<Blog> TestPropertyEndsWithXYZ()
	    {
	        return new AdHocSpecification<Blog>(x => x.Name.EndsWith("XYZ"));
	    }

        /// <summary>
        /// The test property starts with abc and ends with xyz.
        /// </summary>
        /// <returns>
        /// Test Models that start with ABC and end with XYZ.
        /// </returns>
	    public static ISpecification<Blog> TestPropertyStartsWithABCAndEndsWithXYZ()
	    {
	        return new AndSpecification<Blog>(TestPropertyStartsWithABC(), TestPropertyEndsWithXYZ());
	    }

        /// <summary>
        /// The test property starts with abc or ends with xyz.
        /// </summary>
        /// <returns>
        /// Test Models that start with ABC or end with XYZ.
        /// </returns>
        public static ISpecification<Blog> TestPropertyStartsWithABCOrEndsWithXYZ()
        {
            return new OrSpecification<Blog>(TestPropertyStartsWithABC(), TestPropertyEndsWithXYZ());
        }

        /// <summary>
        /// The test property does NOT start with abc or ends with xyz.
        /// </summary>
        /// <returns></returns>
        public static ISpecification<Blog> NotTestPropertyStartsWithABCOrEndsWithXYZ()
        {
            return new NotSpecification<Blog>(TestPropertyStartsWithABC());
        }

		#endregion
	}
}