import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input, OnInit, Inject } from '@angular/core';
import { UserMenu } from 'src/app/models/menu-item';
import { CommonService } from 'src/app/services/common.service';
import { GlobalAutomotiveService } from 'src/app/services/global-automotive.service';
import { faArrowRightFromBracket }  from '@fortawesome/free-solid-svg-icons';
import { APP_BASE_HREF } from '@angular/common';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  @Input() isSmallDevice: boolean = false;
  baseHref: string = '';
  userName: string = '';
  userMenu: Array<UserMenu> = [];
  logoutIcon =  faArrowRightFromBracket;

  constructor(
    private commonService: CommonService,
    private globalAutoService: GlobalAutomotiveService,
    @Inject(APP_BASE_HREF) public baseRef: string
  ) {
    if (typeof this.commonService.userData === 'undefined') {
      this.commonService.userData = {
        Id: 0,
        Active: true,
        ChangePass: false,
        CrudHistoryList: [],
        Email: 'admin@assurant.com',
        Password: 'abc123',
        EmailpasswordRecovery: '',
        Userdata: {
          Goal: 0,
          IdBrand: 0,
          IdDistributor: 0,
          IdLevel: 0,
          IdRole: 1,
          Name: 'Felipe',
          PaternalSurname: 'Calderón',
          MaternalSurname: 'Del Sagrado Corazón De Jesus Hinojosa',
          MiddleName: ''
        }
      }
    }

    this.userName = `${this.commonService.userData.Userdata.Name} ${this.commonService.userData.Userdata.PaternalSurname} ${this.commonService.userData.Userdata.MaternalSurname}`;
  }

  ngOnInit(): void {
    this.globalAutoService.GetMenu(this.commonService.userData.Userdata.IdRole).subscribe(
      {
        next: (value) => {
          value.forEach((item, index) => {
            if (item.MenuData.IdParent === 0) {
              const subMenus = value.filter(m => m.MenuData.IdParent === item.Id);
              this.userMenu.push( { Menu: item, SubMenus: subMenus  } );
            }
          })
        },
        error: (err: HttpErrorResponse) => {
          this.userMenu = [];
        }
      }
    )
  }

  logOut(): void {
    const today: Date = new Date();
    sessionStorage.clear();
    document.cookie = 'user=;expires=' + new Date(today.getFullYear(), today.getMonth(), today.getDate() - 2).toUTCString();
    window.location.reload();
  }

}
