// Generated from CSV file Categories.csv

using System.Collections.Generic;

namespace CsvGenerator.Models
{
    public class Category
    {
        public static readonly List<Category> All = new List<Category>
        {
            new Category { Name = "Stuff" },
            new Category { Name = "Things" },
            new Category { Name = "Whatsits" },
            new Category { Name = "Doo-Dahs" },
            new Category { Name = "Widgets" }
        };

        public string Name { get; set; }
    }
}