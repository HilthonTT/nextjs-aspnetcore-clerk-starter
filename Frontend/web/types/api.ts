/** Mirrors ClerkAPI.Models.WeatherForecast on the backend. */
export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary?: string | null;
}

/** Mirrors ClerkAPI.Models.CurrentUser on the backend. */
export interface CurrentUser {
  id: string;
  firstName?: string | null;
  lastName?: string | null;
  emailAddress?: string | null;
  imageUrl?: string | null;
}
