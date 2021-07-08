import React, {Component} from "react";
import {Consumer} from './context';
//import { Link } from "react-router-dom";
/*Products Users Categories Cart Logon*/

class Navbar extends Component {
  
  render() {
    return (
      <Consumer>
        {value => {
          const isadmin=(value.user.userIsAdmin==='Y');
          const ufname=value.user.userFirstName;
          // const ulname=value.userLastName;
          const haveuser=(value.user.accessToken!==null);
          return (
            <nav className="navbar navbar-expand-sm">
            <div className='container'>
              <ul className='navbar-nav mr-auto'>
                <li className='nav-item'><a href='/' className='nav-link'>Home</a></li>
                <li className='nav-item'><a href='/products' className='nav-link'>Products</a></li>
                {isadmin?(<li className='nav-item'><a href='/categories' className='nav-link'>Categories</a></li>):''}
                {isadmin?(<li className='nav-item'><a href='/users' className='nav-link'>Users</a></li>):''}
                {haveuser?(<li className='nav-item'><a href='/logoff' className='nav-link'>Log off {ufname}</a></li>)
                        :(<li className='nav-item'><a href='/logon' className='nav-link'>Log on</a></li>)}      
                {haveuser?(<li className='nav-item'><a href='/cart' className='nav-link'>Cart</a></li>):''}       
                {haveuser?(<li className='nav-item'><a href='/orders' className='nav-link'>Orders</a></li>):''}       
              </ul>
            </div>
          </nav>
          )
        }}
      </Consumer>
      );
    }
  };

export default Navbar;
