resource "aws_dynamodb_table" "products_table" {
  name           = "${local.products_table_name}"
  read_capacity  = 20
  write_capacity = 20
  hash_key       = "ProductId"

  attribute {
    name = "ProductId"
    type = "S"
  }

  ttl {
    attribute_name = "TimeToExist"
    enabled        = false
  }

  tags {
    Name        = "${local.products_table_name}"
    Environment = "${var.env_name}"
    Application = "${local.application_name}"
  }
}
