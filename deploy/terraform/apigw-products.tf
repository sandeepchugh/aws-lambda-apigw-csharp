
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

resource "aws_api_gateway_integration" "products_get_integration" {
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

  depends_on = ["aws_api_gateway_integration.products_get_integration"]
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
