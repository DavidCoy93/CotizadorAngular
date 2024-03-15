import { Component, OnInit, Inject } from '@angular/core';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DialogComponent } from 'src/app/components/dialog/dialog.component';
import { DialogDataItem } from 'src/app/models/dialog-data-item';
import { User } from 'src/app/models/user-item';
import { GlobalAutomotiveService } from 'src/app/services/global-automotive.service';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { APP_BASE_HREF } from '@angular/common';

interface LoginFormItem {
  User: FormControl<string>,
  Password: FormControl<string>
}

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit  {

  isSmallDevice: boolean = false;
  isLoading: boolean = false;

  loginForm: FormGroup<LoginFormItem> = new FormGroup<LoginFormItem>(
    {
      User: new FormControl<string>('', { nonNullable: true, validators: [Validators.required]}),
      Password: new FormControl<string>('', {nonNullable: true, validators: [Validators.required]})
    }
  )

  get formControls() {return this.loginForm.controls};

  get currentYear() { return new Date().getFullYear() };

  constructor(
    public breakPointObserver: BreakpointObserver,
    private dialog: MatDialog,
    private globalAutoService: GlobalAutomotiveService,
    private commonService: CommonService,
    private router: Router,
    @Inject(APP_BASE_HREF) public baseRef: string
  ) {}

  ngOnInit(): void {
    this.breakPointObserver.observe(["(max-width: 768px)"]).subscribe((state: BreakpointState) => {
      this.isSmallDevice = state.matches;
    })
  }

  submitForm(): void {
    if (this.loginForm.valid) {

      this.isLoading = true;

      const userData: User = {
        Id: null,
        Active: null,
        ChangePass: null,
        CrudHistoryList: [],
        Email: this.formControls.User.value,
        Password: this.formControls.Password.value,
        EmailpasswordRecovery: null,
        Userdata: {
          Goal: 0,
          IdBrand: 0,
          IdDistributor: 0,
          IdLevel: 0,
          IdRole: 0,
          MaternalSurname: '',
          MiddleName: '',
          Name: '',
          PaternalSurname: ''
        }
      }

      this.globalAutoService.LoginAccess(userData).subscribe(
        {
          next: (value) => {
            const today = new Date();
            const expirationDate = new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1, today.getHours(), today.getMinutes(), today.getSeconds());
            sessionStorage.setItem("UserData", JSON.stringify(value));
            document.cookie = 'user=' + value.user.Email + ';expires=' + expirationDate.toUTCString();
            this.commonService.userData = value.user;
            this.commonService.token = value.token;
            this.isLoading = false;
            this.router.navigate(['/', 'home'])
          },
          error: (err: HttpErrorResponse) => {
            this.isLoading = false;
            this.dialog.open<DialogComponent, DialogDataItem, boolean>(DialogComponent, 
              {
                data: { Title: 'Error', Content:  err.error.Message, Type: 'message' },
                height: '300px',
                width: '400px',
                disableClose: true
              }
            )
          }
        }
      )

    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}