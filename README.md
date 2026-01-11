## Carton Caps Referrals Microservice
## Overview

This microservice handles the referral system for Carton Caps, allowing users to refer friends and earn rewards.

## Architecture

This is an MVC .NET Core Web API application that follows a layered architecture pattern. The main layers are:

- **Controllers**: Handle HTTP requests and responses.
- **Services**: Contain business logic and interact with repositories.
- **Repositories**: Handle data access and interact with the database.
- **Models**: Define the data structures used in the application.

## Database

The service uses Entity Framework Core with a SQlite database to store referral data. The main entities are:

- **Referral**: Represents a referral made by a user, including status, tracking ID, and channel.
- **User**: Represents a user in the system, including referral code and other relevant information.

## Authentication

This system asumes that user authentication is handled by a separate service and follows an architecture pattern API Gateway + Microservices. meaning that all requests to this service must be authenticated by the API Gateway before reaching this service.
The service expects an `X-User-ID` header in each request, which contains the unique identifier of the authenticated user.
The API Gateway is responsible for:

- Validating the user's authentication token
- Extracting the user ID 
- Forwarding the request to this service with the `X-User-ID` header set

Authentication and token validation are out of scope for this service.
