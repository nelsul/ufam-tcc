# IcompCare Database

This directory contains the PostgreSQL database configuration for the IcompCare system.

## Prerequisites

- Docker installed on your system
- Docker Compose (optional, for easier management)

## Quick Start

### Option 1: Using Docker directly

Build the database image:

```bash
docker build -t icompcare-db .
```

Run the database container:

```bash
docker run -d \
  --name icompcare-db \
  -p 5432:5432 \
  -e POSTGRES_DB=icompcare \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -v "$(pwd)/data":/var/lib/postgresql/data \
  icompcare-db
```

This will persist the database data in the `./data` directory.

### Option 2: Using Docker Compose (from root directory)

If you have a `docker-compose.yml` in the root directory, simply run:

```bash
docker-compose up -d db
```

## Database Configuration

- **Database Name**: `icompcare`
- **User**: `postgres`
- **Password**: `postgres`
- **Port**: `5432`
- **Host**: `localhost` (when running locally) or `db` (when using Docker Compose)

## Connection String Examples

### .NET (Connection String)

```
Host=localhost;Port=5432;Database=icompcare;Username=postgres;Password=postgres
```

### Node.js/JavaScript

```javascript
{
  host: 'localhost',
  port: 5432,
  database: 'icompcare',
  user: 'postgres',
  password: 'postgres'
}
```

## Managing the Database

### Stop the database

```bash
docker stop icompcare-db
```

### Start the database

```bash
docker start icompcare-db
```

### View logs

```bash
docker logs icompcare-db
```

### Access PostgreSQL CLI

```bash
docker exec -it icompcare-db psql -U postgres -d icompcare
```

### Remove the container

```bash
docker stop icompcare-db
docker rm icompcare-db
```

### Remove the data directory (⚠️ This will delete all data)

```bash
rm -rf ./data
```

## Schema

The database schema is automatically initialized from `schema.sql` when the container is first created. The schema includes:

- User management (with roles: admin, professional, student, assistant, professor)
- Semesters
- And other tables for the IcompCare system

## Development

### Rebuilding after schema changes

If you modify `schema.sql`, you'll need to:

1. Stop and remove the existing container:

```bash
docker stop icompcare-db
docker rm icompcare-db
```

2. Remove the data directory (⚠️ this deletes all data):

```bash
rm -rf ./data
```

3. Rebuild and run:

```bash
docker build -t icompcare-db .
docker run -d \
  --name icompcare-db \
  -p 5432:5432 \
  -e POSTGRES_DB=icompcare \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -v "$(pwd)/data":/var/lib/postgresql/data \
  icompcare-db
```

## Security Notes

⚠️ **Important**: The default credentials (`postgres`/`postgres`) are for development only.

For production:

- Change the default password
- Use environment variables or secrets management
- Restrict network access
- Enable SSL/TLS connections
- Follow PostgreSQL security best practices

## Backup and Restore

### Create a backup

```bash
docker exec icompcare-db pg_dump -U postgres icompcare > backup.sql
```

### Restore from backup

```bash
docker exec -i icompcare-db psql -U postgres icompcare < backup.sql
```
