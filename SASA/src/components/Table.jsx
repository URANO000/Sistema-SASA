import { useState } from "react";

function Table({ onDeleteTicket }) {
  const handleDelete = onDeleteTicket || (() => {});

  const rows = [
    {
      id: "TIQ-001",
      titulo: "Consulta general de servicio",
      estado: "Abierto",
      prioridad: "Alta",
      fecha: "2025-12-01",
      responsable: "Agente 01",
    },
    {
      id: "TIQ-002",
      titulo: "Actualización de datos del asociado",
      estado: "En progreso",
      prioridad: "Media",
      fecha: "2025-12-02",
      responsable: "Agente 02",
    },
    {
      id: "TIQ-003",
      titulo: "Reclamo por cobro",
      estado: "Pendiente",
      prioridad: "Alta",
      fecha: "2025-12-03",
      responsable: "Agente 03",
    },
    {
      id: "TIQ-004",
      titulo: "Seguimiento de caso previo",
      estado: "Cerrado",
      prioridad: "Baja",
      fecha: "2025-11-30",
      responsable: "Agente 01",
    },
    {
      id: "TIQ-005",
      titulo: "Cambio de producto contratado",
      estado: "Abierto",
      prioridad: "Media",
      fecha: "2025-11-29",
      responsable: "Agente 04",
    },
    {
      id: "TIQ-006",
      titulo: "Revisión de condiciones",
      estado: "En progreso",
      prioridad: "Baja",
      fecha: "2025-11-28",
      responsable: "Agente 02",
    },
  ];

  const pageSize = 4;
  const [page, setPage] = useState(1);

  const totalPages = Math.ceil(rows.length / pageSize);

  const startIndex = (page - 1) * pageSize;
  const currentRows = rows.slice(startIndex, startIndex + pageSize);

  const canPrev = page > 1;
  const canNext = page < totalPages;

  const handlePrev = () => canPrev && setPage((p) => p - 1);
  const handleNext = () => canNext && setPage((p) => p + 1);

  const handleGoTo = (p) => {
    if (p >= 1 && p <= totalPages) setPage(p);
  };

  return (
    <div className="bg-slate-900/60 border border-slate-800 rounded-xl overflow-hidden shadow-sm shadow-black/40">
      <div className="p-4 border-b border-slate-800 space-y-3">
        <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <div className="relative w-full md:max-w-xs">
            <input
              type="text"
              className="w-full rounded-lg border border-slate-700 bg-slate-950/80 px-3 py-2 pr-8 text-sm text-slate-100 placeholder:text-slate-500 focus:outline-none focus:ring-1 focus:ring-[#4EBBD6] focus:border-[#4EBBD6]"
              placeholder="Buscar por número de caso, título o responsable"
              readOnly
            />
            <span className="pointer-events-none absolute inset-y-0 right-2 flex items-center text-slate-500 text-sm">
              🔍
            </span>
          </div>

          <div className="flex flex-wrap gap-2 text-xs">
            <select className="min-w-[150px] rounded-lg border border-slate-700 bg-slate-950/80 px-3 py-2 text-slate-100" disabled>
              <option>Estado: Todos</option>
            </select>
            <select className="min-w-[150px] rounded-lg border border-slate-700 bg-slate-950/80 px-3 py-2 text-slate-100" disabled>
              <option>Prioridad: Todas</option>
            </select>
            <select className="min-w-[150px] rounded-lg border border-slate-700 bg-slate-950/80 px-3 py-2 text-slate-100" disabled>
              <option>Ordenar por: Fecha desc</option>
            </select>
          </div>
        </div>

        <p className="text-[11px] text-slate-500">
          Búsqueda · Filtros combinados · Ordenamiento (simulación visual).
        </p>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full text-sm">
          <thead className="bg-slate-900/80 text-sm uppercase tracking-wide text-slate-400">
            <tr>
              <th className="px-4 py-3 text-left">Caso</th>
              <th className="px-4 py-3 text-left">Título</th>
              <th className="px-4 py-3 text-left">Estado</th>
              <th className="px-4 py-3 text-left">Prioridad</th>
              <th className="px-4 py-3 text-left">Fecha</th>
              <th className="px-4 py-3 text-left">Responsable</th>
              <th className="px-4 py-3 text-left">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {currentRows.map((row, idx) => (
              <tr
                key={row.id}
                className={
                  idx % 2 === 0
                    ? "bg-slate-900/40"
                    : "bg-slate-900/10 border-t border-slate-800/60"
                }
              >
                <td className="px-4 py-3 text-sm font-semibold text-slate-100">
                  {row.id}
                </td>
                <td className="px-4 py-3 text-sm text-slate-200">
                  {row.titulo}
                </td>
                <td className="px-4 py-3 text-sm">
                  <span
                    className={[
                      "inline-flex items-center rounded-full px-2 py-0.5 text-[11px] font-medium",
                      row.estado === "Abierto"
                        ? "bg-emerald-500/15 text-emerald-300 border border-emerald-500/40"
                        : row.estado === "En progreso"
                        ? "bg-[#4EBBD6]/15 text-[#4EBBD6] border border-[#4EBBD6]/40"
                        : row.estado === "Pendiente"
                        ? "bg-amber-500/15 text-amber-300 border border-amber-500/40"
                        : "bg-slate-600/20 text-slate-200 border border-slate-500/40",
                    ].join(" ")}
                  >
                    {row.estado}
                  </span>
                </td>
                <td className="px-4 py-3 text-sm text-slate-200">
                  {row.prioridad}
                </td>
                <td className="px-4 py-3 text-sm text-slate-300">
                  {row.fecha}
                </td>
                <td className="px-4 py-3 text-sm text-slate-200">
                  {row.responsable}
                </td>
                <td className="px-4 py-3 text-sm">
                  <button
                    type="button"
                    onClick={() => handleDelete(row.id)}
                    className="px-3 py-1 rounded-lg text-xs font-semibold bg-[#B7041A] text-white hover:bg-[#9a0416]"
                  >
                    Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <div className="flex items-center justify-between px-4 py-3 border-t border-slate-800 text-xs text-slate-400">
        <span>
          Mostrando <span className="text-slate-200">{currentRows.length}</span>{" "}
          de <span className="text-slate-200">{rows.length}</span> registros
        </span>

        <div className="flex items-center gap-2">
          <button
            onClick={handlePrev}
            disabled={!canPrev}
            className={`px-3 py-1 rounded-lg border text-xs ${
              canPrev
                ? "border-slate-700 hover:border-[#4EBBD6] hover:text-[#4EBBD6]"
                : "border-slate-800 text-slate-600 cursor-not-allowed"
            }`}
          >
            Anterior
          </button>

          {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
            <button
              key={p}
              onClick={() => handleGoTo(p)}
              className={`w-7 h-7 rounded-lg text-xs ${
                p === page
                  ? "bg-[#B7041A] text-white"
                  : "bg-slate-900 text-slate-300 hover:bg-slate-800"
              }`}
            >
              {p}
            </button>
          ))}

          <button
            onClick={handleNext}
            disabled={!canNext}
            className={`px-3 py-1 rounded-lg border text-xs ${
              canNext
                ? "border-slate-700 hover:border-[#4EBBD6] hover:text-[#4EBBD6]"
                : "border-slate-800 text-slate-600 cursor-not-allowed"
            }`}
          >
            Siguiente
          </button>
        </div>
      </div>
    </div>
  );
}

export default Table;
