import Image from "next/image";
import { unstable_rethrow } from "next/navigation";
import { UserRound } from "lucide-react";

import { apiFetch } from "@/lib/api";
import type { CurrentUser } from "@/types/api";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { ApiError } from "./api-error";

/**
 * Calls GET /api/Users/me, which the API answers by looking the caller up through
 * Clerk's Backend API using the user id in their own token.
 */
export const ProfileCard = async () => {
  let user: CurrentUser;

  try {
    user = await apiFetch<CurrentUser>("/api/Users/me");
  } catch (cause) {
    // Next.js signals dynamic rendering, redirects and notFound() by throwing.
    // Those must reach the framework, not be reported as an API failure.
    unstable_rethrow(cause);
    console.error("[PROFILE_REQUEST_FAILED]", cause);
    return (
      <ApiError
        title="Could not load your profile"
        message={cause instanceof Error ? cause.message : "Unknown error"}
      />
    );
  }

  const name = [user.firstName, user.lastName].filter(Boolean).join(" ");

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <UserRound className="size-4" />
          Your Clerk profile
        </CardTitle>
        <CardDescription>
          Resolved by the API from your session token, not sent by the browser.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex items-center gap-4">
        {user.imageUrl ? (
          <Image
            src={user.imageUrl}
            alt=""
            width={56}
            height={56}
            className="size-14 rounded-full border object-cover"
            // Unoptimized so any avatar host works without configuring
            // images.remotePatterns. Drop this and add the host to next.config.ts
            // if you want Next.js to optimize it.
            unoptimized
          />
        ) : (
          <div className="bg-muted flex size-14 items-center justify-center rounded-full border">
            <UserRound className="text-muted-foreground size-6" />
          </div>
        )}

        <div className="min-w-0 space-y-1">
          <div className="truncate font-medium">{name || "No name set"}</div>
          <div className="text-muted-foreground truncate text-sm">
            {user.emailAddress ?? "No primary email"}
          </div>
          <Badge variant="secondary" className="font-mono">
            {user.id}
          </Badge>
        </div>
      </CardContent>
    </Card>
  );
};
