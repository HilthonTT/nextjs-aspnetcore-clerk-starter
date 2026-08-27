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

Styling is Tailwind CSS v4, configured entirely in `app/globals.css` — there is no
`tailwind.config.ts`. UI primitives under `components/ui` come from shadcn/ui.

Where things live:

- `proxy.ts` — Clerk middleware; every route except `/sign-in` and `/sign-up` requires auth.
  (Next.js 16 renamed the `middleware` file convention to `proxy`.)
- `lib/api.ts` — `apiFetch`, the single place that talks to the backend. Server-only.
- `types/api.ts` — TypeScript mirrors of the backend models.
- `app/(auth)` — Clerk's sign-in / sign-up pages.
- `app/(main)` — the authenticated area.

Setup, environment variables and troubleshooting are documented in the [root README](../../README.md).
