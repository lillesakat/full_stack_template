import type { StaffGroupDto } from "~/types/api";
import { FormDialog } from "./FormDialog";
import { FormField } from "./FormField";

type StaffGroupFormDialogProps = {
  staffGroup?: StaffGroupDto;
  onClose: () => void;
};

/**
 * Opretter en ny personalegruppe, eller redigerer den personalegruppe, der gives med.
 */
export function StaffGroupFormDialog({ staffGroup, onClose }: StaffGroupFormDialogProps) {
  return (
    <FormDialog
      title={staffGroup ? "Rediger personalegruppe" : "Opret personalegruppe"}
      submitLabel={staffGroup ? "Gem ændringer" : "Opret"}
      intent={staffGroup ? "update" : "create"}
      id={staffGroup?.id}
      onClose={onClose}
    >
      {(fieldErrors) => (
        <FormField label="Navn" name="name" required defaultValue={staffGroup?.name} error={fieldErrors.name} />
      )}
    </FormDialog>
  );
}