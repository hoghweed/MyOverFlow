using System;
using System.Collections.Generic;
using System.Configuration;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using MyOverFlow.Tests.NHibernate;
using NHOverFlow.Tests;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;

namespace MyOverFlow.Tests
{
    public abstract class TestBase
    {
        /// <summary>
        /// The fixture mock repository
        /// </summary>
        protected MockRepository Mocks { get; set; }

        [TestFixtureSetUp]
        public virtual void Configure()
        {
            OnConfigure();
            //initialize the profiler
        }

        protected abstract void OnConfigure();

        /// <summary>
        /// When overridden in a derived fixture class prepares 
        /// the context for a specific test unit
        /// </summary>
        protected virtual void Prepare()
        { }

        /// <summary>
        /// When overridden in a derived fixture class perform a 
        /// context cleanup for a specific test unit
        /// </summary>
        protected virtual void CleanUp()
        { }
    }
}