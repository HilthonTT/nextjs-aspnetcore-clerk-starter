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

  // Normalise first: spreading a Headers instance (or a [key, value][] array) into an
  // object literal silently drops every header it carries.
  const headers = new Headers(init.headers);
  headers.set("Authorization", `Bearer ${token}`);
  headers.set("Accept", "application/json");

  if (init.body !== undefined && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  const response = await fetch(`${getApiUrl()}${path}`, {
    // The demo endpoints return fresh data on every request; pass `cache` or `next`
    // in `init` to opt into caching.
    cache: "no-store",
    ...init,
    headers,
  });

  if (!response.ok) {
    throw new ApiError(response.status, path, await response.text());
  }

  // 204 No Content (and other empty bodies) cannot be parsed as JSON.
  if (response.status === 204 || response.headers.get("Content-Length") === "0") {
    return undefined as T;
  }

  return (await response.json()) as T;
};
