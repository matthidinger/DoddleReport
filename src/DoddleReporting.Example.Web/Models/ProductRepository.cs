using System;
using System.Collections.Generic;
using System.Linq;

namespace DoddleReporting.Example.Web.Models
{
    public static class ProductRepository
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