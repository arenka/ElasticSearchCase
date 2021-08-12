using ElasticSearchCase.Business.ElasticSearchOptions.Concrete;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchCase.Business.ElasticSearchOptions.Abstract
{
    public interface IElasticSearchService
    {
        Task CreateIndexAsync<T, TKey>(string indexName) where T : ElasticEntity<TKey>;
        Task AddAllDocuments<T>(string indexName, List<T> products) where T : class;
        Task AddDocument<T>(string indexName, T product) where T : class;
        Task<List<T>> SearchAllDocument<T>(string indexName) where T : class;
        Task<List<T>> SearchDocumentWithQuery<T>(string indexName, SearchDescriptor<T> query) where T : class;
    }
}
