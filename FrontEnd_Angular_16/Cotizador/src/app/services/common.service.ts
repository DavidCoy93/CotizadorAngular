import { Injectable } from '@angular/core';
import { User } from '../models/user-item';
import { BehaviorSubject } from 'rxjs';
import { MenuItem } from '../models/menu-item';

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  userData!: User;
  token!: string;
  
  constructor() { }

  isNumber(evt: KeyboardEvent): boolean {

    const regExp = /[0-9.]/g

    const isNumeric = regExp.test(evt.key);

    return isNumeric;
  }
}
