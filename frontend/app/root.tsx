import {
  isRouteErrorResponse,
  Link,
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
} from "react-router";

import type { Route } from "./+types/root";
import { primaryButton, secondaryButton } from "~/components/buttonStyles";
import { ApiError } from "~/services/apiClient";
import "./app.css";

export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="da">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <Meta />
        <Links />
      </head>
      <body>
        {children}
        <ScrollRestoration />
        <Scripts />
      </body>
    </html>
  );
}

export default function App() {
  return <Outlet />;
}

/**
 * Vises, når en side ikke findes, eller når der opstår en fejl, som siden ikke selv viser.
 */
export function ErrorBoundary({ error }: Route.ErrorBoundaryProps) {
  let title = "Der opstod en fejl";
  let details = "Noget gik galt. Prøv igen, eller gå til forsiden.";
  let stack: string | undefined;

  if (isRouteErrorResponse(error) && error.status === 404) {
    title = "Siden findes ikke";
    details = "Den side, du leder efter, findes ikke.";
  } else if (error instanceof ApiError) {
    details = error.message;
  } else if (import.meta.env.DEV && error instanceof Error) {
    details = error.message;
    stack = error.stack;
  }

  return (
    <main className="mx-auto max-w-2xl p-8">
      <h1 className="text-2xl font-semibold text-gray-900">{title}</h1>
      <p className="mt-2 text-gray-700">{details}</p>
      <div className="mt-6 flex gap-3">
        <button type="button" onClick={() => window.location.reload()} className={primaryButton}>
          Prøv igen
        </button>
        <Link to="/" className={secondaryButton}>
          Gå til forsiden
        </Link>
      </div>
      {stack && (
        <pre className="mt-6 overflow-x-auto rounded-md bg-gray-100 p-4 text-xs">
          <code>{stack}</code>
        </pre>
      )}
    </main>
  );
}

export function HydrateFallback() {
  return <p>Indlæser...</p>;
}