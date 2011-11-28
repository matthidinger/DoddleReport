using System;
using System.Collections.Generic;
using System.Linq;

namespace DoddleReporting.Sample.Web.Models
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

    public static class DoddleProductRepository
    {
        public static List<Product> GetAll()
        {
            var rand = new Random();
            return Enumerable.Range(1, 200)
                .Select(i => new Product
                                 {
                                     Id = i,
                                     Name = "Product " + i,
                                     Description =
                                         "This is an example description showing long text in some of the items, now I am just rambling",
                                     Price = rand.NextDouble()*100,
                                     OrderCount = rand.Next(1000),
                                     LastPurchase = DateTime.Now.AddDays(rand.Next(1000)),
                                     UnitsInStock = rand.Next(0, 1000)

                                 })
                .ToList();
        }
    }
}