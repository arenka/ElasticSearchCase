# ElasticSearchCase

Getting Started with ElasticSearchCase

Requirements :

⦁	Dotnet core 3.1

⦁	Mysql or Wampserver

⦁	Elasticsearch

⦁	Pomelo.EntityFrameworkCore.Mysql

⦁	Kibana or ElasticSearch Head (Google Extension)

⦁	Automapper

Firstly, open the tools -> nuget package manager -> package manager console in visual studio. While the default project is in the ElasticSearchCase.DataAccess layer, create the database by writing update-database on the console screen.
Make sure Elasticsearch is installed or running on your computer. If it is not installed, you can download and install it from https://www.elasticsearch.co/downloads/elasticsearch. If it is installed, you can check that it is working from the localhost:9200 port.
 
You will see a screen like the one above. Version information may change.
For the project to work, MySql database or wampserver must be installed on your computer. After making sure it is installed and running, right click on the ElasticSearchCase.WebUI layer and set it as set as startup project and you are ready to run the project.
After adding the product, the attached data in the database will be listed.
Kibana can be used for monitoring. I viewed my index and documents with the chrome elastic head plugin.

Proje Katmanları

İlk olarak boş bir solution oluşturuyoruz. ElasticSearchCase olarak isimlendirdim. Bu solutiona katmanlarımızı ekleyeceğiz.​

ElasticSearchCase.Entities : Projede kullanacağımız entity ler bu katmanda tutulacak.​

ElasticSearchCase.DataAccess : Bu katmanda context ve veritabanı ile ilgili işlemlerimizi gerçekleştireceğiz.​

ElasticSearchCase.Business: Projemizin iş katmanıdır.​

ElasticSearchCase.Core : Katmanlar arası ortak kullanılacak dosyaların katmanıdır.​

ElasticSearchCase.WebUI : Projemizin (client-side) kullanıcı tarafıdır.​

Elasticsearch Konfigürasyonu ve Methodları

Elasticsearch bağlantısını kurabilmemiz için bizden kullanıcı adı,şifre ve bağlantı bilgisi bilgilerini girmemiz gerekmektedir.​
Bu bilgilerin girişini iş katmanında ElasticSearchConfigration sınıfı içerisindeki alanları appsetting.json dosyasına konfigüre ettiğimiz alanlar ile setliyoruz.​

appsetting.json;​

    "ElasticSearchOptions": {
      "ConnectionString": {
        "HostUrl": "http://localhost:9200/",
        "UserName": "guest",
        "Password": "guest"
      }
    }
    
ElasticSearchConfigration;​

       public class ElasticSearchConfigration : IElasticSearchConfiguration
       {
        public IConfiguration Configuration { get; }
        public ElasticSearchConfigration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string ConnectionString { get { return Configuration.GetSection("ElasticSearchOptions:ConnectionString:HostUrl").Value; } }
        public string AuthUserName { get { return Configuration.GetSection("ElasticSearchOptions:ConnectionString:UserName").Value; } }
        public string AuthPassword { get { return Configuration.GetSection("ElasticSearchOptions:ConnectionString:Password").Value; } }
       }

GetClient(): ElasticSearchConfigration sınıfı içerisini setlediğimiz alanlar ile elasticsearch bağlantısı kurmamızı sağlar.​

       private ElasticClient GetClient()
        {
            var str = _elasticSearchConfigration.ConnectionString;

            var connectionString = new ConnectionSettings(new Uri(str))
                .DisablePing()
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false);

            if (!string.IsNullOrEmpty(_elasticSearchConfigration.AuthUserName) && !string.IsNullOrEmpty(_elasticSearchConfigration.AuthPassword))
                connectionString.BasicAuthentication(_elasticSearchConfigration.AuthUserName, _elasticSearchConfigration.AuthPassword);

            return new ElasticClient(connectionString);
        }
        
CreateIndexAsync(): Indexin var olup olmadığını kontrol eder eğer yok ise Index oluşturur.​

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

   AddAllDocuments(): Index içerisinde documentin olup olmadığını kontrol eder ve yok ise veritabanındaki verileri document olarak elastıcsearch e ekler.​
   
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
            return;
        }
        
AddDocument(): Mysql veritabanına bir veri eklendiğinde o veriyi elasticsearche document olarak ekler.​

       public virtual async Task AddDocument<T>(string indexName, T product) where T : class
        {
            var exis = ElasticSearchClient.DocumentExists(DocumentPath<T>.Id(new Id(product)), dd => dd.Index(indexName));
            if (!exis.Exists)
            {
                var result = await ElasticSearchClient.IndexAsync(product, ss => ss.Index(indexName));
            }
        }

SearchAllDocument(): Elasticsearch üzerinde index ismine göre arama yapar.​

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

SearchDocumentWithQuery(): Elasticsearch üzerinde index ismi ve girilen veriye göre arama yapar. 

        public virtual async Task<List<T>> SearchDocumentWithQuery<T>(string indexName, SearchDescriptor<T> query) where T : class
        {
            query.Index(indexName).Size(10000).Scroll("10m");
            var res = await ElasticSearchClient.SearchAsync<T>(query);
        }          

Not: Verilerin anlık olarak sayfada görüntülenmesini ajax ile sağlıyoruz. 

Örnek ajax;

       $("#saveButton").click(function () {
            var pName = $("#txtProductName").val();
            var pPrice = $("#txtProductPrice").val();
            var product = {
                ProductName : pName,
                Price : pPrice
            };
         $.ajax({
             url: "@Url.Action("AddProduct", "Home")",
             type: "post",
             dataType: "json",
             data: product,
             success: function (data) {
                 if (data.status) {
                     setTimeout(function () {
                         $('#addProductModal').modal('hide');
                       $("#productList").load("@Url.Action("GetProductList","Home")");
                     }, 3000);
                 }
             }
            });
            
setTimeout fonksiyonu içerisine alınmasının sebebi sunucu hızlı olduğu durumlarda sayfa yenileme işlemini kayıttan önce çağırmasından kaynaklıdır. İstenildiği takdirde varsayılan olarak 3000ms (3sn) olarak belirtilen süre azaltılıp, çoğaltılabilir.

