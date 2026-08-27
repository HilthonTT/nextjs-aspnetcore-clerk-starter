"use client";

import { useRouter } from "next/navigation";
import { useTransition } from "react";
import { RefreshCcw } from "lucide-react";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

/**
 * Re-runs the server components on this route, which call the API again with a fresh token.
 */
export const RefreshButton = () => {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  return (
    <Button
      onClick={() => startTransition(() => router.refresh())}
      disabled={isPending}
      variant="outline"
      size="sm"
      aria-label="Refresh data from the API"
      className="dark:bg-zinc-900 cursor-pointer"
    >
      <RefreshCcw className={cn(isPending && "animate-spin")} />
      Refresh
    </Button>
  );
};
