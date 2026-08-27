import { TriangleAlert } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

interface ApiErrorProps {
  title?: string;
  message: string;
}

export const ApiError = ({
  title = "Could not reach the API",
  message,
}: ApiErrorProps) => {
  return (
    <Card className="border-destructive/40 bg-destructive/5">
      <CardHeader>
        <CardTitle className="text-destructive flex items-center gap-2">
          <TriangleAlert className="size-4" />
          {title}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-3">
        <p className="text-muted-foreground font-mono text-xs break-all">
          {message}
        </p>
        <ul className="text-muted-foreground list-disc space-y-1 pl-5 text-sm">
          <li>
            Is the ASP.NET Core API running on the URL in{" "}
            <code className="font-mono">API_URL</code>?
          </li>
          <li>
            Does <code className="font-mono">Clerk:Authority</code> point at your
            Clerk instance, and{" "}
            <code className="font-mono">Clerk:AuthorizedParty</code> at this
            app&apos;s base URL?
          </li>
          <li>
            Is <code className="font-mono">Clerk:SecretKey</code> set in user
            secrets or the environment?
          </li>
        </ul>
      </CardContent>
    </Card>
  );
};
