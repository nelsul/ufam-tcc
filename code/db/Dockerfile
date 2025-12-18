FROM postgres:16-alpine

# Set environment variables
ENV POSTGRES_DB=icompcare
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=postgres

# Copy the schema file to the initialization directory
COPY schema.sql /docker-entrypoint-initdb.d/

# Expose PostgreSQL port
EXPOSE 5432

# The postgres image already has an entrypoint that initializes the database
# and runs any SQL files in /docker-entrypoint-initdb.d/
