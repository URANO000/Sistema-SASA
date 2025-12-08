function Involucrados() {
  const eventos = [
    {
      id: 1,
      tiquete: "TIQ-001",
      usuario: "Martin",
      mensaje: "Agregó un comentario en el tiquete.",
      fecha: "2025-12-01 08:21",
      notificados: ["Agente 01", "Supervisor"],
      noNotificados: [],
    },
    {
      id: 2,
      tiquete: "TIQ-003",
      usuario: "Agente 02",
      mensaje: "Actualizó el estado a 'En progreso'.",
      fecha: "2025-12-01 07:45",
      notificados: ["Martin", "Supervisor"],
      noNotificados: ["Agente 03 (silenciado)"],
    },
  ];

  return (
    <div className="bg-slate-900/60 border border-slate-800 rounded-xl shadow-sm shadow-black/40 p-6 mt-6">
      <h2 className="text-xl font-semibold mb-2">
        Notificación a los involucrados
      </h2>

      <div className="space-y-4">
        {eventos.map((e) => (
          <div key={e.id} className="flex gap-3">
            <div className="flex flex-col items-center mt-1">
              <div className="h-3 w-3 rounded-full bg-[#4EBBD6]" />
              <div className="flex-1 w-px bg-slate-700" />
            </div>
            <div className="flex-1 border border-slate-700 rounded-lg p-4 bg-slate-950/60">
              <div className="flex justify-between items-center">
                <div>
                  <p className="text-xs text-slate-400">
                    Tiquete <span className="font-semibold">{e.tiquete}</span>
                  </p>
                  <p className="text-sm font-semibold text-slate-100">
                    {e.usuario} · {e.mensaje}
                  </p>
                </div>
                <span className="text-[11px] text-slate-500">{e.fecha}</span>
              </div>

              <div className="mt-3 text-xs text-slate-300 space-y-1">
                <p>
                  <span className="font-semibold text-slate-200">
                    Notificados:
                  </span>{" "}
                  {e.notificados.join(", ")}
                </p>

                {e.noNotificados.length > 0 && (
                  <p>
                    <span className="font-semibold text-[#FF1A3B]">
                      No notificados (silencio activo):
                    </span>{" "}
                    {e.noNotificados.join(", ")}
                  </p>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default Involucrados;
