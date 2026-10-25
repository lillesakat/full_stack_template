import { Outlet } from "react-router";
import { TabNav } from "~/components/TabNav";

const tabs = [
  { to: "/access/users", label: "Adgange" },
  { to: "/access/employments", label: "Ansættelser" },
];

export default function AccessPage() {
  return (
    <>
      <h1 className="mb-4 text-2xl font-semibold">Adgangsstyring</h1>
      <TabNav items={tabs} />
      <Outlet />
    </>
  );
}