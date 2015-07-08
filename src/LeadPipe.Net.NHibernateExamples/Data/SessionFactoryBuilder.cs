// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionFactoryBuilder.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeadPipe.Net.Data.NHibernate;
using LeadPipe.Net.NHibernateExamples.Data.Maps;
using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace LeadPipe.Net.NHibernateExamples.Data
{
	/// <summary>
	/// The session factory builder.
	/// </summary>
	public class SessionFactoryBuilder : ISessionFactoryBuilder
	{
		#region Constants and Fields

        /// <summary>
		/// The NHibernate map document name.
		/// </summary>
		private const string MapDocumentName = "NHibernateMap";

        /// <summary>
		/// The session factory.
		/// </summary>
		private ISessionFactory sessionFactory;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the configuration.
		/// </summary>
		public global::NHibernate.Cfg.Configuration Configuration { get; protected set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Builds a configured session factory.
		/// </summary>
		/// <returns>
		/// The configured session factory.
		/// </returns>
		public ISessionFactory Build()
		{
			if (this.sessionFactory != null)
			{
				return this.sessionFactory;
			}

			this.Configuration = this.Configure();

			var mapping = this.Map();
			
            this.Configuration.AddDeserializedMapping(mapping, MapDocumentName);

            this.BuildSchema();
			
			this.sessionFactory = this.Configuration.BuildSessionFactory();

			return this.sessionFactory;
		}

		#endregion

		#region Methods

        /// <summary>
        /// Builds the schema.
        /// </summary>
        private void BuildSchema()
        {
            new SchemaExport(this.Configuration).Execute(false, true, false);
        }

		/// <summary>
		/// Configures NHibernate.
		/// </summary>
		/// <returns>
		/// The NHibernate configuration.
		/// </returns>
		private global::NHibernate.Cfg.Configuration Configure()
		{
			var configuration = new global::NHibernate.Cfg.Configuration();

			configuration.DataBaseIntegration(
				db =>
				{
                    db.Driver<Sql2008ClientDriver>();
                    db.ConnectionString = "Server=ZIRCON;Database=NHibernateExample;Trusted_Connection=True;";
					db.Dialect<MsSql2008Dialect>();
					db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
                    ////db.BatchSize = 20; // MAGIC!
				});            

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

			return configuration;
		}

		/// <summary>
		/// Gets the NHibernate mapping.
		/// </summary>
		/// <returns>
		/// The NHibernate mapping.
		/// </returns>
		private HbmMapping Map()
		{
			var mapper = new ModelMapper();

			mapper.AddMapping<BlogMap>();
			mapper.AddMapping<PostMap>();
            mapper.AddMapping<CommentMap>();

			var mapping = mapper.CompileMappingFor(new[] { typeof(Blog), typeof(Post), typeof(Comment) });

			return mapping;
		}

		#endregion
	}
}