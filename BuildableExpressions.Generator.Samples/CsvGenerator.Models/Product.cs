// Generated from CSV file Products.csv

using System.Collections.Generic;

namespace CsvGenerator.Models
{
    public class Product
    {
        public static readonly List<Product> All = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Widget 1",
                Price = 10.99d
            },
            new Product
            {
                Id = 2,
                Name = "Widget 2",
                Price = 12.99d
            },
            new Product
            {
                Id = 3,
                Name = "Fast Car",
                Price = 12345.99d
            }
        };

        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }
    }
}