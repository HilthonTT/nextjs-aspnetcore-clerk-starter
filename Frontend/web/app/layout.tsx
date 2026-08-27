import "./globals.css";
import type { Metadata } from "next";

import { Inter, JetBrains_Mono } from "next/font/google";

import { ClerkThemeProvider } from "@/components/clerk-theme-provider";
import { ThemeProvider } from "@/components/theme-provider";

const inter = Inter({
  subsets: ["latin"],
  variable: "--font-inter",
});

const mono = JetBrains_Mono({
  subsets: ["latin"],
  variable: "--font-jetbrains-mono",
});

export const metadata: Metadata = {
  title: {
    default: "Next.js + ASP.NET Core + Clerk",
    template: "%s · Next.js + ASP.NET Core + Clerk",
  },
  description:
    "Starter template: a Next.js app calling a Clerk-secured ASP.NET Core Web API.",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    // suppressHydrationWarning is required by next-themes, which sets the class on <html>.
    <html lang="en" className="h-full" suppressHydrationWarning>
      <body
        className={`${inter.variable} ${mono.variable} h-full font-sans antialiased`}>
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange>
          <ClerkThemeProvider>{children}</ClerkThemeProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}
