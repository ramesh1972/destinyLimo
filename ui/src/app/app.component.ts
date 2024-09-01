import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RouterLinkActive } from '@angular/router';

import { Store } from '@ngrx/store';

import { IconSetService } from '@coreui/icons-angular';
import { iconSubset } from '../components/console/icons/icon-subset';

import { LandingComponent } from '@src/components/landing/landing.component';
import { ConsoleComponent } from '../components/console/console.component';
import { AdminDefaultLayoutComponent } from '../components/console/admin-console-app/layout';
import { LearnerDefaultLayoutComponent } from '../components/console/learner-console-app/layout';

import { invokeContentFetchAPI } from '../store/actions/content.action';
import { invokeMaterialCategoryFetchAPI } from '../store/actions/material.action';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink, RouterLinkActive, LandingComponent, ConsoleComponent, AdminDefaultLayoutComponent, LearnerDefaultLayoutComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {

  constructor(private store: Store,     private iconSetService: IconSetService) {
        // iconSet singleton
        this.iconSetService.icons = { ...iconSubset };
  }

  title = 'destiny-limo';

  ngOnInit() {
    console.log('App component initialized');

    this.store.dispatch(invokeContentFetchAPI());
    this.store.dispatch(invokeMaterialCategoryFetchAPI());
  }
}
