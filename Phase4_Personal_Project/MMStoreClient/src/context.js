import React, {Component} from 'react';

export const Context=React.createContext();
export const Consumer = Context.Consumer;

const ClearUser = (user) => {
  document.cookie='MMSU=;MaxAge=0;Path=/';
  document.cookie='MMST=;MaxAge=0;Path=/';
}

const SaveUser = (user) => {
  const info=encodeURIComponent(user.userEMail)+'|'+encodeURIComponent(user.userFirstName)+'|'+encodeURIComponent(user.userLastName);
  document.cookie='MMSU='+info+';Path=/';
  document.cookie='MMST='+user.userID+'|'+user.userIsAdmin+'|'+encodeURIComponent(user.accessToken)+';Path=/';
}

function GetCookie(name) {
  let value='';
  const cookies = document.cookie.split(";");
  for(var i = 0; i < cookies.length; i++) {
    const cookieval = cookies[i].split("=");
    if(name === cookieval[0].trim()) {
      value= decodeURIComponent(cookieval[1]);
      }
    }
  return value;
}

const RetrieveUser = () => {
  const user={userID:null,userFirstName:null, userLastName:null, userEMail:null, userIsAdmin:'N', accessToken:null};
  let c=GetCookie('MMSU');
  if (c.length>0) {
    c=c.split('|');
    if (c.length===3) {
      user.userFirstName=c[1];
      user.userLastName=c[2];
      user.userEMail=c[0];
      c=GetCookie('MMST');
      if (c.length>0) {
        c=c.split('|');
        if (c.length===3) {
          user.userID=parseInt(c[0]);
          user.userIsAdmin=c[1];
          user.accessToken=c[2];
          }
        }
      }
    }
  return user;
}

const reducer = (state,action) => {
  switch (action.type) {
    case 'UPDATE_USER':
      SaveUser(action.payload)
      return {
        ...state,
        user: action.payload
      }
    case 'LOGOFF_USER':
      ClearUser();
      return {
        ...state,
        user:{userID:null,userFirstName:null, userLastName:null, userEMail:null, userIsAdmin:'N', accessToken:null}
      }
    default: {return state}
  }
}

export class Provider extends Component {
  constructor(props) {
    super(props);
    this.state={
        user:RetrieveUser(),
        dispatch: action => this.setState(state => reducer(state,action))
        }
      }
       
  render() {
    return (
      <Context.Provider value={this.state}>
        {this.props.children}
      </Context.Provider>
      )
  }

}

