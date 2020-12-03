import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NavigationMenuItemDeleteComponent } from './navigation-menu-item-delete.component';

describe('NavigationMenuItemDeleteComponent', () => {
  let component: NavigationMenuItemDeleteComponent;
  let fixture: ComponentFixture<NavigationMenuItemDeleteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavigationMenuItemDeleteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavigationMenuItemDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
