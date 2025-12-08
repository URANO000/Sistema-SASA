import { useState } from "react";

function Notificaciones() {
  const data = [
    {
      id: 1,
      titulo: "Nuevo comentario en TIQ-001",
      detalle: "Martin agregó un comentario",
      fecha: "2025-12-01 08:21",
      leida: false,
      tipo: "comentario",
      silenciado: false,
    },
    {
      id: 2,
      titulo: "Actualización de estado",
      detalle: "TIQ-003 pasó a 'En progreso'",
      fecha: "2025-12-01 07:10",
      leida: true,
      tipo: "estado",
      silenciado: false,
    },
    {
      id: 3,
      titulo: "Tiquete TIQ-004 silenciado",
      detalle: "Silenciado por 8 horas",
      fecha: "2025-11-30 19:30",
      leida: true,
      tipo: "silencio",
      silenciado: true,
    },
  ];

  const [notificaciones, setNotificaciones] = useState(data);
  const [pagina, setPagina] = useState(1);
  const porPagina = 4;

  const totalPaginas = Math.ceil(notificaciones.length / porPagina);
  const inicio = (pagina - 1) * porPagina;
  const visibles = notificaciones.slice(inicio, inicio + porPagina);

  const marcarLeida = (id) => {
    setNotificaciones((prev) =>
      prev.map((n) => (n.id === id ? { ...n, leida: true } : n))
    );
  };

  const marcarNoLeida = (id) => {
    setNotificaciones((prev) =>
      prev.map((n) => (n.id === id ? { ...n, leida: false } : n))
    );
  };

  const marcarTodas = () => {
    setNotificaciones((prev) => prev.map((n) => ({ ...n, leida: true })));
  };

  const silenciar = (id, horas) => {
    setNotificaciones((prev) =>
      prev.map((n) =>
        n.id === id
          ? { ...n, silenciado: true, detalle: `Silenciado por ${horas}h` }
          : n
      )
    );
  };

  return (
    <div className="bg-slate-900/60 border border-slate-800 rounded-xl shadow-sm shadow-black/40 p-6 mt-6">
      <h2 className="text-xl font-semibold mb-2 text-slate-50">
        Centro de Notificaciones
      </h2>
      <p className="text-sm text-slate-400 mb-4">
        Listado cronológico con paginación, marcado de leídas y silencios por
        tiquete.
      </p>

      <button
        onClick={marcarTodas}
        className="mb-4 px-4 py-2 bg-[#B7041A] rounded-lg text-sm font-semibold hover:bg-[#9a0416]"
      >
        Marcar todas como leídas
      </button>

      {/* Lista de notificaciones - ocupa todo el ancho disponible */}
      <div className="space-y-3">
        {visibles.map((n) => (
          <div
            key={n.id}
            className={`border border-slate-700 rounded-lg p-4 ${
              n.leida ? "bg-slate-900/70" : "bg-slate-800/70"
            }`}
          >
            <div className="flex flex-col gap-3 md:flex-row md:justify-between md:items-start">
              <div>
                <h3 className="text-sm font-semibold text-slate-100">
                  {n.titulo}
                </h3>
                <p className="text-xs text-slate-400">{n.detalle}</p>
                <p className="text-[10px] text-slate-500 mt-1">{n.fecha}</p>
              </div>

              <div className="flex flex-col gap-2 text-xs items-start md:items-end">
                {/* Leída / no leída */}
                {!n.leida ? (
                  <button
                    onClick={() => marcarLeida(n.id)}
                    className="px-3 py-1 bg-[#4EBBD6]/20 text-[#4EBBD6] border border-[#4EBBD6]/40 rounded-lg"
                  >
                    Marcar leída
                  </button>
                ) : (
                  <button
                    onClick={() => marcarNoLeida(n.id)}
                    className="px-3 py-1 bg-amber-500/20 text-amber-300 border border-amber-500/40 rounded-lg"
                  >
                    Marcar como no leída
                  </button>
                )}

                {/* Silenciar */}
                {!n.silenciado ? (
                  <div className="flex flex-wrap gap-1">
                    <button
                      onClick={() => silenciar(n.id, 1)}
                      className="px-2 py-1 bg-slate-800 rounded-lg hover:bg-slate-700"
                    >
                      Silenciar 1h
                    </button>
                    <button
                      onClick={() => silenciar(n.id, 8)}
                      className="px-2 py-1 bg-slate-800 rounded-lg hover:bg-slate-700"
                    >
                      Silenciar 8h
                    </button>
                    <button
                      onClick={() => silenciar(n.id, 24)}
                      className="px-2 py-1 bg-slate-800 rounded-lg hover:bg-slate-700"
                    >
                      Silenciar 24h
                    </button>
                  </div>
                ) : (
                  <span className="text-[11px] text-slate-500 italic">
                    Silenciado
                  </span>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Paginación */}
      <div className="flex justify-end mt-4 gap-2 text-xs text-slate-400">
        <button
          disabled={pagina === 1}
          onClick={() => setPagina((p) => p - 1)}
          className="px-3 py-1 rounded-lg border border-slate-700 disabled:opacity-30 hover:border-[#4EBBD6] hover:text-[#4EBBD6]"
        >
          Anterior
        </button>

        {Array.from({ length: totalPaginas }, (_, i) => i + 1).map((p) => (
          <button
            key={p}
            onClick={() => setPagina(p)}
            className={`w-7 h-7 rounded-lg ${
              p === pagina
                ? "bg-[#B7041A] text-white"
                : "bg-slate-900 text-slate-300 hover:bg-slate-800"
            }`}
          >
            {p}
          </button>
        ))}

        <button
          disabled={pagina === totalPaginas}
          onClick={() => setPagina((p) => p + 1)}
          className="px-3 py-1 rounded-lg border border-slate-700 disabled:opacity-30 hover:border-[#4EBBD6] hover:text-[#4EBBD6]"
        >
          Siguiente
        </button>
      </div>
    </div>
  );
}

export default Notificaciones;
