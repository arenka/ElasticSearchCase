using ElasticSearchCase.Core.DataAccess;
using ElasticSearchCase.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.DataAccess.Abstract
{
   public interface IProductDal : IEntityRepository<Product>
    {
    }
}
