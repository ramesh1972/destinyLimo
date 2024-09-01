import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AdminConsoleAppComponent } from './admin-console-app.component';

describe('AdminConsoleAppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [
        RouterTestingModule,
        AdminConsoleAppComponent
    ],
}).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AdminConsoleAppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'CoreUI Angular Admin Template'`, () => {
    const fixture = TestBed.createComponent(AdminConsoleAppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('CoreUI Angular Admin Template');
  });
});
