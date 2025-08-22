Absolutely, Marcos. Here's a polished and visually engaging version of your `.md` file that’s clean, readable, and developer-friendly:

---

# 🔐 JWT Authentication with Token Refresh

A minimal WebAPI project demonstrating how to generate, validate, and refresh JWT tokens using ASP.NET Core and Entity Framework.

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
