import { redirect } from "react-router";

export function clientLoader() {
  return redirect("/periods");
}

export default function Home() {
  return null;
}