import { NavLink, Outlet } from "react-router";

const menuItems = [
  { to: "/periods", label: "Perioder" },
  { to: "/organization", label: "Organisering" },
  { to: "/access", label: "Adgangsstyring" },
  { to: "/staff", label: "Personale" },
];

export default function AppLayout() {
  return (
    <div className="flex min-h-screen bg-gray-50 text-gray-900">
      <nav aria-label="Hovedmenu" className="w-56 shrink-0 border-r border-gray-200 bg-white p-4">
        <p className="mb-6 px-3 text-lg font-semibold">Vagtplan</p>
        <ul className="space-y-1">
          {menuItems.map((item) => (
            <li key={item.to}>
              <NavLink
                to={item.to}
                className={({ isActive }) =>
                  `block rounded-md px-3 py-2 text-sm font-medium ${
                    isActive ? "bg-blue-50 text-blue-700" : "text-gray-700 hover:bg-gray-100"
                  }`
                }
              >
                {item.label}
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>

      <main className="flex-1 p-8">
        <Outlet />
      </main>
    </div>
  );
}