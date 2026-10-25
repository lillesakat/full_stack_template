import { useState } from "react";
import { DataTable, type Column, type Filter } from "~/components/DataTable";
import { DeleteDialog } from "~/components/DeleteDialog";
import { RowActions } from "~/components/RowActions";
import { UserFormDialog } from "~/components/UserFormDialog";
import { primaryButton } from "~/components/buttonStyles";
import { yesNoOptions } from "~/components/filterOptions";
import { runAction } from "~/services/actionResult";
import { usersApi } from "~/services/apiService";
import type { SaveUserRequest, UserDto } from "~/types/api";
import { getCheckbox, getId, getText } from "~/utils/formData";
import type { Route } from "./+types/users";

type DialogState =
  | { type: "closed" }
  | { type: "form"; user?: UserDto }
  | { type: "delete"; user: UserDto };

export function meta() {
  return [{ title: "Adgange – Vagtplan" }];
}

export async function clientLoader() {
  return await usersApi.getAll();
}

export async function clientAction({ request }: Route.ClientActionArgs) {
  const formData = await request.formData();
  const intent = formData.get("intent");

  switch (intent) {
    case "create":
      return runAction(() => usersApi.create(readUser(formData)));
    case "update":
      return runAction(() => usersApi.update(getId(formData), readUser(formData)));
    case "delete":
      return runAction(() => usersApi.remove(getId(formData)));
    default:
      throw new Error(`Ukendt handling: ${intent}`);
  }
}

function readUser(formData: FormData): SaveUserRequest {
  return {
    name: getText(formData, "name"),
    email: getText(formData, "email"),
    isAdmin: getCheckbox(formData, "isAdmin"),
    isStaff: getCheckbox(formData, "isStaff"),
  };
}

function yesNo(value: boolean) {
  return value ? "Ja" : "Nej";
}

const columns: Column<UserDto>[] = [
  { key: "name", header: "Navn", text: (user) => user.name },
  { key: "email", header: "E-mail", text: (user) => user.email },
  { key: "isAdmin", header: "Admin", text: (user) => yesNo(user.isAdmin) },
  { key: "isStaff", header: "Personale", text: (user) => yesNo(user.isStaff) },
];

const filters: Filter<UserDto>[] = [
  { key: "isAdmin", label: "Admin", options: yesNoOptions((user) => user.isAdmin) },
  { key: "isStaff", label: "Personale", options: yesNoOptions((user) => user.isStaff) },
];

export default function UsersPage({ loaderData }: Route.ComponentProps) {
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
        searchable={false}
        getRowKey={(user) => user.id}
        emptyMessage="Der er ingen brugere endnu."
        defaultSort={{ key: "name", direction: "asc" }}
        toolbarAction={
          <button type="button" onClick={() => setDialog({ type: "form" })} className={primaryButton}>
            Opret bruger
          </button>
        }
        rowActions={(user) => (
          <RowActions
            itemName={user.name}
            onEdit={() => setDialog({ type: "form", user })}
            onDelete={() => setDialog({ type: "delete", user })}
          />
        )}
      />

      {dialog.type === "form" && <UserFormDialog user={dialog.user} onClose={closeDialog} />}
      {dialog.type === "delete" && (
        <DeleteDialog
          title="Slet bruger"
          message={`Er du sikker på, at du vil slette "${dialog.user.name}"?`}
          id={dialog.user.id}
          onClose={closeDialog}
        />
      )}
    </>
  );
}