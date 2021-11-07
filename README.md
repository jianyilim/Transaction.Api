# Transaction.Api

Assumptions:
1) Transaction that already exists in database from the import file will have record updated instead of import unsuccessful.
2) No pagination for Get API.
3) CSV Amount should not have ','.
4) CSV file should not have "" as delimiter.

Solutions:
1) Transaction.Api - RESTful Api
2) Transaction.Domain - Domain layer. (Business logic)
3) Transaction.Infrastructure - Infrastructure layer. Input/output implementation.
4) Transaction.Domain.Test - Unit testing for domain layer.

