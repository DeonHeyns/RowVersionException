﻿using Funq;
using ServiceStack;
using NUnit.Framework;
using RowVersionFailure.ServiceInterface;
using RowVersionFailure.ServiceModel;
using RowVersionFailure.ServiceModel.Types;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;

namespace RowVersionFailure.Tests
{
    public class IntegrationTest
    {
        const string BaseUri = "http://localhost:2000/";
        private readonly ServiceStackHost appHost;
        public Customer Customer { get; set; }
        public CustomerOrder Order { get; set; }
        public Address Address { get; set; }

        class AppHost : AppSelfHostBase
        {
            public AppHost() : base(nameof(IntegrationTest), typeof(OrderingServices).Assembly) { }

            public override void Configure(Container container)
            {
                container.Register<IDbConnectionFactory>(c => new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));
                var dbFactory = container.Resolve<IDbConnectionFactory>();

                using (var db = dbFactory.Open())
                {
                    db.CreateTableIfNotExists<CustomerOrder>();
                    db.CreateTableIfNotExists<Customer>();
                    db.CreateTableIfNotExists<Address>();

                }
            }
        }

        public IntegrationTest()
        {
            appHost = new AppHost()
                .Init()
                .Start(BaseUri);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();

        public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

        [Test]
        public void Can_call_Order_Service()
        {
            var client = CreateClient();

			var post = new PostCustomer
			{
				FirstName = "James",
				LastName = "Olsen",
				Street = "Lansdowne Road",
				ProvinceOrState = "Leinster",
				City = "Dublin",
				Country = "Ireland",
			};

			var customer = client.Post(post);
			var put = post.ConvertTo<PutCustomer>();
			put.Id = customer.Id; // don't set the row version to force an optimisitc concurrency exception

			var customer2 = client.Put(put);
            Assert.That(customer2.Id, Is.EqualTo(customer.Id));
        }

        // This fails with Message: System.InvalidOperationException : The current SynchronizationContext may not be used as a TaskScheduler.
        // Commenting out the RowVersion property will make this work or removing the async keyword, awaits and calling sync methods
        [SetUp]
        public void BootStrap()
        {
            using (var db = appHost.Resolve<IDbConnectionFactory>().Open())
            {
                Customer = new Customer
                {
                    FirstName = "Joe",
                    LastName = "Schmidt",
                };

                db.Save(Customer);

                Address = new Address
                {
                    Street = "Lansdowne Road",
                    ProvinceOrState = "Leinster",
                    City = "Dublin",
                    Country = "Ireland",
                    CustomerId = Customer.Id
                };

                db.Save(Address);

                Order = new CustomerOrder
                {
                    CustomerId = Customer.Id,
                    Details = "Random Order"
                };

                db.Save(Order);
            }
        }
    }
}