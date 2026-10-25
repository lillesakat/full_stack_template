import { useId } from "react";

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
      <label htmlFor={id} className="mb-1 block text-sm font-medium text-gray-700">
        {label}
        {required && (
          <span aria-hidden="true" className="text-red-600">
            {" "}*
          </span>
        )}
      </label>
      <input
        id={id}
        name={name}
        type={type}
        required={required}
        defaultValue={defaultValue}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? errorId : undefined}
        className={`w-full rounded-md border px-3 py-2 text-sm ${error ? "border-red-500" : "border-gray-300"}`}
      />
      {error && (
        <p id={errorId} className="mt-1 text-sm text-red-600">
          {error}
        </p>
      )}
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