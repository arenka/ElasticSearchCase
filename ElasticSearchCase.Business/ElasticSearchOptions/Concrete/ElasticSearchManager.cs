using ElasticSearchCase.Business.ElasticSearchOptions.Abstract;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchCase.Business.ElasticSearchOptions.Concrete
{
    public class ElasticSearchManager : IElasticSearchService
    {
        public IElasticClient ElasticSearchClient { get; set; }
        private readonly IElasticSearchConfiguration _elasticSearchConfigration;
        public ElasticSearchManager(IElasticSearchConfiguration elasticSearchConfigration)
        {
            _elasticSearchConfigration = elasticSearchConfigration;
            ElasticSearchClient = GetClient();
        }

        private ElasticClient GetClient()
        {
            var str = _elasticSearchConfigration.ConnectionString;
            //var strs = str.Split('|');
            //var nodes = strs.Select(s => new Uri(s)).ToList();

            var connectionString = new ConnectionSettings(new Uri(str))
                .DisablePing()
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false);

            if (!string.IsNullOrEmpty(_elasticSearchConfigration.AuthUserName) && !string.IsNullOrEmpty(_elasticSearchConfigration.AuthPassword))
                connectionString.BasicAuthentication(_elasticSearchConfigration.AuthUserName, _elasticSearchConfigration.AuthPassword);

            return new ElasticClient(connectionString);
        }

        public virtual async Task CreateIndexAsync<T, TKey>(string indexName) where T : ElasticEntity<TKey>
        {
            var exis = ElasticSearchClient.Indices.Exists(indexName);
            if (exis.Exists)
                return;

            var newName = indexName + DateTime.Now.Ticks;
            var result = await ElasticSearchClient
                .Indices.CreateAsync(newName,
                    ss => ss.Index(indexName)
                    .Settings(s => s.NumberOfShards(3).NumberOfReplicas(1).Setting("max_result_window", int.MaxValue)
                                         .Analysis(a => a
                        .TokenFilters(tkf => tkf.AsciiFolding("my_ascii_folding", af => af.PreserveOriginal(true)))
                        .Analyzers(aa => aa
                        .Custom("turkish_analyzer", ca => ca
                         .Filters("lowercase", "my_ascii_folding")
                         .Tokenizer("standard")
                         )))
                        )
                    .Map<T>(m => m.AutoMap().Properties(p => p
                  .Text(t => t.Name(n => n.SearchingArea)
                 .Analyzer("turkish_analyzer")
                      ))));
            return;
        }

        public virtual async Task AddAllDocuments<T>(string indexName, List<T> products) where T : class
        {
            if (products.Any())
            {
                foreach (var product in products)
                {
                    var exis = ElasticSearchClient.DocumentExists(DocumentPath<T>.Id(new Id(product)), dd => dd.Index(indexName));
                    if (!exis.Exists)
                    {
                        var result = await ElasticSearchClient.IndexAsync(product, ss => ss.Index(indexName));
                    }
                }
            }
            //await ElasticSearchClient.IndexManyAsync(products, index: indexName);
            return;
        }
        public virtual async Task<List<T>> SearchAllDocument<T>(string indexName) where T : class
        {
            var res = await ElasticSearchClient.SearchAsync<T>(q => q.Index(indexName).MatchAll().Size(10000)
          .Scroll("10m"));
            return res.Documents.ToList();
        }
        public virtual async Task<List<T>> SearchDocumentWithQuery<T>(string indexName, SearchDescriptor<T> query) where T : class
        {
            query.Index(indexName).Size(10000).Scroll("10m");
            var res = await ElasticSearchClient.SearchAsync<T>(query);
            return res.Documents.ToList();
        }
        public virtual async Task AddDocument<T>(string indexName, T product) where T : class
        {
            var exis = ElasticSearchClient.DocumentExists(DocumentPath<T>.Id(new Id(product)), dd => dd.Index(indexName));
            if (!exis.Exists)
            {
                var result = await ElasticSearchClient.IndexAsync(product, ss => ss.Index(indexName));
            }
        }
    }
}
