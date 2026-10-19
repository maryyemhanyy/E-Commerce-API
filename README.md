# E-Commerce API

An e-commerce REST API built with ASP.NET Core 8, featuring authentication, product browsing, shopping baskets, order management, and Stripe payment integration.

## Technologies

- C# and ASP.NET Core 8
- Entity Framework Core 8
- ASP.NET Core Identity and JWT authentication
- SQL Server
- Redis
- Stripe
- Swagger / OpenAPI
- AutoMapper

## Project Structure

- **E-Commerce** — API controllers, configuration, and application startup.
- **E-Commerce.Application** — Application services, DTOs, contracts, and specifications.
- **E-Commerce.Domain** — Domain entities and repository contracts.
- **E-Commerec.Infrastructure** — Database access, migrations, repositories, and payment integration.

## Features

- User registration and login.
- JWT-based authentication.
- Product filtering, sorting, and pagination.
- Shopping basket management.
- Order creation and retrieval.
- Stripe payment intent creation and updates.
- Redis caching.
- Interactive API documentation with Swagger.

## Prerequisites

- .NET 8 SDK
- SQL Server
- Redis
- Stripe test credentials for payment functionality

## Local Setup

### 1. Clone the repository

```bash
git clone https://github.com/maryyemhanyy/E-Commerce-API.git
cd E-Commerce-API
```

### 2. Configure local settings

In Visual Studio, right-click the API project and select **Manage User Secrets**.

Add the following configuration and replace the placeholder values on your machine:

```json
{
  "ConnectionStrings": {
    "StoreDbContext": "<store-database-connection-string>",
    "IdentityConnection": "<identity-database-connection-string>",
    "RedisConnection": "localhost:6379"
  },
  "UrlSettings": {
    "BaseUrl": "<local-api-base-url>"
  },
  "JWT": {
    "SecretKey": "<strong-random-signing-key>",
    "Issuer": "<jwt-issuer>",
    "Audience": "<jwt-audience>"
  },
  "Stripe": {
    "SecretKey": "<stripe-test-secret-key>",
    "WebhookSecret": "<stripe-test-webhook-secret>"
  }
}
```

Keep real credentials out of source control. User Secrets are intended for local development; configure production secrets through your hosting environment or a dedicated secret store.

### 3. Optional development admin

To let the identity seeder create a local test admin, add this section to User Secrets:

```json
{
  "SeedAdmin": {
    "Email": "<local-test-admin-email>",
    "Password": "<strong-local-test-password>",
    "DisplayName": "Test Admin"
  }
}
```

Merge this section into the existing JSON object. Test admin creation runs only in the Development environment and does not update an existing account's password.

### 4. Restore and build

```bash
dotnet restore
dotnet build
```

### 5. Run the API

Start SQL Server and Redis, then run the API project from Visual Studio using a Development launch profile.

Database migrations are applied during startup by the configured seeders. The database user needs the necessary permissions.

Use the local URL shown when the application starts. Swagger is available at `/swagger` when enabled in Development.

## Security

- Real credentials are not included in the repository.
- Local secrets are configured through User Secrets.
- Build output and local IDE files are excluded through `.gitignore`.
- Use Stripe test mode when testing payment functionality.