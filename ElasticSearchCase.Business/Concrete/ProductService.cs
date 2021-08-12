using ElasticSearchCase.Business.Abstract;
using ElasticSearchCase.Business.ElasticSearchOptions.Abstract;
using ElasticSearchCase.DataAccess.Abstract;
using ElasticSearchCase.Entities.Concrete;
using Nest;
using ElasticSearchCase.Business.Constrants;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using ElasticSearchCase.Business.Dtos;
using System.Threading.Tasks;
using System;

namespace ElasticSearchCase.Business.Concrete
{
    public class ProductService : IProductService
    {
        public IElasticClient EsClient { get; set; }
        private IProductDal _productDal;
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IMapper _mapper;
        public ProductService(IProductDal productDal, IElasticSearchService elasticSearchService, IMapper mapper, IElasticSearchConfiguration elasticSearchConfigration)
        {
            _productDal = productDal;
            _elasticSearchService = elasticSearchService;
            _mapper = mapper;
        }


        public void Add(ProductDto product)
        {
            var p = _mapper.Map<Product>(product);
            _productDal.Add(p);
            _elasticSearchService.AddDocument<ProductElasticSearchIndexDto>(ElasticSearchConstrants.ProductIndexName, _mapper.Map<ProductElasticSearchIndexDto>(p));
        }

        public void Delete(int productId)
        {
            _productDal.Delete(new Product { Id = productId });
        }

        public async Task<List<ProductElasticSearchIndexDto>> GetAll()
        {
            //elasticsearch index oluşturuluyor.
            await _elasticSearchService.CreateIndexAsync<ProductElasticSearchIndexDto, int>(ElasticSearchConstrants.ProductIndexName);
            //veritabanından veriler çekiliyor.
            var products = _productDal.GetList().ToList();
            if (!products.Any())
            {
                return new List<ProductElasticSearchIndexDto>();
            }
            //elasticsearch indexin içine veriler document olarak ekleniyor.
            await _elasticSearchService.AddAllDocuments<ProductElasticSearchIndexDto>(ElasticSearchConstrants.ProductIndexName, _mapper.Map<List<ProductElasticSearchIndexDto>>(products));
            //elasticsearch üzerinden tüm veriler listeleniyor.
            var response = _elasticSearchService.SearchAllDocument<ProductElasticSearchIndexDto>(ElasticSearchConstrants.ProductIndexName);
            return response.Result.OrderBy(x => x.ProductName).ToList();
        }


        public async Task<List<ProductElasticSearchIndexDto>> GetElasticSearchDocumentByQuery(string query)
        {
            var searchQuery = new Nest.SearchDescriptor<ProductElasticSearchIndexDto>()
                             .Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.ProductName).Query(query)) || q.Regexp(r => r.Field(f => f.ProductName).Value(".*" + query + ".*")));

            var response = await _elasticSearchService.SearchDocumentWithQuery<ProductElasticSearchIndexDto>(ElasticSearchConstrants.ProductIndexName, searchQuery);
            return response.OrderBy(x => x.ProductName).ToList();
        }
        public Product GetById(int productId)
        {
            return _productDal.Get(c => c.Id == productId);
        }

        public void Update(Product product)
        {
            _productDal.Update(product);
        }
    }
}
