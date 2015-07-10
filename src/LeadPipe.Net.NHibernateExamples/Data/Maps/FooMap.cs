// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FooMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentNHibernate.Mapping;
using LeadPipe.Net.Data;
using LeadPipe.Net.NHibernateExamples.Domain;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
	/// <summary>
	/// The Foo NHibernate map.
	/// </summary>
    public class FooMap : ClassMap<Foo>, IUseSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooMap"/> class.
        /// </summary>
        public FooMap()
        {
            // Set the schema name...
            this.Schema(this.SchemaName);

            // Set the surrogate id field...
            this.Id(x => x.Sid).GeneratedBy.GuidComb();

            // Set the natural id...
            this.NaturalId().Property(x => x.Name);

            // Use optimistic locking based on version...
            this.OptimisticLock.Version();

            // Map the IPersistable fields...
            this.Version(x => x.PersistenceVersion).Column("Ver");

            ////// Map the IChangeAudited fields...
            ////this.Map(x => x.CreatedBy);
            ////this.Map(x => x.CreatedOn);
            ////this.Map(x => x.UpdatedBy);
            ////this.Map(x => x.UpdatedOn);

            // Map the class fields...
            this.Map(x => x.Key).Column("DomainId");
        }

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        public string SchemaName
        {
            get
            {
                return "dbo";
            }
        }
    }
}