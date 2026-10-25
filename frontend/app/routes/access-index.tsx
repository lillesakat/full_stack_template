import { redirect } from "react-router";

export function clientLoader() {
  return redirect("/access/users");
}

export default function AccessIndex() {
  return null;
}