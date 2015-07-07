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
			this.Id(x => x.Sid, m => m.Generator(Generators.HighLow));

			this.Property(x => x.Key, m => m.Access(Accessor.ReadOnly));

			this.Property(x => x.User);

            this.ManyToOne(x => x.Post, m =>
            {
                m.Cascade(Cascade.DeleteOrphans);
                m.NotNullable(false);
                m.Fetch(FetchKind.Join);
            });
		}

		#endregion
	}
}