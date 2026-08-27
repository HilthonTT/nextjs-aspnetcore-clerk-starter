import Link from "next/link";
import { FileQuestion } from "lucide-react";

import { Button } from "@/components/ui/button";

const NotFound = () => {
  return (
    <div className="bg-dotted flex min-h-full flex-col items-center justify-center gap-4 px-4 text-center">
      <FileQuestion className="text-muted-foreground size-10" />
      <h1 className="text-2xl font-semibold tracking-tight">Page not found</h1>
      <p className="text-muted-foreground max-w-sm text-sm text-pretty">
        That route does not exist in this app.
      </p>
      <Button asChild size="sm">
        <Link href="/">Back home</Link>
      </Button>
    </div>
  );
};

export default NotFound;
