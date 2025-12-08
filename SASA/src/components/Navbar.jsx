import React from "react";
import logoSasa from "../assets/logo-sasa.png";

function Navbar() {
  return (
    <header className="sticky top-0 z-40 bg-[#B7041A] text-white shadow-lg shadow-black/40">
      <div className="flex items-center justify-between px-6 py-4 md:px-10">
        <div className="flex items-center gap-3 pr-6">
          <img
            src={logoSasa}
            alt="Sistema Analíticos SASA"
            className="h-10 md:h-16 w-auto rounded-sm bg-black/10 p-1"
          />
        </div>

        <div className="relative mr-3 cursor-pointer">
          <span className="text-xl">🔔</span>
          <span className="absolute -top-1 -right-1 bg-[#FF1A3B] text-white text-[10px] px-1.5 py-[1px] rounded-full">
            3
          </span>
        </div>
        <div className="flex items-center gap-4">
          <button className="hidden sm:inline-flex text-[11px] px-4 py-2 rounded-full border border-white/40 bg-black/20 hover:bg-black/30 transition-colors">
            Accesibilidad
          </button>

          <div className="flex items-center gap-2">
            <div className="h-8 w-8 rounded-full bg-[#4EBBD6] flex items-center justify-center text-xs font-semibold text-black shadow-sm shadow-black/40">
              A
            </div>
            <div className="hidden sm:flex flex-col">
              <span className="text-sm font-semibold leading-tight">
                Admin
              </span>
              <span className="text-[11px] text-white/80">
                Rol: Administrador
              </span>
            </div>
            <button className="ml-1 text-[11px] underline decoration-white/60 underline-offset-2 hover:decoration-white">
              Cerrar sesión
            </button>
          </div>
        </div>
      </div>
    </header>
  );
}

export default Navbar;
