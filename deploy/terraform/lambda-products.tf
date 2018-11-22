resource "aws_lambda_function" "products_lambda" {
  function_name = "${local.products_lambda_name}${var.env_name}"
  handler = ""
  role = ""
  runtime = "dotnetcore2.1"  
}

resource "aws_iam_role" "lambda_exec" {
  name = "products_lambda_role"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

resource "aws_lambda_permission" "products_apigateway_permission" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.products_lambda.arn}"
  principal     = "apigateway.amazonaws.com"

  # The /*/* portion grants access from any method on any resource
  # within the API Gateway "REST API".
  source_arn = "${aws_api_gateway_deployment.products_apigateway_deployment.execution_arn}/*/*"
}