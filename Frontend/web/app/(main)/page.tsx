import { Suspense } from "react";

import { Badge } from "@/components/ui/badge";
import { CardSkeleton } from "./_components/card-skeleton";
import { ForecastCard } from "./_components/forecast-card";
import { ProfileCard } from "./_components/profile-card";

// The Clerk middleware in proxy.ts already guarantees a signed-in user here.
const MainPage = () => {
  return (
    <div className="space-y-8">
      <section className="space-y-3">
        <Badge variant="success">Signed in</Badge>
        <h1 className="text-3xl font-semibold tracking-tight text-balance">
          Your Next.js app is talking to a Clerk-secured ASP.NET Core API
        </h1>
        <p className="text-muted-foreground max-w-2xl text-pretty">
          Both cards below are React Server Components. Each one calls the API
          from Node with your Clerk session token attached — the token and the
          API URL never reach the browser.
        </p>
      </section>

      {/* Each card streams in on its own, so one slow endpoint cannot block the other. */}
      <div className="grid gap-6">
        <Suspense fallback={<CardSkeleton rows={1} />}>
          <ProfileCard />
        </Suspense>

        <Suspense fallback={<CardSkeleton rows={5} />}>
          <ForecastCard />
        </Suspense>
      </div>
    </div>
  );
};

export default MainPage;
