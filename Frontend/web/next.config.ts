import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Type errors fail the production build rather than shipping. Linting is a separate
  // step (`npm run lint`) — Next.js 16 no longer runs ESLint during `next build`.
  typescript: { ignoreBuildErrors: false },
};

export default nextConfig;
