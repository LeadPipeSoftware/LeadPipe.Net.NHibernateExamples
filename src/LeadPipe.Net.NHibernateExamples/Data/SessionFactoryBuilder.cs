// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionFactoryBuilder.cs" company="Lead Pipe Software">
//   Copyright (c) Lead Pipe Software All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using LeadPipe.Net.Data.NHibernate;
using LeadPipe.Net.NHibernateExamples.Domain;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace LeadPipe.Net.NHibernateExamples.Data
{
    /// <summary>
    /// The session factory builder.
    /// </summary>
    public class SessionFactoryBuilder : ISessionFactoryBuilder
    {
        #region Public Properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public NHibernate.Cfg.Configuration Configuration { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Builds an NHibernate session factory instance.
        /// </summary>
        /// <returns>
        /// An NHibernate session factory.
        /// </returns>
        /// <exception cref="Exception">An error occurred while configuring the database connection.</exception>
        public ISessionFactory Build()
        {
            ISessionFactory sessionFactory = null;

            try
            {
                sessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString("Server=ZIRCON;Database=NHibernateExample;Trusted_Connection=True;"))
                    .Mappings(m => { m.FluentMappings.AddFromAssemblyOf<Blog>(); })
                    .ExposeConfiguration(config =>
                    {
                        Configuration = config;
                        new SchemaExport(config).Execute(true, true, false);
                    })
                    .Diagnostics(d => d.Enable()).BuildSessionFactory();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while configuring the database connection.", ex);
            }

            NHibernateProfiler.Initialize();

            return sessionFactory;
        }

        #endregion
    }
}
