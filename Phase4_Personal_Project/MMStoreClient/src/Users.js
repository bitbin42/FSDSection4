import React, {Component} from "react";
import {Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class Users extends Component {
  constructor(props) {
    super(props);
    this.state={token:null,
      searchText: '',
      userresp:{offset:0,pageSize:5,totalRecords:0,prevPage:null,nextPage:null,users:{}}};
    }

  static contextType = Context;
  
  getData() {
  const options=apiFetchOptions('GET',null,this.state.token);
  const ps=this.state.userresp.pageSize;
  const st=this.state.searchText;
  const url=apiBaseURL+'/users/list?offset=0&sort=name&pagesize='+ps+'&search='+encodeURIComponent(st);
  fetch(url,options).then(r => r.json()).then(us => this.setState({userresp:us}));
  }
      
  searchSelected(searchtext) {
  this.setState({searchText:searchtext},() => this.getData());
  }

  navigateList(newpage) {
  const options=apiFetchOptions('GET',null,this.state.token);
  fetch(newpage,options).then(r => r.json()).then(us => this.setState({userresp:us}));
  }
    
  componentDidMount() {
  const context = this.context;
  this.setState({token:(context===undefined)?null:context.user.accessToken},
    () => this.getData());
  }

NeedLogon() {return <Link to='/logon'><button className='action'>Log On</button></Link>}
NotAdmin() {return 'Access denied'}
NoUsers() {return 'No users found'}

ShowUsers(isadmin) {
  let userfrom=0;
  let userto=0;
  if (this.state.userresp.totalRecords!==0) {
    userfrom=this.state.userresp.offset+1;
    userto=this.state.userresp.offset+this.state.userresp.users.length;  
    }
  return (
    <React.Fragment>
      <div className="container-fluid">
        Search: <input id='searchTarget' type='text' />
        <button id='doSearch' className='downright'
          onClick={(e) => this.searchSelected(document.getElementById('searchTarget').value)}>Find</button>
        Users {userfrom} to {userto} of {this.state.userresp.totalRecords}
      </div>
      <div className='container-fluid'>
        {!Array.isArray(this.state.userresp.users)?null:
          this.state.userresp.users.map(u=> {return <User user={u} key={u.userID} />})}
      </div>
      <div className='container-fluid productbox'>
        <Link to={{pathname:'/edituser',state:{id:0}}}><button className='downright'>Add User</button></Link>
        {this.state.userresp.prevPage==null?null:
          <button id='backButton' className='downright' onClick={(e) => this.navigateList(this.state.userresp.prevPage)}>&lt;&lt; Back</button>}
        {this.state.userresp.nextPage==null?null:
          <button id='nextButton' className='downright' onClick={(e) => this.navigateList(this.state.userresp.nextPage)}>&gt;&gt; Next</button>}
      </div>
    </React.Fragment>
    );
}


render() {
  const haveuser=(this.state.token!=null);
  const isadmin=(this.context.user!==undefined && this.context.user.userIsAdmin==='Y');
  const haveusers=(this.state.userresp!=null && this.state.userresp.totalRecords>0);
  return (
    <div className="MainBody">
      {(haveuser===false)?this.NeedLogon():
        (isadmin===false)?this.NotAdmin():
          (haveusers===false)?this.NoUsers():
            this.ShowUsers()
      }
    </div>
  );
}
  
}

// ====================================== User

export class User extends Component {

  static contextType = Context;
  
  render() {
    return (
      <div className="container-fluid productbox">
        <div className="boxed usernamebox">
          {this.props.user.userEMail} [{(this.props.user.userIsActive==="Y")?"Active":"Inactive"} {(this.props.user.userIsAdmin==="Y")?"Admin":"User"}]<br />
          {this.props.user.userLastName},({this.props.user.userFirstName})<br /> 
        </div>
        <div className="boxed">
          <Link to={{pathname:'/edituser',state:{id:this.props.user.userID}}}><button>Edit</button></Link>
        </div>
      </div>
      );
    }
}
