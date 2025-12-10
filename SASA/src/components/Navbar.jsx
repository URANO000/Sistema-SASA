import React, { useState, useRef } from "react";
import { Menubar } from 'primereact/menubar';
import { Menu } from "primereact/menu";
import { Avatar } from 'primereact/avatar';


function Navbar() {

  //Items inside profile
  const profileItems = [
    {label: 'Perfil', icon: 'pi pi-user', className: 'custom-profile-item'},
    {label: 'Ajustes', icon: 'pi pi-cog', className: 'custom-profile-item'},
    {label: 'Logout', icon: 'pi pi-sign-out', className: 'custom-profile-item log-out-item'}
  ];

  //Show menu
  const [showProfileMenu, setShowProfileMenu] = useState(false);
  const profileMenuRef = useRef(null);

  //Start, I am using the menu here
  const start =
  <div className="flex align-middle nav-start">
    <i className="pi pi-bars custom-icon"></i>
    <img alt="logo sasa" src="/logo-sasa-transparent.png" className="nav-img ml-3"></img>
  </div>

  //End, here I have my icons and other
  const end =
    <div className="flex align-middle nav-end">
      <span>
        <a type="button">
          <i className="pi pi-bell custom-icon pt-1" ></i>
        </a>
      </span>
      <div onClick={(e) => profileMenuRef.current.toggle(e)}>
        <Avatar shape="circle" label="MS" className="bg-primary text-white" />
        <i className="pi pi-chevron-down ml-1"></i>
      </div>
    </div>

  return (
    <div>
      <Menubar
        start={start}
        end={end}
        className="custom-navbar mb-5 flex justify-between" />

        <Menu model={profileItems} popup ref={profileMenuRef} className="profile-menu" />
    </div>
  );
}


export default Navbar;
