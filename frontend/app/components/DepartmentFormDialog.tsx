import type { DepartmentDto } from "~/types/api";
import { ColorField } from "./ColorField";
import { FormDialog } from "./FormDialog";
import { FormField } from "./FormField";

type DepartmentFormDialogProps = {
  department?: DepartmentDto;
  onClose: () => void;
};

/**
 * Opretter et nyt afsnit, eller redigerer det afsnit, der gives med.
 */
export function DepartmentFormDialog({ department, onClose }: DepartmentFormDialogProps) {
  return (
    <FormDialog
      title={department ? "Rediger afsnit" : "Opret afsnit"}
      submitLabel={department ? "Gem ændringer" : "Opret"}
      intent={department ? "update" : "create"}
      id={department?.id}
      onClose={onClose}
    >
      {(fieldErrors) => (
        <>
          <FormField label="Navn" name="name" required defaultValue={department?.name} error={fieldErrors.name} />
          <ColorField label="Farve" name="color" required defaultValue={department?.color} error={fieldErrors.color} />
        </>
      )}
    </FormDialog>
  );
}