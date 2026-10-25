import { useState } from "react";
import { DataTable, type Column, type Filter } from "~/components/DataTable";
import { DeleteDialog } from "~/components/DeleteDialog";
import { DepartmentFormDialog } from "~/components/DepartmentFormDialog";
import { RowActions } from "~/components/RowActions";
import { primaryButton } from "~/components/buttonStyles";
import { yesNoOptions } from "~/components/filterOptions";
import { runAction } from "~/services/actionResult";
import { departmentsApi } from "~/services/apiService";
import type { DepartmentDto, SaveDepartmentRequest } from "~/types/api";
import { getId, getText } from "~/utils/formData";
import type { Route } from "./+types/departments";

type DialogState =
  | { type: "closed" }
  | { type: "form"; department?: DepartmentDto }
  | { type: "delete"; department: DepartmentDto };

export function meta() {
  return [{ title: "Afsnit – Vagtplan" }];
}

export async function clientLoader() {
  return await departmentsApi.getAll();
}

export async function clientAction({ request }: Route.ClientActionArgs) {
  const formData = await request.formData();
  const intent = formData.get("intent");

  switch (intent) {
    case "create":
      return runAction(() => departmentsApi.create(readDepartment(formData)));
    case "update":
      return runAction(() => departmentsApi.update(getId(formData), readDepartment(formData)));
    case "delete":
      return runAction(() => departmentsApi.remove(getId(formData)));
    default:
      throw new Error(`Ukendt handling: ${intent}`);
  }
}

function readDepartment(formData: FormData): SaveDepartmentRequest {
  return {
    name: getText(formData, "name"),
    color: getText(formData, "color"),
  };
}

const columns: Column<DepartmentDto>[] = [
  { key: "name", header: "Navn", text: (department) => department.name },
  {
    key: "color",
    header: "Farve",
    text: (department) => department.color,
    render: (department) => (
      <span className="flex items-center gap-2">
        <span
          aria-hidden="true"
          className="h-4 w-4 rounded-full border border-gray-300"
          style={{ backgroundColor: department.color }}
        />
        <span className="font-mono">{department.color}</span>
      </span>
    ),
  },
  {
    key: "employmentCount",
    header: "Ansættelser",
    text: (department) => String(department.employmentCount),
    sortValue: (department) => department.employmentCount,
  },
];

const filters: Filter<DepartmentDto>[] = [
  {
    key: "hasEmployments",
    label: "Har ansættelser",
    options: yesNoOptions((department) => department.employmentCount > 0),
  },
];

export default function DepartmentsPage({ loaderData }: Route.ComponentProps) {
  const [dialog, setDialog] = useState<DialogState>({ type: "closed" });

  function closeDialog() {
    setDialog({ type: "closed" });
  }

  return (
    <>
      <DataTable
        rows={loaderData}
        columns={columns}
        filters={filters}
        getRowKey={(department) => department.id}
        emptyMessage="Der er ingen afsnit endnu."
        defaultSort={{ key: "name", direction: "asc" }}
        toolbarAction={
          <button type="button" onClick={() => setDialog({ type: "form" })} className={primaryButton}>
            Opret afsnit
          </button>
        }
        rowActions={(department) => (
          <RowActions
            itemName={department.name}
            onEdit={() => setDialog({ type: "form", department })}
            onDelete={() => setDialog({ type: "delete", department })}
          />
        )}
      />

      {dialog.type === "form" && <DepartmentFormDialog department={dialog.department} onClose={closeDialog} />}
      {dialog.type === "delete" && (
        <DeleteDialog
          title="Slet afsnit"
          message={`Er du sikker på, at du vil slette "${dialog.department.name}"?`}
          id={dialog.department.id}
          onClose={closeDialog}
        />
      )}
    </>
  );
}