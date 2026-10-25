import { useState } from "react";
import { ColorDot } from "~/components/ColorDot";
import { DataTable, type Column, type Filter } from "~/components/DataTable";
import { DeleteDialog } from "~/components/DeleteDialog";
import { EmploymentFormDialog } from "~/components/EmploymentFormDialog";
import { RowActions } from "~/components/RowActions";
import { primaryButton } from "~/components/buttonStyles";
import { optionsFromValues } from "~/components/filterOptions";
import { runAction } from "~/services/actionResult";
import { departmentsApi, employmentsApi, staffGroupsApi, usersApi } from "~/services/apiService";
import type { EmploymentDto, SaveEmploymentRequest } from "~/types/api";
import { formatDate } from "~/utils/dates";
import { getId, getOptionalNumber, getOptionalText } from "~/utils/formData";
import type { Route } from "./+types/employments";

type DialogState =
  | { type: "closed" }
  | { type: "form"; employment?: EmploymentDto }
  | { type: "delete"; employment: EmploymentDto };

export function meta() {
  return [{ title: "Ansættelser – Vagtplan" }];
}

export async function clientLoader() {
  const [employments, users, departments, staffGroups] = await Promise.all([
    employmentsApi.getAll(),
    usersApi.getAll(),
    departmentsApi.getAll(),
    staffGroupsApi.getAll(),
  ]);

  return { employments, users, departments, staffGroups };
}

export async function clientAction({ request }: Route.ClientActionArgs) {
  const formData = await request.formData();
  const intent = formData.get("intent");

  switch (intent) {
    case "create":
      return runAction(() => employmentsApi.create(readEmployment(formData)));
    case "update":
      return runAction(() => employmentsApi.update(getId(formData), readEmployment(formData)));
    case "delete":
      return runAction(() => employmentsApi.remove(getId(formData)));
    default:
      throw new Error(`Ukendt handling: ${intent}`);
  }
}

function readEmployment(formData: FormData): SaveEmploymentRequest {
  return {
    userId: getOptionalNumber(formData, "userId"),
    departmentId: getOptionalNumber(formData, "departmentId"),
    staffGroupId: getOptionalNumber(formData, "staffGroupId"),
    hoursPerWeek: getOptionalNumber(formData, "hoursPerWeek"),
    startDate: getOptionalText(formData, "startDate"),
    endDate: getOptionalText(formData, "endDate"),
  };
}

const columns: Column<EmploymentDto>[] = [
  { key: "userName", header: "Person", text: (employment) => employment.userName },
  {
    key: "departmentName",
    header: "Afsnit",
    text: (employment) => employment.departmentName,
    render: (employment) => (
      <span className="flex items-center gap-2">
        <ColorDot color={employment.departmentColor} />
        {employment.departmentName}
      </span>
    ),
  },
  { key: "staffGroupName", header: "Personalegruppe", text: (employment) => employment.staffGroupName },
  {
    key: "hoursPerWeek",
    header: "Timer pr. uge",
    text: (employment) => String(employment.hoursPerWeek),
    sortValue: (employment) => employment.hoursPerWeek,
  },
  {
    key: "startDate",
    header: "Start",
    text: (employment) => formatDate(employment.startDate),
    sortValue: (employment) => employment.startDate,
  },
  {
    key: "endDate",
    header: "Slut",
    text: (employment) => formatDate(employment.endDate),
    sortValue: (employment) => employment.endDate,
  },
];

function describeEmployment(employment: EmploymentDto) {
  return `${employment.userName} på ${employment.departmentName}`;
}

export default function EmploymentsPage({ loaderData }: Route.ComponentProps) {
  const { employments, users, departments, staffGroups } = loaderData;
  const [dialog, setDialog] = useState<DialogState>({ type: "closed" });

  const filters: Filter<EmploymentDto>[] = [
    {
      key: "department",
      label: "Afsnit",
      options: optionsFromValues(employments, (employment) => employment.departmentName),
    },
    {
      key: "staffGroup",
      label: "Personalegruppe",
      options: optionsFromValues(employments, (employment) => employment.staffGroupName),
    },
  ];

  function closeDialog() {
    setDialog({ type: "closed" });
  }

  return (
    <>
      <DataTable
        rows={employments}
        columns={columns}
        filters={filters}
        getRowKey={(employment) => employment.id}
        emptyMessage="Der er ingen ansættelser endnu."
        defaultSort={{ key: "startDate", direction: "asc" }}
        toolbarAction={
          <button type="button" onClick={() => setDialog({ type: "form" })} className={primaryButton}>
            Opret ansættelse
          </button>
        }
        rowActions={(employment) => (
          <RowActions
            itemName={describeEmployment(employment)}
            onEdit={() => setDialog({ type: "form", employment })}
            onDelete={() => setDialog({ type: "delete", employment })}
          />
        )}
      />

      {dialog.type === "form" && (
        <EmploymentFormDialog
          employment={dialog.employment}
          users={users}
          departments={departments}
          staffGroups={staffGroups}
          onClose={closeDialog}
        />
      )}
      {dialog.type === "delete" && (
        <DeleteDialog
          title="Slet ansættelse"
          message={`Er du sikker på, at du vil slette ansættelsen for ${describeEmployment(dialog.employment)} fra ${formatDate(dialog.employment.startDate)} til ${formatDate(dialog.employment.endDate)}?`}
          id={dialog.employment.id}
          onClose={closeDialog}
        />
      )}
    </>
  );
}