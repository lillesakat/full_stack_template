
/**
 * Dato i formatet ÅÅÅÅ-MM-DD, som API'et sender, og som HTML's datofelter bruger.
 */
export type DateString = string;

export type PeriodDto = {
  id: number;
  name: string;
  startDate: DateString;
  endDate: DateString;
};

export type SavePeriodRequest = {
  name: string;
  startDate: DateString | null;
  endDate: DateString | null;
};

export type DepartmentDto = {
  id: number;
  name: string;
  color: string;
};

export type SaveDepartmentRequest = Omit<DepartmentDto, "id">;

export type StaffGroupDto = {
  id: number;
  name: string;
};

export type SaveStaffGroupRequest = Omit<StaffGroupDto, "id">;

export type UserDto = {
  id: number;
  name: string;
  email: string;
  isAdmin: boolean;
  isStaff: boolean;
};

export type SaveUserRequest = Omit<UserDto, "id">;

export type EmploymentDto = {
  id: number;
  userId: number;
  userName: string;
  departmentId: number;
  departmentName: string;
  departmentColor: string;
  staffGroupId: number;
  staffGroupName: string;
  hoursPerWeek: number;
  startDate: DateString;
  endDate: DateString;
};

export type SaveEmploymentRequest = {
  userId: number | null;
  departmentId: number | null;
  staffGroupId: number | null;
  hoursPerWeek: number | null;
  startDate: DateString | null;
  endDate: DateString | null;
};

export type StaffMemberDto = {
  employmentId: number;
  name: string;
  departmentName: string;
  departmentColor: string;
  staffGroupName: string;
  hoursPerWeek: number;
};