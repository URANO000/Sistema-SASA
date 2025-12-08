import React from "react";

const sections = [
  {
    title: "Principal",
    items: [
      { key: "dashboard", label: "Dashboard", icon: "📊" },
      { key: "tiquetes", label: "Tiquetes", icon: "📂" },
      { key: "notificaciones", label: "Notificaciones", icon: "🔔" },
    ],
  },
  {
    title: "Configuración",
    items: [
      { key: "usuarios", label: "Usuarios", icon: "👥", disabled: true },
      { key: "parametros", label: "Parámetros", icon: "⚙️", disabled: true },
    ],
  },
];

function Sidebar({ page, onChangePage }) {
  return (
    <aside className="hidden md:flex w-72 shrink-0 bg-slate-950/95 border-r border-slate-800 flex-col">
      <div className="px-4 pt-4 pb-2 text-[11px] uppercase tracking-[0.25em] text-slate-500">
        Módulos
      </div>

      <nav className="flex-1 px-2 space-y-4 text-sm">
        {sections.map((section) => (
          <div key={section.title}>
            <p className="px-2 mb-1 text-[11px] font-semibold text-slate-500 uppercase tracking-wide">
              {section.title}
            </p>
            <ul className="space-y-1">
              {section.items.map((item) => {
                const isActive = item.key === page;
                const isDisabled = item.disabled;

                return (
                  <li key={item.key}>
                    <button
                      disabled={isDisabled}
                      onClick={() => !isDisabled && onChangePage(item.key)}
                      className={[
                        "w-full flex items-center gap-2 px-3 py-2 rounded-lg text-left transition-colors",
                        isActive
                          ? "bg-[#B7041A] text-white shadow-sm shadow-black/40 border border-[#B7041A]"
                          : "text-slate-300 hover:bg-slate-900 hover:text-white border border-transparent",
                        isDisabled ? "opacity-40 cursor-not-allowed" : "",
                      ].join(" ")}
                    >
                      <span className="text-base">{item.icon}</span>
                      <span className="flex-1 text-sm font-medium">
                        {item.label}
                      </span>
                      {isActive && (
                        <span className="h-2 w-2 rounded-full bg-[#4EBBD6]" />
                      )}
                    </button>
                  </li>
                );
              })}
            </ul>
          </div>
        ))}
      </nav>

      <div className="px-4 py-3 border-t border-slate-800 text-[11px] text-slate-500">
        Rol actual: <span className="font-semibold text-slate-200">Admin</span>
      </div>
    </aside>
  );
}

export default Sidebar;
