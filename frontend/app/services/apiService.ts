import { request } from "./apiClient";
import type {
  DepartmentDto,
  EmploymentDto,
  PeriodDto,
  SaveDepartmentRequest,
  SaveEmploymentRequest,
  SavePeriodRequest,
  SaveStaffGroupRequest,
  SaveUserRequest,
  StaffGroupDto,
  StaffMemberDto,
  UserDto,
} from "~/types/api";

/**
 * Laver de fire standardkald for en ressource, der følger samme REST-mønster i backend.
 */
function createCrudApi<TDto, TSaveRequest>(resource: string) {
  return {
    getAll: () => request<TDto[]>(`/${resource}`),
    create: (data: TSaveRequest) => request<TDto>(`/${resource}`, "POST", data),
    update: (id: number, data: TSaveRequest) => request<TDto>(`/${resource}/${id}`, "PUT", data),
    remove: (id: number) => request<void>(`/${resource}/${id}`, "DELETE"),
  };
}

export const periodsApi = createCrudApi<PeriodDto, SavePeriodRequest>("periods");
export const departmentsApi = createCrudApi<DepartmentDto, SaveDepartmentRequest>("departments");
export const staffGroupsApi = createCrudApi<StaffGroupDto, SaveStaffGroupRequest>("staff-groups");
export const usersApi = createCrudApi<UserDto, SaveUserRequest>("users");
export const employmentsApi = createCrudApi<EmploymentDto, SaveEmploymentRequest>("employments");

export const staffApi = {
  getForPeriod: (periodId: number) => request<StaffMemberDto[]>(`/periods/${periodId}/staff`),
};