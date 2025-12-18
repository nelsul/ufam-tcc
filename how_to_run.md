# How to Run

First, create a network for db and api

```bash
docker network create icompcare-network
```

Build the database image:

```bash
docker build -t icompcare-db .
```

Run the database container:

```bash
docker run -d \
  --name icompcare-db \
  --network icompcare-network \
  -p 5416:5432 \
  -e POSTGRES_DB=icompcare \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -v "./data":/var/lib/postgresql/data \
  icompcare-db
```

Then run the script inside `code/db/schema.sql`

Build the API image

```bash
docker build -t icompcare-api .
```

Run the API container

```bash
docker run -d \
  --name icompcare-api \
  --network icompcare-network \
  --dns 8.8.8.8 \
  --dns 8.8.4.4 \
  -p 8016:8080 \
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

Build the APP image

```bash
docker build -t icompcare-app .
```

Run the APP container with backend URL

```bash
docker run -d \
  --name icompcare-app \
  --network icompcare-network \
  -p 3016:80 \
  -e VITE_API_BASE_URL=http://localhost:8016/api \
  icompcare-app
```
