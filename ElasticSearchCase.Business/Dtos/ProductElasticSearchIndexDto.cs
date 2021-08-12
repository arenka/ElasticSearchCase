using ElasticSearchCase.Business.ElasticSearchOptions.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.Business.Dtos
{
    public class ProductElasticSearchIndexDto : ElasticEntity<int>
    {
    
        public string ProductName { get; set; }
        public double? Price { get; set; }
    }
}
