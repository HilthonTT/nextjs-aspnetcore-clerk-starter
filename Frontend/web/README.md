# Frontend (Next.js)

Next.js App Router frontend. Clerk handles sign-in; every call to the ASP.NET Core API is made
server-side with the user's Clerk session token attached.

```bash
cp .env.example .env.local   # then fill in your Clerk keys
npm install
npm run dev                  # http://localhost:3000
```

| Script              | What it does                       |
| ------------------- | ---------------------------------- |
| `npm run dev`       | Start the dev server               |
| `npm run build`     | Production build                   |
| `npm run start`     | Serve the production build         |
| `npm run lint`      | ESLint (flat config)               |
| `npm run typecheck` | `tsc --noEmit`                     |

Where things live:

- `middleware.ts` — Clerk middleware; every route except `/sign-in` and `/sign-up` requires auth.
- `lib/api.ts` — `apiFetch`, the single place that talks to the backend. Server-only.
- `types/api.ts` — TypeScript mirrors of the backend models.
- `app/(auth)` — Clerk's sign-in / sign-up pages.
- `app/(main)` — the authenticated area.

Setup, environment variables and troubleshooting are documented in the [root README](../../README.md).
