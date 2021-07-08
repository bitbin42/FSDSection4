import React, {Component} from "react";
import {Consumer, Context} from './context';
import {apiBaseURL,apiFetchOptions} from './api';
import {Link} from "react-router-dom";

export default class Categories extends Component {
  constructor(props) {
    super(props);
    this.state={token:null,
      categoryresp:{offset:0,pageSize:5,totalRecords:0,prevPage:null,nextPage:null,categories:{}}};
    }

  static contextType = Context;
  
  navigateList(newpage) {
  const options=apiFetchOptions('GET',null,this.state.token);
  fetch(newpage,options).then(r => r.json()).then(cs => this.setState({categoryresp:cs}));
  }

  getData() {
  if (this.state.token!=null) {
    const ps=this.state.categoryresp.pageSize;
    const url=apiBaseURL+'/categories/list?offset=0&sort=name&pagesize='+ps;
    let options=apiFetchOptions('GET',null,this.state.token);
    fetch(url,options).then(r => r.json()).then(cs => this.setState({categoryresp:cs}));
    }
  }
    
  componentDidMount() {
  const context = this.context;
  this.setState({token:(context===undefined)?null:context.user.accessToken},
    () => this.getData());
  }

NeedLogon() {return <Link to='/logon'><button className='action'>Log On</button></Link>}
NotAdmin() {return 'Access denied'}
NoCategories() {return 'No categories found'}

ShowCategories(isadmin) {
let catfrom=0;
let catto=0;
if (this.state.categoryresp.totalRecords!==0) {
  catfrom=this.state.categoryresp.offset+1;
  catto=this.state.categoryresp.offset+this.state.categoryresp.categories.length;  
  }
return (
  <React.Fragment>
    <div className="container-fluid">
      Categories {catfrom} to {catto} of {this.state.categoryresp.totalRecords}
    </div>
    <div className='container-fluid'>
      {!Array.isArray(this.state.categoryresp.categories)?null:
        this.state.categoryresp.categories.map(p => {return <Category category={p} key={p.categoryID} />})}
    </div>
    <div className='container-fluid productbox'>
      {isadmin?<Link to={{pathname:'/editcategory',state:{id:0}}}><button className='downright'>Add category</button></Link>:null}
      {this.state.categoryresp.prevPage==null?null:
        <button id='backButton' className='downright' onClick={(e) => this.navigateList(this.state.categoryresp.prevPage)}>&lt;&lt; Back</button>}
      {this.state.categoryresp.nextPage==null?null:
        <button id='nextButton' className='downright' onClick={(e) => this.navigateList(this.state.categoryresp.nextPage)}>&gt;&gt; Next</button>}
    </div>
  </React.Fragment>
  );
}

render() {
      return (
      <Consumer>
        {value => {
          const haveuser=(value.user.accessToken!==null);
          const isadmin=(value.user.userIsAdmin==='Y');
          const havecats=(this.state.categoryresp.totalRecords>0);
          return (
            <div className="MainBody">
              {(haveuser===false)?this.NeedLogon():
                (isadmin===false)?this.NotAdmin():
                  (havecats===false)?this.NoCategories():
                      this.ShowCategories(isadmin)
              } 
            </div>
            )
          }
        }
      </Consumer>
    );
  }
};

// ====================================== category

export class Category extends Component {

  static contextType = Context;

  render() {
      return (
      <Consumer>
        {value => {
          const isadmin=(value.user.userIsAdmin==='Y');
          return (
            <div className="container-fluid productbox">
              <div className="boxed productnamebox">
                {this.props.category.categoryName} [{(this.props.category.categoryIsActive==="Y")?"Active":"Inactive"}]<br />
                {this.props.category.categoryInfo}
              </div>
                {(isadmin===false)?null:
                  <div className="boxed">
                    <Link to={{pathname:'/editcategory',state:{id:this.props.category.categoryID}}}><button>Edit</button></Link>
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
