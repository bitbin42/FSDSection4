import React, {Component} from "react";
import {Consumer} from './context';
import Nav from "react-bootstrap/Nav";
import Navbar from "react-bootstrap/Navbar";

class Menubar extends Component {
  
  render() {
    return (
      <Consumer>
        {value => {
          const isadmin=(value.user.userIsAdmin==='Y');
          const ufname=value.user.userFirstName;
          // const ulname=value.userLastName;
          const haveuser=(value.user.accessToken!==null);
          return (
            <Navbar collapseOnSelect expand="sm" bg="light">
              <Navbar.Toggle aria-controls="responsive-navbar-nav" />
              <Navbar.Collapse id="responsive-navbar-nav">
                <Nav className="me-auto">
                  <Nav.Link href='/'><span className="menutext">Home</span></Nav.Link>
                  <Nav.Link href='/products'><span className="menutext">Products</span></Nav.Link>
                  {isadmin?(<Nav.Link href='/categories'><span className="menutext">Categories</span></Nav.Link>):''}
                  {isadmin?(<Nav.Link href='/users'><span className="menutext">Users</span></Nav.Link>):''}
                  {haveuser?(<Nav.Link href='/logoff'><span className="menutext">Log off {ufname}</span></Nav.Link>)
                          :(<Nav.Link href='/logon'><span className="menutext">Log On</span></Nav.Link>)}      
                  {haveuser?(<Nav.Link href='/cart'><span className="menutext">Cart</span></Nav.Link>):''}       
                  {haveuser?(<Nav.Link href='/orders'><span className="menutext">Orders</span></Nav.Link>):''}       
                </Nav>
              </Navbar.Collapse>
            </Navbar>
          )
        }}
      </Consumer>
      );
    }
  };

export default Menubar;
