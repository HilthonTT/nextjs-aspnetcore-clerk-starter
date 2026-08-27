# Security policy

## Reporting a vulnerability

Please **do not open a public issue** for a security problem in this template.

Use GitHub's [private vulnerability reporting](https://github.com/HilthonTT/nextjs-aspnetcore-clerk-starter/security/advisories/new)
on this repository. Include what an attacker can do, the steps to reproduce it, and the commit you
tested against. You can expect an initial reply within a week.

If the issue is in a dependency rather than in this template's own code, report it upstream —
[Clerk](https://clerk.com/docs/security), [Clerk.Net](https://github.com/Hawxy/Clerk.Net/security),
[ASP.NET Core](https://github.com/dotnet/aspnetcore/security), or
[Next.js](https://github.com/vercel/next.js/security) — and open an issue here so the pinned
version can be bumped.

## Scope

This is a starter template, not a hosted service. In scope: anything that would leave a project
generated from this template insecure by default, for example a missing token check, a secret that
would be committed by following the README, or a permissive default in the CORS or auth setup.

## What the template already does

- **Tokens are validated as JWTs against Clerk's JWKS** (`Clerk:Authority`), not merely decoded.
- **The `azp` claim is checked** against `Clerk:AuthorizedParty`, so a token minted for another
  frontend origin is rejected.
- **Every controller is `[Authorize]` by default.** Only `/health` is anonymous, and
  `AuthorizationTests` fails the build if a secured route stops returning `401`.
- **The API is never called from the browser.** `API_URL` has no `NEXT_PUBLIC_` prefix and
  `lib/api.ts` is marked `server-only`, so neither the token nor the API URL reaches the client.
- **Unhandled exceptions return RFC 9457 problem details**, not stack traces.
- **NuGet auditing is on** (`NuGetAuditMode=all`), so known-vulnerable transitive packages surface
  as build warnings.

## Secrets

Never commit real keys.

- Backend: `dotnet user-secrets` locally, environment variables (`Clerk__SecretKey`) in production.
- Frontend: `.env.local`, which is gitignored. `.env.example` holds placeholders only.

If you ever commit a key, rotate it in the Clerk dashboard — removing it from git history is not
enough, since the value may already have been fetched.
