import { useId, useState } from "react";

const fallbackColor = "#000000";
const hexColorPattern = /^#[0-9A-Fa-f]{6}$/;

type ColorFieldProps = {
  label: string;
  name: string;
  required?: boolean;
  defaultValue?: string;
  error?: string;
};

/**
 * Farvevælger og HEX-felt, der holder hinanden opdateret. Kun HEX-feltet sendes med formularen,
 * så det er præcis den viste kode, der gemmes og valideres i backend.
 */
export function ColorField({ label, name, required = false, defaultValue = fallbackColor, error }: ColorFieldProps) {
  const [color, setColor] = useState(defaultValue);
  const id = useId();
  const errorId = `${id}-error`;
  const pickerValue = hexColorPattern.test(color) ? color.toLowerCase() : fallbackColor;

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
      <div className="flex items-center gap-2">
        <input
          type="color"
          value={pickerValue}
          onChange={(event) => setColor(event.target.value.toUpperCase())}
          aria-label={`${label}: vælg visuelt`}
          className="h-10 w-14 shrink-0 cursor-pointer rounded-md border border-gray-300 bg-white p-1"
        />
        <input
          id={id}
          name={name}
          type="text"
          required={required}
          value={color}
          onChange={(event) => setColor(event.target.value)}
          aria-invalid={error ? true : undefined}
          aria-describedby={error ? errorId : undefined}
          className={`w-full rounded-md border px-3 py-2 font-mono text-sm ${error ? "border-red-500" : "border-gray-300"}`}
        />
      </div>
      {error && (
        <p id={errorId} className="mt-1 text-sm text-red-600">
          {error}
        </p>
      )}
    </div>
  );
}