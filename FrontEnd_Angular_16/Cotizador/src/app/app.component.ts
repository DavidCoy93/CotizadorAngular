import { Component, OnInit } from '@angular/core';
import { fromEvent } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'GlobalAutomotive';


  $closeAppEvent = fromEvent(window, 'beforeunload');

  ngOnInit(): void {
    this.$closeAppEvent.subscribe(evt => {
      
    })
  }


}
