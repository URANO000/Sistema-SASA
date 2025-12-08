import React from "react";

function Footer() {
  return (
    <footer className="border-t border-slate-800 bg-slate-950/95 text-[11px] text-slate-500">
      <div className="max-w-6xl mx-auto flex flex-col gap-1 px-6 py-3 md:flex-row md:items-center md:justify-between">
        <span>
          © 2025 Sistema de Atención y Servicios al Asociado
        </span>
        <span>Prototipo de frontend </span>
      </div>
    </footer>
  );
}

export default Footer;
