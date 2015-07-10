// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentNHibernate.Mapping;
using LeadPipe.Net.Data;
using LeadPipe.Net.NHibernateExamples.Domain;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
    public class PostMap : ClassMap<Post>, IUseSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostMap"/> class.
        /// </summary>
        public PostMap()
        {
            // Set the schema name...
            this.Schema(this.SchemaName);

            // Set the surrogate id field...
            this.Id(x => x.Sid).GeneratedBy.GuidComb();

            // Set the natural id...
            this.NaturalId().Property(x => x.Title);

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
            this.Map(x => x.CommentsEnabled);

            // Map the collections...
            this.HasMany(x => x.Comments)
                .Inverse()
                .Schema(this.SchemaName)
                .LazyLoad()
                .Cascade.AllDeleteOrphan();

            // Map the references...
            this.References(x => x.Blog);
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