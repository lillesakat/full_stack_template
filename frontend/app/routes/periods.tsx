import { DataTable, type Column } from "~/components/DataTable";
import { periodsApi } from "~/services/apiService";
import type { PeriodDto } from "~/types/api";
import type { Route } from "./+types/periods";

export function meta() {
  return [{ title: "Perioder – Vagtplan" }];
}

export async function clientLoader() {
  return await periodsApi.getAll();
}

const columns: Column<PeriodDto>[] = [
  { key: "name", header: "Navn", text: (period) => period.name },
  { key: "startDate", header: "Start", text: (period) => period.startDate },
  { key: "endDate", header: "Slut", text: (period) => period.endDate },
];

export default function PeriodsPage({ loaderData }: Route.ComponentProps) {
  return (
    <>
      <h1 className="mb-6 text-2xl font-semibold">Perioder</h1>
      <DataTable
        rows={loaderData}
        columns={columns}
        getRowKey={(period) => period.id}
        emptyMessage="Der er ingen perioder endnu."
        defaultSort={{ key: "startDate", direction: "asc" }}
      />
    </>
  );
}