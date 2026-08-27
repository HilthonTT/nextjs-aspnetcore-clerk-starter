import { Skeleton } from "@/components/ui/skeleton";
import { CardSkeleton } from "./_components/card-skeleton";

const MainLoading = () => {
  return (
    <div className="space-y-8">
      <div className="space-y-3">
        <Skeleton className="h-5 w-24" />
        <Skeleton className="h-9 w-full max-w-2xl" />
        <Skeleton className="h-4 w-full max-w-xl" />
      </div>
      <div className="grid gap-6">
        <CardSkeleton rows={1} />
        <CardSkeleton rows={5} />
      </div>
    </div>
  );
};

export default MainLoading;
