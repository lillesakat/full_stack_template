import type { DateString } from "~/types/api";

/**
 * Viser en dato som DD-MM-ÅÅÅÅ
 */
export function formatDate(date: DateString): string {
  const [year, month, day] = date.split("-");
  return `${day}-${month}-${year}`;
}

/**
 * Dagens dato som ÅÅÅÅ-MM-DD i brugerens egen tidszone.
 */
export function todayAsDateString(): DateString {
  const now = new Date();
  const year = now.getFullYear();
  const month = String(now.getMonth() + 1).padStart(2, "0");
  const day = String(now.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}