# Carton Caps Referrals Microservice

Mock REST API service for the Carton Caps referral feature.

## Overview

This microservice handles the referral system for Carton Caps, allowing users to:

- Create trackable referral links for sharing
- Validate referral tracking IDs from deep links
- View their referral history with status filtering

## Architecture

This is an MVC .NET Core Web API application that follows a layered architecture pattern. The main layers are:

- **Controllers**: Handle HTTP requests and responses.
- **Services**: Contain business logic and interact with repositories.
- **Repositories**: Handle data access and interact with the database.
- **Models**: Define the data structures used in the application.

## Database

The service uses Entity Framework Core with a SQlite database to store referral data. The main entities are:

### Main Entities

#### User

- `Id` (Guid, PK): User unique identifier
- `Email` (string): User email address
- `FirstName` (string): User first name
- `LastName` (string): User last name
- `ReferralCode` (string): User's unique referral code (e.g., "JOHN123")
- `CreatedAt` (DateTime): Account creation timestamp

#### Referral

- `Id` (Guid, PK): Referral unique identifier
- `TrackingId` (string): Unique tracking identifier (e.g., "trk_1736553600_a1b2c3d4")
- `ReferrerCode` (string): Referrer's code
- `Channel` (string): Share channel ("sms", "email", "generic")
- `ReferrerUserId` (Guid, FK): User who created the referral
- `RefereeUserId` (Guid, FK, nullable): User who was referred (null until registration)
- `RefereeName` (string, nullable): Name of referred user (set upon completion)
- `Status` (enum): PENDING, COMPLETED, or EXPIRED
- `CreatedAt` (DateTime): Referral creation timestamp
- `CompletedAt` (DateTime, nullable): Completion timestamp
- `ExpiresAt` (DateTime): Expiration timestamp (7 days from creation)

## API Endpoints

### 1. Create Referral Link
**POST** `/api/v1/referrals`

Creates a new tracking link for sharing. Returns pre-populated templates for SMS and Email.

**Authentication**: Required (`X-User-Id` header)

**Request Body**:
```json
{
  "channel": "sms",  // optional: "sms", "email", "generic"
  "referrerCode": "ANDRES123" // Referral code send by the Mobile App
}
```

**Response** (201 Created):
```json
{
  "trackingId": "trk_1768249757_09e0d275",
  "shareUrl": "https://mockdeeplink.com/referral/trk_1768249757_09e0d275",
  "expiresAt": "2026-01-19T20:29:17.600399Z",
  "createdAt": "2026-01-12T20:29:17.600329Z",
  "shareContent": {
    "sms": {
      "type": "sms",
      "body": "Hi! Join me in earning money for our school by using the Carton Caps app. It's an easy way to make a difference. Use the link below to download the Carton Caps app: https://mockdeeplink.com/referral/trk_1768249757_09e0d275 (referral_code=ANDRES123)"
    },
    "email": {
      "type": "email",
      "subject": "You're invited to try the Carton Caps app!",
      "body": "Hey!\n\nJoin me in earning cash for our school by using the Carton Caps app. It's an easy way to make a difference. All you have to do is buy Carton Caps participating products (like Cheerios!) and scan your grocery receipt. Carton Caps are worth $.10 each and they add up fast! Twice a year, our school receives a check to help pay for whatever we need - equipment, supplies, or experiences the kids love!\n\nDownload the Carton Caps app here: https://mockdeeplink.com/referral/trk_1768249757_09e0d275\n(referral_code=ANDRES123)"
    }
  }
}
```

### 2. List User Referrals
**GET** `/api/v1/referrals`

Returns all referrals created by the authenticated user.

**Authentication**: Required (`X-User-Id` header)

**Query Parameters**:
- `status` (optional): Filter by status (PENDING, COMPLETED, EXPIRED, ALL). Default: returns all.

**Response** (200 OK):
```json
{
  "data": [
    {
      "status": "Pending",
      "channel": "sms",
      "refereeName": null,
      "createdAt": "2026-01-10T20:07:26.259959",
      "completedAt": null,
      "expiresAt": "2026-02-09T20:07:26.259959"
    },
    {
      "status": "Completed",
      "channel": "email",
      "refereeName": "Lorenzo",
      "createdAt": "2026-01-02T20:07:26.259565",
      "completedAt": "2026-01-07T20:07:26.259704",
      "expiresAt": "2026-02-01T20:07:26.259839"
    },
  ]
}
```

### 3. Validate Tracking ID
**GET** `/api/v1/referrals/{trackingId}`

Validates a tracking ID from a deep link. Called when a new user opens the app after clicking a referral link.

**Authentication**: NOT required (public endpoint)

**Response** (200 OK - Valid):
```json
{
  "valid": true,
  "referrerCode": "ANDRES123",
  "referrerName": "Andres",
}
```

**Response** (200 OK - Invalid):
```json
{
  "valid": false,
  "error": "TRACKING_ID_EXPIRED",
  "message": "Tracking ID has expired"
}
```

**Error Codes**:

- `TRACKING_ID_NOT_FOUND`: Tracking ID doesn't exist
- `TRACKING_ID_EXPIRED`: Tracking ID expired (>7 days old)
- `INVALID_FORMAT`: Tracking ID format is invalid

## Assumptions

## Authentication

This microservice asumes that user authentication is handled by a separate service and follows an architecture pattern API Gateway + Microservices. meaning that all requests to this service must be authenticated by the API Gateway before reaching this service.
The service expects an `X-User-ID` header in each request, which contains the unique identifier of the authenticated user.
The API Gateway is responsible for:

- Validating the user's authentication token
- Extracting the user ID 
- Forwarding the request to this service with the `X-User-ID` header set

**Mock Implementation**:
This service includes a `FakeAuthenticationHandler` for testing purposes that accepts any valid GUID in the `X-User-Id` header.

**Authentication and token validation are out of scope for this service.**

**Steps to set the user authentication**:

1. Open Swagger UI: http://localhost:5038/swagger/index.html
2. Clik in Authorize button in the top-right screen.
3. Add a new value for the X-User-Id= "11111111-1111-1111-1111-111111111111"
4. Save the header.
5. Now you can send request as an Authenticated user.

### User Management

This microservice assumes a **Users microservice** exists separately.

**Expected Integration**:

- User profile data (firstName, lastName, referralCode) comes from the Users service
- For this mock, user data is seeded directly in the database

**Referral Redemption Flow**:
```
1. User B validates tracking => GET /api/v1/referrals/{trackingId}
   Response: { "valid": true, "referralCode": "ANDRES123" }

2. User B registers => POST /api/v1/users/register
   Request: { "email": "userB@example.com", "referralCode": "ANDRES123" }

3. Users service creates account and calls Referrals service:
   => POST /api/v1/referrals/complete
```

**Out of Scope**:

- User registration
- Referral code generation (assumed to be done by Users service)
- Referral redemption endpoint => /api/v1/referrals/complete

### Deep Linking

This microservice assumes a third-party deep linking provider.

**Expected Flow**:
```
1. Service generates tracking ID: "trk_123_abc"
2. Service calls deep link provider API
3. Provider returns: "https://cartoncaps.app.link/r/trk_123_abc"
4. Mobile app SDK captures click and returns tracking ID on first open
```

**Mock Implementation**:
- `MockDeepLinkGenerator` formats URLs without calling a real provider
- Format: `https://mockdeeplink.com/referral/{trackingId}`

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- macOS, Linux, or Windows

### Run Locally
```bash
# Clone repository
git clone https://github.com/abenitez1987/carton-caps-referrals.git
cd carton-caps-referrals

# Restore dependencies
dotnet restore

# Run the service
dotnet run --project src/CartonCaps.Referrals.Api

# Swagger UI: http://localhost:5038/swagger/index
```

---

## Testing

### Run All Tests
```bash
dotnet test
```