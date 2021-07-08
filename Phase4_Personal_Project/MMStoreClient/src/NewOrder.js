import React, {Component} from "react";
import {Consumer, Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class NewOrder extends Component {
  constructor(props) {
    super(props);
    this.state={token:null,
      cartresp:{cart:{}}};
      }

  static contextType = Context;
  
  getCart() {
  const options=apiFetchOptions('GET',null,this.state.token);
  const url=apiBaseURL+'/cart';
  fetch(url,options).then(r => r.json()).then(cs => this.setState({cartresp:cs}));
  }

  componentDidMount() {
   this.setState({token:(this.context===undefined)?null:this.context.user.accessToken},
    () => this.getCart());
  }

  getTotals(updateditem) {
    let totalprod=0;
    let totalqty=0;
    let totalcost=0;
    if(this.state.cartresp!==undefined && this.state.cartresp.cart!==null && Array.isArray(this.state.cartresp.cart)) {
      const cart=this.state.cartresp.cart;
      for (var i in cart) {
        if (updateditem!==null && updateditem.prod===cart[i].productID) {cart[i].quantity=updateditem.quantity;}
        let q=cart[i].quantity;
        let p=cart[i].productPrice;
        if (cart[i].quantity>0) {totalprod++;}
        totalqty+=q;
        totalcost+=p*q;
        }
      }
  return {products:totalprod,quantity:totalqty,cost:totalcost};
  }

  render() {
      const totals=this.getTotals(null);
      return (
      <Consumer>
        {value => {
          const haveuser=(value.user.accessToken!==null);
          return (
            <div className="MainBody">
              {haveuser?(
                <div className='container-fluid'>
                  {!Array.isArray(this.state.cartresp.cart)?null:
                    <React.Fragment>
                      <div className='container-fluid row'>
                        <div className='container-fluid'>
                          This is where we'd validate payment info and shipping address...
                        </div>
                        <div className='col-auto'>
                          Products: <span id='totalprod'>{totals.products}</span><br />
                          Quantity: <span id='totalqty'>{totals.quantity}</span><br />
                          Total: $<span id='totalcost'>{totals.cost}</span>
                        </div>
                        {totals.quantity<1?null:
                          <div id='orderbutton' style={{display:"block"}} className='col-auto'>
                            <Link to={{pathname:'/submitorder',state:{fromneworder:'fno'}}}><button className='action'>Submit</button></Link>
                          </div>
                        }
                      </div>
                    </React.Fragment>
                  }
                </div>
              ):(
                <Link to='/logon'><button id='cmdLogon' className='action'>Log On</button></Link>
              )}
            </div>
            );
          }
        }
      </Consumer>
    );
  }
};


