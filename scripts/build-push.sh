#!/bin/bash
set -e

echo "=========================================="
echo "Docker Build and Push Script"
echo "=========================================="
echo ""

# Project name
PROJECT_NAME="PotatoDistroHR"

# Sanitize project name for Docker tag
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "Project: $PROJECT_NAME"
echo "Sanitized image name: $IMAGE_NAME"
echo ""

# Prompt for image tag
read -p "Enter image tag (default: latest): " IMAGE_TAG
IMAGE_TAG=${IMAGE_TAG:-latest}

# Sanitize tag
IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
IMAGE_TAG=${IMAGE_TAG:-latest}

echo "Using tag: $IMAGE_TAG"
echo ""

# Select registry type
echo "Select registry type:"
echo "1. AWS ECR"
echo "2. Docker Hub"
read -p "Enter choice (1 or 2): " REGISTRY_CHOICE

if [ "$REGISTRY_CHOICE" = "1" ]; then
    echo ""
    echo "--- AWS ECR Configuration ---"
    read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
    read -p "Enter AWS account ID: " AWS_ACCOUNT_ID
    read -p "Enter ECR repository name (default: $IMAGE_NAME): " ECR_REPO
    ECR_REPO=${ECR_REPO:-$IMAGE_NAME}
    
    REGISTRY_URL="$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com"
    FULL_IMAGE_NAME="$REGISTRY_URL/$ECR_REPO:$IMAGE_TAG"
    
    echo ""
    echo "Logging in to AWS ECR..."
    aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $REGISTRY_URL
    
    if [ $? -ne 0 ]; then
        echo "Error: ECR login failed"
        exit 1
    fi
    
    echo "Checking if ECR repository exists..."
    aws ecr describe-repositories --repository-names $ECR_REPO --region $AWS_REGION >/dev/null 2>&1 || {
        echo "Repository does not exist. Creating ECR repository: $ECR_REPO"
        aws ecr create-repository --repository-name $ECR_REPO --region $AWS_REGION
    }
    
elif [ "$REGISTRY_CHOICE" = "2" ]; then
    echo ""
    echo "--- Docker Hub Configuration ---"
    read -p "Enter Docker Hub username: " DOCKER_USERNAME
    read -sp "Enter Docker Hub password or token: " DOCKER_PASSWORD
    echo ""
    
    FULL_IMAGE_NAME="$DOCKER_USERNAME/$IMAGE_NAME:$IMAGE_TAG"
    
    echo "Logging in to Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username $DOCKER_USERNAME --password-stdin
    
    if [ $? -ne 0 ]; then
        echo "Error: Docker Hub login failed"
        exit 1
    fi
    
else
    echo "Invalid choice. Exiting."
    exit 1
fi

echo ""
echo "=========================================="
echo "Building Docker image: $FULL_IMAGE_NAME"
echo "=========================================="
echo ""

# Build Docker image from repository root
docker build -f Dockerfile -t $FULL_IMAGE_NAME .

if [ $? -ne 0 ]; then
    echo "Error: Docker build failed"
    exit 1
fi

echo ""
echo "=========================================="
echo "Pushing image to registry: $FULL_IMAGE_NAME"
echo "=========================================="
echo ""

docker push $FULL_IMAGE_NAME

if [ $? -ne 0 ]; then
    echo "Error: Docker push failed"
    exit 1
fi

echo ""
echo "=========================================="
echo "Build and push completed successfully!"
echo "Image: $FULL_IMAGE_NAME"
echo "=========================================="
echo ""