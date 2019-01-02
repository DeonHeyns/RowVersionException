using ServiceStack;
using RowVersionFailure.ServiceModel;
using RowVersionFailure.ServiceModel.Types;
using ServiceStack.OrmLite;

namespace RowVersionFailure.ServiceInterface
{
    public class OrderingServices : Service
    {
        public object Post(PostCustomer request)
        {
            var customer = request.ConvertTo<Customer>();
            customer.Address = request.ConvertTo<Address>();
            Db.Save(customer, true);
            return customer;
        }

        public object Get(GetCustomer request)
        {
            return Db.SingleById<Customer>(request.Id);
        }

        public object Post(PostOrder request)
        {
            var customer = Db.SingleById<Customer>(request.CustomerId);
            var order = request.ConvertTo<CustomerOrder>();
            order.CustomerId = customer.Id;
            customer.Orders.Add(order);
            Db.Save(order);
            return order;
        }

        public object Get(GetOrder request)
        {
            return Db.SingleById<Customer>(request.CustomerId);
        }
    }
}
