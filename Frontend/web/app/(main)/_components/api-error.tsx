import { TriangleAlert } from "lucide-react";

interface ApiErrorProps {
  message: string;
}

export const ApiError = ({ message }: ApiErrorProps) => {
  return (
    <div className="w-full max-w-2xl rounded-2xl border border-destructive/50 bg-destructive/10 p-4">
      <div className="flex items-center gap-x-2 font-semibold">
        <TriangleAlert className="h-5 w-5" />
        Could not reach the API
      </div>
      <p className="mt-2 text-sm text-muted-foreground">{message}</p>
      <ul className="mt-3 list-disc pl-5 text-sm text-muted-foreground space-y-1">
        <li>Is the ASP.NET Core API running on the URL in `API_URL`?</li>
        <li>
          Does `Clerk:Authority` point at your Clerk instance, and
          `Clerk:AuthorizedParty` at this app&apos;s base URL?
        </li>
        <li>Is `Clerk:SecretKey` set in user secrets or environment?</li>
      </ul>
    </div>
  );
};
