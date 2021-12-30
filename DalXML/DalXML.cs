using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLAPI;
using DO;

namespace DalXML
{

    sealed partial class DalXML : IDal
    {
        #region singelton
        class Nested
        {
            static Nested() { }
            internal static readonly DalXML instance = new DalXML();
        }

        private static object syncRoot = new object();
        public static DalXML Instance
        {
            get
            {
                if (Nested.instance == null)
                {
                    lock (syncRoot)
                    {
                        if (Nested.instance == null)
                            return Nested.instance;
                    }
                }

                return Nested.instance;
            }
        }

        #endregion


        #region customer methods
        /// <summary>
        /// method to add a customer
        /// </summary>
        /// <param name="customer"></param>
        void AddCustomer(Customer customer)
        {
           List<Customer> cus=  XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (cus.Any(cos => cos.Id == customer.Id))
            {
                throw new CostumerExeption("id allready exist");
            }
            cus.Add(customer);
            XMLTolls.SaveListToXMLSerializer(cus, @"customer.xml");

        }
        /// <summary>
        /// detele element
        /// </summary>
        /// <param name="id"></id element>
        public void DeleteCustomer(int id)
        {
            List<Customer> cus = XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (!cus.Any(cos => cos.Id == id))
            {
                throw new DLAPI.DeleteException($"Customer with {id}as Id does not exist");
            }
            cus.RemoveAll(p => p.Id == id);
            XMLTolls.SaveListToXMLSerializer(cus, @"customer.xml");
        }

        /// <summary>
        /// gets customer from database and return it to main
        /// </summary>
        /// <param name="id"></param>
        /// <returns></the customer got>
        public Customer GetCustomer(int id)
        {
            List<Customer> cus = XMLTolls.LoadListFromXMLSerializer<Customer>(@"customer.xml");
            if (!(cus.Any(customer => customer.Id == id)))
            {
                throw new DLAPI.CostumerExeption()
            }

            Customer? costumer = null;

            costumer = DataSource.Customers.Find(cs => cs.Id == id);
            if (costumer == null)
                throw new CostumerExeption($"id {id}of customer not found");
            return (Customer)costumer;
        }


        #endregion

    }

}
