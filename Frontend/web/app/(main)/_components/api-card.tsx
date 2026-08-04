"use client";

import { useRouter } from "next/navigation";
import { useTransition } from "react";
import { RefreshCcw } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { cn } from "@/lib/utils";
import type { WeatherForecast } from "@/types/api";

interface ApiCardProps {
  forecasts: WeatherForecast[];
}

export const ApiCard = ({ forecasts }: ApiCardProps) => {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  // Re-runs the server component, which calls the API again with a fresh token.
  const onRefresh = () => startTransition(() => router.refresh());

  return (
    <div className="w-full max-w-2xl rounded-2xl border bg-secondary p-4">
      <div className="flex items-center justify-between">
        <div className="font-semibold text-xl">Make an API call</div>
        <Button
          onClick={onRefresh}
          disabled={isPending}
          size="icon"
          aria-label="Refresh">
          <RefreshCcw className={cn("h-4 w-4", isPending && "animate-spin")} />
        </Button>
      </div>
      <Separator className="my-2" />
      <Table>
        <TableCaption>
          {forecasts.length > 0
            ? "Weather forecast report"
            : "The API returned no rows."}
        </TableCaption>
        <TableHeader>
          <TableRow>
            <TableHead>Date</TableHead>
            <TableHead>Temp (°C)</TableHead>
            <TableHead>Temp (°F)</TableHead>
            <TableHead>Summary</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {forecasts.map((forecast) => (
            <TableRow key={forecast.date}>
              <TableCell className="font-medium">{forecast.date}</TableCell>
              <TableCell>{forecast.temperatureC}</TableCell>
              <TableCell>{forecast.temperatureF}</TableCell>
              <TableCell>{forecast.summary}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};
