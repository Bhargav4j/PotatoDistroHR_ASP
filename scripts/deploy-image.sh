#!/bin/bash
set -e
set -o pipefail

echo "=========================================="
echo "AWS ECS Fargate Deployment Script"
echo "=========================================="
echo ""

# Prompt for AWS configuration
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNETS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): " IMAGE_URI

# Parse subnets
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNETS"
SUBNET_1=${SUBNET_ARRAY[0]}
SUBNET_2=${SUBNET_ARRAY[1]:-$SUBNET_1}

echo ""
echo "--- Configuration Summary ---"
echo "Region: $AWS_REGION"
echo "Cluster: $CLUSTER_NAME"
echo "VPC: $VPC_ID"
echo "Subnets: $SUBNET_1, $SUBNET_2"
echo "Security Group: $SECURITY_GROUP"
echo "Image: $IMAGE_URI"
echo ""

# Get AWS Account ID
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "Account ID: $ACCOUNT_ID"
echo ""

# Check if ECS cluster exists, create if not
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters $CLUSTER_NAME --region $AWS_REGION >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name $CLUSTER_NAME --region $AWS_REGION
}
echo "Cluster ready: $CLUSTER_NAME"
echo ""

# Prompt for database configuration
echo "--- Database Configuration ---"
read -p "Enter database host: " DB_HOST
read -p "Enter database name: " DB_NAME
read -p "Enter database user: " DB_USER
read -sp "Enter database password: " DB_PASSWORD
echo ""

# Prompt for Redis configuration (optional)
echo "--- Redis Configuration (optional) ---"
read -p "Enter Redis connection string (leave empty to skip): " REDIS_CONNECTION_STRING
echo ""

# Ask about load balancer
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [[ "$NEED_LB" == "y" || "$NEED_LB" == "Y" ]]; then
    echo ""
    echo "Creating Application Load Balancer and Target Group..."
    
    # Create ALB
    ALB_NAME="potatodistrohr-alb"
    echo "Creating ALB: $ALB_NAME"
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name $ALB_NAME \
        --subnets $SUBNET_1 $SUBNET_2 \
        --security-groups $SECURITY_GROUP \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region $AWS_REGION \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || aws elbv2 describe-load-balancers --names $ALB_NAME --region $AWS_REGION --query 'LoadBalancers[0].LoadBalancerArn' --output text)
    
    echo "ALB ARN: $ALB_ARN"
    
    # Get ALB DNS name
    ALB_DNS=$(aws elbv2 describe-load-balancers --load-balancer-arns $ALB_ARN --region $AWS_REGION --query 'LoadBalancers[0].DNSName' --output text)
    echo "ALB DNS: $ALB_DNS"
    
    # Create Target Group with target-type ip (required for Fargate)
    TG_NAME="potatodistrohr-tg"
    echo "Creating Target Group: $TG_NAME"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name $TG_NAME \
        --protocol HTTP \
        --port 8080 \
        --vpc-id $VPC_ID \
        --target-type ip \
        --health-check-enabled \
        --health-check-protocol HTTP \
        --health-check-path /health \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region $AWS_REGION \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null || aws elbv2 describe-target-groups --names $TG_NAME --region $AWS_REGION --query 'TargetGroups[0].TargetGroupArn' --output text)
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create listener
    echo "Creating ALB listener..."
    aws elbv2 create-listener \
        --load-balancer-arn $ALB_ARN \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn=$TARGET_GROUP_ARN \
        --region $AWS_REGION >/dev/null 2>&1 || echo "Listener already exists"
    
    echo "Load balancer setup complete"
    echo ""
else
    TARGET_GROUP_ARN=""
    echo "Skipping load balancer configuration"
    echo ""
fi

# Replace placeholders in task definition
echo "Preparing ECS task definition..."
cp ecs/task-definition.json /tmp/task-definition.json
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" /tmp/task-definition.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" /tmp/task-definition.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" /tmp/task-definition.json
sed -i "s|{{DB_HOST}}|$DB_HOST|g" /tmp/task-definition.json
sed -i "s|{{DB_NAME}}|$DB_NAME|g" /tmp/task-definition.json
sed -i "s|{{DB_USER}}|$DB_USER|g" /tmp/task-definition.json
sed -i "s|{{DB_PASSWORD}}|$DB_PASSWORD|g" /tmp/task-definition.json
sed -i "s|{{REDIS_CONNECTION_STRING}}|$REDIS_CONNECTION_STRING|g" /tmp/task-definition.json

# Create CloudWatch log group
echo "Creating CloudWatch log group..."
aws logs create-log-group --log-group-name /ecs/potatodistrohr --region $AWS_REGION 2>/dev/null || echo "Log group already exists"

# Register task definition
echo "Registering ECS task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file:///tmp/task-definition.json \
    --region $AWS_REGION \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

echo "Task definition registered: $TASK_DEF_ARN"
echo ""

# Prepare service definition
echo "Preparing ECS service definition..."
cp ecs/service-definition.json /tmp/service-definition.json
sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" /tmp/service-definition.json
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" /tmp/service-definition.json
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" /tmp/service-definition.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" /tmp/service-definition.json

if [[ -n "$TARGET_GROUP_ARN" ]]; then
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" /tmp/service-definition.json
else
    # Remove loadBalancers section if no load balancer
    jq 'del(.loadBalancers, .healthCheckGracePeriodSeconds)' /tmp/service-definition.json > /tmp/service-definition-no-lb.json
    mv /tmp/service-definition-no-lb.json /tmp/service-definition.json
fi

# Check if service exists
SERVICE_NAME="potatodistrohr-service"
echo "Checking if ECS service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster $CLUSTER_NAME \
    --services $SERVICE_NAME \
    --region $AWS_REGION \
    --query 'services[0].serviceName' \
    --output text 2>/dev/null || echo "None")

if [[ "$EXISTING_SERVICE" == "None" || "$EXISTING_SERVICE" == "" ]]; then
    echo "Service does not exist. Creating new service: $SERVICE_NAME"
    aws ecs create-service \
        --cli-input-json file:///tmp/service-definition.json \
        --region $AWS_REGION
else
    echo "Service exists. Updating service: $SERVICE_NAME"
    aws ecs update-service \
        --cluster $CLUSTER_NAME \
        --service $SERVICE_NAME \
        --task-definition $TASK_DEF_ARN \
        --region $AWS_REGION
fi

echo ""
echo "Waiting for service to become stable..."
aws ecs wait services-stable \
    --cluster $CLUSTER_NAME \
    --services $SERVICE_NAME \
    --region $AWS_REGION

echo ""
echo "=========================================="
echo "Deployment completed successfully!"
echo "=========================================="
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Task Definition: $TASK_DEF_ARN"

if [[ -n "$ALB_DNS" ]]; then
    echo "Load Balancer DNS: http://$ALB_DNS"
fi

echo "CloudWatch Logs: /ecs/potatodistrohr"
echo ""
echo "To view service status:"
echo "  aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo ""
echo "To view logs:"
echo "  aws logs tail /ecs/potatodistrohr --follow --region $AWS_REGION"
echo "=========================================="
echo ""