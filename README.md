# Next.js + ASP.NET Core + Clerk

A starter template for a **Next.js** frontend calling an **ASP.NET Core Web API** that is locked
down with **Clerk** authentication. The frontend signs the user in with Clerk, then attaches their
session token to every API call; the API validates that token as a JWT against your Clerk instance
and can look the user up through Clerk's Backend API.

> Click **Use this template** on GitHub to start a new project from this repository.

---

## What you get

- **Next.js 16** (App Router, React 19, TypeScript, Tailwind CSS, shadcn/ui components)
- **ASP.NET Core 8** Web API with controllers, Swagger, and a `/health` probe
- **Clerk** on both sides — hosted sign-in/sign-up pages on the frontend, JWT bearer validation on
  the backend, including the `azp` check that stops tokens from other origins being replayed
- Config that **fails fast**: the API refuses to start if the Clerk settings are missing
- One place to call the API (`lib/api.ts`), one place to configure it (`Options/ClerkOptions.cs`)
- GitHub Actions CI that builds and lints both apps

Auth on the .NET side is powered by [Hawxy's Clerk.Net](https://github.com/Hawxy/Clerk.Net).

## Repository layout

```
.
├─ Backend/
│  ├─ CsharpAPIClerk.sln
│  └─ ClerkAPI/
│     ├─ Controllers/       WeatherForecast (sample data) and Users (/me via Clerk)
│     ├─ Extensions/        ClaimsPrincipal.GetUserId()
│     ├─ Models/            Response models
│     ├─ Options/           Bound + validated configuration
│     ├─ Program.cs         Middleware pipeline
│     └─ RegisterServices.cs  DI registration: auth, Clerk client, CORS, Swagger
└─ Frontend/
   └─ web/
      ├─ app/(auth)/        Clerk sign-in / sign-up
      ├─ app/(main)/        Authenticated area
      ├─ lib/api.ts         apiFetch — the only place that calls the API
      ├─ types/api.ts       Mirrors of the backend models
      └─ middleware.ts      Clerk route protection
```

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- A free [Clerk](https://clerk.com) application

## Getting started

### 1. Collect your Clerk values

From the [Clerk dashboard](https://dashboard.clerk.com):

| Value                   | Where to find it                                                            |
| ----------------------- | --------------------------------------------------------------------------- |
| Publishable key         | **API Keys** — starts with `pk_`                                            |
| Secret key              | **API Keys** — starts with `sk_`                                            |
| Frontend API URL        | **API Keys → Show API URLs** — e.g. `https://verb-noun-00.clerk.accounts.dev` |

### 2. Configure and run the API

```bash
cd Backend/ClerkAPI

# Secrets stay out of source control — user-secrets writes outside the repo.
dotnet user-secrets set "Clerk:SecretKey" "sk_test_..."
dotnet user-secrets set "Clerk:Authority" "https://verb-noun-00.clerk.accounts.dev"

dotnet run
```

The API listens on `http://localhost:5014` and `https://localhost:7080`, with Swagger at
`/swagger` in development.

### 3. Configure and run the frontend

```bash
cd Frontend/web
cp .env.example .env.local   # fill in your Clerk keys
npm install
npm run dev
```

Open <http://localhost:3000>, sign up, and the home page will render data fetched from the secured
API.

## Configuration reference

### Backend — `Backend/ClerkAPI/appsettings.json`

| Setting                | Required | Description                                                                       |
| ---------------------- | -------- | --------------------------------------------------------------------------------- |
| `Clerk:SecretKey`      | yes      | Clerk Backend API key (`sk_...`). Use user-secrets or an environment variable.     |
| `Clerk:Authority`      | yes      | Your Clerk Frontend API URL. Tokens are validated against its JWKS.                |
| `Clerk:AuthorizedParty`| yes      | Base URL of the frontend allowed to call the API. Checked against the `azp` claim. |
| `Cors:AllowedOrigins`  | no       | Browser origins allowed to call the API directly. Empty by default.                |

Every setting can also be supplied as an environment variable using `__` as the separator, e.g.
`Clerk__SecretKey`. If any required value is missing the app throws an
`OptionsValidationException` at startup naming the offending setting.

### Frontend — `Frontend/web/.env.local`

| Variable                             | Required | Description                                             |
| ------------------------------------ | -------- | ------------------------------------------------------- |
| `NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY`  | yes      | Clerk publishable key (`pk_...`)                        |
| `CLERK_SECRET_KEY`                   | yes      | Clerk secret key (`sk_...`), server-side only           |
| `NEXT_PUBLIC_CLERK_SIGN_IN_URL`      | no       | Defaults to `/sign-in`                                  |
| `NEXT_PUBLIC_CLERK_SIGN_UP_URL`      | no       | Defaults to `/sign-up`                                  |
| `API_URL`                            | yes      | Base URL of the ASP.NET Core API                        |

`API_URL` deliberately has no `NEXT_PUBLIC_` prefix: the API is only ever called from server
components, so neither the URL nor the token reaches the browser.

## Endpoints

| Method | Route                   | Auth | Description                              |
| ------ | ----------------------- | ---- | ---------------------------------------- |
| GET    | `/health`               | no   | Liveness probe                            |
| GET    | `/api/WeatherForecast`  | yes  | Sample data                               |
| GET    | `/api/Users/me`         | yes  | The Clerk profile behind the bearer token |

`Backend/ClerkAPI/ClerkAPI.http` has ready-made requests for all three.

## How the auth flow works

1. Clerk middleware (`middleware.ts`) protects every route except the sign-in and sign-up pages.
2. A server component calls `apiFetch`, which grabs the current user's session token via
   `(await auth()).getToken()` and sends it as `Authorization: Bearer <token>`.
3. The API validates the token's signature against Clerk's JWKS (`Clerk:Authority`) and rejects
   tokens whose `azp` claim isn't `Clerk:AuthorizedParty`.
4. `User.GetUserId()` gives you the Clerk user id, which `UsersController` uses to fetch the full
   profile from Clerk's Backend API.

## Adding your own endpoint

1. Add a controller under `Backend/ClerkAPI/Controllers` with `[Authorize]` and
   `[Route("api/[controller]")]`; use `User.GetUserId()` for the caller's Clerk id.
2. Add the response model in `Backend/ClerkAPI/Models`.
3. Mirror the model in `Frontend/web/types/api.ts`.
4. Call it from a server component with `apiFetch<YourType>("/api/YourController")`.

## Troubleshooting

| Symptom                                                     | Likely cause                                                                        |
| ----------------------------------------------------------- | ----------------------------------------------------------------------------------- |
| API exits at startup with `OptionsValidationException`       | A `Clerk:*` setting is missing — the message names it.                              |
| `401` with `AZP claim is invalid or missing`                 | `Clerk:AuthorizedParty` doesn't match the frontend's base URL (e.g. `http` vs `https`). |
| `401` on every request                                       | `Clerk:Authority` points at the wrong instance, or frontend and API use different Clerk apps. |
| "Could not reach the API" card in the UI                     | The API isn't running, or `API_URL` points at the wrong port.                        |
| `fetch failed` against `https://localhost:7080`              | The dev certificate isn't trusted — run `dotnet dev-certs https --trust`, or use the HTTP URL. |

## License

[MIT](LICENSE)
