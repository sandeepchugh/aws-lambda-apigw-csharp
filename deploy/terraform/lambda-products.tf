resource "aws_lambda_function" "products_lambda" {
  function_name    = "products-lambda${var.env_suffix}"
  handler          = "Shop.Catalog.Api::Shop.Catalog.Api.Functions::GetProductsAsync"
  role             = "${aws_iam_role.products_lambda_role.arn}"
  runtime          = "dotnetcore2.1"
  memory_size      = "512"
  timeout          = "30"
  filename         = "Shop.Catalog.Api.zip"
  source_code_hash = "${base64sha256(file("Shop.Catalog.Api.zip"))}"

  environment {
    variables = {
      LogLevel = "Information"
      Region   = "${data.aws_region.current.name}"
    }
  }

  tags {
    Name        = "products-lambda${var.env_suffix}"
    Environment = "${var.env_name}"
    Application = "${local.application_name}"
  }
}

resource "aws_lambda_permission" "products_apigateway_permission" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.products_lambda.function_name}"
  principal     = "apigateway.amazonaws.com"

  # The /*/* portion grants access from any method on any resource
  # within the API Gateway "REST API".
  source_arn = "${aws_api_gateway_deployment.products_apigateway_deployment.execution_arn}/*/*"
}

resource "aws_lambda_permission" "products_lambda_cloudwatch_permission" {
  statement_id  = "AllowCloudwatchInvoke"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.products_lambda.function_name}"
  principal     = "events.amazonaws.com"
  source_arn = "arn:aws:events:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:/*/*"
}

resource "aws_cloudwatch_log_group" "products_lambda_log_group" {
  name              = "/shop/api/catalog/lambda${var.env_suffix}"
  retention_in_days = "7"
}

resource "aws_cloudwatch_log_stream" "products_lambda_log_stream" {
  name           = "shop-catalog-api-lambda${var.env_suffix}"
  log_group_name = "${aws_cloudwatch_log_group.products_lambda_log_group.name}"
}
