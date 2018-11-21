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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shop.Catalog.Domain.Entities;
using Shop.Catalog.Domain.Repositories;
using Shop.Catalog.Domain.Services;
using Shop.Catalog.Repositories;

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
        private readonly IProductService _productService;
        private readonly ILogger<Functions> _logger;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            // Check to see if a table name was passed in through environment variables and if so 
            // add the table mapping.
            var tableName = Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
            _logger = new ConsoleLogger<Functions>();
            _logger.LogInformation($"[Functions:Constructor] TableName: {tableName??"[NULL]"}");

            var productRepository = new ProductRepository(new DynamoDbContextFactory(tableName),new ConsoleLogger<ProductRepository>());
            _productService = new ProductService(productRepository, new ConsoleLogger<ProductService>());
        }

        /// <summary>
        /// Constructor used for testing passing in a preconfigured DynamoDB client.
        /// </summary>
        /// <param name="ddbClient"></param>
        /// <param name="tableName"></param>
        public Functions(IAmazonDynamoDB ddbClient, string tableName)
        {
            //if (!string.IsNullOrEmpty(tableName))
            //{
            //    AWSConfigsDynamoDB.Context.TypeMappings[typeof(Product)] = new Amazon.Util.TypeMapping(typeof(Product), tableName);
            //}

            //var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            //this.DDBContext = new DynamoDBContext(ddbClient, config);
        }

        /// <summary>
        /// A Lambda function that returns back a page worth of blog posts.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>The list of products</returns>
        public async Task<APIGatewayProxyResponse> GetProductsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            _logger.LogInformation("Getting products");

            var products = await _productService.GetProductsAsync(new ProductOptions());

            _logger.LogInformation($"Found {products.Count} products");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(products),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            _logger.LogInformation($"Respose Status Code: {response.StatusCode}");

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
            var productId = GetProductId(request);

            if (string.IsNullOrEmpty(productId))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = $"Missing required parameter {ID_QUERY_STRING_NAME}"
                };
            }

            _logger.LogInformation($"Getting product {productId}");

            var product = (await _productService.GetProductsAsync(new ProductOptions {ProductId =  productId}))
                            .FirstOrDefault();

            _logger.LogInformation($"Found product: {product != null}");

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

            _logger.LogInformation($"Respose Status Code: {response.StatusCode}");

            return response;
        }

        /// <summary>
        /// Returns the product id from path parameters or querystring 
        /// </summary>
        /// <param name="request">An instance of <see cref="APIGatewayProxyRequest"/></param>
        /// <returns>Product id</returns>
        private string GetProductId(APIGatewayProxyRequest request)
        {
            string productId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                productId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                productId = request.QueryStringParameters[ID_QUERY_STRING_NAME];

            _logger.LogInformation($"Product id from input {productId ?? "[NULL]"}");

            return productId;
        }
    }
}
