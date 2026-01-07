# PotatoDistroHR Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Docker Deployment](#docker-deployment)
4. [AWS ECS Fargate Deployment](#aws-ecs-fargate-deployment)
5. [Configuration Management](#configuration-management)
6. [Monitoring and Logging](#monitoring-and-logging)
7. [Troubleshooting](#troubleshooting)
8. [Security Considerations](#security-considerations)

---

## Prerequisites

### Required Tools
- **.NET 8.0 SDK** - Download from https://dotnet.microsoft.com/download/dotnet/8.0
- **Docker** - Version 20.10 or later
- **AWS CLI** - Version 2.x (for ECS deployment)
- **Git** - For source code management

### AWS Requirements (for ECS Fargate)
- Active AWS account
- AWS CLI configured with appropriate credentials
- IAM permissions for:
  - ECS (task definitions, services, clusters)
  - ECR (container registry)
  - CloudWatch Logs
  - VPC, subnets, and security groups
  - IAM roles (ecsTaskExecutionRole, ecsTaskRole)

### External Services
- **PostgreSQL Database** - Version 12 or later
- **Redis Cache** - Version 6 or later (optional, falls back to in-memory cache)

---

## Local Development Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd potato
```

### 2. Configure Application Settings
Create `src/PotatoDistroHR.Web/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=potatodistrohr;Username=postgres;Password=postgres"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### 3. Run Database Migrations
```bash
cd src/PotatoDistroHR.Web
dotnet ef database update
```

### 4. Run the Application
```bash
dotnet run --project src/PotatoDistroHR.Web
```

The application will be available at `http://localhost:5000`

---

## Docker Deployment

### Build Docker Image Locally
```bash
# Build the image
docker build -t potatodistrohr-web:latest .

# Run the container
docker run -d \
  -p 8080:8080 \
  -e DB_HOST=host.docker.internal \
  -e DB_NAME=potatodistrohr \
  -e DB_USER=postgres \
  -e DB_PASSWORD=postgres \
  --name potatodistrohr \
  potatodistrohr-web:latest
```

### Using Docker Compose
```bash
# Start the application
docker-compose up -d

# View logs
docker-compose logs -f

# Stop the application
docker-compose down
```

### Health Check
Verify the application is running:
```bash
curl http://localhost:8080/health
```

---

## AWS ECS Fargate Deployment

### Architecture Overview
The application runs on AWS ECS Fargate with:
- **Compute**: Fargate tasks (serverless containers)
- **Networking**: VPC with public/private subnets
- **Load Balancing**: Application Load Balancer (ALB)
- **Logging**: CloudWatch Logs
- **Container Registry**: Amazon ECR

### Step 1: AWS Prerequisites

#### 1.1 Create VPC and Networking (if not exists)
```bash
# Create VPC
aws ec2 create-vpc --cidr-block 10.0.0.0/16

# Create subnets in different availability zones
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.1.0/24 --availability-zone us-east-1a
aws ec2 create-subnet --vpc-id <vpc-id> --cidr-block 10.0.2.0/24 --availability-zone us-east-1b

# Create internet gateway
aws ec2 create-internet-gateway
aws ec2 attach-internet-gateway --vpc-id <vpc-id> --internet-gateway-id <igw-id>
```

#### 1.2 Create Security Group
```bash
aws ec2 create-security-group \
  --group-name potatodistrohr-sg \
  --description "Security group for PotatoDistroHR" \
  --vpc-id <vpc-id>

# Allow inbound HTTP traffic
aws ec2 authorize-security-group-ingress \
  --group-id <sg-id> \
  --protocol tcp \
  --port 8080 \
  --cidr 0.0.0.0/0
```

#### 1.3 Create IAM Roles

**ECS Task Execution Role** (required for Fargate):
```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

**ECS Task Role** (optional, for application permissions):
```bash
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'
```

### Step 2: Build and Push Docker Image

#### Using the Build Script
```bash
# Make script executable
chmod +x scripts/build-push.sh

# Run the script
./scripts/build-push.sh
```

The script will prompt for:
- Registry type (AWS ECR or Docker Hub)
- AWS region (if ECR)
- Repository name
- Image tag

It will automatically:
- Create ECR repository if it doesn't exist
- Build the Docker image
- Push to the selected registry

### Step 3: Deploy to ECS Fargate

#### Using the Deployment Script
```bash
# Make script executable
chmod +x scripts/deploy-image.sh

# Run the script
./scripts/deploy-image.sh
```

The script will prompt for:
- AWS region
- ECS cluster name
- VPC ID
- Subnet IDs
- Security group ID
- Docker image URI
- Database configuration
- Redis configuration (optional)
- Load balancer setup (y/n)

The script will automatically:
- Create ECS cluster if it doesn't exist
- Create Application Load Balancer and Target Group (if requested)
- Register task definition with proper Fargate configuration
- Create or update ECS service
- Wait for service to become stable

### Step 4: Verify Deployment

#### Check Service Status
```bash
aws ecs describe-services \
  --cluster <cluster-name> \
  --services potatodistrohr-service \
  --region <region>
```

#### View Running Tasks
```bash
aws ecs list-tasks \
  --cluster <cluster-name> \
  --service-name potatodistrohr-service \
  --region <region>
```

#### Test the Application
If using a load balancer:
```bash
curl http://<alb-dns-name>/health
```

Or get the task's public IP:
```bash
aws ecs describe-tasks \
  --cluster <cluster-name> \
  --tasks <task-id> \
  --region <region> \
  --query 'tasks[0].attachments[0].details[?name==`networkInterfaceId`].value' \
  --output text
```

---

## ECS Fargate Configuration Details

### Task Definition
The task definition (`ecs/task-definition.json`) includes:

- **Launch Type**: FARGATE
- **Network Mode**: awsvpc (required for Fargate)
- **CPU**: 512 (.5 vCPU)
- **Memory**: 1024 MB (1 GB)
- **Execution Role**: Required for pulling images from ECR and writing logs
- **Task Role**: Optional, for application-specific AWS permissions

**Valid Fargate CPU/Memory Combinations**:
| CPU (units) | Memory (MB) |
|-------------|-------------|
| 256 | 512, 1024, 2048 |
| 512 | 1024, 2048, 3072, 4096 |
| 1024 | 2048-8192 (increments of 1024) |
| 2048 | 4096-16384 (increments of 1024) |
| 4096 | 8192-30720 (increments of 1024) |

### Container Definition
- **Port Mappings**: Only containerPort is specified (no hostPort for Fargate)
- **Environment Variables**: Application configuration
- **Logging**: CloudWatch Logs with awslogs driver
- **Health Check**: Uses application's `/health` endpoint

### Service Configuration
The service definition (`ecs/service-definition.json`) includes:

- **Launch Type**: FARGATE
- **Desired Count**: 2 (for high availability)
- **Network Configuration**: awsvpc with subnets and security groups
- **Load Balancer**: Optional ALB integration
- **Deployment Configuration**:
  - Maximum: 200% (allows rolling updates)
  - Minimum Healthy: 50%
  - Circuit Breaker: Enabled with automatic rollback

---

## Configuration Management

### Environment Variables
The application uses the following environment variables:

| Variable | Description | Required |
|----------|-------------|----------|
| `ASPNETCORE_ENVIRONMENT` | Application environment (Development/Production) | Yes |
| `ASPNETCORE_URLS` | URL bindings for Kestrel | Yes |
| `DB_HOST` | PostgreSQL host | Yes |
| `DB_NAME` | Database name | Yes |
| `DB_USER` | Database username | Yes |
| `DB_PASSWORD` | Database password | Yes |
| `REDIS_CONNECTION_STRING` | Redis connection string | No |

### Secrets Management
For production deployments, use AWS Secrets Manager:

1. Create secrets in AWS Secrets Manager
2. Update task definition to reference secrets:
```json
"secrets": [
  {
    "name": "DB_PASSWORD",
    "valueFrom": "arn:aws:secretsmanager:region:account:secret:db-password"
  }
]
```

---

## Monitoring and Logging

### CloudWatch Logs
Application logs are sent to CloudWatch Logs:
- **Log Group**: `/ecs/potatodistrohr`
- **Log Stream**: `ecs/<task-id>`

#### View Logs
```bash
# Tail logs
aws logs tail /ecs/potatodistrohr --follow --region <region>

# Filter logs
aws logs filter-log-events \
  --log-group-name /ecs/potatodistrohr \
  --filter-pattern "ERROR" \
  --region <region>
```

### Health Checks
The application exposes multiple health check endpoints:
- `/health` - General health status
- `/health/live` - Liveness probe
- `/health/ready` - Readiness probe

### Metrics
ECS automatically provides metrics in CloudWatch:
- CPU utilization
- Memory utilization
- Network I/O
- Task count

---

## Troubleshooting

### Common Issues

#### Task Fails to Start
**Symptoms**: Tasks immediately stop after starting

**Solutions**:
1. Check CloudWatch logs for application errors
2. Verify environment variables are correctly set
3. Ensure database connectivity
4. Check IAM role permissions

```bash
# View task stopped reason
aws ecs describe-tasks \
  --cluster <cluster-name> \
  --tasks <task-id> \
  --query 'tasks[0].stoppedReason'
```

#### Network Connectivity Issues
**Symptoms**: Cannot connect to database or Redis

**Solutions**:
1. Verify security group rules allow outbound traffic
2. Check subnet route tables have internet gateway
3. Ensure database security group allows inbound from ECS tasks
4. Verify assignPublicIp is ENABLED if accessing public endpoints

#### Health Check Failures
**Symptoms**: Tasks are marked as unhealthy

**Solutions**:
1. Increase `healthCheckGracePeriodSeconds` in service definition
2. Verify application is listening on port 8080
3. Check `/health` endpoint returns 200 OK
4. Review application startup logs

#### Invalid CPU/Memory Configuration
**Symptoms**: "Invalid CPU or memory value specified"

**Solutions**:
- Use valid Fargate CPU/memory combinations (see table above)
- Default safe values: cpu: "512", memory: "1024"

#### Image Pull Errors
**Symptoms**: "CannotPullContainerError"

**Solutions**:
1. Verify executionRoleArn has ECR permissions
2. Check image URI is correct
3. Ensure image exists in ECR
4. Verify ECR repository permissions

### Debugging Commands

```bash
# Get task details
aws ecs describe-tasks --cluster <cluster> --tasks <task-id>

# Get service events
aws ecs describe-services --cluster <cluster> --services <service> --query 'services[0].events'

# Check task ENI details
aws ecs describe-tasks --cluster <cluster> --tasks <task-id> \
  --query 'tasks[0].attachments[0].details'

# View application logs
aws logs tail /ecs/potatodistrohr --follow
```

---

## ECS Fargate Scaling and Management

### Manual Scaling
```bash
aws ecs update-service \
  --cluster <cluster-name> \
  --service potatodistrohr-service \
  --desired-count 3
```

### Auto Scaling
Configure Service Auto Scaling:

1. Register scalable target:
```bash
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/<cluster-name>/potatodistrohr-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10
```

2. Create scaling policy (target tracking):
```bash
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/<cluster-name>/potatodistrohr-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-target-tracking \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json
```

`scaling-policy.json`:
```json
{
  "TargetValue": 70.0,
  "PredefinedMetricSpecification": {
    "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
  },
  "ScaleOutCooldown": 60,
  "ScaleInCooldown": 300
}
```

### Rolling Updates
Update the service with a new task definition:
```bash
# Register new task definition
TASK_DEF_ARN=$(aws ecs register-task-definition --cli-input-json file://ecs/task-definition.json --query 'taskDefinition.taskDefinitionArn' --output text)

# Update service
aws ecs update-service \
  --cluster <cluster-name> \
  --service potatodistrohr-service \
  --task-definition $TASK_DEF_ARN
```

### Blue/Green Deployment
For zero-downtime deployments, use AWS CodeDeploy with ECS:
1. Configure CodeDeploy application and deployment group
2. Create appspec.yaml for ECS
3. Use CodeDeploy to manage traffic shifting

---

## Security Considerations

### Application Security
1. **Non-root User**: Dockerfile uses non-root user for runtime
2. **HTTPS**: Configure SSL/TLS certificates in ALB
3. **Secrets**: Use AWS Secrets Manager for sensitive data
4. **Security Groups**: Restrict inbound traffic to necessary ports only

### Network Security
1. Use private subnets for tasks when possible
2. Use NAT Gateway for outbound internet access from private subnets
3. Restrict security group rules to minimum required access
4. Enable VPC Flow Logs for network monitoring

### IAM Security
1. Use least privilege principle for IAM roles
2. Separate execution role from task role
3. Regularly audit IAM permissions
4. Enable CloudTrail for API auditing

### Container Security
1. Scan images for vulnerabilities (ECR image scanning)
2. Use official Microsoft base images
3. Keep base images and dependencies updated
4. Implement runtime security monitoring

---

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [AWS ECS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Serilog Documentation](https://serilog.net/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

---

## Support

For issues or questions:
1. Check application logs in CloudWatch
2. Review this deployment guide
3. Consult AWS ECS documentation
4. Contact your infrastructure team
