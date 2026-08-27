# Frontend (Next.js)

Next.js App Router frontend. Clerk handles sign-in; every call to the ASP.NET Core API is made
server-side with the user's Clerk session token attached.

```bash
cp .env.example .env.local   # then fill in your Clerk keys
npm install
npm run dev                  # http://localhost:3000
```

| Script              | What it does               |
| ------------------- | -------------------------- |
| `npm run dev`       | Start the dev server       |
| `npm run build`     | Production build           |
| `npm run start`     | Serve the production build |
| `npm run lint`      | ESLint (flat config)       |
| `npm run typecheck` | `tsc --noEmit`             |

Styling is Tailwind CSS v4, configured entirely in `app/globals.css` — there is no
`tailwind.config.ts`. UI primitives under `components/ui` come from shadcn/ui.

## Where things live

- `proxy.ts` — Clerk middleware; every route except `/sign-in` and `/sign-up` requires auth.
  (Next.js 16 renamed the `middleware` file convention to `proxy`.)
- `lib/api.ts` — `apiFetch`, the single place that talks to the backend. Server-only.
- `types/api.ts` — TypeScript mirrors of the backend models.
- `app/(auth)` — Clerk's sign-in / sign-up pages.
- `app/(main)` — the authenticated area: navbar shell, plus one card per API endpoint.
- `components/ui` — shadcn/ui primitives.
- `components/theme-*.tsx` — dark mode, via `next-themes`.

## The dashboard

`app/(main)/page.tsx` renders two cards, one per secured endpoint:

| Component          | Endpoint               | Shows                                          |
| ------------------ | ---------------------- | ---------------------------------------------- |
| `ProfileCard`      | `/api/Users/me`        | Your Clerk profile, resolved by the API         |
| `ForecastCard`     | `/api/WeatherForecast` | The sample payload                              |

Both are **async server components** wrapped in their own `<Suspense>`, so the page shell paints
immediately and each card streams in when its request finishes — one slow endpoint cannot block
the other. `CardSkeleton` is the fallback. `RefreshButton` is the only client component involved;
it calls `router.refresh()`, which re-runs both server components with a fresh token.

If a call fails, the card renders `ApiError` in place rather than taking down the page.

`ForecastCard` and the `WeatherForecast` type exist only to prove the wiring works — delete them
once you have endpoints of your own.

## Theming

Colours are CSS variables in `app/globals.css`, defined once for light and again under `.dark`.
`next-themes` toggles that class on `<html>`; `ThemeToggle` in the navbar switches between
light, dark, and system.

Clerk's own components follow along through `components/clerk-theme-provider.tsx`, which passes
`appearance.theme` based on the resolved theme. `@clerk/themes` also ships a `shadcn` theme that
reads these CSS variables directly if you would rather not manage it explicitly.

Setup, environment variables and troubleshooting are documented in the [root README](../../README.md).
