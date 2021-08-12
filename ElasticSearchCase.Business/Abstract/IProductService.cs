using ElasticSearchCase.Business.Dtos;
using ElasticSearchCase.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchCase.Business.Abstract
{
    public interface IProductService
    {
        Task<List<ProductElasticSearchIndexDto>> GetAll();
        Task<List<ProductElasticSearchIndexDto>> GetElasticSearchDocumentByQuery(string query);
        void Add(ProductDto product);
        void Update(Product product);
        void Delete(int productId);
        Product GetById(int productId);
    }
}
