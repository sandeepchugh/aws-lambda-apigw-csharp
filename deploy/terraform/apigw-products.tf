resource "aws_api_gateway_rest_api" "products_apigateway" {
  name        = "${local.products_table_name}"
  description = "Shop Catalog Api Gateway"
}

resource "aws_api_gateway_resource" "products_apigateway_resource" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  parent_id   = "${aws_api_gateway_rest_api.products_apigateway.root_resource_id}"
  path_part   = "{proxy+}"
}

resource "aws_api_gateway_method" "products_apigateway_proxy" {
  rest_api_id   = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id   = "${aws_api_gateway_resource.products_apigateway_resource.id}"
  http_method   = "ANY"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "lambda" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.products_apigateway_proxy.resource_id}"
  http_method = "${aws_api_gateway_method.products_apigateway_proxy.http_method}"

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "${aws_lambda_function.products_lambda.invoke_arn}"  
}

resource "aws_api_gateway_method" "proxy_root" {
  rest_api_id   = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id   = "${aws_api_gateway_rest_api.products_apigateway.root_resource_id}"
  http_method   = "ANY"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "lambda_root" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.proxy_root.resource_id}"
  http_method = "${aws_api_gateway_method.proxy_root.http_method}"

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "${aws_lambda_function.products_lambda.invoke_arn}"
}

resource "aws_api_gateway_deployment" "products_apigateway_deployment" {
  depends_on = [
    "aws_api_gateway_integration.lambda",
    "aws_api_gateway_integration.lambda_root",
  ]

  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  stage_name  = "dev"
}

output "base_url" {
  value = "${aws_api_gateway_deployment.products_apigateway_deployment.invoke_url}"
}