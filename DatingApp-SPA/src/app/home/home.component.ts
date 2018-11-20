import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  values: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  registerToggle() {
    // this.registerMode = !this.registerMode;

    // Child To Parent : Step 1: Switch to the child component (once register mode is true,
    // the entire register HTML container is deactivated: *ngIf="registerMode"
    this.registerMode = true;
  }

  // Parent To Child : Step 1: Prepare values in the local VALUES variable of the HOME component
  getValues() {
    this.http.get('http://localhost:5000/api/values').subscribe(response => {
      this.values = response;
    }, error => {
      console.log(error);
    });
  }

  // Child To Parent : Step 2: Switch comming from the child componet (home HTML) back to this parent component (Back to FALSE)
  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }

}
