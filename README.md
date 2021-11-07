# Transaction.Api

Assumptions:
1) Transaction that already exists in database from the import file will have record updated instead of import unsuccessful.
2) No pagination for Get API.
3) CSV Amount should not have ','.
4) CSV file should not have "" as delimiter.

Solutions:
Transaction.Api - RESTful Api
Transaction.Domain - Domain layer. (Business logic)
Transaction.Infrastructure - Infrastructure layer. Input/output implementation.
Transaction.Domain.Test - Unit testing for domain layer.

