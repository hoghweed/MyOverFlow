using System.Collections.Generic;
using MyOverFlow.Tests.NHibernate.Support;

namespace MyOverFlow.Tests.NHibernate
{
    public static class TestHelper
    {
        private static IList<string> _assemblies;

        public static bool RecreateSchema { get; set; }
        public static bool Profiling { get; private set; }
        public static NHSupport NHibernateSupport { get; private set; }

        public static bool IsUpFor(string assemblyName)
        {
            return _assemblies != null 
                   && _assemblies.Contains(assemblyName)
                   && NHibernateSupport != null;
        }

        public static void InitializeDataFactory(DataSupoortSettings settings)
        {
            _assemblies = settings.MappingAssemblies;

            Profiling = settings.Profiling;
            var connection = !settings.InMemory
                ? settings.ConnectionString
                : "Data Source=:memory:;Version=3;New=True;Pooling=True;Max Pool Size=1";
            var express = settings.Express;
            var recreateMedia = settings.RecreateMedia;
            RecreateSchema = settings.RecreateSchema;

            if (recreateMedia && !settings.InMemory)
            {
                var builder = new MsSqlConfigurator(connection, express);
                builder.DestroyMedia();
                builder.CreateMedia();
            }
            NHibernateSupport = new NHSupport(connection, _assemblies, settings.InMemory);

            //create the schema
            if (RecreateSchema)
                NHibernateSupport.CreateSchema(true);
            else
                NHibernateSupport.UpdateSchema();
        }
    }

    public class DataSupoortSettings
    {
        public string ConnectionString { get; set; }
        public bool InMemory { get; set; }
        public bool Express { get; set; }
        public bool RecreateSchema { get; set; }
        public bool RecreateMedia { get; set; }
        public bool Profiling { get; set; }
        public IList<string> MappingAssemblies { get; set; }
    }

}