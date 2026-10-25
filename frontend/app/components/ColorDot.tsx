type ColorDotProps = {
  color: string;
};

export function ColorDot({ color }: ColorDotProps) {
  return (
    <span
      aria-hidden="true"
      className="inline-block h-3 w-3 shrink-0 rounded-full border border-gray-300"
      style={{ backgroundColor: color }}
    />
  );
}