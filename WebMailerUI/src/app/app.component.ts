import { Router} from '@angular/router';
import { Component } from '@angular/core';
import { Mail } from './mail';
import { APIServiceService } from '../app/apiservice.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent{

  title = 'WebMailerUI';
  mailModel = new Mail('chit269369@gmail.com','chitlaynaing@gmail.com','WebMailerAPI-Test','This is test mail content.','','');
  topicHasError = true;
  submitted = false;
  errorMsg = '';
  message = '';
  
  constructor(private _apiService: APIServiceService,private _router: Router) {}

  validateTopic(value) {
    if (value === 'default') {
      this.topicHasError = true;
    } else {
      this.topicHasError = false;
    }
  }

  ngOnInit(): void {
    this._router.navigate([''])
  }

  onSubmit() {

    this.submitted = true;

    this._apiService.sendMail(this.mailModel)
      .subscribe(
        response => { this.submitted =false;
                      this.message = 'Successfully Sent !'; 
                    },
        error => { this.submitted =false;
                  this.errorMsg =  'Error Occurs :_(';   
                 }
      );
      setTimeout(() => {
        this.message ='';
        this.errorMsg ='';
      }, 3000);
      
  }
}

