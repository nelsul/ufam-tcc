# IcompCare App

Frontend application for the Academic Support System (Sistema de Apoio Acadêmico) of ICOMP/UFAM.

## Technologies

- **Vue 3** with TypeScript
- **Vite** for build tooling
- **Tailwind CSS** for styling
- **Vue Router** for navigation
- **Vue i18n** for internationalization

## Prerequisites

- Docker installed on your system
- API and database running (see `../api/README.md` and `../db/README.md`)

## Quick Start

### Build the app image

```bash
docker build -t icompcare-app .
```

### Run the app container

```bash
docker run -d \
  --name icompcare-app \
  -p 80:80 \
  icompcare-app
```

The application will be available at `http://localhost`.

### Backend URL configuration

Pass the backend URL at runtime (must include `/api`):

```bash
docker run -d \
  --name icompcare-app \
  -p 80:80 \
  -e VITE_API_BASE_URL=http://localhost:8080/api \
  icompcare-app
```

Examples for `VITE_API_BASE_URL`:
- Local host port mapping: `http://localhost:8080/api`
- Docker network service name: `http://icompcare-api:8080/api`
- Production: `https://api.example.com/api`

If not specified, defaults to `http://localhost:5133/api`.

### Running with custom port

```bash
docker run -d \
  --name icompcare-app \
  -p 3000:80 \
  icompcare-app
```

The application will be available at `http://localhost:3000`.

## Managing the App

### Stop the app

```bash
docker stop icompcare-app
```

### Start the app

```bash
docker start icompcare-app
```

### View logs

```bash
docker logs icompcare-app
```

### Remove the container

```bash
docker stop icompcare-app
docker rm icompcare-app
```

### Rebuild after code changes

```bash
docker stop icompcare-app
docker rm icompcare-app
docker build -t icompcare-app .
docker run -d \
  --name icompcare-app \
  -p 80:80 \
  icompcare-app
```

## Development

For local development without Docker:

### Project Setup

```bash
npm install
```

### Compile and Hot-Reload for Development

```bash
npm run dev
```

The development server will start at `http://localhost:5173`.

### Compile and Minify for Production

```bash
npm run build
```

### Lint and Format

```bash
npm run lint
npm run format
```

## Recommended IDE Setup

[VS Code](https://code.visualstudio.com/) + [Vue (Official)](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Recommended Browser Extensions

- Chromium-based browsers (Chrome, Edge, Brave, etc.):
  - [Vue.js devtools](https://chromewebstore.google.com/detail/vuejs-devtools/nhdogjmejiglipccpnnnanhbledajbpd)
  - [Turn on Custom Object Formatter in Chrome DevTools](http://bit.ly/object-formatters)
- Firefox:
  - [Vue.js devtools](https://addons.mozilla.org/en-US/firefox/addon/vue-js-devtools/)
  - [Turn on Custom Object Formatter in Firefox DevTools](https://fxdx.dev/firefox-devtools-custom-object-formatters/)

## Configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Connecting to the API

The app expects the API to be running. Update the API endpoint configuration in your environment or source code to point to the correct API URL (e.g., `http://localhost:8080`).
