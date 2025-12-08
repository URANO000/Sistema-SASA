import { useState } from "react";
import "./index.css";
import "./App.css";

import Navbar from "./components/Navbar";
import Sidebar from "./components/Sidebar";
import Footer from "./components/Footer";
import Breadcrumb from "./components/Breadcrumb";

import Dashboard from "./components/Dashboard";
import Table from "./components/Table";
import Formulario from "./components/Formulario";
import Modal from "./components/Modal";

import Notificaciones from "./components/Notificaciones";
import Auditoria from "./components/Auditoria";
import Involucrados from "./components/Involucrados";

function App() {
  const [page, setPage] = useState("dashboard");
  const [openModal, setOpenModal] = useState(false);
  const [ticketSeleccionado, setTicketSeleccionado] = useState(null);
  const abrirModalEliminar = (idTicket) => {
    setTicketSeleccionado(idTicket);
    setOpenModal(true);
  };

  const cerrarModal = () => {
    setOpenModal(false);
    setTicketSeleccionado(null);
  };

  return (
    <div className="min-h-screen bg-slate-950 text-slate-50 flex flex-col">
      <Navbar />

      <div className="flex flex-1">
        <Sidebar page={page} onChangePage={setPage} />

        <main className="flex-1 px-6 py-6 md:px-10">
          <section className="max-w-7xl mx-auto space-y-6">
            {page === "dashboard" && (
              <>
            <Breadcrumb
                  items={[
                    { label: "Inicio" },
                    { label: "Dashboard" },
                  ]}
                />
                <header className="flex flex-col gap-2">
                  <h1 className="text-2xl font-semibold tracking-tight">
                    Dashboard por rol
                  </h1>
                  <p className="text-sm text-slate-400">
                    Tarjetas de estado y métricas generales del sistema.
                  </p>
                </header>
                <Dashboard />
              </>
            )}
            {page === "tiquetes" && (
              <>
                   <Breadcrumb
                  items={[
                    { label: "Inicio" },
                    { label: "Tiquetes" },
                    { label: "Listado" },
                  ]}
                />
                <header className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
                  <div>
                    <h1 className="text-2xl font-semibold tracking-tight">
                      Gestión de tiquetes
                    </h1>
                  </div>
                </header>
                <Table onDeleteTicket={abrirModalEliminar} />
                <Formulario />
              </>
            )}
            {page === "notificaciones" && (
              <>
               <Breadcrumb
                  items={[
                    { label: "Inicio" },
                    { label: "Notificaciones" },
                    { label: "Centro" },
                  ]}
                />
                <header className="flex flex-col gap-2">
                  <h1 className="text-2xl font-semibold tracking-tight">
                    Centro de notificaciones
                  </h1>
                </header>
                <Notificaciones />
                <Involucrados />
                <Auditoria />
              </>
            )}
          </section>
        </main>
      </div>
      <Footer />
      <Modal
        open={openModal}
        ticketId={ticketSeleccionado}
        onCancel={cerrarModal}
        onConfirm={() => {
          alert(
            ticketSeleccionado
              ? `Se eliminaría el caso ${ticketSeleccionado} (solo UI).`
              : "Se eliminaría el caso seleccionado (solo UI)."
          );
          cerrarModal();
        }}
      />
    </div>
  );
}

export default App;
