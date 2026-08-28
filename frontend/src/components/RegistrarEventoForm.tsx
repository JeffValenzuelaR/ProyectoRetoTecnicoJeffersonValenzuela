import { useState } from "react";
import type { Zone } from "../types/event";
import { createEvent } from "../api/eventClient";

interface ZoneErrors {
  name?: string;
  price?: string;
  capacity?: string;
}

interface FormErrors {
  name?: string;
  date?: string;
  venue?: string;
  zones: ZoneErrors[];
}

const emptyZone: Zone = { name: "", price: 0, capacity: 0 };

function validate(
  name: string,
  date: string,
  venue: string,
  zones: Zone[]
): FormErrors {
  const errors: FormErrors = { zones: zones.map(() => ({})) };

  if (!name.trim()) errors.name = "El nombre del evento es obligatorio.";
  if (!date.trim()) errors.date = "La fecha es obligatoria.";
  if (!venue.trim()) errors.venue = "El lugar es obligatorio.";

  zones.forEach((zone, index) => {
    const zoneErrors: ZoneErrors = {};
    if (!zone.name.trim()) zoneErrors.name = "Nombre de zona requerido.";
    if (zone.capacity <= 0) zoneErrors.capacity = "Debe ser mayor a 0.";
    if (zone.price < 0) zoneErrors.price = "No puede ser negativo.";
    errors.zones[index] = zoneErrors;
  });

  return errors;
}

function hasErrors(errors: FormErrors): boolean {
  if (errors.name || errors.date || errors.venue) return true;
  return errors.zones.some(
    (z) => z.name || z.price || z.capacity
  );
}

export default function RegistrarEventoForm() {
  const [name, setName] = useState("");
  const [date, setDate] = useState("");
  const [venue, setVenue] = useState("");
  const [zones, setZones] = useState<Zone[]>([{ ...emptyZone }]);
  const [errors, setErrors] = useState<FormErrors>({ zones: [{}] });
  const [loading, setLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  function updateZone(index: number, patch: Partial<Zone>) {
    setZones((prev) =>
      prev.map((z, i) => (i === index ? { ...z, ...patch } : z))
    );
  }

  function addZone() {
    setZones((prev) => [...prev, { ...emptyZone }]);
  }

  function removeZone(index: number) {
    setZones((prev) => prev.filter((_, i) => i !== index));
  }

  function resetForm() {
    setName("");
    setDate("");
    setVenue("");
    setZones([{ ...emptyZone }]);
    setErrors({ zones: [{}] });
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSuccessMessage(null);
    setErrorMessage(null);

    const validation = validate(name, date, venue, zones);
    setErrors(validation);
    if (hasErrors(validation)) {
      return;
    }

    setLoading(true);
    try {
      const created = await createEvent({
        name,
        date: new Date(date).toISOString(),
        venue,
        zones,
      });
      setSuccessMessage(`Evento "${created.name}" registrado correctamente.`);
      resetForm();
    } catch (err) {
      if (axiosErrorMessage(err)) {
        setErrorMessage(axiosErrorMessage(err)!);
      } else {
        setErrorMessage(
          "No se pudo registrar el evento. Intenta nuevamente."
        );
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="mx-auto max-w-2xl space-y-6 rounded-xl border border-slate-200 bg-white p-8 shadow-sm"
    >
      <div>
        <h1 className="text-2xl font-semibold text-slate-900">
          Registrar Evento
        </h1>
        <p className="mt-1 text-sm text-slate-500">
          Completa los datos del evento y sus zonas de venta.
        </p>
      </div>

      {successMessage && (
        <div className="rounded-md border border-green-200 bg-green-50 px-4 py-3 text-sm text-green-800">
          {successMessage}
        </div>
      )}
      {errorMessage && (
        <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {errorMessage}
        </div>
      )}

      <div className="space-y-4">
        <Field label="Nombre del evento" error={errors.name}>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
            placeholder="Concierto de Rock en el Parque"
          />
        </Field>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Field label="Fecha" error={errors.date}>
            <input
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
              className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
            />
          </Field>

          <Field label="Lugar" error={errors.venue}>
            <input
              type="text"
              value={venue}
              onChange={(e) => setVenue(e.target.value)}
              className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              placeholder="Estadio Nacional"
            />
          </Field>
        </div>
      </div>

      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <h2 className="text-sm font-medium text-slate-700">Zonas</h2>
          <button
            type="button"
            onClick={addZone}
            className="text-sm font-medium text-indigo-600 hover:text-indigo-700"
          >
            + Agregar zona
          </button>
        </div>

        {zones.map((zone, index) => (
          <div
            key={index}
            className="grid grid-cols-1 gap-3 rounded-md border border-slate-200 p-3 sm:grid-cols-[1fr_1fr_1fr_auto]"
          >
            <Field label="Nombre" error={errors.zones[index]?.name} compact>
              <input
                type="text"
                value={zone.name}
                onChange={(e) =>
                  updateZone(index, { name: e.target.value })
                }
                className="w-full rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
                placeholder="Platea"
              />
            </Field>
            <Field label="Precio" error={errors.zones[index]?.price} compact>
              <input
                type="number"
                min={0}
                step="0.01"
                value={zone.price}
                onChange={(e) =>
                  updateZone(index, { price: Number(e.target.value) })
                }
                className="w-full rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              />
            </Field>
            <Field
              label="Capacidad"
              error={errors.zones[index]?.capacity}
              compact
            >
              <input
                type="number"
                min={0}
                value={zone.capacity}
                onChange={(e) =>
                  updateZone(index, { capacity: Number(e.target.value) })
                }
                className="w-full rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              />
            </Field>
            <div className="flex items-end">
              <button
                type="button"
                onClick={() => removeZone(index)}
                disabled={zones.length === 1}
                className="rounded-md px-3 py-1.5 text-sm font-medium text-red-600 hover:bg-red-50 disabled:cursor-not-allowed disabled:text-slate-300 disabled:hover:bg-transparent"
              >
                Quitar
              </button>
            </div>
          </div>
        ))}
      </div>

      <button
        type="submit"
        disabled={loading}
        className="w-full rounded-md bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-indigo-700 disabled:cursor-not-allowed disabled:bg-indigo-300"
      >
        {loading ? "Guardando..." : "Guardar"}
      </button>
    </form>
  );
}

function Field({
  label,
  error,
  children,
  compact = false,
}: {
  label: string;
  error?: string;
  children: React.ReactNode;
  compact?: boolean;
}) {
  return (
    <label className="block">
      <span
        className={`mb-1 block font-medium text-slate-700 ${
          compact ? "text-xs" : "text-sm"
        }`}
      >
        {label}
      </span>
      {children}
      {error && <span className="mt-1 block text-xs text-red-600">{error}</span>}
    </label>
  );
}

function axiosErrorMessage(err: unknown): string | null {
  if (
    typeof err === "object" &&
    err !== null &&
    "response" in err &&
    typeof (err as { response?: { data?: { message?: string } } }).response
      ?.data?.message === "string"
  ) {
    return (err as { response: { data: { message: string } } }).response.data
      .message;
  }
  return null;
}
