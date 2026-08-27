import Link from "next/link";
import { UserButton } from "@clerk/nextjs";
import { Hexagon } from "lucide-react";

import { ThemeToggle } from "@/components/theme-toggle";

export const Navbar = () => {
  return (
    <header className="bg-background/80 sticky top-0 z-50 w-full border-b backdrop-blur-sm">
      <div className="mx-auto flex h-14 max-w-5xl items-center justify-between px-4">
        <Link
          href="/"
          className="flex items-center gap-2 font-semibold transition-opacity hover:opacity-80">
          <Hexagon className="size-5" />
          <span>Clerk Starter</span>
        </Link>

        <div className="flex items-center gap-1">
          <ThemeToggle />
          <div className="ml-1 flex items-center">
            <UserButton
              appearance={{ elements: { avatarBox: "size-8" } }}
            />
          </div>
        </div>
      </div>
    </header>
  );
};
