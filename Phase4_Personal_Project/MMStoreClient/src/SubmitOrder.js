import React, {Component} from "react";
import {Consumer, Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class SubmitOrder extends Component {
  constructor(props) {
    super(props);
    this.state={token:null,orderresp:null}
    this.fromneworder=(this.props.location!==undefined && this.props.location.state!==undefined 
          && this.props.location.state.fromneworder==='fno');
  }
  
  static contextType=Context;
  fromneworder=false;

  placeOrder() {
  if (this.fromneworder===true && this.state.token!==null) {
    const options=apiFetchOptions('GET',null,this.state.token);
    const url=apiBaseURL+'/cart/submit ';
    fetch(url,options).then(r => r.json()).then(os => this.setState({orderresp:os}))
      .catch(err => alert('Unable to submit order'));
    } 
  }
  
  componentDidMount() {
  this.setState({token:(this.context===undefined)?null:this.context.user.accessToken},
      () => this.placeOrder());
  }
  
  NeedLogon() {return <Link to='/logon'><button className='action'>Log On</button></Link>}
  NotFromCart() {return <span>Please review the shopping cart first<br /><Link to='/cart'><button className='action'>Cart</button></Link></span>}
  NoOrder() {return <Link to='/products'><button className='action'>No order placed</button></Link>}
  OrderPlaced() {return <span>Thank you for your custom.  Your order number is: {this.state.orderresp.orderID}</span>}
  
  render() {
    return (
    <Consumer>
      {value => {
        const haveuser=(value.user.accessToken!==null);
        const fromcart=(this.fromneworder===true);
        const orderplaced=(this.state.orderresp!==null && this.state.orderresp.orderID!==null);
        return (
          <div className="MainBody">
            {(haveuser===false)?this.NeedLogon():
              ((fromcart===false)?this.NotFromCart():
              ((orderplaced===false)?this.NoOrder():
                this.OrderPlaced()))
              }
          </div>
        );
      }}
    </Consumer>
  );
  }
  
  }
  
  