import Link from "next/link";
import { Hexagon } from "lucide-react";

const AuthLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="bg-dotted flex min-h-full flex-col items-center justify-center gap-8 px-4 py-12">
      <Link
        href="/"
        className="flex items-center gap-2 font-semibold transition-opacity hover:opacity-80">
        <Hexagon className="size-5" />
        <span>Clerk Starter</span>
      </Link>

      {children}

      <p className="text-muted-foreground max-w-sm text-center text-xs text-balance">
        A starter template for Next.js, ASP.NET Core and Clerk. Sign in to see
        data fetched from the secured API.
      </p>
    </div>
  );
};

export default AuthLayout;
