# 🔐 JWT Authentication with Token Refresh

A minimal WebAPI project demonstrating how to generate, validate, and refresh JWT tokens using ASP.NET Core and Entity Framework.

When a call is made to the endpoint POST /users with the below data in the body of JSON:
{
   "Username" : "James",
   "Password" : "YourFavoritePassword"
}
A new user is created and stored in the database.
The data stored are: 

UserName and the HashPassword. 

In this project, I am using a third-party 'NugetPackage' to encrypt the password into a hash format before storing it in the database. Another functionality of this library is to verify the password entry with the HashPassword stored in the database.

When a user calls the /auth/login endpoint and submits the username and password (POST), the user is authenticated, and a JWT token is generated. The token is then sent with every subsequent call; for example, if the user wants to call the GET /weatherforecast endpoint, the token is included in the request payload.

The token is then verified; if it has expired, a 405 code is returned to the client. The client will then call the endpoint POST /auth/refresh with the refresh token received with the original token. After validating this refresh token, a new token is sent back to the client, which continues with subsequent calls to the endpoint GET /weatherforecast using this new token. Every time the token expires, the process repeats.


---

## 🧰 Tech Stack

- **ASP.NET Core Minimal API**
- **Entity Framework Core**
- **JWT Authentication**
- **Password Hashing via NuGet Package**
- **Client-Side Token Refresh**
- **xUnit for Unit Testing**

---

## 🗄️ Database Schema

This project uses EF Core to manage CRUD operations. Two tables are defined:

| Table         | Description                          |
|---------------|--------------------------------------|
| `Users`       | Stores user credentials              |
| `RefreshToken`| Stores refresh tokens for JWT renewal|

---

## 🚀 API Workflow

### 🧑‍💻 Create a New User

**Endpoint:** `POST /users`  
**Request Body:**

```json
{
  "Username": "James",
  "Password": "YourFavoritePassword"
}
```

- The password is hashed using a third-party NuGet package.
- Stored fields: `UserName`, `HashPassword`.

---

### 🔐 Authenticate and Receive JWT

**Endpoint:** `POST /auth/login`  
**Request Body:** Username and Password

- Validates credentials.
- Returns a JWT token and a refresh token.
- JWT must be included in subsequent requests (e.g. `GET /weatherforecast`).

---

### ⏳ Token Expiry and Refresh

If the JWT token expires:

1. The server returns **HTTP 405**.
2. The client calls `POST /auth/refresh` with the refresh token.
3. If valid, a new JWT is issued.
4. The client continues using the new token for protected endpoints.

This cycle repeats every time the token expires.

---

## 🧪 Supporting Projects

- **JwtClient**: A client project to consume the WebAPI.
- **Unit Test Project**: Validates service logic and endpoint behavior using xUnit.

---

Let me know if you'd like a badge section, diagram, or README header image to take it even further.
