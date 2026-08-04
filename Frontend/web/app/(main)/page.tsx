import { Server } from "lucide-react";

import { apiFetch } from "@/lib/api";
import type { WeatherForecast } from "@/types/api";
import { ApiCard } from "./_components/api-card";
import { ApiError } from "./_components/api-error";

// Every request hits the API with the caller's own token, so nothing here can be prerendered.
export const dynamic = "force-dynamic";

// The middleware already guarantees a signed-in user here.
const MainPage = async () => {
  let forecasts: WeatherForecast[] = [];
  let error: string | null = null;

  try {
    forecasts = await apiFetch<WeatherForecast[]>("/api/WeatherForecast");
  } catch (cause) {
    console.error("[API_REQUEST_FAILED]", cause);
    error = cause instanceof Error ? cause.message : "Unknown error";
  }

  return (
    <div className="flex flex-col items-center w-full h-full px-4 pt-24 pb-12">
      <h1 className="my-4 text-xl font-semibold flex items-center gap-x-2">
        <Server className="h-6 w-6" />
        API calls to an ASP.NET Core API
      </h1>
      {error ? <ApiError message={error} /> : <ApiCard forecasts={forecasts} />}
    </div>
  );
};

export default MainPage;
