import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-manage-home',
  templateUrl: './manage-home.component.html',
  styleUrls: ['./manage-home.component.css']
})
export class ManageHomeComponent implements OnInit {
  constructor() { }

  ngOnInit(): void {
  }

  moduleHeading : string = 'Manage Bank Account';
}
