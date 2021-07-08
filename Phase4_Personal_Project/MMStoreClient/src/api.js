export const apiBaseURL = 'http://localhost:5000/api';

export function apiFetchOptions(action,body,accesstoken) {
  let Options={method:action};
  let Headers={};
  if (body!=null) {
    Headers['Content-Type']='application/json';
    Options['body']=JSON.stringify(body);
    }
  if (accesstoken!==null && accesstoken!==undefined && accesstoken.length > 0) {
    Headers['Authorization']='Bearer '+accesstoken;
    }
  Options['headers']=Headers;
  return Options;
};

