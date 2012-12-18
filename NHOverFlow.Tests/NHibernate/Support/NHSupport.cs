using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NHEnvironment = NHibernate.Cfg.Environment;

namespace MyOverFlow.Tests.NHibernate.Support
{
    public class NHSupport
    {
        private readonly string _connectionString;
        private readonly IList<string> _assemblies;
        private readonly bool _inMemory;
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHSupport"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="inMemory">True to setup the SessioneFactory to use the in memory provider with SQLLite</param>
        public NHSupport(string connectionString, IList<string> assemblies, bool inMemory)
        {
            _connectionString = connectionString;
            _assemblies = assemblies;
            _inMemory = inMemory;
        }

        /// <summary>
        /// Get session factory
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory ?? (_sessionFactory = CreateSessionFactory()); }
        }

        public void UpdateSchema()
        {
            if (_sessionFactory == null)
                _sessionFactory = CreateSessionFactory();

            new SchemaUpdate(_configuration).Execute(false, true);
        }

        /// <summary>
        /// Create database schema
        /// </summary>
        public void CreateSchema()
        {
            CreateSchema(false);
        }
        public void CreateSchema(bool recreate)
        {
            if (_sessionFactory == null)
                _sessionFactory = CreateSessionFactory();

            var schemaExport = new SchemaExport(_configuration);
            if (recreate)
                schemaExport.Execute(false, true, true);
            schemaExport.Execute(false, true, false);
        }

        private void ConfigureNHibernate()
        {
            _configuration = new Configuration();
            _configuration.DataBaseIntegration(x =>
                {
                    if (_inMemory)
                    {
                        x.Dialect<SQLiteDialect>();
                        x.Driver<SQLite20Driver>();
                        x.ConnectionProvider<DriverConnectionProvider>();
                    }
                    else
                        x.Dialect<MsSql2008Dialect>();
                    x.ConnectionString = _connectionString;
                    x.SchemaAction = SchemaAutoAction.Update;
                    x.IsolationLevel = IsolationLevel.ReadCommitted;

                    x.HqlToSqlSubstitutions = "True 1, False 0, true 1, false 0, yes 'Y', no 'N'";
                    x.BatchSize = 200;
                });

            var mappingAssemblies = _assemblies;
            foreach (var mappingAssembly in mappingAssemblies.Where(mappingAssembly => !String.IsNullOrWhiteSpace(mappingAssembly)))
            {
                _configuration.AddAssembly(mappingAssembly);
            }
            _configuration.BuildMappings();
            _configuration.SetProperty(NHEnvironment.CacheDefaultExpiration, 120.ToString());
            _configuration.SetProperty(NHEnvironment.ShowSql, "false");
            _configuration.Proxy(cfg => cfg.ProxyFactoryFactory<DefaultProxyFactoryFactory>());
            _configuration.SessionFactory()
                          .GenerateStatistics();

        }
        /// <summary>
        /// Create session factory
        /// </summary>
        /// <returns>session factory</returns>
        private ISessionFactory CreateSessionFactory()
        {
            ConfigureNHibernate();
            return _configuration.BuildSessionFactory();
        }

    }
}