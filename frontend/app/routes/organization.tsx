import { Outlet } from "react-router";
import { TabNav } from "~/components/TabNav";

const tabs = [
  { to: "/organization/departments", label: "Afsnit" },
  { to: "/organization/staff-groups", label: "Personalegrupper" },
];

export default function OrganizationPage() {
  return (
    <>
      <h1 className="mb-4 text-2xl font-semibold">Organisering</h1>
      <TabNav items={tabs} />
      <Outlet />
    </>
  );
}