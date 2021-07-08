import React, {Component} from "react";
import {Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';

export class Logon extends Component {

  static contextType = Context;

  completeLogon(resp) {
    this.context.dispatch({type:'UPDATE_USER',payload:resp});
    document.getElementById('logonmessage').innerText='Welcome back '+resp.userFirstName+' '+resp.userLastName;
    }
  
    logonFailed(err) {
    document.getElementById('logonmessage').innerText='Logon failed';
    }
  
    doLogon() {
    const body={userEMail:document.getElementById('txtEMail').value,userPassword:document.getElementById('txtPW').value};
    const url=apiBaseURL+'/authorize/login';
    const options=apiFetchOptions('POST',body,null);
    document.getElementById('logonmessage').innerText='Logging on...';
    fetch(url,options).then(r => r.json()).then(r => this.completeLogon(r)).catch(e => this.logonFailed(e));
    }
  
    completeRegistration(resp) {
      this.context.dispatch({type:'UPDATE_USER',payload:resp});
      document.getElementById('registermessage').innerText='Welcome '+resp.userFirstName+' '+resp.userLastName;
      }
    
    registrationFailed(err) {
      document.getElementById('registermessage').innerText='Registration failed';
      }
    
    doRegister() {
      const email=document.getElementById('txtNewEMail').value.trim();
      const pw1=document.getElementById('txtNewPW').value.trim();
      const pw2=document.getElementById('txtNewPW2').value.trim();
      const first=document.getElementById('txtNewFN').value.trim();
      const last=document.getElementById('txtNewLN').value.trim();
      const msg=document.getElementById('registermessage');
      if (email.length===0) {msg.innerText='An email address is required';}
      else if (pw1.length===0) {msg.innerText='A password is required';}
      else if (pw1!==pw2) {msg.innerText='The passwords do not match';}
      else if (first.length===0 || last.length===0) {msg.innerText='First and Last name are required';}
      else {
        const body={userEMail:email,userPassword:pw1,userFirstName:first,userLastName:last};
        const url=apiBaseURL+'/authorize/register';
        const options=apiFetchOptions('POST',body,null);
        msg.innerText='Registering...';
        fetch(url,options).then(r => r.json()).then(r => this.completeRegistration(r)).catch(e => this.registrationFailed(e));
        }
      }
    
  render () {
    return (
      <div className="MainBody row">
        <div className="col-auto downright border">
          Returning users:<br />
          EMail address: <input id='txtEMail' required={true} width={250} type='email' /><br />
          Password: <input id='txtPW' required={true} width={250} type='password' /><br />
          <button id='cmdLogon' onClick={(x) => this.doLogon()} className='ActionButton'>Log on</button><br /><br />
          <span id='logonmessage' />
        </div>
        <div className="col-auto border">
          New users:<br />
          EMail address: <input id='txtNewEMail' required={true} width={250} type='email' /><br />
          Password: <input id='txtNewPW' required={true} width={250} type='password' /><br />
          Confirm Password: <input id='txtNewPW2' required={true} width={250} type='password' /><br />
          First Name: <input id='txtNewFN' required={true} width={250} type='text' /><br />
          Last Name: <input id='txtNewLN' required={true} width={250} type='text' /><br />
          <button id='cmdRegister' onClick={(x) => this.doRegister()} className='ActionButton'>Register</button><br /><br />
          <span id='registermessage' />
        </div>
      </div>
    );
  }
};

export class Logoff extends Component {

  static contextType = Context;

  msg='Please log on to make purchases';

  componentDidMount() {
  if (this.context!==undefined && this.context.user!=null && this.context.user.accessToken!==null) {
    this.context.dispatch({type:'LOGOFF_USER',payload:null});
    this.msg='Come back soon!';
    }
  }
 
  render () {
    return (
      <div className="MainBody">
        <span>{this.msg}</span>
      </div>
    )
  }

  }
