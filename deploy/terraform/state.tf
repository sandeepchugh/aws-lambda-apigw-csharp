terraform{
    backend "s3"{
        bucket = "shop-tf-state"
        dynamodb_table = "shop-tf-state-table"
        key = "shop-catalog-api-tf-np"
        region = "us-east-2"
        encrypt = "true"
    }
}