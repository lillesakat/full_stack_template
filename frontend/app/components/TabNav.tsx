import { NavLink } from "react-router";

export type TabItem = {
  to: string;
  label: string;
};

type TabNavProps = {
  items: TabItem[];
};

export function TabNav({ items }: TabNavProps) {
  return (
    <nav aria-label="Faner" className="mb-6 flex gap-1 border-b border-gray-200">
      {items.map((item) => (
        <NavLink
          key={item.to}
          to={item.to}
          className={({ isActive }) =>
            `-mb-px border-b-2 px-4 py-2 text-sm font-medium ${
              isActive
                ? "border-blue-600 text-blue-700"
                : "border-transparent text-gray-600 hover:text-gray-900"
            }`
          }
        >
          {item.label}
        </NavLink>
      ))}
    </nav>
  );
}