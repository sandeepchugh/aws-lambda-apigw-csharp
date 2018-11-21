using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Newtonsoft.Json;
using Shop.Catalog.Domain.Entities;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Shop.Catalog.Api
{
    public class Functions
    {
        // This const is the name of the environment variable that the serverless.template will use to set
        // the name of the DynamoDB table used to store blog posts.
        const string TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP = "ProductTable";

        public const string ID_QUERY_STRING_NAME = "Id";
        IDynamoDBContext DDBContext { get; set; }

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            // Check to see if a table name was passed in through environment variables and if so 
            // add the table mapping.
            var tableName = System.Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
            if(!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Product)] = new Amazon.Util.TypeMapping(typeof(Product), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// Constructor used for testing passing in a preconfigured DynamoDB client.
        /// </summary>
        /// <param name="ddbClient"></param>
        /// <param name="tableName"></param>
        public Functions(IAmazonDynamoDB ddbClient, string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Product)] = new Amazon.Util.TypeMapping(typeof(Product), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(ddbClient, config);
        }

        /// <summary>
        /// A Lambda function that returns back a page worth of blog posts.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>The list of products</returns>
        public async Task<APIGatewayProxyResponse> GetProductsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Getting blogs");
            var search = this.DDBContext.ScanAsync<dynamic>(null);
            var page = await search.GetNextSetAsync();
            context.Logger.LogLine($"Found {page.Count} products");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(page),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        /// <summary>
        /// A Lambda function that returns the product identified by productId
        /// </summary>
        /// <param name="request"></param>
        /// <paramref name="context"/>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetProductAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string productId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                productId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                productId = request.QueryStringParameters[ID_QUERY_STRING_NAME];

            if (string.IsNullOrEmpty(productId))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = $"Missing required parameter {ID_QUERY_STRING_NAME}"
                };
            }

            context.Logger.LogLine($"Getting blog {productId}");
            var product = await DDBContext.LoadAsync<dynamic>(productId);
            context.Logger.LogLine($"Found product: {product != null}");

            if (product == null)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(product),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            return response;
        }
    }
}
