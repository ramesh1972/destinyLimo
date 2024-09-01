import { Component } from '@angular/core';
import { NavigationEnd, Router, } from '@angular/router';
import { CommonModule } from '@angular/common';

import { Title } from '@angular/platform-browser';

import { Store } from '@ngrx/store';

import { IconSetService } from '@coreui/icons-angular';
import { iconSubset } from './icons/icon-subset';

import { AdminConsoleAppComponent } from './admin-console-app/admin-console-app.component';
import { LearnerConsoleAppComponent } from './learner-console-app/learner-console-app.component';

@Component({
  selector: 'app-console',
  standalone: true,
  imports: [CommonModule, AdminConsoleAppComponent, LearnerConsoleAppComponent],
  templateUrl: './console.component.html',
  styleUrl: './console.component.css'
})
export class ConsoleComponent {
  constructor(private store: Store, private router: Router,
    private titleService: Title,
    private iconSetService: IconSetService
  ) {
    this.titleService.setTitle("destiny-limo admin console");
    // iconSet singleton
    this.iconSetService.icons = { ...iconSubset };
  }

  loggedInUserRole: string = "learner";

  ngOnInit() {
    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
    });
  }
}
