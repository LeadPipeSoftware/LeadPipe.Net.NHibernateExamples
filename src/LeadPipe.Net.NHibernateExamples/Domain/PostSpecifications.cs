// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostSpecifications.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using LeadPipe.Net.Specifications;

namespace LeadPipe.Net.NHibernateExamples.Domain
{
	/// <summary>
	/// The post specifications.
	/// </summary>
	public static class PostSpecifications
	{
        /// <summary>
        /// Determines whether the post has printable comments.
        /// </summary>
        /// <returns>True if the post has printable comments.</returns>
		public static ISpecification<Post> HasPrintableComments()
		{
			return new AdHocSpecification<Post>(post => post.Comments.Any(comment => comment.ApprovedByModerator && !comment.ContainsRestrictedLanguage));
		}
	}
}