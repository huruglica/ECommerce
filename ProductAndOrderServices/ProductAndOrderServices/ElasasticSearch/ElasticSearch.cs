using Elasticsearch.Net;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Nest;
using ProductAndOrderServices.Helpers;
using ProductAndOrderServices.Model;
using System.Xml.Linq;

namespace ProductAndOrderServices.ElasasticSearch
{
    public class ElasticSearch
    {
        private readonly ElasticClient _elasticClient;

        private string indexName = "products";

        public ElasticSearch(ElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task BulkInster(List<Product> products)
        {
            await _elasticClient.BulkAsync(x => x.Index(indexName).IndexMany(products));
        }

        public async Task Insert(Product product)
        {
            await _elasticClient.IndexAsync(product, i => i.Index(indexName));
        }

        public async Task Update(string id, Product product)
        {
            await _elasticClient.UpdateAsync<Product>(id, x =>
                  x.Index(indexName).Doc(product).Refresh(Refresh.True));
        }

        public async Task Delete(string id)
        {
            await _elasticClient.DeleteAsync<Product>(id, x =>
                  x.Index(indexName).Refresh(Refresh.True));
        }

        public async Task<PagedInfo<Product>> GetAll(SearchAndSort searchAndSort)
        {
            var query = GetQuery(searchAndSort.Name, searchAndSort.StartPrice, searchAndSort.EndPrice);
            var sort = GetSort(searchAndSort.IsAscending);

            var productDocuments = await _elasticClient.SearchAsync<Product>(i => i.Index(indexName)
                         .Query(q => query).Sort(so => sort));

            var products = productDocuments.Documents.ToList();

            var productsPaged = new PagedInfo<Product>()
            {
                TotalCount = products.Count,
                Page = searchAndSort.Page,
                PageSize = searchAndSort.PageSize,
                Data = products
                       .Skip((searchAndSort.Page - 1) * searchAndSort.PageSize)
                       .Take(searchAndSort.PageSize)
                       .ToList()
            };

            return productsPaged;
        }

        private QueryContainerDescriptor<Product> GetQuery(string? name, double? startPrice, double? endPrice)
        {
            var filter = new QueryContainerDescriptor<Product>();
            if (!name.IsNullOrEmpty())
            {
                filter.Bool(b => b
                        .Must(mu =>
                            mu.Match(m => m.Field(f => f.Name).Query(name))
                        ));
            }

            if (startPrice != null)
            {
                filter.Bool(b =>
                            b.Filter(f =>
                                f.Range(r =>
                                    r.Field(fi =>
                                        fi.Price).GreaterThanOrEquals(startPrice))));
            }

            if (endPrice != null)
            {
                filter.Bool(b =>
                            b.Filter(f =>
                                f.Range(r =>
                                    r.Field(fi =>
                                        fi.Price).LessThanOrEquals(endPrice))));
            }

            return filter;
        }

        private SortDescriptor<Product> GetSort(bool? isAscending)
        {
            var sort = new SortDescriptor<Product>();

            if (isAscending == true)
            {
                sort.Ascending(a => a.Price);
            }
            else if (isAscending == false)
            {
                sort.Descending(a => a.Price);
            }

            return sort;
        }
    }
}
