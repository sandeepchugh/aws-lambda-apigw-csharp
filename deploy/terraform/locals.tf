locals {
  application_name          = "shop-catalog-api"
  products_table_name       = "products${var.env_suffix}"
  products_lambda_name      = "products${var.env_suffix}"
  products_apigateway_name  = "products${var.env_suffix}"
}
