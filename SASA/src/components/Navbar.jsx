import React from "react";
import { Menubar } from 'primereact/menubar';
import { Button } from "primereact/button";


function Navbar() {


  //Start, I am using the menu here
  const start = <img alt="logo sasa" src="/logo-sasa-transparent.png" className="nav-img ml-3"></img>;
  const end = <Button label="Logout" className="accentBtn"  icon=<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
    <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 9V5.25A2.25 2.25 0 0 1 10.5 3h6a2.25 2.25 0 0 1 2.25 2.25v13.5A2.25 2.25 0 0 1 16.5 21h-6a2.25 2.25 0 0 1-2.25-2.25V15M12 9l3 3m0 0-3 3m3-3H2.25" />
  </svg>
  />

  return (
    <div className="flex justify-between">
      <Menubar
        start={start}
        end={end}
        className="custom-navbar mb-5" />
    </div>
  );
}


export default Navbar;
