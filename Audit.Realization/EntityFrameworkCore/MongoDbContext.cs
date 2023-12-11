using Audit.Realization.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Audit.Realization.EntityFrameworkCore
{
    /// <summary>
    /// MongoDb
    /// </summary>
    public class MongoDbContext : IDynamicDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<AuditLog> _collection;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <exception cref="ArgumentException"></exception>
        public MongoDbContext(string? connectionString = null, string? databaseName = null, string? collectionName = "AuditLog")
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentException("Invalid database configuration information specified in appsettings.json");
            }

            var client = new MongoClient(connectionString);
            switch (true)
            {
                case true:
                    _database   = client.GetDatabase(databaseName);
                    _collection = _database.GetCollection<AuditLog>(collectionName);
                    break;
            }
        }

        public IMongoCollection<AuditLog> Collection => _collection;

        /// <summary>
        /// 插入审计日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsertAsync(AuditLog entity)
        {
           await  _collection.InsertOneAsync(entity);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="serviceName">服务名词</param>
        /// <param name="clientIpAddress">客户端IP</param>
        /// <returns></returns>
        public async Task<List<AuditLog>> GetByWhereAsync(string serviceName, string clientIpAddress, int pageIndex, int pageSize)
        {

            var filterBuilder = Builders<AuditLog>.Filter;
            var filter = filterBuilder.Empty; // 创建一个空的过滤器

            if (!string.IsNullOrEmpty(serviceName))
            {
                filter &= filterBuilder.Regex("ServiceName", new BsonRegularExpression(serviceName, "i")); // 不区分大小写的模糊查询
            }

            if (!string.IsNullOrEmpty(clientIpAddress))
            {
                filter &= filterBuilder.Regex("ClientIpAddress", new BsonRegularExpression(clientIpAddress, "i")); // 不区分大小写的模糊查询
            }

            // 按照分页参数进行分页查询
            var result = await _collection.Find(filter)
                                    .Skip((pageIndex - 1) * pageSize)
                                    .Limit(pageSize)
                                    .ToListAsync();

            return result;
        }

        /// <summary>
        /// 获取所有审计信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        public bool TableExists()
        {
            var filter = new BsonDocument("name", "AuditLog");
            var collections = _database.ListCollectionNames(new ListCollectionNamesOptions { Filter = filter });

            return collections.Any();
        }
    }
}
