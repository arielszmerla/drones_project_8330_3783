using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLAPI;
using DO;
using DS;


namespace DalObject
{
    internal partial class DalObject : IDal
    {
        /// <summary>
        /// send a new customer to database
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
        {
            if (DataSource.Customers.Any(cos => cos.Id == customer.Id))
            {
                throw new CostumerExeption("id allready exist");
            }
            DataSource.Customers.Add(customer);
        }
        /// <summary>
        /// gets customer from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></the customer got>
        public Customer GetCustomer(int id)
        {
            Customer? costumer = null;

            costumer = DataSource.Customers.Find(cs => cs.Id == id);
            if (costumer == null)
                throw new CostumerExeption($"id {id}of customer not found");
            return (Customer)costumer;
        }
        /// <summary>
        /// func that returns list to print in console
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomerList(Func<Customer, bool> predicate = null)
        {

            if (predicate == null)
                return DataSource.Customers.ToList();
            else
                //return drones.Where(predicate);
                return (from item in DataSource.Customers
                        where predicate(item)
                        select item);

        }
        public void UpdateCustomerInfoFromBL(Customer customer)
        {
            int index = DataSource.Customers.FindIndex(cs => cs.Id == customer.Id);
            DataSource.Customers[index] = customer;
        }


    }
}
