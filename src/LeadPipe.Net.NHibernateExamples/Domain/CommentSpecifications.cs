// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommentSpecifications.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using LeadPipe.Net.Specifications;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The comment specifications.
	/// </summary>
	public static class CommentSpecifications
	{
        /// <summary>
        /// Determines whether the comment is printable.
        /// </summary>
        /// <returns>True if the comment is printable.</returns>
		public static ISpecification<Comment> IsPrintable()
		{
			return new AdHocSpecification<Comment>(comment => comment.ApprovedByModerator && !comment.ContainsRestrictedLanguage);
		}
	}
}