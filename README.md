### Carton Caps Referrals Microservice
This microservice handles the referral system for Carton Caps, allowing users to refer friends and earn rewards.

### Features

- User referral tracking first time
- Referral list management

## Authentication
This system asumes that user authentication is handled by a separate service and follows an architecture pattern API Gateway + Microservices. meaning that all requests to this service must be authenticated by the API Gateway before reaching this service.
The service expects an `X-User-ID` header in each request, which contains the unique identifier of the authenticated user.
The API Gateway is responsible for:

- Validating the user's authentication token
- Extracting the user ID 
- Forwarding the request to this service with the `X-User-ID` header set

Authentication and token validation are out of scope for this service.
