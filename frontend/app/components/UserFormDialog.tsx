import { useId } from "react";
import type { UserDto } from "~/types/api";
import { FormDialog } from "./FormDialog";
import { CheckboxField, FieldError, FormField, RequiredMark } from "./FormField";

type UserFormDialogProps = {
  user?: UserDto;
  onClose: () => void;
};

/**
 * Opretter en ny bruger, eller redigerer den bruger, der gives med.
 */
export function UserFormDialog({ user, onClose }: UserFormDialogProps) {
  const rolesErrorId = useId();

  return (
    <FormDialog
      title={user ? "Rediger bruger" : "Opret bruger"}
      submitLabel={user ? "Gem ændringer" : "Opret"}
      intent={user ? "update" : "create"}
      id={user?.id}
      onClose={onClose}
    >
      {(fieldErrors) => (
        <>
          <FormField label="Navn" name="name" required defaultValue={user?.name} error={fieldErrors.name} />
          <FormField label="E-mail" name="email" type="email" required defaultValue={user?.email} error={fieldErrors.email} />

          <fieldset className="mb-4" aria-describedby={fieldErrors.roles ? rolesErrorId : undefined}>
            <legend className="mb-2 text-sm font-medium text-gray-700">
              Roller
              <RequiredMark />
            </legend>
            <CheckboxField label="Admin" name="isAdmin" defaultChecked={user?.isAdmin} error={fieldErrors.isAdmin} />
            <CheckboxField label="Personale" name="isStaff" defaultChecked={user?.isStaff} error={fieldErrors.isStaff} />
            <FieldError id={rolesErrorId} message={fieldErrors.roles} />
          </fieldset>
        </>
      )}
    </FormDialog>
  );
}