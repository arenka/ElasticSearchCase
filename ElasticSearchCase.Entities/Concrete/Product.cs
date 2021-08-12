using System;
using System.Collections.Generic;
using System.Text;
using ElasticSearchCase.Core.Entities;


namespace ElasticSearchCase.Entities.Concrete
{
    public class Product: IEntity
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double? Price { get; set; }
    }
}
