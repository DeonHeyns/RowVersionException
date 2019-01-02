using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using RowVersionFailure.ServiceInterface;
using RowVersionFailure.ServiceModel;
using RowVersionFailure.ServiceModel.Types;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace RowVersionFailure.Tests
{
    public class UnitTest
    {
        private readonly ServiceStackHost appHost;

        public UnitTest()
        {
            appHost = new BasicAppHost().Init();
            appHost.Container.AddTransient<OrderingServices>();
            appHost.Container.Register<IDbConnectionFactory>(c => new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));
            var dbFactory = appHost.Container.Resolve<IDbConnectionFactory>();

            using (var db = dbFactory.Open())
            {
                db.CreateTableIfNotExists<CustomerOrder>();
                db.CreateTableIfNotExists<Customer>();
                db.CreateTableIfNotExists<Address>();

            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        [Test]
        public void Can_call_OrderingServices()
        {
            var service = appHost.Container.Resolve<OrderingServices>();

            var response = (Customer)service.Post(new PostCustomer
            {
                FirstName = "Joe",
                LastName = "Schmidt",
                Street = "Lansdowne Road",
                ProvinceOrState = "Leinster",
                City = "Dublin",
                Country = "Ireland" });

            Assert.That(response.FirstName, Is.EqualTo("Joe"));
        }
    }
}
