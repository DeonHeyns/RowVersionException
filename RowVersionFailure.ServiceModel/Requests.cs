using RowVersionFailure.ServiceModel.Types;
using ServiceStack;

namespace RowVersionFailure.ServiceModel
{
    [Route("/customer", "POST")]
    public class PostCustomer : IReturn<Customer>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ProvinceOrState { get; set; }
        public string Country { get; set; }
        public ulong RowVersion { get; set; }
    }

    [Route("/customer", "GET")]
    public class GetCustomer : IReturn<Customer>
    {
        public long Id { get; set; }
    }

    [Route("/customers")]
    public class FindCustomers : QueryDb<Customer>
    {
        public long[] Ids { get; set; }
    }

    [Route("/order")]
    public class PostOrder : IReturn<CustomerOrder>
    {
        public long CustomerId { get; set; }
        public string Details { get; set; }
        public ulong RowVersion { get; set; }
    }

    [Route("/order")]
    public class GetOrder : IReturn<CustomerOrder>
    {
        public long CustomerId { get; set; }
    }

    [Route("/order")]
    public class FindOrders : QueryDb<CustomerOrder> { }

}
