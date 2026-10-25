import { useState } from "react";
import { DataTable, type Column, type Filter } from "~/components/DataTable";
import { DeleteDialog } from "~/components/DeleteDialog";
import { RowActions } from "~/components/RowActions";
import { StaffGroupFormDialog } from "~/components/StaffGroupFormDialog";
import { primaryButton } from "~/components/buttonStyles";
import { yesNoOptions } from "~/components/filterOptions";
import { runAction } from "~/services/actionResult";
import { staffGroupsApi } from "~/services/apiService";
import type { SaveStaffGroupRequest, StaffGroupDto } from "~/types/api";
import { getId, getText } from "~/utils/formData";
import type { Route } from "./+types/staff-groups";

type DialogState =
  | { type: "closed" }
  | { type: "form"; staffGroup?: StaffGroupDto }
  | { type: "delete"; staffGroup: StaffGroupDto };

export function meta() {
  return [{ title: "Personalegrupper – Vagtplan" }];
}

export async function clientLoader() {
  return await staffGroupsApi.getAll();
}

export async function clientAction({ request }: Route.ClientActionArgs) {
  const formData = await request.formData();
  const intent = formData.get("intent");

  switch (intent) {
    case "create":
      return runAction(() => staffGroupsApi.create(readStaffGroup(formData)));
    case "update":
      return runAction(() => staffGroupsApi.update(getId(formData), readStaffGroup(formData)));
    case "delete":
      return runAction(() => staffGroupsApi.remove(getId(formData)));
    default:
      throw new Error(`Ukendt handling: ${intent}`);
  }
}

function readStaffGroup(formData: FormData): SaveStaffGroupRequest {
  return {
    name: getText(formData, "name"),
  };
}

const columns: Column<StaffGroupDto>[] = [
  { key: "name", header: "Navn", text: (staffGroup) => staffGroup.name },
  {
    key: "employmentCount",
    header: "Ansættelser",
    text: (staffGroup) => String(staffGroup.employmentCount),
    sortValue: (staffGroup) => staffGroup.employmentCount,
  },
];

const filters: Filter<StaffGroupDto>[] = [
  {
    key: "hasEmployments",
    label: "Har ansættelser",
    options: yesNoOptions((staffGroup) => staffGroup.employmentCount > 0),
  },
];

export default function StaffGroupsPage({ loaderData }: Route.ComponentProps) {
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
        getRowKey={(staffGroup) => staffGroup.id}
        emptyMessage="Der er ingen personalegrupper endnu."
        defaultSort={{ key: "name", direction: "asc" }}
        toolbarAction={
          <button type="button" onClick={() => setDialog({ type: "form" })} className={primaryButton}>
            Opret personalegruppe
          </button>
        }
        rowActions={(staffGroup) => (
          <RowActions
            itemName={staffGroup.name}
            onEdit={() => setDialog({ type: "form", staffGroup })}
            onDelete={() => setDialog({ type: "delete", staffGroup })}
          />
        )}
      />

      {dialog.type === "form" && <StaffGroupFormDialog staffGroup={dialog.staffGroup} onClose={closeDialog} />}
      {dialog.type === "delete" && (
        <DeleteDialog
          title="Slet personalegruppe"
          message={`Er du sikker på, at du vil slette "${dialog.staffGroup.name}"?`}
          id={dialog.staffGroup.id}
          onClose={closeDialog}
        />
      )}
    </>
  );
}