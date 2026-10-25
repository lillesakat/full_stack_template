import { useActionFetcher } from "~/hooks/useActionFetcher";
import { dangerButton, secondaryButton } from "./buttonStyles";
import { Dialog } from "./Dialog";
import { FormError } from "./FormField";

type DeleteDialogProps = {
  title: string;
  message: string;
  id: number;
  onClose: () => void;
};

export function DeleteDialog({ title, message, id, onClose }: DeleteDialogProps) {
  const { fetcher, isSubmitting, failure } = useActionFetcher(onClose);

  return (
    <Dialog title={title} onClose={onClose}>
      <p className="mb-4 text-sm text-gray-700">{message}</p>
      {failure && <FormError message={failure.message} />}
      <fetcher.Form method="post">
        <input type="hidden" name="intent" value="delete" />
        <input type="hidden" name="id" value={id} />
        <div className="mt-6 flex justify-end gap-2">
          <button type="button" onClick={onClose} className={secondaryButton}>
            Annuller
          </button>
          <button type="submit" disabled={isSubmitting} className={dangerButton}>
            {isSubmitting ? "Sletter..." : "Slet"}
          </button>
        </div>
      </fetcher.Form>
    </Dialog>
  );
}