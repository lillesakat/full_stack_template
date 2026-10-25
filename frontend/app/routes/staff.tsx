import { useId } from "react";
import { useSearchParams } from "react-router";
import { ColorDot } from "~/components/ColorDot";
import { DataTable, type Column, type Filter } from "~/components/DataTable";
import { optionsFromValues } from "~/components/filterOptions";
import { periodsApi, staffApi } from "~/services/apiService";
import type { StaffMemberDto } from "~/types/api";
import { formatDate } from "~/utils/dates";
import { findDefaultPeriod } from "~/utils/periods";
import type { Route } from "./+types/staff";

export function meta() {
  return [{ title: "Personale – Vagtplan" }];
}

/**
 * Den valgte periode læses fra adressen (?periodId=3). Mangler den, vises standardperioden.
 */
export async function clientLoader({ request }: Route.ClientLoaderArgs) {
  const periods = await periodsApi.getAll();

  const requestedPeriodId = Number(new URL(request.url).searchParams.get("periodId"));
  const selectedPeriod = periods.find((period) => period.id === requestedPeriodId) ?? findDefaultPeriod(periods);

  const staff = selectedPeriod ? await staffApi.getForPeriod(selectedPeriod.id) : [];

  return { periods, selectedPeriod, staff };
}

const columns: Column<StaffMemberDto>[] = [
  { key: "name", header: "Navn", text: (member) => member.name },
  {
    key: "departmentName",
    header: "Afsnit",
    text: (member) => member.departmentName,
    render: (member) => (
      <span className="flex items-center gap-2">
        <ColorDot color={member.departmentColor} />
        {member.departmentName}
      </span>
    ),
  },
  { key: "staffGroupName", header: "Personalegruppe", text: (member) => member.staffGroupName },
  {
    key: "hoursPerWeek",
    header: "Timer pr. uge",
    text: (member) => String(member.hoursPerWeek),
    sortValue: (member) => member.hoursPerWeek,
  },
];

export default function StaffPage({ loaderData }: Route.ComponentProps) {
  const { periods, selectedPeriod, staff } = loaderData;
  const [, setSearchParams] = useSearchParams();
  const periodSelectId = useId();

  const filters: Filter<StaffMemberDto>[] = [
    {
      key: "department",
      label: "Afsnit",
      options: optionsFromValues(staff, (member) => member.departmentName),
    },
    {
      key: "staffGroup",
      label: "Personalegruppe",
      options: optionsFromValues(staff, (member) => member.staffGroupName),
    },
  ];

  return (
    <>
      <h1 className="mb-6 text-2xl font-semibold">Personale</h1>

      <div className="mb-6 flex flex-wrap items-end gap-4">
        <div>
          <label htmlFor={periodSelectId} className="mb-1 block text-sm font-medium text-gray-700">
            Periode
          </label>
          <select
            id={periodSelectId}
            value={selectedPeriod?.id ?? ""}
            onChange={(event) => setSearchParams({ periodId: event.target.value })}
            className="min-w-56 rounded-md border border-gray-300 bg-white px-3 py-2 text-sm"
          >
            {periods.length === 0 && <option value="">Der er ingen perioder</option>}
            {periods.map((period) => (
              <option key={period.id} value={period.id}>
                {period.name}
              </option>
            ))}
          </select>
        </div>
        {selectedPeriod && (
          <p className="pb-2 text-sm text-gray-600">
            {formatDate(selectedPeriod.startDate)} til {formatDate(selectedPeriod.endDate)}
          </p>
        )}
      </div>

      <DataTable
        rows={staff}
        columns={columns}
        filters={filters}
        getRowKey={(member) => member.employmentId}
        emptyMessage={
          selectedPeriod ? "Der er intet personale i den valgte periode." : "Opret en periode for at se personale."
        }
        defaultSort={{ key: "name", direction: "asc" }}
      />
    </>
  );
}