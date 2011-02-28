using Mike.AsyncWcf.Core;
using Mike.AsyncWcf.Server;
using NUnit.Framework;

namespace Mike.AsyncWcf.Tests.Server
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private CustomerService customerService;

        [SetUp]
        public void SetUp()
        {
            customerService = new CustomerService();
        }

        [Test]
        public void Should_be_able_to_get_a_customer()
        {
            const int customerId = 22;
            Customer customer = null;

            customerService.BeginGetCustomerDetails(customerId, result => 
            {
                customer = customerService.EndGetCustomerDetails(result);
            }, null);

            customer.ShouldNotBeNull();
        }
    }
}