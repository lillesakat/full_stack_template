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

/**
 * Et tomt felt bliver til null, så backend kan svare med "påkrævet".
 */
export function getOptionalNumber(formData: FormData, name: string): number | null {
  const value = getText(formData, name);
  return value === "" ? null : Number(value);
}

/**
 * Et afkrydsningsfelt sendes kun med, når det er markeret, og har så værdien "on".
 */
export function getCheckbox(formData: FormData, name: string): boolean {
  return formData.get(name) === "on";
}

export function getId(formData: FormData): number {
  return Number(formData.get("id"));
}