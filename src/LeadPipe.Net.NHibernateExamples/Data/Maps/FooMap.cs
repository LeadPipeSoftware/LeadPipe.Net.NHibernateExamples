// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FooMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
	/// <summary>
	/// The Foo NHibernate map.
	/// </summary>
	public class FooMap : ClassMapping<Foo>
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FooMap"/> class.
		/// </summary>
		public FooMap()
		{
			this.Id(foo => foo.Sid, map => map.Generator(Generators.HighLow));

			this.Property(foo => foo.Key, map =>
			{
			    map.Access(Accessor.ReadOnly);
                map.Column("DomainKey");
			});

			this.Property(foo => foo.Name);
        }

		#endregion
	}
}