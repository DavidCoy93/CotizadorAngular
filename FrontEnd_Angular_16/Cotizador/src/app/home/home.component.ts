import { Component, OnInit } from '@angular/core';
import { BreakpointObserver, BreakpointState } from '@angular/cdk/layout';
import { CommonService } from '../services/common.service';
import { GlobalAutomotiveService } from '../services/global-automotive.service';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  isSmallDevice: boolean = false;
  $IsSmall: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.isSmallDevice);
  

  constructor(
    private breakPointObserver: BreakpointObserver
  ) {}

  ngOnInit(): void {
    this.breakPointObserver.observe(["(max-width: 768px)"]).subscribe((state: BreakpointState) => {
      this.isSmallDevice = state.matches;
      this.$IsSmall.next(this.isSmallDevice);
    });
  }
}
