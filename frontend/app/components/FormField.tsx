import { useId } from "react";

export function RequiredMark() {
  return (
    <span aria-hidden="true" className="text-red-600">
      {" "}*
    </span>
  );
}

type FieldLabelProps = {
  htmlFor: string;
  label: string;
  required: boolean;
};

export function FieldLabel({ htmlFor, label, required }: FieldLabelProps) {
  return (
    <label htmlFor={htmlFor} className="mb-1 block text-sm font-medium text-gray-700">
      {label}
      {required && <RequiredMark />}
    </label>
  );
}

type FieldErrorProps = {
  id: string;
  message?: string;
};

export function FieldError({ id, message }: FieldErrorProps) {
  if (!message) return null;

  return (
    <p id={id} className="mt-1 text-sm text-red-600">
      {message}
    </p>
  );
}

export function inputClassName(hasError: boolean) {
  return `w-full rounded-md border bg-white px-3 py-2 text-sm ${hasError ? "border-red-500" : "border-gray-300"}`;
}

type FormFieldProps = {
  label: string;
  name: string;
  type?: "text" | "date" | "email" | "number";
  required?: boolean;
  defaultValue?: string | number;
  error?: string;
};

export function FormField({ label, name, type = "text", required = false, defaultValue, error }: FormFieldProps) {
  const id = useId();
  const errorId = `${id}-error`;

  return (
    <div className="mb-4">
      <FieldLabel htmlFor={id} label={label} required={required} />
      <input
        id={id}
        name={name}
        type={type}
        required={required}
        defaultValue={defaultValue}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? errorId : undefined}
        className={inputClassName(Boolean(error))}
      />
      <FieldError id={errorId} message={error} />
    </div>
  );
}

export type SelectOption = {
  value: number;
  label: string;
};

type SelectFieldProps = {
  label: string;
  name: string;
  options: SelectOption[];
  placeholder: string;
  required?: boolean;
  defaultValue?: number;
  error?: string;
};

export function SelectField({ label, name, options, placeholder, required = false, defaultValue, error }: SelectFieldProps) {
  const id = useId();
  const errorId = `${id}-error`;

  return (
    <div className="mb-4">
      <FieldLabel htmlFor={id} label={label} required={required} />
      <select
        id={id}
        name={name}
        required={required}
        defaultValue={defaultValue ?? ""}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? errorId : undefined}
        className={inputClassName(Boolean(error))}
      >
        <option value="">{placeholder}</option>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
      <FieldError id={errorId} message={error} />
    </div>
  );
}

type CheckboxFieldProps = {
  label: string;
  name: string;
  defaultChecked?: boolean;
  error?: string;
};

export function CheckboxField({ label, name, defaultChecked = false, error }: CheckboxFieldProps) {
  const id = useId();
  const errorId = `${id}-error`;

  return (
    <div className="mb-2">
      <div className="flex items-center gap-2">
        <input
          id={id}
          name={name}
          type="checkbox"
          defaultChecked={defaultChecked}
          aria-invalid={error ? true : undefined}
          aria-describedby={error ? errorId : undefined}
          className="h-4 w-4"
        />
        <label htmlFor={id} className="text-sm text-gray-700">
          {label}
        </label>
      </div>
      <FieldError id={errorId} message={error} />
    </div>
  );
}

export function RequiredFieldsNote() {
  return <p className="mb-4 text-sm text-gray-500">Felter markeret med * skal udfyldes.</p>;
}

export function FormError({ message }: { message: string }) {
  return (
    <p role="alert" className="mb-4 rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">
      {message}
    </p>
  );
}