import React from "react";
import logo from './images/logo.gif'

const Header = () => {
  return (
    <div className="MainHeader">
      <img src={logo} className="MMMLogo" alt="MMM" />
      <div className="HeaderText">
        <h1>Mumbai Medicine Mart</h1>
      </div>
    </div>
  );
};

export default Header;