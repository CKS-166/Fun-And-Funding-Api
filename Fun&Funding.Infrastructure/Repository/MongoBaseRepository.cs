using Fun_Funding.Application.IRepository;
using Fun_Funding.Infrastructure.Database;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Infrastructure.Repository
{
    public class MongoBaseRepository<T> : IMongoBaseRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoBaseRepository(MongoDBContext mongoDB, string collectionName)
        {
            // Get the collection for type T from the database
            _collection = mongoDB.GetCollection<T>(collectionName);
        }

        // Create a new document
        public void Create(T entity)
        {
            _collection.InsertOne(entity);
        }

        // Get a single document based on a filter
        public T Get(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).FirstOrDefault();
        }

        // Get all documents in the collection
        public List<T> GetAll()
        {
            return _collection.Find(FilterDefinition<T>.Empty).ToList();
        }

        // Get a list of documents based on a filter
        public List<T> GetList(Expression<Func<T, bool>> filter)
        {
            return _collection.Find(filter).ToList();
        }

        // Remove a document based on a filter
        public void Remove(Expression<Func<T, bool>> filter)
        {
            _collection.DeleteOne(filter);
        }

        // Update a document based on a filter
        public void Update(Expression<Func<T, bool>> filter, T entity)
        {
            _collection.ReplaceOne(filter, entity);
        }
    }

}
