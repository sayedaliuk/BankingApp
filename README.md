## Installation

Entity Framework core code first approach is used to generate the database. In an ideal world, with huge volume of data we can use DACPAC project to managed the db and seed data.

Pantheon.Banking.Data> SeedDataScripts contains seed data scripts. Run the scripts in asc order once the database is created by running the ef command (update-database).

## High-level architecture

Generic repository layer is implemented for CRUD operations.
RefDataRepostiory is implemented to get the reference/static data.

Service layer contains the business logic and consumes the generic repositry and the RefDataRepository.

The UI project consists of the Web api controller and the Dto's, The controller makes call to the service layer.

Given the scope of the project, Authentication/authorisation has not been implemented. 

Integration and Unit tests are written for the core functionality. 

## Swagger Endpoint

https://localhost:5001/swagger/index.html








