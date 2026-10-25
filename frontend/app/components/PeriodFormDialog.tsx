import type { PeriodDto } from "~/types/api";
import { FormDialog } from "./FormDialog";
import { FormField } from "./FormField";

type PeriodFormDialogProps = {
  period?: PeriodDto;
  onClose: () => void;
};

/**
 * Opretter en ny periode, eller redigerer den periode, der gives med.
 */
export function PeriodFormDialog({ period, onClose }: PeriodFormDialogProps) {
  return (
    <FormDialog
      title={period ? "Rediger periode" : "Opret periode"}
      submitLabel={period ? "Gem ændringer" : "Opret"}
      intent={period ? "update" : "create"}
      id={period?.id}
      onClose={onClose}
    >
      {(fieldErrors) => (
        <>
          <FormField label="Navn" name="name" required defaultValue={period?.name} error={fieldErrors.name} />
          <FormField label="Start" name="startDate" type="date" required defaultValue={period?.startDate} error={fieldErrors.startDate} />
          <FormField label="Slut" name="endDate" type="date" required defaultValue={period?.endDate} error={fieldErrors.endDate} />
        </>
      )}
    </FormDialog>
  );
}