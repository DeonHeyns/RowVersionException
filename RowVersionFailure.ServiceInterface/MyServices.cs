using RowVersionFailure.ServiceModel;
using RowVersionFailure.ServiceModel.Types;
using ServiceStack;
using ServiceStack.OrmLite;
using System.Threading.Tasks;

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

		public async Task<object> Put(PutCustomer request)
		{
			var customer = await Db.SingleByIdAsync<Customer>(request.Id);
			customer = customer.PopulateWith(request);
			var address = customer.Address.PopulateWith(request);
			customer.Address = address;

			try
			{
				// this deadlocks/hangs
				await Db.UpdateAsync(customer);
				await Db.UpdateAsync(address);

				// this deadlocks/hangs as well
				await Db.UpdateAsync(customer).ConfigureAwait(false);
				await Db.UpdateAsync(address).ConfigureAwait(false);

				// this works
				Db.Update(customer);
				Db.Update(address);
			}
			catch (System.Exception ex)
			{
			}

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
