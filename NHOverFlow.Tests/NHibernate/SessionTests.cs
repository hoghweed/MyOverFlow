using System.Collections;
using FluentAssertions;
using MyOverFlow.Tests.NHibernate.Domain;
using NHibernate;
using NUnit.Framework;

namespace MyOverFlow.Tests.NHibernate
{
    [TestFixture]
    public class SessionTests : NHBehaviourTest
    {
        private ISession session = null;
        private IList departments = null; 

        protected override void Prepare()
        {
            using (var session = SessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var acme = new CompanyEntity{ Name = "ACME"};
                var wheels = new CompanyEntity{ Name = "Wheels"};
                session.SaveOrUpdate(acme);
                session.SaveOrUpdate(wheels);

                var rootDep = new DepartmentEntity {Name = "Information Technology", Company = acme};
                session.SaveOrUpdate(rootDep);

                var subDep1 = new DepartmentEntity { Name = "Software", Parent = rootDep };
                session.SaveOrUpdate(subDep1);

                //var otheDep = new DepartmentEntity { Name = "Tires", Company = wheels };
                //session.SaveOrUpdate(otheDep);
                tx.Commit();
            }
        }

        protected override void CleanUp()
        {
            
        }

        [Test]
        public void Cannot_access_referenced_object_after_session_closed()
        {
            foreach (DepartmentEntity dep in departments)
            {
                dep.Invoking(d => { var t = d.Company; }).ShouldNotThrow();
            }
        }

        protected override void Given()
        {
            session = SessionFactory.OpenSession();
            var hql = "FROM DepartmentEntity as dpe join fetch dpe.Company";
            departments = session.CreateQuery(hql).List();
        }

        protected override void When()
        {
            session.Close();
        }
    }
}
