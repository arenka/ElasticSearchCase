using ElasticSearchCase.Business.Dtos;
using ElasticSearchCase.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchCase.WebUI.Models
{
    public class ProductListViewModel
    {
        public List<ProductElasticSearchIndexDto> Products { get; set; }
    }
}
