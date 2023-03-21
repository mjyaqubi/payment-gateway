# payment-gateway

## Introduction
Show case for a payment gateway that contain two project as listed below:

- Mock Acquiring Bank API
  - Payment Process 
- Payment Gateway API
  - Payment Process
  - Payment History

The payment gateway service use an PaymentGatewayID to indicate the client at the Acquiring Bank side, the ID pass by URL path.

## Run
To run the project end to end you need to run the Acquiring Bank API and Payment Gateway API together, if the Acquiring Bank API running port was different to the `appsettings.json` file then update it.

## Database
The database configuration is in the `appsettings.json`.

## API documentation
API documentation provided by swagger under `/swagger` path.

## Data validation


| Variable         | Validation rules  
| ---------------- | ----------------
| PaymentGatewayID | GUID
| MerchantID       | GUID
| MerchantOrderID  | GUID
| CardNumber       | string (16 digit number)
| ExpiryMonth      | string (2 digit number between 01 to 12)
| ExpiryYear       | string (2 digit number)
| CVV              | string (3 digit number)
| Currency         | string (3 character)
| Amount           | Decimal, Range between 0.01 to 999999999.99


## Tests
The solution contains two test project for Acquiring bank API and the Payment Gateway API.