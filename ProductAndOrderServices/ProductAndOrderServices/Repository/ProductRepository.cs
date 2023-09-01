using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductAndOrderServices.Data;
using ProductAndOrderServices.Helpers;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Repository.IRepository;

namespace ProductAndOrderServices.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _dbContext;

        public ProductRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedInfo<Product>> GetAll(SearchAndSort searchAndSort)
        {
            var filter = await GetFilter(searchAndSort.Name, searchAndSort.StartPrice, searchAndSort.EndPrice);
            var sort = await GetSort(searchAndSort.IsAscending);

            var products = await _dbContext.Product
                                 .Find(filter)
                                 .Sort(sort)
                                 .ToListAsync();

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

        private async Task<FilterDefinition<Product>> GetFilter(string? name, double? startPrice, double? endPrice)
        {
            var buildFilter = await Task.Run(() => Builders<Product>.Filter);
            var filter = await Task.Run(() => buildFilter.Empty);

            if (!name.IsNullOrEmpty())
            {
                filter = filter & buildFilter
                    .Regex("Name", new BsonRegularExpression(name, "i"));
            }

            if (startPrice != null)
            {
                filter = filter & buildFilter.Gte("Price", startPrice);
            }

            if (endPrice != null)
            {
                filter = filter & buildFilter.Lte("Price", endPrice);
            }

            return filter;
        }

        private async Task<SortDefinition<Product>?> GetSort(bool? isAscending)
        {
            var buildSort = await Task.Run(() => Builders<Product>.Sort);

            if (isAscending == true)
            {
                var sort = buildSort.Ascending("Price");
                return sort;
            }
            else if (isAscending == false)
            {
                var sort = buildSort.Descending("Price");
                return sort;
            }

            return null;
        }

        public async Task<List<Product>> GetMyProducts(string userId)
        {
            var filter = Builders<Product>.Filter.Eq("SellerId", userId);
            return await _dbContext.Product.Find(filter).ToListAsync();
        }

        public async Task<Product> GetById(string id)
        {
            var filter = Builders<Product>.Filter.Eq("Id", id);
            return await _dbContext.Product.Find(filter).FirstOrDefaultAsync()
                         ?? throw new Exception("Not found");
        }

        public async Task Post(Product product)
        {
            await _dbContext.Product.InsertOneAsync(product);
        }

        public async Task<Product> Update(string id, ProductUpdateDto product)
        {
            var pipeline = GeneratePipeline(product);

            var filter = Builders<Product>.Filter.Eq("Id", id);

            var update = Builders<Product>.Update.Pipeline(pipeline);

            var options = new FindOneAndUpdateOptions<Product, Product>();
            options.ReturnDocument = ReturnDocument.After;

            return await _dbContext.Product.FindOneAndUpdateAsync(filter, update, options)
                         ?? throw new Exception("Not updated");
        }

        private List<BsonDocument> GeneratePipeline(ProductUpdateDto product)
        {
            var pipeline = new List<BsonDocument>();

            if (product.Name != null)
            {
                pipeline.Add(new BsonDocument("$set",
                           new BsonDocument("Name", product.Name)));
            }

            if (product.Description != null)
            {
                pipeline.Add(new BsonDocument("$set",
                           new BsonDocument("Description", product.Description)));
            }

            if (product.Discount != null)
            {
                pipeline.Add(new BsonDocument("$set",
                           new BsonDocument("Discount", product.Discount)));

                pipeline.Add(new BsonDocument("$set",
                    new BsonDocument("Price",
                    new BsonDocument("$subtract",
                    new BsonArray
                                {
                                    "$BasePrice",
                                    new BsonDocument("$multiply",
                                    new BsonArray
                                        {
                                            "$BasePrice",
                                            new BsonDocument("$divide",
                                            new BsonArray
                                                {
                                                    product.Discount,
                                                    100
                                                })
                                        })
                                }))));
            }

            return pipeline;
        }

        public async Task UpdateStock(string id, int stock)
        {
            var filter = Builders<Product>.Filter.Eq("Id", id);
            var update = Builders<Product>.Update
                .Inc(x => x.Stock, stock);

            await _dbContext.Product.UpdateOneAsync(filter, update);
        }

        public async Task<Product> UpdateStock(IClientSessionHandle session, string id, int stock)
        {
            var filter = Builders<Product>.Filter.Eq("Id", id);
            var update = Builders<Product>.Update
                .Inc(x => x.Stock, stock);

            return await _dbContext.Product.FindOneAndUpdateAsync(session, filter, update)
                         ?? throw new Exception("Not updated");
        }

        public async Task<DeleteResult> Delete(string id)
        {
            var filter = Builders<Product>.Filter.Eq("Id", id);
            return await _dbContext.Product.DeleteOneAsync(filter);
          
        }

        public async Task<List<Product>> Get(List<string> filters)
        {
            var filter = Builders<Product>.Filter.In("Id", filters);
            return await _dbContext.Product.Find(filter).ToListAsync();
        }
    }
}
