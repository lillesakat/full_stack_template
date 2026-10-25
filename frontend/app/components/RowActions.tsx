import { rowActionButton, rowDeleteButton } from "./buttonStyles";

type RowActionsProps = {
  itemName: string;
  onEdit: () => void;
  onDelete: () => void;
};

/**
 * Rediger og Slet på en række. Navnet bruges i aria-label, så skærmlæsere kan skelne rækkerne.
 */
export function RowActions({ itemName, onEdit, onDelete }: RowActionsProps) {
  return (
    <div className="flex justify-end gap-4">
      <button type="button" onClick={onEdit} aria-label={`Rediger ${itemName}`} className={rowActionButton}>
        Rediger
      </button>
      <button type="button" onClick={onDelete} aria-label={`Slet ${itemName}`} className={rowDeleteButton}>
        Slet
      </button>
    </div>
  );
}