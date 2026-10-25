import { useState, type ReactNode } from "react";

export type Column<T> = {
  key: string;
  header: string;
  text: (row: T) => string;
  render?: (row: T) => ReactNode;
  sortValue?: (row: T) => string | number;
};

export type FilterOption<T> = {
  label: string;
  matches: (row: T) => boolean;
};

export type Filter<T> = {
  key: string;
  label: string;
  options: FilterOption<T>[];
};

export type SortState = {
  key: string;
  direction: "asc" | "desc";
};

type DataTableProps<T> = {
  rows: T[];
  columns: Column<T>[];
  getRowKey: (row: T) => string | number;
  emptyMessage: string;
  searchable?: boolean;
  filters?: Filter<T>[];
  defaultSort?: SortState;
  actions?: (row: T) => ReactNode;
};

/**
 * Tabel med søgning, sortering og dropdown-filtre. Alt sker i browseren,
 * da hver tabel kun indeholder få rækker.
 */
export function DataTable<T>({
  rows,
  columns,
  getRowKey,
  emptyMessage,
  searchable = true,
  filters = [],
  defaultSort,
  actions,
}: DataTableProps<T>) {
  const [search, setSearch] = useState("");
  const [selectedFilters, setSelectedFilters] = useState<Record<string, string>>({});
  const [sort, setSort] = useState<SortState | undefined>(defaultSort);

  const matchingRows = rows.filter(
    (row) => matchesSearch(row, columns, search) && matchesFilters(row, filters, selectedFilters),
  );
  const visibleRows = sortRows(matchingRows, columns, sort);

  function toggleSort(key: string) {
    setSort((current) =>
      current?.key === key && current.direction === "asc"
        ? { key, direction: "desc" }
        : { key, direction: "asc" },
    );
  }

  function selectFilter(key: string, label: string) {
    setSelectedFilters((current) => ({ ...current, [key]: label }));
  }

  return (
    <div>
      <div className="mb-4 flex flex-wrap items-end gap-3">
        {searchable && (
          <input
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Søg..."
            aria-label="Søg"
            className="w-64 rounded-md border border-gray-300 bg-white px-3 py-2 text-sm"
          />
        )}

        {filters.map((filter) => (
          <label key={filter.key} className="flex flex-col text-sm text-gray-700">
            {filter.label}
            <select
              value={selectedFilters[filter.key] ?? ""}
              onChange={(event) => selectFilter(filter.key, event.target.value)}
              className="mt-1 rounded-md border border-gray-300 bg-white px-3 py-2"
            >
              <option value="">Alle</option>
              {filter.options.map((option) => (
                <option key={option.label} value={option.label}>
                  {option.label}
                </option>
              ))}
            </select>
          </label>
        ))}
      </div>

      <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-gray-200 bg-gray-50 text-gray-600">
            <tr>
              {columns.map((column) => (
                <th
                  key={column.key}
                  scope="col"
                  aria-sort={ariaSort(sort, column.key)}
                  className="px-4 py-3 font-medium"
                >
                  <button
                    type="button"
                    onClick={() => toggleSort(column.key)}
                    className="flex items-center gap-1 hover:text-gray-900"
                  >
                    {column.header}
                    <span aria-hidden="true" className={sort?.key === column.key ? "" : "text-gray-300"}>
  {                     sortIndicator(sort, column.key)}
                    </span>
                  </button>
                </th>
              ))}
              {actions && (
                <th scope="col" className="px-4 py-3">
                  <span className="sr-only">Handlinger</span>
                </th>
              )}
            </tr>
          </thead>

          <tbody className="divide-y divide-gray-100">
            {visibleRows.map((row) => (
              <tr key={getRowKey(row)} className="hover:bg-gray-50">
                {columns.map((column) => (
                  <td key={column.key} className="px-4 py-3">
                    {column.render ? column.render(row) : column.text(row)}
                  </td>
                ))}
                {actions && <td className="px-4 py-3 text-right">{actions(row)}</td>}
              </tr>
            ))}
          </tbody>
        </table>

        {visibleRows.length === 0 && (
          <p className="px-4 py-6 text-center text-sm text-gray-500">
            {rows.length === 0 ? emptyMessage : "Ingen rækker matcher søgningen eller filtrene."}
          </p>
        )}
      </div>
    </div>
  );
}

/**
 * Laver en filtermulighed for hver forskellig værdi
 */
export function optionsFromValues<T>(rows: T[], getValue: (row: T) => string): FilterOption<T>[] {
  const values = [...new Set(rows.map(getValue))].sort((a, b) => a.localeCompare(b, "da"));
  return values.map((value) => ({ label: value, matches: (row) => getValue(row) === value }));
}

function matchesSearch<T>(row: T, columns: Column<T>[], search: string) {
  const query = search.trim().toLocaleLowerCase("da");
  if (query === "") return true;

  return columns.some((column) => column.text(row).toLocaleLowerCase("da").includes(query));
}

function matchesFilters<T>(row: T, filters: Filter<T>[], selectedFilters: Record<string, string>) {
  return filters.every((filter) => {
    const selectedOption = filter.options.find((option) => option.label === selectedFilters[filter.key]);
    return selectedOption === undefined || selectedOption.matches(row);
  });
}

function sortRows<T>(rows: T[], columns: Column<T>[], sort: SortState | undefined) {
  const column = columns.find((c) => c.key === sort?.key);
  if (!sort || !column) return rows;

  const getValue = column.sortValue ?? column.text;
  const direction = sort.direction === "asc" ? 1 : -1;

  return [...rows].sort((a, b) => compareValues(getValue(a), getValue(b)) * direction);
}

function compareValues(a: string | number, b: string | number) {
  if (typeof a === "number" && typeof b === "number") return a - b;
  return String(a).localeCompare(String(b), "da");
}

function ariaSort(sort: SortState | undefined, key: string): "none" | "ascending" | "descending" {
  if (sort?.key !== key) return "none";
  return sort.direction === "asc" ? "ascending" : "descending";
}

function sortIndicator(sort: SortState | undefined, key: string) {
  if (sort?.key !== key) return "↕";
  return sort.direction === "asc" ? "▲" : "▼";
}