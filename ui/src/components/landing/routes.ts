import { Routes } from '@angular/router';

export const landingRoutes: Routes = [
  {
    path: 'about-us',
    loadComponent: () => import('./about-us/about-us.component').then(m => m.AboutUsComponent),
    data: {
      title: 'About Us'
    }
  },
  {
    path: 'register',
    loadComponent: () => import('./home-register/home-register.component').then(m => m.HomeRegisterComponent),
    data: {
      title: 'Change Password'
    }
  }
];

