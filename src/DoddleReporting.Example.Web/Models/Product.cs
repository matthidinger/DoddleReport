using System;

namespace DoddleReporting.Example.Web.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int OrderCount { get; set; }
        public DateTime LastPurchase { get; set; }
        public int UnitsInStock { get; set; }
        public bool OutOfStock
        {
            get { return UnitsInStock == 0; }
        }
    }
}