import React, {Component} from "react";
import {Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class EditUser extends Component {
  constructor(props) {
    super(props);
    const id=(this.props.location!==undefined && this.props.location.state!==undefined 
      && this.props.location.state.id!==undefined)?this.props.location.state.id:null;
    this.state={token:null,
      userID:id,
      userresp:null};
    }

static contextType = Context;

getData() {
if (this.state.userID!=null && this.state.token!=null) {
  if (this.state.userID===0) { // adding new User
    const nc={userID:0,userFirstName:'',userLastName:'',userEMail:'',userIsActive:"Y"}
    this.setState({userresp:nc});
    }
  else { // editing User
    const options=apiFetchOptions('GET',null,this.state.token);
    const url=apiBaseURL+'/users/'+this.state.userID;
    fetch(url,options).then(r => r.json()).then(us => this.setState({userresp:us}));
    }
  }
}

componentDidMount() {
this.setState({token:(this.context===undefined)?null:this.context.user.accessToken},
    () => this.getData());
}

SaveUser() {
const body={userFirstName:document.getElementById('ufname').value,userLastName:document.getElementById('ulname').value,
      userEMail:document.getElementById('uemail').value,
      userIsAdmin:(document.getElementById('uadmin').checked?"Y":"N"),
      userPassword:(document.getElementById('upw').value),
      userIsActive:(document.getElementById('uactive').checked?"Y":"N")};
const adding=(this.state.userresp.userID===0);
const options=apiFetchOptions(adding?'POST':'PATCH',body,this.state.token);
const url=apiBaseURL+'/users'+(adding?'':'/'+this.state.userresp.userID);
fetch(url,options).then(r => r.json()).then(us => this.setState({userresp:us},() => alert('User updated')))
  .catch(err => alert('Unable to save User'));
}

NeedLogon() {return <Link to='/logon'><button className='action'>Log On</button></Link>}
NotAdmin() {return 'Access denied'}
NotSelf() {return 'Can not edit self'}
NoUser() {return 'No user selected'}

EditUser() {
  const isactive=(this.state.userresp.userIsActive==='Y');
  const isadmin=(this.state.userresp.userIsAdmin==='Y');
  return (
  <React.Fragment>
    User EMail: <input id='uemail' type='text' style={{width:"300px"}} maxLength='64' defaultValue={this.state.userresp.userEMail} /><br />
    First Name: <input id='ufname' type='text' style={{width:"300px"}} maxLength='64' defaultValue={this.state.userresp.userFirstName} /><br />
    Last Name: <input id='ulname' type='text' style={{width:"300px"}} maxLength='64' defaultValue={this.state.userresp.userLastName} /><br />
    Password: <input id='upw' type='password' style={{width:"300px"}} maxLength='64' /><br />
    Admin: <input id='uadmin' type='checkbox' defaultChecked={isadmin} /><br />
    Active: <input id='uactive' type='checkbox' defaultChecked={isactive} /><br />
    <button className='Action' onClick={(e) => this.SaveUser()}>Save</button>
  </React.Fragment>
  )
}

render() {
  const haveuser=(this.state.token!=null);
  const haveuserrec=(this.state.userresp!=null);
  const isadmin=(this.context.user!==undefined && this.context.user.userIsAdmin==='Y');
  const isself=(this.state.userresp!=null && this.state.userresp.userID===this.context.user.userID);
  return (
    <div className="MainBody">
      {(haveuser===false)?this.NeedLogon():
        (isadmin===false)?this.NotAdmin():
          (haveuserrec===false)?this.NoUser():
            (isself)?this.NotSelf():
              this.EditUser()
      }
    </div>
  );
}

}