import { DataTable, type Column, type Filter } from "~/components/DataTable";
import { periodsApi } from "~/services/apiService";
import type { PeriodDto } from "~/types/api";
import { formatDate, todayAsDateString } from "~/utils/dates";
import type { Route } from "./+types/periods";

type PeriodStatus = "Afsluttet" | "Igangværende" | "Kommende";

export function meta() {
  return [{ title: "Perioder – Vagtplan" }];
}

export async function clientLoader() {
  return await periodsApi.getAll();
}

/**
 * Status beregnes ud fra dags dato og bruges kun til filtrering i visningen.
 */
function getPeriodStatus(period: PeriodDto): PeriodStatus {
  const today = todayAsDateString();
  if (period.endDate < today) return "Afsluttet";
  if (period.startDate > today) return "Kommende";
  return "Igangværende";
}

const columns: Column<PeriodDto>[] = [
  { key: "name", header: "Navn", text: (period) => period.name },
  {
    key: "startDate",
    header: "Start",
    text: (period) => formatDate(period.startDate),
    sortValue: (period) => period.startDate,
  },
  {
    key: "endDate",
    header: "Slut",
    text: (period) => formatDate(period.endDate),
    sortValue: (period) => period.endDate,
  },
];

const statuses: PeriodStatus[] = ["Afsluttet", "Igangværende", "Kommende"];

const filters: Filter<PeriodDto>[] = [
  {
    key: "status",
    label: "Status",
    options: statuses.map((status) => ({
      label: status,
      matches: (period) => getPeriodStatus(period) === status,
    })),
  },
];

export default function PeriodsPage({ loaderData }: Route.ComponentProps) {
  return (
    <>
      <h1 className="mb-6 text-2xl font-semibold">Perioder</h1>
      <DataTable
        rows={loaderData}
        columns={columns}
        filters={filters}
        getRowKey={(period) => period.id}
        emptyMessage="Der er ingen perioder endnu."
        defaultSort={{ key: "startDate", direction: "asc" }}
      />
    </>
  );
}