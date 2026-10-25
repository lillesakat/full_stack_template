import type { DepartmentDto, EmploymentDto, StaffGroupDto, UserDto } from "~/types/api";
import { FormDialog } from "./FormDialog";
import { FormField, SelectField } from "./FormField";

type EmploymentFormDialogProps = {
  employment?: EmploymentDto;
  users: UserDto[];
  departments: DepartmentDto[];
  staffGroups: StaffGroupDto[];
  onClose: () => void;
};

/**
 * Opretter en ny ansættelse, eller redigerer den ansættelse, der gives med.
 * Kun brugere med rollen Personale kan vælges, da kun de kan have ansættelser.
 */
export function EmploymentFormDialog({ employment, users, departments, staffGroups, onClose }: EmploymentFormDialogProps) {
  const personOptions = users
    .filter((user) => user.isStaff)
    .map((user) => ({ value: user.id, label: `${user.name} (${user.email})` }));
  const departmentOptions = departments.map((department) => ({ value: department.id, label: department.name }));
  const staffGroupOptions = staffGroups.map((staffGroup) => ({ value: staffGroup.id, label: staffGroup.name }));

  return (
    <FormDialog
      title={employment ? "Rediger ansættelse" : "Opret ansættelse"}
      submitLabel={employment ? "Gem ændringer" : "Opret"}
      intent={employment ? "update" : "create"}
      id={employment?.id}
      onClose={onClose}
    >
      {(fieldErrors) => (
        <>
          <SelectField
            label="Person"
            name="userId"
            placeholder="Vælg person"
            options={personOptions}
            required
            defaultValue={employment?.userId}
            error={fieldErrors.userId}
          />
          <SelectField
            label="Afsnit"
            name="departmentId"
            placeholder="Vælg afsnit"
            options={departmentOptions}
            required
            defaultValue={employment?.departmentId}
            error={fieldErrors.departmentId}
          />
          <SelectField
            label="Personalegruppe"
            name="staffGroupId"
            placeholder="Vælg personalegruppe"
            options={staffGroupOptions}
            required
            defaultValue={employment?.staffGroupId}
            error={fieldErrors.staffGroupId}
          />
          <FormField
            label="Timer pr. uge"
            name="hoursPerWeek"
            type="number"
            required
            defaultValue={employment?.hoursPerWeek}
            error={fieldErrors.hoursPerWeek}
          />
          <FormField
            label="Startdato"
            name="startDate"
            type="date"
            required
            defaultValue={employment?.startDate}
            error={fieldErrors.startDate}
          />
          <FormField
            label="Slutdato"
            name="endDate"
            type="date"
            required
            defaultValue={employment?.endDate}
            error={fieldErrors.endDate}
          />
        </>
      )}
    </FormDialog>
  );
}