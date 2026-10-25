import type { PeriodDto } from "~/types/api";
import { todayAsDateString } from "./dates";

export type PeriodStatus = "Afsluttet" | "Igangværende" | "Kommende";

/**
 * Status beregnes ud fra dags dato og bruges kun i visningen.
 */
export function getPeriodStatus(period: PeriodDto): PeriodStatus {
  const today = todayAsDateString();
  if (period.endDate < today) return "Afsluttet";
  if (period.startDate > today) return "Kommende";
  return "Igangværende";
}

/**
 * Vælger den igangværende periode. Findes der ingen, fordi dags dato ikke ligger i en periode,
 * vælges den næste kommende. Er alle perioder afsluttede, vælges den seneste.
 */
export function findDefaultPeriod(periods: PeriodDto[]): PeriodDto | undefined {
  const sortedPeriods = [...periods].sort((a, b) => a.startDate.localeCompare(b.startDate));

  return (
    sortedPeriods.find((period) => getPeriodStatus(period) === "Igangværende") ??
    sortedPeriods.find((period) => getPeriodStatus(period) === "Kommende") ??
    sortedPeriods.at(-1)
  );
}