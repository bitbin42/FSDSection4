import React, { Component } from "react";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import {Provider} from './context';
import Header from './Header';
import Navbar from './Navbar';
import Footer from './Footer';
import Welcome from './Welcome';
import {Logon,Logoff} from './Logon';
import Products from './Products';
import EditProduct from './EditProduct';
import Cart from './Cart';
import NewOrder from './NewOrder';
import SubmitOrder from './SubmitOrder';
import Orders from './Orders';
import Categories from './Categories';
import EditCategory from './EditCategory';
import Users from './Users';
import EditUser from './EditUser';

class App extends Component {

  render() {
    return (
      <Provider>
        <Header />
        <Navbar />
        <div id='BodyBlock'>
          <Router>
            <Switch>
              <Route path="/" exact render={(props) => (<Welcome {...props} />)} />
              <Route path="/logon" exact render={(props) => (<Logon {...props} />)} />
              <Route path="/logoff" exact render={(props) => (<Logoff {...props} />)} />
              <Route path="/products" exact render={(props) => (<Products {...props} />)} />
              <Route path="/cart" exact render={(props) => (<Cart {...props} />)} />
              <Route path="/neworder" exact render={(props) => (<NewOrder {...props} />)} />
              <Route path="/submitorder" exact render={(props) => (<SubmitOrder {...props} />)} />
              <Route path="/orders" exact render={(props) => (<Orders {...props} />)} />
              <Route path="/editproduct" exact render={(props) => (<EditProduct {...props} />)} />
              <Route path="/categories" exact render={(props) => (<Categories {...props} />)} />
              <Route path="/editcategory" exact render={(props) => (<EditCategory {...props} />)} />
              <Route path="/users" exact render={(props) => (<Users {...props} />)} />
              <Route path="/edituser" exact render={(props) => (<EditUser {...props} />)} />
            </Switch>
          </Router>
        </div>
        <Footer />
      </Provider>
    );
  }
}

export default App;
