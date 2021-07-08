import React from "react";
import logo from './images/logo.gif'

const Header = () => {
  return (
    <div className="MainHeader">
      <img src={logo} className="MMSLogo" alt="MMS" />
      <div className="HeaderText">
        <h1>Mumbai Medicine Store</h1>
      </div>
    </div>
  );
};

export default Header;