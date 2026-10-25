import { ApiError, type FieldErrors } from "./apiClient";

export type ActionResult =
  | { ok: true }
  | { ok: false; message: string; fieldErrors: FieldErrors };

/**
 * Udfører et API-kald fra en clientAction. Fejl fra API'et returneres, så formularen
 * kan vise dem. Andre fejl kastes videre til ErrorBoundary.
 */
export async function runAction(operation: () => Promise<unknown>): Promise<ActionResult> {
  try {
    await operation();
    return { ok: true };
  } catch (error) {
    if (error instanceof ApiError) {
      return { ok: false, message: error.message, fieldErrors: error.fieldErrors };
    }
    throw error;
  }
}