using ElasticSearchCase.Core.DataAccess;
using ElasticSearchCase.DataAccess.Abstract;
using ElasticSearchCase.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.DataAccess.Concrete
{
    public class EfProductDal : EfEntityRepositoryBase<Product, ProjectContext>, IProductDal
    {

    }
}
