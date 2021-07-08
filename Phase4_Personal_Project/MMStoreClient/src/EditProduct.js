import React, {Component} from "react";
import {Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class EditProduct extends Component {
  constructor(props) {
    super(props);
    const id=(this.props.location!==undefined && this.props.location.state!==undefined 
      && this.props.location.state.id!==undefined)?this.props.location.state.id:null;
    this.state={token:null,
      productID:id,
      categoryresp:null,
      productresp:null};
    }

static contextType = Context;

getProduct() {
if (this.state.productID!=null && this.state.token!=null) {
  if (this.state.productID===0) { // adding new product
    const np={productID:0,productName:'',productInfo:'',productCode:'',productPrice:0,categoryID:1,productIsActive:"Y"}
    this.setState({productresp:np});
    }
  else { // editing product
    const options=apiFetchOptions('GET',null,this.state.token);
    const url=apiBaseURL+'/products/'+this.state.productID;
    fetch(url,options).then(r => r.json()).then(ps => this.setState({productresp:ps}));
    }
  }
}

getData() {
if (this.state.productID!=null && this.state.token!=null) {
  let url=apiBaseURL+'/categories/list?pagesize=500&offset=0'; // note: if more than 500 then this will need to recurse
  let options=apiFetchOptions('GET',null,this.state.token);
  fetch(url,options).then(r => r.json()).then(cs => this.setState({categoryresp:cs.categories}));
  this.getProduct();
  }
}

componentDidMount() {
this.setState({token:(this.context===undefined)?null:this.context.user.accessToken},
    () => this.getData());
}

SaveProduct() {
const body={productName:document.getElementById('prodname').value,productInfo:document.getElementById('prodinfo').value,
      productCode:document.getElementById('prodcode').value, productPrice:document.getElementById('prodcost').value,
      categoryID:document.getElementById('prodcat').value, productIsActive:(document.getElementById('prodok').checked?"Y":"N")};
const adding=(this.state.productresp.productID===0);
const options=apiFetchOptions(adding?'POST':'PATCH',body,this.state.token);
const url=apiBaseURL+'/products'+(adding?'':'/'+this.state.productresp.productID);
fetch(url,options).then(r => r.json()).then(ps => this.setState({productresp:ps},() => alert('Product updated')))
  .catch(err => alert('Unable to save product'));
}

NeedLogon() {return <Link to='/logon'><button className='action'>Log On</button></Link>}
NotAdmin() {return 'Access denied'}
NoProduct() {return 'No product selected'}
NoCategories() {return 'No categories found'}

EditProduct() {
  return (
  <React.Fragment>
    Product Name: <input id='prodname' type='text' style={{width:"300px"}} maxLength='64' defaultValue={this.state.productresp.productName} /><br />
    Category: <select id='prodcat' defaultValue={this.state.productresp.categoryID}>
      {this.state.categoryresp.map(c => <option key={c.categoryID} value={c.categoryID}>{c.categoryName}</option>)}
    </select><br />
    Code: <input id='prodcode' type='text' style={{width:"350px"}} maxLength='64' defaultValue={this.state.productresp.productCode} /><br />
    Info: <input id='prodinfo' type='text' style={{width:"350px"}} maxLength='512' defaultValue={this.state.productresp.productInfo} /><br />
    Price: <input id='prodcost' type='number' style={{width:"100px"}} defaultValue={this.state.productresp.productPrice} /><br />
    Active: <input id='prodok' type='checkbox' defaultChecked={this.state.productresp.productIsActive==='Y'} /><br />
    <button className='Action' onClick={(e) => this.SaveProduct()}>Save</button>
  </React.Fragment>
  )
}

render() {
  const haveuser=(this.state.token!=null);
  const isadmin=(this.context.user!==undefined && this.context.user.userIsAdmin==='Y');
  const havecats=(this.state.categoryresp!=null);
  const haveproduct=(this.state.productresp!=null);
  return (
    <div className="MainBody">
      {(haveuser===false)?this.NeedLogon():
        (isadmin===false)?this.NotAdmin():
          (havecats===false)?this.NoCategories():
            (haveproduct===false)?this.NoProduct():
              this.EditProduct()
      }
    </div>
  );
}

}