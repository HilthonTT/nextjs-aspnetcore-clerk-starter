import "server-only";

import { auth } from "@clerk/nextjs/server";

/**
 * Thrown when the API answers with a non-2xx status, so callers can branch on `status`.
 */
export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly path: string,
    readonly body: string
  ) {
    super(`API request to ${path} failed with ${status}`);
    this.name = "ApiError";
  }
}

const getApiUrl = () => {
  const apiUrl = process.env.API_URL;

  if (!apiUrl) {
    throw new Error("API_URL is not set. Copy .env.example to .env.local.");
  }

  return apiUrl.replace(/\/$/, "");
};

/**
 * Calls the ASP.NET Core API with the signed-in user's Clerk token attached.
 *
 * Server-side only: the token never reaches the browser, so the API's `azp` check keeps
 * working and the secret key stays on the server.
 */
export const apiFetch = async <T>(
  path: string,
  init: RequestInit = {}
): Promise<T> => {
  const { getToken } = await auth();
  const token = await getToken();

  if (!token) {
    throw new ApiError(401, path, "No Clerk session token available.");
  }

  const response = await fetch(`${getApiUrl()}${path}`, {
    ...init,
    headers: {
      ...init.headers,
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    // The demo endpoint returns fresh data on every request; drop this to opt into caching.
    cache: "no-store",
  });

  if (!response.ok) {
    throw new ApiError(response.status, path, await response.text());
  }

  return (await response.json()) as T;
};
