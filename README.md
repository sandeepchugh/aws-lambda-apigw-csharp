# Shop < Catalog Api >
> Product catalog api for shop

This repository contains an aws lambda hosted api exposed using aws api gateway and cloudfront cdn for retrieving products by category or product id. The api is a read only api used by the shop site to display products. 

## Design
AWS Services Used
- AWS Lambda
- API Gateway
- Cloudfront CDN
- DynamoDB Accelerator (DAX)
> TODO - Add design and implementation details

## Build and Deploy
The following infrastructure as code deployment options have been setup for deploying the api
- AWS SAM (Serverless Application Model)
- AWS Cloudformation
- Terraform

#### AWS SAM

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "Shop.Catalog.Api/test/Shop.Catalog.Api.Tests"
    dotnet test
```    
Deploy application
```
    cd "Shop.Catalog.Api/src/Shop.Catalog.Api"
    dotnet lambda deploy-serverless
```

#### CloudFormation
> TODO

#### Terraform
> TODO

