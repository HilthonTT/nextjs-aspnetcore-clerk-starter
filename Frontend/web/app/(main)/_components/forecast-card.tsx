import { unstable_rethrow } from "next/navigation";
import { CloudSun } from "lucide-react";

import { apiFetch } from "@/lib/api";
import type { WeatherForecast } from "@/types/api";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ApiError } from "./api-error";
import { RefreshButton } from "./refresh-button";

/**
 * Calls the sample secured endpoint. Delete this once you have endpoints of your own.
 */
export const ForecastCard = async () => {
  let forecasts: WeatherForecast[];

  try {
    forecasts = await apiFetch<WeatherForecast[]>("/api/WeatherForecast");
  } catch (cause) {
    // Next.js signals dynamic rendering, redirects and notFound() by throwing.
    // Those must reach the framework, not be reported as an API failure.
    unstable_rethrow(cause);
    console.error("[FORECAST_REQUEST_FAILED]", cause);
    return (
      <ApiError
        message={cause instanceof Error ? cause.message : "Unknown error"}
      />
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <CloudSun className="size-4" />
          Sample secured endpoint
        </CardTitle>
        <CardDescription>
          <code className="font-mono">GET /api/WeatherForecast</code> — rejected
          without a valid Clerk token.
        </CardDescription>
        <CardAction>
          <RefreshButton />
        </CardAction>
      </CardHeader>
      <CardContent>
        {forecasts.length === 0 ? (
          <p className="text-muted-foreground py-6 text-center text-sm">
            The API returned no rows.
          </p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow className="hover:bg-transparent">
                <TableHead>Date</TableHead>
                <TableHead className="text-right">°C</TableHead>
                <TableHead className="text-right">°F</TableHead>
                <TableHead>Summary</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {forecasts.map((forecast) => (
                <TableRow key={forecast.date}>
                  <TableCell className="font-mono text-xs">
                    {forecast.date}
                  </TableCell>
                  <TableCell className="text-right tabular-nums">
                    {forecast.temperatureC}
                  </TableCell>
                  <TableCell className="text-muted-foreground text-right tabular-nums">
                    {forecast.temperatureF}
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">{forecast.summary}</Badge>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
};
