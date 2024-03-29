using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Newtonsoft.Json;
using Shop.Catalog.Domain.Entities;
using Xunit;

namespace Shop.Catalog.Api.Tests
{
    public class FunctionTest : IDisposable
    { 
        string TableName { get; }
        IAmazonDynamoDB DDBClient { get; }
        
        public FunctionTest()
        {
            this.TableName = "Shop.Catalog.Api-Products-" + DateTime.Now.Ticks;
            this.DDBClient = new AmazonDynamoDBClient(RegionEndpoint.USWest2);

            SetupTableAsync().Wait();
        }

        //[Fact]
        //public async Task BlogTestAsync()
        //{
            //TestLambdaContext context;
            //APIGatewayProxyRequest request;
            //APIGatewayProxyResponse response;

            //Functions functions = new Functions(this.DDBClient, this.TableName);


   //         // Add a new blog post
   //         Product myBlog = new Product();
   //         myBlog.ProductId= "P1234";
   //         myBlog.Category = "C1234";

   //         request = new APIGatewayProxyRequest
   //         {
   //             Body = JsonConvert.SerializeObject(myBlog)
   //         };
   //         context = new TestLambdaContext();
   //         response = await functions.GetProductAsync(request, context);
   //         Assert.Equal(200, response.StatusCode);

   //         var blogId = response.Body;

   //         // Confirm we can get the blog post back out
   //         request = new APIGatewayProxyRequest
   //         {
   //             PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, blogId } }
   //         };
   //         context = new TestLambdaContext();
   //         response = await functions.GetBlogAsync(request, context);
   //         Assert.Equal(200, response.StatusCode);

   //         Blog readBlog = JsonConvert.DeserializeObject<Blog>(response.Body);
   //         Assert.Equal(myBlog.Name, readBlog.Name);
   //         Assert.Equal(myBlog.Content, readBlog.Content);

   //         // List the blog posts
   //         request = new APIGatewayProxyRequest
   //         {
   //         };
   //         context = new TestLambdaContext();
   //         response = await functions.GetBlogsAsync(request, context);
   //         Assert.Equal(200, response.StatusCode);

   //         Blog[] blogPosts = JsonConvert.DeserializeObject<Blog[]>(response.Body);
			//Assert.Single(blogPosts);
   //         Assert.Equal(myBlog.Name, blogPosts[0].Name);
   //         Assert.Equal(myBlog.Content, blogPosts[0].Content);


   //         // Delete the blog post
   //         request = new APIGatewayProxyRequest
   //         {
   //             PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, blogId } }
   //         };
   //         context = new TestLambdaContext();
   //         response = await functions.RemoveBlogAsync(request, context);
   //         Assert.Equal(200, response.StatusCode);

   //         // Make sure the post was deleted.
   //         request = new APIGatewayProxyRequest
   //         {
   //             PathParameters = new Dictionary<string, string> { { Functions.ID_QUERY_STRING_NAME, blogId } }
   //         };
   //         context = new TestLambdaContext();
   //         response = await functions.GetBlogAsync(request, context);
   //         Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
        //}



        /// <summary>
        /// Create the DynamoDB table for testing. This table is deleted as part of the object dispose method.
        /// </summary>
        /// <returns></returns>
        private async Task SetupTableAsync()
        {
            
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = this.TableName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        KeyType = KeyType.HASH,
                        AttributeName = Functions.ID_QUERY_STRING_NAME
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = Functions.ID_QUERY_STRING_NAME,
                        AttributeType = ScalarAttributeType.S
                    }
                }
            };

            await this.DDBClient.CreateTableAsync(request);

            var describeRequest = new DescribeTableRequest { TableName = this.TableName };
            DescribeTableResponse response = null;
            do
            {
                Thread.Sleep(1000);
                response = await this.DDBClient.DescribeTableAsync(describeRequest);
            } while (response.Table.TableStatus != TableStatus.ACTIVE);
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.DDBClient.DeleteTableAsync(this.TableName).Wait();
                    this.DDBClient.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
