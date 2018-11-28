import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;

  constructor(public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      // console.log('Logged in successfully');
      this.alertify.success('Logged in successfully');
    }, error => {
      // console.log(error);
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/members']);
    });
    // console.log(this.model);
  }

  loggedIn() {
    // This would simply check if there is something in that token, then return TRUE.
    // const token = localStorage.getItem('token');
    // return !!token;

    // Better is to check the token on the client side (without full validation - will happen anyway on the server)
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    // console.log('logged out');
    this.alertify.message('logged out');
    this.router.navigate(['/home']);
  }

}
