using System;
using System.Collections.Generic;
using System.Text;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Util;
using Shop.Catalog.Domain.Entities;
using DynamoDBContextConfig = Amazon.DynamoDBv2.DataModel.DynamoDBContextConfig;

namespace Shop.Catalog.Repositories
{
    public class DynamoDbContextFactory : IDynamoDbContextFactory
    {
        public DynamoDbContextFactory(string tableName)
        {
            if(string.IsNullOrWhiteSpace(tableName)){
                throw new ArgumentNullException(nameof(tableName));
            }

            AWSConfigsDynamoDB.Context.TypeMappings[typeof(Product)] = 
                new TypeMapping(typeof(Product), tableName);
            
        }

        public IDynamoDBContext CreateContext()
        {
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            var dbContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);

            return dbContext;
        }
    }
}
