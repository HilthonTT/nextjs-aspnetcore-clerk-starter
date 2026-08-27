import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

/**
 * Rendered by <Suspense> while a card's server component awaits the API, so the
 * page shell paints immediately instead of blocking on the slowest request.
 */
export const CardSkeleton = ({ rows = 3 }: { rows?: number }) => {
  return (
    <Card>
      <CardHeader className="gap-2">
        <Skeleton className="h-4 w-40" />
        <Skeleton className="h-3 w-64" />
      </CardHeader>
      <CardContent className="space-y-3">
        {Array.from({ length: rows }, (_, index) => (
          <Skeleton key={index} className="h-8 w-full" />
        ))}
      </CardContent>
    </Card>
  );
};
