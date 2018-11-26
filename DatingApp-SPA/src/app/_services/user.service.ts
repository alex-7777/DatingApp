import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

// Simple way of sending token within a header, but it doesnt work onInit, because in that moment token is not ready
// const httpOptions = {
//   headers: new HttpHeaders({
//     'Authorization': 'Bearer' + ' ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> {
  return this.http.get<User[]>(this.baseUrl + 'users');
  // return this.http.get<User[]>(this.baseUrl + 'users', httpOptions); // Simple way of sending token within a header
}

getUser(id): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'users/' + id);
  // return this.http.get<User>(this.baseUrl + 'user/' + id, httpOptions); // Simple way of sending token within a header
}

updateUser(id: number, user: User) {
  return this.http.put(this.baseUrl + 'users/' + id, user);
}



}
