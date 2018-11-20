import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { initDomAdapter } from '@angular/platform-browser/src/browser';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // Parent To Child : Step 3: Getting from Input the VALUES FROM HOME which includes values from the HOME component
  @Input() valuesFromHome: any;
  // Child To Parent : Step 4: Add Output Property amd assign EventEmmitter
  // in order to get a Switch comming from the child componet (home HTML) back to this parent component
  @Output() cancelRegister = new EventEmitter();

  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    // console.log(this.model);
    this.authService.register(this.model).subscribe(() => {
      console.log('registration sucessful');
    }, error => {
      console.log(error);
    });
  }

  cancel() {
    // Child To Parent : Step 6: Use provided EventEmitter to provide FALSE to the caller (parent)
    // In this case it is a simple boolean. But it could be an object.
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }

}
