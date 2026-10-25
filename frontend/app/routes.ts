import { type RouteConfig, index, layout, route } from "@react-router/dev/routes";

export default [
  layout("layouts/app-layout.tsx", [
    index("routes/home.tsx"),
    route("periods", "routes/periods.tsx"),
    route("organization", "routes/organization.tsx", [
      index("routes/organization-index.tsx"),
      route("departments", "routes/departments.tsx"),
      route("staff-groups", "routes/staff-groups.tsx"),
    ]),
    route("access", "routes/access.tsx", [
      index("routes/access-index.tsx"),
      route("users", "routes/users.tsx"),
      route("employments", "routes/employments.tsx"),
    ]),
    route("staff", "routes/staff.tsx"),
  ]),
] satisfies RouteConfig;