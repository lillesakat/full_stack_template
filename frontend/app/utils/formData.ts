
export function getText(formData: FormData, name: string): string {
  const value = formData.get(name);
  return typeof value === "string" ? value.trim() : "";
}

/**
 * Et tomt felt bliver til null, så backend kan svare med "påkrævet".
 */
export function getOptionalText(formData: FormData, name: string): string | null {
  const value = getText(formData, name);
  return value === "" ? null : value;
}

export function getId(formData: FormData): number {
  return Number(formData.get("id"));
}