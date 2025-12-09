import React from "react";

function Footer() {
  return (
    <footer className="rounded-base shadow-xs ">
      <div className="h-1 w-full mx-auto max-w-7xl md:flex md:items-center md:justify-between">
        {/* <div className="footer-logo flex justify-center md:jusitfy-end">
          <img alt="logo sasa" src="../public/logo-sasa.png" className="img-logo"></img>
        </div> */}
        <span className="justify-center text-sm text-body sm:text-center">© 2025 
          <a href="#" className="hover:underline"> Sistemas Analíticos</a>
        </span>
      </div>
    </footer>
  );
}

export default Footer;
