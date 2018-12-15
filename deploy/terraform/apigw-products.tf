# api
resource "aws_api_gateway_rest_api" "products_apigateway" {
  name        = "products-api${var.env_suffix}"
  description = "Shop Catalog Api Gateway"
}

# stage
resource "aws_api_gateway_stage" "stage" {
  stage_name = "${var.env_name}"
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  deployment_id = "${aws_api_gateway_deployment.products_apigateway_deployment.id}"

  lifecycle{
    ignore_changes = ["deployment_id"]
  }
}



# deployment
resource "aws_api_gateway_deployment" "products_apigateway_deployment" {
  depends_on = [
    "aws_api_gateway_rest_api.products_apigateway"
  ]

  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  stage_name  = "${var.env_name}"

  lifecycle{
    create_before_destroy = true
  }
}

# usage plan
resource "aws_api_gateway_usage_plan" "products_apigateway_usage_plan" {
  name = "products-apigateway-usage-plan${var.env_suffix}"

  api_stages {
    api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
    stage  = "${aws_api_gateway_deployment.products_apigateway_deployment.stage_name}"
  }
}

# resource /products GET
resource "aws_api_gateway_resource" "products_apigateway_resource" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  parent_id   = "${aws_api_gateway_rest_api.products_apigateway.root_resource_id}"
  path_part   = "products"
}

resource "aws_api_gateway_method" "products_apigateway_get" {
  rest_api_id   = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id   = "${aws_api_gateway_resource.products_apigateway_resource.id}"
  http_method   = "GET"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "products_lambda_integration" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.products_apigateway_get.resource_id}"
  http_method = "${aws_api_gateway_method.products_apigateway_get.http_method}"

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "${aws_lambda_function.products_lambda.invoke_arn}"
}

resource "aws_api_gateway_method_response" "products_get_response" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.products_apigateway_get.resource_id}"
  http_method = "${aws_api_gateway_method.products_apigateway_get.http_method}"
  status_code = "200"
}

resource "aws_api_gateway_integration_response" "products_get_integration_response" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.products_apigateway_get.resource_id}"
  http_method = "${aws_api_gateway_method.products_apigateway_get.http_method}"
  status_code = "${aws_api_gateway_method_response.products_get_response.status_code}"
}

# resource /products OPTIONS

resource "aws_api_gateway_method" "products_apigateway_options" {
  rest_api_id   = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id   = "${aws_api_gateway_resource.products_apigateway_resource.id}"
  http_method   = "OPTIONS"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "products_options_integration" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.products_apigateway_options.resource_id}"
  http_method = "${aws_api_gateway_method.products_apigateway_options.http_method}"
  type        = "MOCK"

  request_templates {
    "application/json" = <<EOF
{"statusCode":200}
EOF
  }
}

resource "aws_api_gateway_method_response" "products_options_response_200" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.products_apigateway_options.resource_id}"
  http_method = "${aws_api_gateway_method.products_apigateway_options.http_method}"
  status_code = "200"

  response_parameters = {
      "method.response.header.Access-Control-Allow-Headers" = true
      "method.response.header.Access-Control-Allow-Methods" = true
      "method.response.header.Access-Control-Allow-Origin" = true
  }
}

resource "aws_api_gateway_integration_response" "products_options_integration_response" {
    rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_method.products_apigateway_options.resource_id}"
  http_method = "${aws_api_gateway_method.products_apigateway_options.http_method}"
  status_code = "${aws_api_gateway_method_response.products_options_response_200.status_code}"

  response_parameters = {
      "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,Authorization'"
      "method.response.header.Access-Control-Allow-Methods" = "'GET,OPTIONS'"
      "method.response.header.Access-Control-Allow-Origin" = "'*'"
  }
}

# Healthcheck
# resource /healthcheck GET

resource "aws_api_gateway_resource" "healthcheck_apigateway_resource" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  parent_id   = "${aws_api_gateway_rest_api.products_apigateway.root_resource_id}"
  path_part   = "healthcheck"
}

resource "aws_api_gateway_method" "healthcheck_get" {
  rest_api_id      = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id      = "${aws_api_gateway_resource.healthcheck_apigateway_resource.id}"
  http_method      = "GET"
  authorization    = "NONE"
  api_key_required = "false"
}

resource "aws_api_gateway_integration" "healthcheck_get_integration" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_resource.healthcheck_apigateway_resource.id}"
  http_method = "${aws_api_gateway_method.healthcheck_get.http_method}"
  type        = "MOCK"

  request_templates {
    "application/json" = <<EOF
{"statusCode":200}
EOF
  }
}

resource "aws_api_gateway_method_response" "healthcheck_get_response_200" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_resource.healthcheck_apigateway_resource.id}"
  http_method = "${aws_api_gateway_method.healthcheck_get.http_method}"
  status_code = "200"
}

resource "aws_api_gateway_integration_response" "healthcheck_get_integration_response" {
  rest_api_id = "${aws_api_gateway_rest_api.products_apigateway.id}"
  resource_id = "${aws_api_gateway_resource.healthcheck_apigateway_resource.id}"
  http_method = "${aws_api_gateway_method.healthcheck_get.http_method}"
  status_code = "${aws_api_gateway_method_response.healthcheck_get_response_200.status_code}"

  response_templates {
    "application/json" = <<EOF
{"alive":"true"}
EOF
  }
}

#output

output "base_url" {
  value = "${aws_api_gateway_deployment.products_apigateway_deployment.invoke_url}"
}

resource "aws_cloudwatch_log_group" "products_apigateway_log_group" {
  name              = "/shop/api/catalog/apigw${var.env_suffix}"
  retention_in_days = "7"
}

resource "aws_cloudwatch_log_stream" "products_apigateway_log_stream" {
  name           = "shop-catalog-api-apigw${var.env_suffix}"
  log_group_name = "${aws_cloudwatch_log_group.products_apigateway_log_group.name}"
}
