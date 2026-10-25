import { useActionFetcher } from "~/hooks/useActionFetcher";
import type { PeriodDto } from "~/types/api";
import { primaryButton, secondaryButton } from "./buttonStyles";
import { Dialog } from "./Dialog";
import { FormError, FormField, RequiredFieldsNote } from "./FormField";

type PeriodFormDialogProps = {
  period?: PeriodDto;
  onClose: () => void;
};

/**
 * Opretter en ny periode, eller redigerer den periode, der gives med.
 */
export function PeriodFormDialog({ period, onClose }: PeriodFormDialogProps) {
  const { fetcher, isSubmitting, failure } = useActionFetcher(onClose);
  const isEditing = period !== undefined;
  const submitLabel = isEditing ? "Gem ændringer" : "Opret";

  return (
    <Dialog title={isEditing ? "Rediger periode" : "Opret periode"} onClose={onClose}>
      <fetcher.Form method="post" noValidate>
        <input type="hidden" name="intent" value={isEditing ? "update" : "create"} />
        {period && <input type="hidden" name="id" value={period.id} />}

        <RequiredFieldsNote />
        {failure && <FormError message={failure.message} />}

        <FormField label="Navn" name="name" required defaultValue={period?.name} error={failure?.fieldErrors.name} />
        <FormField label="Start" name="startDate" type="date" required defaultValue={period?.startDate} error={failure?.fieldErrors.startDate} />
        <FormField label="Slut" name="endDate" type="date" required defaultValue={period?.endDate} error={failure?.fieldErrors.endDate} />

        <div className="mt-6 flex justify-end gap-2">
          <button type="button" onClick={onClose} className={secondaryButton}>
            Annuller
          </button>
          <button type="submit" disabled={isSubmitting} className={primaryButton}>
            {isSubmitting ? "Gemmer..." : submitLabel}
          </button>
        </div>
      </fetcher.Form>
    </Dialog>
  );
}