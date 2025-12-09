import React, { useState } from "react";
import { Sidebar } from "primereact/sidebar";
import { Button } from "primereact/button";

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

function Sidenav() {
  const [visible, setVisible] = useState(false);
  return (

    <div className="flex justify-center">
      <Sidebar
        visible={visible}
        onHide={() => setVisible(false)}

      />

    </div>

  );
}

export default Sidenav;
