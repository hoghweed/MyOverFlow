namespace MyOverFlow.Tests.NHibernate.Domain
{
    public class DepartmentEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual DepartmentEntity Parent { get; set; }
        public virtual CompanyEntity Company { get; set; }
    }
}