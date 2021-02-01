#Databse

- **Portfolio DB**
  - `Add-Migration InitialCreate -Context PortfolioContext`
  - `Update-Database -Context PortfolioContext`

- **Movie DB**
  - `Add-Migration InitialCreate -Context MovieContext`
  - `Update-Database`

- **Training DB**
  - `Add-Migration InitialCreate -Context TrainingContext`
  - `Update-Database`

- **Commands**
  - **`Add-Migration`** Adds a new migration.
    - `-Name {string}` The name of the migration. This is a positional parameter and is required.
  - **`Remove-Migration`** Removes the last migration (rolls back the code changes that were done for the migration).
    - `-Force`	Revert the migration (roll back the changes that were applied to the database).
  - **`Script-Migration`** Generates a SQL script that applies all of the changes from one selected migration to another selected migration.
    - `-From` The starting migration. Migrations may be identified by name or by ID. The number 0 is a special case that means before the first migration. Defaults to 0.
  - **`Update-Database`** Updates the database to the last migration or to a specified migration.
    - `-Migration {String}` The target migration. Migrations may be identified by name or by ID. The number 0 is a special case that means before the first migration and causes all migrations to be reverted. If no migration is specified, the command defaults to the last migration.
    - `-Connection {String}` The connection string to the database. Defaults to the one specified in AddDbContext or OnConfiguring.
  - `Drop-Database` Drops the database.
    - `-WhatIf` Show which database would be dropped, but don't drop it.