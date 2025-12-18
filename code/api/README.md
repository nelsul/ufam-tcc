# IcompCare API

Backend service for the Academic Support System (Sistema de Apoio Acadêmico) of ICOMP/UFAM.

## Technologies

- **.NET 8**
- **PostgreSQL**
- **Swagger / OpenAPI** for documentation

## Project Structure

The solution follows a Clean Architecture approach:

- **`src/IcompCare.Api`**: The entry point, API controllers, and configuration.
- **`src/IcompCare.Application`**: Business logic, services, and use cases.
- **`src/IcompCare.Domain`**: Core entities, interfaces, and business rules.
- **`src/IcompCare.Infrastructure`**: Data access, database context, and external services implementation.

## Prerequisites

- Docker installed on your system
- PostgreSQL database running (see `../db/README.md`)

## Quick Start

### Build the API image

```bash
docker build -t icompcare-api .
```

### Run the API container

```bash
docker run -d \
  --name icompcare-api \
  -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Database=icompcare;Username=postgres;Password=postgres" \
  icompcare-api
```

The API will be available at `http://localhost:8080`.

### Environment Variables

You can configure the API using environment variables:

**Database:**
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string

**JWT Authentication:**
- `JwtSettings__SecretKey`: JWT secret key for authentication (min 32 characters)
- `JwtSettings__ExpirationHours`: JWT token expiration time in hours

**Email (SMTP):**
- `Email__SmtpHost`: SMTP server hostname (e.g., smtp.gmail.com)
- `Email__SmtpPort`: SMTP server port (e.g., 587)
- `Email__SmtpUsername`: SMTP authentication username
- `Email__SmtpPassword`: SMTP authentication password
- `Email__FromEmail`: Sender email address
- `Email__FromName`: Sender display name
- `Email__EnableSsl`: Enable SSL/TLS (true/false)
- `Email__MaxRetryAttempts`: Maximum retry attempts for failed emails (default: 3)
- `Email__RetryDelaySeconds`: Delay between retries in seconds (default: 30)

### Connecting to the Database

If you're running both the API and database in Docker:

1. Create a Docker network:

   ```bash
   docker network create icompcare-network
   ```

2. Run the database (from `../db` directory):

   ```bash
   docker run -d \
     --name icompcare-db \
     --network icompcare-network \
     -p 5432:5432 \
     -v "$(pwd)/data":/var/lib/postgresql/data \
     icompcare-db
   ```

3. Run the API:
   ```bash
   docker run -d \
     --name icompcare-api \
     --network icompcare-network \
     --dns 8.8.8.8 \
     --dns 8.8.4.4 \
     -p 8080:8080 \
     -e "ConnectionStrings__DefaultConnection=Host=icompcare-db;Database=icompcare;Username=postgres;Password=postgres" \
     -e "JwtSettings__SecretKey=super_secret_key_that_should_be_long_enough_for_hmac_sha256" \
     -e "JwtSettings__ExpirationHours=8" \
     -e "Email__SmtpHost=smtp.gmail.com" \
     -e "Email__SmtpPort=587" \
     -e "Email__SmtpUsername=your-email@gmail.com" \
     -e "Email__SmtpPassword=your-app-password" \
     -e "Email__FromEmail=your-email@gmail.com" \
     -e "Email__FromName=IcompCare" \
     -e "Email__EnableSsl=true" \
     icompcare-api
   ```

## Managing the API

### Stop the API

```bash
docker stop icompcare-api
```

### Start the API

```bash
docker start icompcare-api
```

### View logs

```bash
docker logs icompcare-api
```

### Follow logs in real-time

```bash
docker logs -f icompcare-api
```

### Remove the container

```bash
docker stop icompcare-api
docker rm icompcare-api
```

### Rebuild after code changes

```bash
docker stop icompcare-api
docker rm icompcare-api
docker build -t icompcare-api .
docker run -d \
  --name icompcare-api \
  --network icompcare-network \
  --dns 8.8.8.8 \
  --dns 8.8.4.4 \
  -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Host=icompcare-db;Database=icompcare;Username=postgres;Password=postgres" \
  -e "JwtSettings__SecretKey=super_secret_key_that_should_be_long_enough_for_hmac_sha256" \
  -e "JwtSettings__ExpirationHours=8" \
  -e "Email__SmtpHost=smtp.gmail.com" \
  -e "Email__SmtpPort=587" \
  -e "Email__SmtpUsername=your-email@gmail.com" \
  -e "Email__SmtpPassword=your-app-password" \
  -e "Email__FromEmail=your-email@gmail.com" \
  -e "Email__FromName=IcompCare" \
  -e "Email__EnableSsl=true" \
  icompcare-api
```

## API Documentation

- **Swagger UI**: Interactive API documentation is available at `http://localhost:8080/swagger`
- **Health Check**: A health check endpoint is available at `http://localhost:8080/health` to verify the service status

## Development

For local development without Docker:

1. Ensure you have [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
2. Navigate to the API project:
   ```bash
   cd src/IcompCare.Api
   ```
3. Update the connection string in `appsettings.Development.json` to point to your local database
4. Run the application:
   ```bash
   dotnet run
   ```

## Security Notes

⚠️ **Important**: The default credentials and JWT secret key are for development only.

For production:

- Use strong, unique passwords for database connections
- Generate a secure JWT secret key (at least 32 characters)
- Use environment variables or secrets management
- Enable HTTPS/TLS
- Follow .NET security best practices
