import React, {Component} from "react";
import {Consumer, Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class Products extends Component {
  constructor(props) {
    super(props);
    this.state={token:null,
      catSelected: 0,
      searchText: '',
      categoryresp:[],
      productresp:{offset:0,pageSize:5,totalRecords:0,prevPage:null,nextPage:null,products:{}}};
    }

  static contextType = Context;
  
  getProducts() {
  const options=apiFetchOptions('GET',null,this.state.token);
  const ps=this.state.productresp.pageSize;
  const ci=this.state.catSelected;
  const st=this.state.searchText;
  const url=apiBaseURL+'/products/list?offset=0&sort=name&pagesize='+ps+'&categoryid='+ci+'&Search='+encodeURIComponent(st);
  fetch(url,options).then(r => r.json()).then(ps => this.setState({productresp:ps}));
  }

  categorySelected(catid) {
  this.setState({catSelected:catid},() => this.getProducts());
  }
      
  searchSelected(searchtext) {
  this.setState({searchText:searchtext},() => this.getProducts());
  }

  navigateList(newpage) {
  const options=apiFetchOptions('GET',null,this.state.token);
  fetch(newpage,options).then(r => r.json()).then(ps => this.setState({productresp:ps}));
  }

  getData() {
  let url=apiBaseURL+'/categories/list?pagesize=500&offset=0'; // note: if more than 500 then this will need to recurse
  let options=apiFetchOptions('GET',null,this.state.token);
  fetch(url,options).then(r => r.json()).then(cs => this.setState({categoryresp:cs.categories}));
  this.getProducts();
  }
    
  componentDidMount() {
  const context = this.context;
  this.setState({token:(context===undefined)?null:context.user.accessToken},
    () => this.getData());
  }

  render() {
      return (
      <Consumer>
        {value => {
          const haveuser=(value.user.accessToken!==null);
          const isadmin=(value.user.userIsAdmin==='Y');
          let prodfrom=0;
          let prodto=0;
          if (this.state.productresp.totalRecords!==0) {
            prodfrom=this.state.productresp.offset+1;
            prodto=this.state.productresp.offset+this.state.productresp.products.length;  
            }
          return (
            <div className="MainBody">
              <div className="container-fluid">
                <div className="boxed">Category: <select id='catselect' className='downright' onChange={(e) => this.categorySelected(e.target.value)}>
                  <option value='0'>All</option>
                  {this.state.categoryresp.map(c => <option key={c.categoryID} value={c.categoryID}>{c.categoryName}
                            {(isadmin && c.categoryIsActive!=='Y')?' [inactive]':''}</option>)}
                  </select></div>
                <div className="boxed">Search: <input id='searchTarget' type='text' />
                <button id='doSearch' className='downright'
                  onClick={(e) => this.searchSelected(document.getElementById('searchTarget').value)}>Find</button></div>
                <div className="boxed">Products {prodfrom} to {prodto} of {this.state.productresp.totalRecords}</div>
              </div>
              <div className='container-fluid'>
                {!Array.isArray(this.state.productresp.products)?null:
                  this.state.productresp.products.map(p => {return <Product product={p} key={p.productID} />})}
              </div>
              <div className='container-fluid productbox'>
                {haveuser?null:<Link to="/logon"><button className='downright'>Log on</button></Link>}
                {isadmin?<Link to={{pathname:'/editproduct',state:{id:0}}}><button className='downright'>Add Product</button></Link>:null}
                {this.state.productresp.prevPage==null?null:
                  <button id='backButton' className='downright' onClick={(e) => this.navigateList(this.state.productresp.prevPage)}>&lt;&lt; Back</button>}
                {this.state.productresp.nextPage==null?null:
                  <button id='nextButton' className='downright' onClick={(e) => this.navigateList(this.state.productresp.nextPage)}>&gt;&gt; Next</button>}
              </div>
            </div>
            );
          }
        }
      </Consumer>
    );
  }
};

// ====================================== Product

export class Product extends Component {

  static contextType = Context;

  cartUpdated = (item) => {
    const q=item.quantity;
    if (q===0) {alert('The item has been removed from the shopping cart');}
    else {alert('The item has been added to the shooping cart');}
  }

  cartFailed = (err) => {
    alert('Sorry, the shopping cart was not updated');
  }

  updateCart = (id,qty) => {
    const body={"productID":id,"quantity":qty};
    const url=apiBaseURL+'/cart';
    const token=(this.context===undefined)?null:this.context.user.accessToken;
    const options=apiFetchOptions('POST',body,token);
    fetch(url,options).then(r => r.json()).then(r => this.cartUpdated(r)).catch(e => this.cartFailed(e));
  }
  
  render() {
      return (
      <Consumer>
        {value => {
          const haveuser=(value.user.accessToken!==null);
          const isadmin=(value.user.userIsAdmin==='Y');
          return (
            <div className="container-fluid productbox">
              <div className="boxed productnamebox">
                {this.props.product.productName} ({this.props.product.categoryName})<br />
                {this.props.product.productInfo}<br />
                {this.props.product.productCode} 
              </div>
              <div className="boxed">
                ${this.props.product.productPrice.toFixed(2)}
                {(haveuser===false || this.props.product.productIsActive!=='Y')?null:
                  <span className='p-4'>
                    <button id='cmdBuy' onClick={(e) => this.updateCart(this.props.product.productID,1)}>Purchase</button>
                  </span>
                }
                </div>
                {(isadmin===false)?null:
                  <div className="boxed">
                    <Link to={{pathname:'/editproduct',state:{id:this.props.product.productID}}}><button>Edit</button></Link>
                  </div>
                  }
            </div>
            );
          }
        }
      </Consumer>
    );
  }
};
