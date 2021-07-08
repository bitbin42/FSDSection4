import React, {Component} from "react";
import {Consumer, Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class Cart extends Component {
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

  newQuantity = (cartitem) => {  // avoid re-rendering everything
  const totals=this.getTotals(cartitem);
  document.getElementById('totalprod').innerText=totals.products;
  document.getElementById('totalqty').innerText=totals.quantity;
  document.getElementById('totalcost').innerText=totals.cost.toFixed(2);
  document.getElementById('orderbutton').style.display=(totals.quantity>0)?"block":"none";
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
                      <table className='table'>
                        <thead><tr><th>Product</th><th>Quantity</th><th>Price</th><th>Total</th></tr></thead><tbody>
                        {this.state.cartresp.cart.map(c => {return <CartItem
                           token={this.state.token} item={c} key={c.productID} callback={this.newQuantity} />
                          })}
                      </tbody></table>
                      <div className='container-fluid row'>
                        <div className='col-auto'>
                          Products: <span id='totalprod'>{totals.products}</span><br />
                          Quantity: <span id='totalqty'>{totals.quantity}</span><br />
                          Total: $<span id='totalcost'>{totals.cost}</span>
                        </div>
                        {totals.quantity<1?null:
                          <div id='orderbutton' style={{display:"block"}} className='col-auto'>
                            <Link to='/neworder'><button className='action'>Place order</button></Link>
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


// ====================================== Items

export class CartItem extends Component {

  static contextType = Context;

  updatedquantity(cartitem) {
  this.props.item.quantity=cartitem.quantity;
  this.forceUpdate();
  this.props.callback(cartitem);
  }

  updateItemQuantity(productid,newqty) {
  const body={productID:productid,quantity:newqty};
  const options=apiFetchOptions('POST',body,this.props.token);
  const url=apiBaseURL+'/cart';
  fetch(url,options).then(r => r.json()).then(cs => this.updatedquantity(cs))
    .catch(err => alert('Unable to update shopping cart'));
  }

  render() {
      const q=this.props.item.quantity;
      const p=this.props.item.productPrice;
      return (
      <Consumer>
        {value => {
          return (
            <React.Fragment>
              <tr><td>
                {this.props.item.productName} ({this.props.item.categoryName})<br />
                {this.props.item.productCode} 
              </td><td><select value={q} onChange={(e) => this.updateItemQuantity(this.props.item.productID,e.target.value)}>
                        <option value='0'>0</option>
                        <option value='1'>1</option>
                        <option value='2'>2</option>
                        <option value='3'>3</option>
                        <option value='4'>4</option>
                        <option value='5'>5</option>
                       </select>
              </td><td>
                ${p.toFixed(2)}
              </td><td>
                ${(p*q).toFixed(2)}              
              </td></tr>
            </React.Fragment>
            );
          }
        }
      </Consumer>
    );
  }
};
