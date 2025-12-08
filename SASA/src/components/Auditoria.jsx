function Auditoria() {
  const registro = [
    {
      id: 1,
      accion: "Notificación enviada",
      usuario: "Martin",
      fecha: "2025-12-01 08:21",
      destino: "Agente 01",
    },
    {
      id: 2,
      accion: "Silencio aplicado",
      usuario: "Admin",
      fecha: "2025-11-30 19:30",
      destino: "TIQ-004",
    },
  ];

  return (
    <div className="bg-slate-900/60 border border-slate-800 rounded-xl shadow-sm shadow-black/40 p-6 mt-6">
      <h2 className="text-xl font-semibold mb-4">Auditoría de Notificaciones</h2>

      <table className="min-w-full text-sm">
        <thead className="text-slate-400 text-xs border-b border-slate-800">
          <tr>
            <th className="py-2 text-left">Acción</th>
            <th className="py-2 text-left">Usuario</th>
            <th className="py-2 text-left">Destino</th>
            <th className="py-2 text-left">Fecha</th>
          </tr>
        </thead>

        <tbody>
          {registro.map((r) => (
            <tr
              key={r.id}
              className="border-b border-slate-800/50 text-slate-300"
            >
              <td className="py-2">{r.accion}</td>
              <td className="py-2">{r.usuario}</td>
              <td className="py-2">{r.destino}</td>
              <td className="py-2">{r.fecha}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Auditoria;
