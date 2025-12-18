#!/bin/sh
set -e

# Generate env.js with runtime environment variables
cat > /usr/share/nginx/html/env.js <<EOF
window.__ENV__ = {
  VITE_API_BASE_URL: "${VITE_API_BASE_URL:-http://localhost:5133/api}"
};
EOF

echo "Generated env.js with VITE_API_BASE_URL=${VITE_API_BASE_URL:-http://localhost:5133/api}"

# Start nginx
exec nginx -g "daemon off;"
