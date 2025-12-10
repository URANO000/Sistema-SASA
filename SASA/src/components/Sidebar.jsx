import React, { useState } from "react";
import { Sidebar } from "primereact/sidebar";
import { Button } from "primereact/button";

const sections = [
  {
    title: "Principal",
    items: [
      { key: "dashboard", label: "Dashboard", icon: "pi pi-chart-bar" },
      { key: "tiquetes", label: "Módulo de Tiquetes", icon: "pi pi-ticket" },
      { key: "admin", label: "Módulo Administrativo", icon: "pi pi-sliders-v" },
    ],
  },
  {
    title: "Configuración",
    items: [
      { key: "ajustes", label: "Parámetros", icon: "pi pi-cog"},
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
