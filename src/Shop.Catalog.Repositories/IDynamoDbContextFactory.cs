using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;

namespace Shop.Catalog.Repositories
{
    public interface IDynamoDbContextFactory
    {
        IDynamoDBContext CreateContext();
    }
}
