// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlogMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentNHibernate.Mapping;
using LeadPipe.Net.Data;
using LeadPipe.Net.NHibernateExamples.Domain;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
    public class BlogMap : ClassMap<Blog>, IUseSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogMap"/> class.
        /// </summary>
        public BlogMap()
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
            this.Map(x => x.IsActive);

            // Map the collections...
            this.HasMany(x => x.Posts)
                .Inverse()
                .Schema(this.SchemaName)
                .LazyLoad()
                .Cascade.AllDeleteOrphan();
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

    /////// <summary>
    /////// The Blog NHibernate map.
    /////// </summary>
    ////public class BlogMap : ClassMapping<Blog>
    ////{
    ////    #region Constructors and Destructors

    ////    /// <summary>
    ////    /// Initializes a new instance of the <see cref="BlogMap"/> class.
    ////    /// </summary>
    ////    public BlogMap()
    ////    {
    ////        this.Id(blog => blog.Sid, map => map.Generator(Generators.HighLow));

    ////        this.Property(blog => blog.Key, map =>
    ////        {
    ////            map.Access(Accessor.ReadOnly);
    ////            map.Column("DomainKey");
    ////        });

    ////        this.Property(blog => blog.Name);

    ////        this.Property(blog => blog.IsActive);

    ////        this.Bag(x => x.Posts, bag =>
    ////        {
    ////            bag.Cascade(Cascade.All);
    ////            bag.Access(Accessor.Field);
    ////            bag.Fetch(CollectionFetchMode.Join);
    ////            bag.Key(k =>
    ////            {
    ////                k.NotNullable(true);
    ////            });
    ////            bag.Inverse(true);                  // SEE BELOW
    ////        }
    ////            , relation =>
    ////            {
    ////                relation.OneToMany();
    ////            }
    ////            );

    ////        /*
    ////         * You use the inverse attribute to specify the owner of the association. Every
    ////         * association can have only one owner. When defining a relationship, set the owner as
    ////         * inverse=false and the other side as inverse=true.
    ////         * 
    ////         * In a one-to-many association, if you screw this up then NHibernate will perform an
    ////         * additional UPDATE. NHibernate will first insert the entity that is contained in the
    ////         * collection then, if necessary, it will insert the entity that owns the collection,
    ////         * and finally it will update the 'collection entity' so that the foreign key is set
    ////         * and the association is made.
    ////         */
    ////    }

    ////    #endregion
    ////}
}