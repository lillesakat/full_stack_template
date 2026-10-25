import type { FilterOption } from "./DataTable";

/**
 * Laver en filtermulighed for hver forskellig værdi, fx ét valg pr. afsnit.
 */
export function optionsFromValues<T>(rows: T[], getValue: (row: T) => string): FilterOption<T>[] {
  const values = [...new Set(rows.map(getValue))].sort((a, b) => a.localeCompare(b, "da"));
  return values.map((value) => ({ label: value, matches: (row) => getValue(row) === value }));
}

/**
 * Laver valgene Ja og Nej ud fra en betingelse, fx "har ansættelser".
 */
export function yesNoOptions<T>(condition: (row: T) => boolean): FilterOption<T>[] {
  return [
    { label: "Ja", matches: condition },
    { label: "Nej", matches: (row) => !condition(row) },
  ];
}