import { redirect } from "react-router";

export function clientLoader() {
  return redirect("/organization/departments");
}

export default function OrganizationIndex() {
  return null;
}