## What does this change?

<!-- A sentence or two. Link the issue if there is one: Fixes #123 -->

## Why?

<!-- What was wrong or missing. Skip if the "what" already makes it obvious. -->

## Checklist

- [ ] `dotnet build` and `dotnet test` pass in `Backend/`
- [ ] `npm run lint`, `npm run typecheck`, and `npm run build` pass in `Frontend/web/`
- [ ] No secrets, Clerk keys, or `.env.local` contents in the diff
- [ ] README / `.env.example` updated if configuration changed
- [ ] Package versions changed in `Backend/Directory.Packages.props` (not in a `.csproj`)
