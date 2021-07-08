import React, {Component} from "react";
import {Consumer, Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {DateText} from './Support'
import {Link} from "react-router-dom";

export default class Orders extends Component {
  constructor(props) {
    super(props);
    this.state={token:null,
      status:'all',
      ordersresp:{offset:0,pageSize:5,totalRecords:0,prevPage:null,nextPage:null,orders:{}}};
    }

  static contextType = Context;
  
  getOrders() {
  const options=apiFetchOptions('GET',null,this.state.token);
  const ps=this.state.ordersresp.pageSize;
  const os=document.getElementById('selectstatus').value;
  const url=apiBaseURL+'/orders?offset=0&pagesize='+ps+((os==='all')?'':'&status='+os);
  fetch(url,options).then(r => r.json()).then(os => this.setState({ordersresp:os}));
  }

  statusSelected(newStatus) {
  this.setState({status:newStatus},() => this.getOrders());
  }
      
  navigateList(newpage) {
  const options=apiFetchOptions('GET',null,this.state.token);
  fetch(newpage,options).then(r => r.json()).then(os => this.setState({ordersresp:os}));
  }
    
  componentDidMount() {
  const context = this.context;
  this.setState({token:(context===undefined)?null:context.user.accessToken},
    () => this.getOrders());
  }

  showOrders() {
    let orderfrom=0;
    let orderto=0;
    if (this.state.ordersresp.totalRecords!==0) {
      orderfrom=this.state.ordersresp.offset+1;
      orderto=this.state.ordersresp.offset+this.state.ordersresp.orders.length;  
      }
    return (
      <div className="MainBody">
        <div className="container-fluid">
          Order status: <select id='selectstatus' className='downright' onChange={(e) => this.orderSelected(e.target.value)}>
          <option value='all'>All</option>
          <option value='processing'>Processing</option>
          <option value='shipped'>Shipped</option>
          <option value='complete'>Complete</option>
          <option value='cancelled'>Cancelled</option>
            </select>
          Orders {orderfrom} to {orderto} of {this.state.ordersresp.totalRecords}
        </div>
        <div className='container-fluid'>
          {!Array.isArray(this.state.ordersresp.orders)?null:
            <table className='table'><thead><tr><th>Order</th><th>Status</th><th>Date</th></tr></thead><tbody>
            {this.state.ordersresp.orders.map(p => {return <Order order={p} key={p.orderID} />})}
            </tbody></table>
            }
        </div>
        <div className='container-fluid productbox'>
          {this.state.ordersresp.prevPage==null?null:
            <button id='backButton' className='downright' onClick={(e) => this.navigateList(this.state.ordersresp.prevPage)}>&lt;&lt; Back</button>}
          {this.state.ordersresp.nextPage==null?null:
            <button id='nextButton' className='downright' onClick={(e) => this.navigateList(this.state.ordersresp.nextPage)}>&gt;&gt; Next</button>}
        </div>
      </div>
      );
  }

  render() {
      return (
      <Consumer>
        {value => {
          const haveuser=(value.user.accessToken!==null);
          return (
            (haveuser===false)?(
              <Link to='/logon'><button className='action'>Log on</button></Link>
            ):this.showOrders()
          );
        }}
      </Consumer>
    );
  }
};

// ====================================== Order item

export class Order extends Component {

  static contextType = Context;

  render() {
    const dt=DateText(this.props.order.orderDate,'MMM d, yyyy h:mm@');
    return (
      <Consumer>
        {value => {
          return (
            <React.Fragment>
              <tr><td>{this.props.order.orderID}</td><td>{this.props.order.orderStatus}</td><td>{dt}</td></tr>
            </React.Fragment>
            );
          }
        }
      </Consumer>
    );
  }
};
