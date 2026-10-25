import { useEffect } from "react";
import { useFetcher } from "react-router";
import type { ActionResult } from "~/services/actionResult";

/**
 * Sender en formular til sidens clientAction og kalder onSuccess, når den lykkes.
 * Returnerer fejlen fra API'et, hvis den fejler.
 */
export function useActionFetcher(onSuccess: () => void) {
  const fetcher = useFetcher<ActionResult>();
  const result = fetcher.data;
  const isSubmitting = fetcher.state !== "idle";
  const failure = result && !result.ok ? result : undefined;

  useEffect(() => {
    if (fetcher.state === "idle" && result?.ok) {
      onSuccess();
    }
  }, [fetcher.state, result, onSuccess]);

  return { fetcher, isSubmitting, failure };
}