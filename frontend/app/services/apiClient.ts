export type FieldErrors = Record<string, string>;

/**
 * En fejl fra API'et, oversat fra ProblemDetails, så siderne kan vise
 * den samlede besked og hver feltfejl ved det rigtige felt.
 */
export class ApiError extends Error {
  readonly status: number;
  readonly fieldErrors: FieldErrors;

  constructor(status: number, message: string, fieldErrors: FieldErrors = {}) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.fieldErrors = fieldErrors;
  }
}

type ProblemDetails = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

const fallbackMessage = "Der opstod en uventet fejl. Prøv igen.";

export async function request<T>(path: string, method = "GET", body?: unknown): Promise<T> {
  const response = await fetch(`/api${path}`, {
    method,
    headers: body === undefined ? undefined : { "Content-Type": "application/json" },
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (!response.ok) {
    throw await toApiError(response);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function toApiError(response: Response): Promise<ApiError> {
  const problem = await readProblemDetails(response);

  const fieldErrors: FieldErrors = {};
  for (const [field, messages] of Object.entries(problem?.errors ?? {})) {
    fieldErrors[field] = messages[0];
  }

  const message = problem?.detail ?? problem?.title ?? fallbackMessage;
  return new ApiError(response.status, message, fieldErrors);
}

/**
 * Returnerer null, hvis svaret ikke er JSON, fx hvis backend ikke kører.
 */
async function readProblemDetails(response: Response): Promise<ProblemDetails | null> {
  try {
    return (await response.json()) as ProblemDetails;
  } catch {
    return null;
  }
}