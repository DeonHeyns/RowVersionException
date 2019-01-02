using ServiceStack.DataAnnotations;
using System.Collections.Generic;

namespace RowVersionFailure.ServiceModel.Types
{
    public class Customer
    {
        [AutoIncrement]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ulong RowVersion { get; set; }
        [Reference]
        public Address Address { get; set; }
        [Reference]
        public List<CustomerOrder> Orders { get; set; } = new List<CustomerOrder>();
    }

    public class CustomerOrder
    {
        [AutoIncrement]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string Details { get; set; }
        public ulong RowVersion { get; set; }
    }

    public class Address
    {
        [AutoIncrement]
        public long Id { get; set; }
        [References(typeof(Customer))]
        public long CustomerId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ProvinceOrState { get; set; }
        public string Country { get; set; }
        public ulong RowVersion { get; set; }
    }
}
