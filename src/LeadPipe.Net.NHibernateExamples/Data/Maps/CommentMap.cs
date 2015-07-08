// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommentMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
	/// <summary>
	/// The Comment NHibernate map.
	/// </summary>
	public class CommentMap : ClassMapping<Comment>
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CommentMap"/> class.
		/// </summary>
		public CommentMap()
		{
			this.Id(comment => comment.Sid, map => map.Generator(Generators.HighLow));

			this.Property(comment => comment.Key, map =>
			{
			    map.Access(Accessor.ReadOnly);
			    map.Column("DomainKey");
			});

			this.Property(comment => comment.Commenter);

            this.Property(comment => comment.ApprovedByModerator);

            this.Property(comment => comment.Text);

            this.ManyToOne(comment => comment.Post, map =>
            {
                map.Cascade(Cascade.DeleteOrphans);
                map.NotNullable(true);
                map.Fetch(FetchKind.Join);
                map.Class(typeof(Post));
            });
		}

		#endregion
	}
}