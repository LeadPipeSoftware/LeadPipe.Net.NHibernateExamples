// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BarMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
	/// <summary>
	/// The Bar NHibernate map.
	/// </summary>
	public class BarMap : ClassMapping<Bar>
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BarMap"/> class.
		/// </summary>
		public BarMap()
		{
			this.Id(bar => bar.Sid, map => map.Generator(Generators.HighLow));

			this.Property(bar => bar.Key, map =>
			{
			    map.Access(Accessor.ReadOnly);
                map.Column("DomainKey");
			});

			this.Property(bar => bar.Name);
        }

		#endregion
	}
}