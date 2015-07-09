// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FooBarMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Mapping.ByCode.Conformist;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
	/// <summary>
	/// The FooBar NHibernate map.
	/// </summary>
	public class FooBarMap : ClassMapping<FooBar>
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FooBarMap"/> class.
		/// </summary>
		public FooBarMap()
		{
            this.ComposedId(m =>
            {
                m.Property(x => x.FooId, p => p.Column("FooId"));
                m.Property(x => x.BarId, p => p.Column("BarId"));
            });

			this.Property(fooBar => fooBar.Name);
        }

		#endregion
	}
}