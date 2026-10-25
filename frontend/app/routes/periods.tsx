import { useState } from "react";
import { DataTable, type Column, type Filter } from "~/components/DataTable";
import { DeleteDialog } from "~/components/DeleteDialog";
import { PeriodFormDialog } from "~/components/PeriodFormDialog";
import { RowActions } from "~/components/RowActions";
import { primaryButton } from "~/components/buttonStyles";
import { runAction } from "~/services/actionResult";
import { periodsApi } from "~/services/apiService";
import type { PeriodDto, SavePeriodRequest } from "~/types/api";
import { formatDate, todayAsDateString } from "~/utils/dates";
import { getId, getOptionalText, getText } from "~/utils/formData";
import type { Route } from "./+types/periods";

type PeriodStatus = "Afsluttet" | "Igangværende" | "Kommende";

type DialogState =
  | { type: "closed" }
  | { type: "form"; period?: PeriodDto }
  | { type: "delete"; period: PeriodDto };

export function meta() {
  return [{ title: "Perioder – Vagtplan" }];
}

export async function clientLoader() {
  return await periodsApi.getAll();
}

export async function clientAction({ request }: Route.ClientActionArgs) {
  const formData = await request.formData();
  const intent = formData.get("intent");

  switch (intent) {
    case "create":
      return runAction(() => periodsApi.create(readPeriod(formData)));
    case "update":
      return runAction(() => periodsApi.update(getId(formData), readPeriod(formData)));
    case "delete":
      return runAction(() => periodsApi.remove(getId(formData)));
    default:
      throw new Error(`Ukendt handling: ${intent}`);
  }
}

function readPeriod(formData: FormData): SavePeriodRequest {
  return {
    name: getText(formData, "name"),
    startDate: getOptionalText(formData, "startDate"),
    endDate: getOptionalText(formData, "endDate"),
  };
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
  const [dialog, setDialog] = useState<DialogState>({ type: "closed" });

  function closeDialog() {
    setDialog({ type: "closed" });
  }

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
        toolbarAction={
          <button type="button" onClick={() => setDialog({ type: "form" })} className={primaryButton}>
            Opret periode
          </button>
        }
        rowActions={(period) => (
          <RowActions
            itemName={period.name}
            onEdit={() => setDialog({ type: "form", period })}
            onDelete={() => setDialog({ type: "delete", period })}
          />
        )}
      />

      {dialog.type === "form" && <PeriodFormDialog period={dialog.period} onClose={closeDialog} />}
      {dialog.type === "delete" && (
        <DeleteDialog
          title="Slet periode"
          message={`Er du sikker på, at du vil slette "${dialog.period.name}"?`}
          id={dialog.period.id}
          onClose={closeDialog}
        />
      )}
    </>
  );
}