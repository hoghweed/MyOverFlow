using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;

namespace MyOverFlow.Tests.NHibernate
{
    public abstract class NHBehaviourTest 
        : BehaviourTest
    {
        /// <summary>
        /// The fixture SessionFactory 
        /// </summary>
        public ISessionFactory SessionFactory { get { return TestHelper.NHibernateSupport.SessionFactory; } }

        protected override void OnConfigure()
        {
            var asm = typeof(TestBase).Assembly.FullName;

            var settings = new DataSupoortSettings
            {
                ConnectionString = ConfigurationManager.AppSettings["connection"],
                Express = Convert.ToBoolean(ConfigurationManager.AppSettings["express"]),
                InMemory = ConfigurationManager.AppSettings["mode"] == "memory",
                MappingAssemblies = new List<string> { asm },
                Profiling = Convert.ToBoolean(ConfigurationManager.AppSettings["profiling"]),
                RecreateMedia = Convert.ToBoolean(ConfigurationManager.AppSettings["recreateMedia"]),
                RecreateSchema =
                    Convert.ToBoolean(ConfigurationManager.AppSettings["recreateSchema"])
            };

            if (settings.Profiling)
                NHibernateProfiler.Initialize();
            if (!TestHelper.IsUpFor(asm))
                TestHelper.InitializeDataFactory(settings);
        }
    }
}
