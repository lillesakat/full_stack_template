import type { ReactNode } from "react";
import { useActionFetcher } from "~/hooks/useActionFetcher";
import type { FieldErrors } from "~/services/apiClient";
import { primaryButton, secondaryButton } from "./buttonStyles";
import { Dialog } from "./Dialog";
import { FormError, RequiredFieldsNote } from "./FormField";

type FormDialogProps = {
  title: string;
  submitLabel: string;
  intent: "create" | "update";
  id?: number;
  onClose: () => void;
  children: (fieldErrors: FieldErrors) => ReactNode;
};

/**
 * Fælles ramme for opret- og rediger-formularer. Hver formular leverer kun sine egne felter
 * og får backendens feltfejl med, så de kan vises ved det rigtige felt.
 */
export function FormDialog({ title, submitLabel, intent, id, onClose, children }: FormDialogProps) {
  const { fetcher, isSubmitting, failure } = useActionFetcher(onClose);

  return (
    <Dialog title={title} onClose={onClose}>
      <fetcher.Form method="post" noValidate>
        <input type="hidden" name="intent" value={intent} />
        {id !== undefined && <input type="hidden" name="id" value={id} />}

        <RequiredFieldsNote />
        {failure && <FormError message={failure.message} />}

        {children(failure?.fieldErrors ?? {})}

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