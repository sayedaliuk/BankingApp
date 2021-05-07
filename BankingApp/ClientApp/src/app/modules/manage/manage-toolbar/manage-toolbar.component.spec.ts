import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageToolbarComponent } from './manage-toolbar.component';

describe('ManageToolbarComponent', () => {
  let component: ManageToolbarComponent;
  let fixture: ComponentFixture<ManageToolbarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManageToolbarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageToolbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
