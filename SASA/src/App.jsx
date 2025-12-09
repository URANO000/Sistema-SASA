import { useState } from "react";
import "./index.css";
import { Routes, Route, Outlet, BrowserRouter } from 'react-router-dom';

import Navbar from "./components/Navbar";
import Sidenav from "./components/Sidebar";
import Home from "./pages/Misc/Homepage";
import Footer from "./components/Footer";
// import Breadcrumb from "./components/Breadcrumb";

// import Dashboard from "./components/Dashboard";
// import Table from "./components/Table";
// import Formulario from "./components/Formulario";
// import Modal from "./components/Modal";

// import Notificaciones from "./components/Notificaciones";
// import Auditoria from "./components/Auditoria";
// import Involucrados from "./components/Involucrados";

function App() {

  return (
    <BrowserRouter>
      <Routes>
        {/*Login page here */}

        {/*Main wrapper, creates layout for the site */}
        <Route element={
          <div className="flex flex-col h-screen">
            {/*The flex helps us put navbar on top, sidebar on side and main next to that */}
            <Navbar />
            <div className="flex flex-1 overflow-hidden">
              <Sidenav />
              {/*This is all the main pages, on the side or the middle, depending on the sidebar */}
              <main className="flex-1 overflow-auto">
                <Outlet />
              </main>
            </div>
            {/* <Footer /> */}
          </div>

        }>
          {/*Child Routes. Render inside Outlet */}
          <Route path="/" element={<Home />}></Route>
        </Route>

      </Routes>
    </BrowserRouter>


  );
}

export default App;
