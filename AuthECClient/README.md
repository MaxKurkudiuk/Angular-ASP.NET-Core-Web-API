# AuthECClient

Frontend client for an **Authentication & Authorization** system built with **Angular 21**. Demonstrates JWT-based auth with role/claim-based route guarding, conditional UI rendering, and HTTP interceptors.

## Tech Stack

- **Angular 21** (standalone components, `bootstrapApplication`)
- **Bootstrap 5.3** — layout & styling (loaded via CDN)
- **ngx-toastr** — toast notifications
- **Reactive Forms** — login & registration forms with custom validators
- **HttpClient** with functional interceptors
- **View Transitions** — built-in CSS route transitions

## Prerequisites

- [Node.js](https://nodejs.org/) >= 18
- npm (comes with Node.js)
- The backend ASP.NET Core Web API running at `http://localhost:5292/api`

## Setup

```bash
npm install
```

## Development Server

```bash
ng serve
```

Navigate to `http://localhost:4200/`. The app auto-reloads on file changes.

## Build

```bash
ng build
```

Build artifacts are output to the `dist/` directory. Production builds optimize for performance.

## Project Structure

```
src/
├── app/
│   ├── app.component.ts|html      # Root component (<router-outlet/>)
│   ├── app.config.ts              # Providers: router, HttpClient, Toastr
│   ├── app.routes.ts              # All routes with guards & claim requirements
│   ├── core/
│   │   ├── guards/
│   │   │   └── auth-guard.ts      # authGuard & isNotLoggedInGuard
│   │   ├── interceptors/
│   │   │   └── auth.interceptor.ts# Attaches JWT; handles 401/403
│   │   ├── models/                # (empty — no TypeScript models yet)
│   │   └── services/
│   │       ├── auth.service.ts    # signin, createUser, token management
│   │       └── user.service.ts    # getUserProfile API call
│   ├── layouts/
│   │   └── main-layout/           # Post-login sidebar layout + logout
│   └── shared/
│       ├── components/
│       │   ├── user/              # Login & Registration shell + card UI
│       │   │   ├── login/         # Sign-in form
│       │   │   └── registration/  # Sign-up form with password validators
│       │   ├── dashboard/         # Welcome page with user name
│       │   ├── forbidden/         # 403 access denied page
│       │   └── authorizeDemo/     # Claim-based demo pages:
│       │       ├── admin-only/
│       │       ├── admin-or-teacher/
│       │       ├── apply-for-maternity-leave/
│       │       ├── library-members-only/
│       │       └── under10-and-female/
│       ├── constants/
│       │   └── constants.ts       # localStorage token key
│       ├── directives/
│       │   └── hide-if-claims-not-met.directive.ts  # Conditional DOM visibility
│       ├── pipes/
│       │   └── first-key.pipe.ts  # Extracts first key from errors object
│       └── utils/
│           └── claimReq-utils.ts  # Claim requirement functions
├── environments/
│   ├── environment.ts
│   └── environment.development.ts
└── styles.css                     # Global styles + sidebar layout
```

## Features

### Authentication
- User registration with full name, email, password (with uppercase, lowercase, special char, and min-length validation)
- Login with email/password
- JWT stored in `localStorage`
- Auth guard redirects unauthenticated users to `/signin`
- `isNotLoggedInGuard` redirects authenticated users away from login/register

### Authorization
- **Claim-based route guarding** — each route can specify a `claimReq` function that evaluates JWT claims
- **Conditional UI** — `HideIfClaimsNotMetDirective` hides nav items and buttons based on user claims
- **HTTP interceptor** — on 401 (expired/invalid token), clears token and redirects to login; on 403, shows a toast

### Demo Pages
The app includes demo pages to showcase claim-based authorization:

| Route | Required Claims |
|---|---|
| `/admin-only` | `role == "Admin"` |
| `/admin-or-teacher` | `role == "Admin" \|\| role == "Teacher"` |
| `/apply-for-maternity-leave` | `gender == "Female" && role == "Teacher"` |
| `/library-members-only` | Has a `libraryID` claim |
| `/under10-and-female` | `gender == "Female" && age <= 10` |

## Backend API

The client expects an ASP.NET Core Web API at the configured `apiBaseUrl`. Required endpoints:

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/signup` | Create a new user |
| POST | `/api/signin` | Sign in, returns JWT token |
| GET | `/api/userprofile` | Get current user profile (requires auth) |

## Running Tests

```bash
ng test
```
