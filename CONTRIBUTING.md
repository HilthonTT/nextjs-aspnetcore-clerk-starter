# Contributing

Thanks for helping improve this template. It is a **starter**, so the guiding question for any
change is: *would most people beginning a Next.js + ASP.NET Core + Clerk project want this on
day one?* Anything more specialised is better added by each project after cloning.

## Local setup

Follow [Getting started](README.md#getting-started) in the README. You need a free Clerk
application; the template cannot run without one.

## Running the checks

The same two commands CI runs:

```bash
# Backend
cd Backend
dotnet build CsharpAPIClerk.slnx --configuration Release
dotnet test CsharpAPIClerk.slnx --configuration Release

# Frontend
cd Frontend/web
npm ci
npm run lint && npm run typecheck && npm run build
```

The backend builds with `TreatWarningsAsErrors`, so an analyzer warning fails the build. That is
deliberate — a template should not hand people warnings on their first build.

## Conventions

- **NuGet versions belong in `Backend/Directory.Packages.props`.** Central Package Management is
  on; a `<PackageReference>` with a `Version` attribute will fail the build.
- **Shared MSBuild settings belong in `Backend/Directory.Build.props`**, not in individual
  `.csproj` files.
- **Tailwind is configured in CSS** (`Frontend/web/app/globals.css`). There is no
  `tailwind.config.ts` in Tailwind v4.
- Formatting is governed by [`.editorconfig`](.editorconfig). Run `dotnet format` before pushing
  if your editor does not apply it.
- Add a test in `Backend/ClerkAPI.Tests` for anything that changes the auth surface. The
  `AuthorizationTests` list of secured routes should stay complete.

## Commit messages and PRs

Short, imperative subject lines (`Add health check probe`, not `added health check probe`).
Fill in the PR template's checklist — it is the same list a reviewer would otherwise ask about.

## Reporting security issues

Please do not open a public issue. See [SECURITY.md](SECURITY.md).
