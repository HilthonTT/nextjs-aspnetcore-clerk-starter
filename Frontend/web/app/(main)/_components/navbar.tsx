import Link from "next/link";
import { UserButton } from "@clerk/nextjs";
import { Home } from "lucide-react";

export const Navbar = () => {
  return (
    <nav className="fixed top-0 z-10 h-14 w-full flex items-center justify-between bg-secondary px-3">
      <Link
        href="/"
        className="font-semibold hover:opacity-75 transition flex items-center">
        <Home className="h-5 w-5 mr-1" />
        Home
      </Link>
      <UserButton />
    </nav>
  );
};
