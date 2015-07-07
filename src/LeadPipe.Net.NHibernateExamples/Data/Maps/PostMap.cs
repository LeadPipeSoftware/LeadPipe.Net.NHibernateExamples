// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostMap.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace LeadPipe.Net.NHibernateExamples.Data.Maps
{
	/// <summary>
	/// The Post NHibernate map.
	/// </summary>
	public class PostMap : ClassMapping<Post>
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PostMap"/> class.
		/// </summary>
		public PostMap()
		{
			this.Id(x => x.Sid, m => m.Generator(Generators.HighLow));

			this.Property(x => x.Key, m => m.Access(Accessor.ReadOnly));

			this.Property(x => x.Title);

            this.ManyToOne(x => x.Blog, m =>
            {
                m.Cascade(Cascade.DeleteOrphans);
                m.NotNullable(false);
                m.Fetch(FetchKind.Join);
            });

            this.Bag(x => x.Comments, c =>
            {
                c.Cascade(Cascade.All);
                c.Lazy(CollectionLazy.NoLazy);
                c.Inverse(true);                  // SEE BELOW
            }
                , r =>
                {
                    r.OneToMany();
                }
                );

            /*
             * You use the inverse attribute to specify the owner of the association. Every
             * association can have only one owner. When defining a relationship, set the owner as
             * inverse=false and the other side as inverse=true.
             * 
             * In a one-to-many association, if you screw this up then NHibernate will perform an
             * additional UPDATE. NHibernate will first insert the entity that is contained in the
             * collection then, if necessary, it will insert the entity that owns the collection,
             * and finally it will update the 'collection entity' so that the foreign key is set
             * and the association is made.
             */
		}

		#endregion
	}
}