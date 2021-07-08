import React, {Component} from "react";
import {Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class EditCategory extends Component {
  constructor(props) {
    super(props);
    const id=(this.props.location!==undefined && this.props.location.state!==undefined 
      && this.props.location.state.id!==undefined)?this.props.location.state.id:null;
    this.state={token:null,
      categoryID:id,
      categoryresp:null};
    }

static contextType = Context;

getData() {
if (this.state.categoryID!=null && this.state.token!=null) {
  if (this.state.categoryID===0) { // adding new category
    const nc={categoryID:0,categoryName:'',categoryInfo:'',categoryCode:'',categoryIsActive:"Y"}
    this.setState({categoryresp:nc});
    }
  else { // editing category
    const options=apiFetchOptions('GET',null,this.state.token);
    const url=apiBaseURL+'/categories/'+this.state.categoryID;
    fetch(url,options).then(r => r.json()).then(cs => this.setState({categoryresp:cs}));
    }
  }
}

componentDidMount() {
this.setState({token:(this.context===undefined)?null:this.context.user.accessToken},
    () => this.getData());
}

SaveCategory() {
const body={categoryName:document.getElementById('catname').value,categoryInfo:document.getElementById('catinfo').value,
      categoryIsActive:(document.getElementById('catok').checked?"Y":"N")};
const adding=(this.state.categoryresp.categoryID===0);
const options=apiFetchOptions(adding?'POST':'PATCH',body,this.state.token);
const url=apiBaseURL+'/categories'+(adding?'':'/'+this.state.categoryresp.categoryID);
fetch(url,options).then(r => r.json()).then(ps => this.setState({categoryresp:ps},() => alert('Category updated')))
  .catch(err => alert('Unable to save category'));
}

NeedLogon() {return <Link to='/logon'><button className='action'>Log On</button></Link>}
NotAdmin() {return 'Access denied'}
NoCategory() {return 'No category selected'}

EditCategory() {
  return (
  <React.Fragment>
    Category Name: <input id='catname' type='text' style={{width:"300px"}} maxLength='64' defaultValue={this.state.categoryresp.categoryName} /><br />
    Info: <input id='catinfo' type='text' style={{width:"350px"}} maxLength='512' defaultValue={this.state.categoryresp.categoryInfo} /><br />
    Active: <input id='catok' type='checkbox' defaultChecked={this.state.categoryresp.categoryIsActive==='Y'} /><br />
    <button className='Action' onClick={(e) => this.SaveCategory()}>Save</button>
  </React.Fragment>
  )
}

render() {
  const haveuser=(this.state.token!=null);
  const isadmin=(this.context.user!==undefined && this.context.user.userIsAdmin==='Y');
  const havecategory=(this.state.categoryresp!=null);
  return (
    <div className="MainBody">
      {(haveuser===false)?this.NeedLogon():
        (isadmin===false)?this.NotAdmin():
            (havecategory===false)?this.NoCategory():
              this.EditCategory()
      }
    </div>
  );
}

}