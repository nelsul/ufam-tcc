FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

# Install Node.js, NPM, and common development tools
# git, openssh, bash are essential for VS Code Dev Containers
RUN apk add --no-cache \
    nodejs \
    npm \
    git \
    openssh \
    bash \
    curl \
    sudo \
    shadow

# Install LaTeX support
# texlive-full provides a complete TeX environment
# biber is for bibliography management
RUN apk add --no-cache \
    texlive-full \
    biber

# Create a non-root user 'vscode' with sudo access
# This is standard for Dev Containers to avoid running as root
RUN addgroup -g 1000 vscode \
    && adduser -u 1000 -G vscode -s /bin/bash -D vscode \
    && echo "vscode ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers

# Set the default user
USER vscode
