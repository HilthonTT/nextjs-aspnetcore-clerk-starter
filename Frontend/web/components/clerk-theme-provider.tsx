"use client";

import { ClerkProvider } from "@clerk/nextjs";
import { dark } from "@clerk/themes";
import { useTheme } from "next-themes";

/**
 * Keeps Clerk's hosted components (sign-in, sign-up, UserButton) in step with the
 * app's theme. `resolvedTheme` is used rather than `theme` so "system" maps to the
 * OS preference instead of falling through to light.
 *
 * @clerk/themes also ships a `shadcn` theme that reads the CSS variables in globals.css
 * directly. Swap it in here if you would rather Clerk track your tokens automatically.
 */
export const ClerkThemeProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const { resolvedTheme } = useTheme();

  return (
    <ClerkProvider
      afterSignOutUrl="/"
      appearance={{ theme: resolvedTheme === "dark" ? dark : undefined }}>
      {children}
    </ClerkProvider>
  );
};
