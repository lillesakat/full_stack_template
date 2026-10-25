import { periodsApi } from "~/services/apiService";
import type { Route } from "./+types/periods";

export function meta() {
  return [{ title: "Perioder – Vagtplan" }];
}

export async function clientLoader() {
  return await periodsApi.getAll();
}

export default function PeriodsPage({ loaderData }: Route.ComponentProps) {
  return (
    <>
      <h1 className="mb-6 text-2xl font-semibold">Perioder</h1>
      <ul>
        {loaderData.map((period) => (
          <li key={period.id}>{period.name}</li>
        ))}
      </ul>
    </>
  );
}