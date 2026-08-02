import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ParqueosPage } from './parqueos.page';

describe('ParqueosPage', () => {
  let component: ParqueosPage;
  let fixture: ComponentFixture<ParqueosPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(ParqueosPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
