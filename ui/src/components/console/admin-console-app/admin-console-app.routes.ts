import { Routes } from '@angular/router';
import { DefaultLayoutComponent } from './layout';

export const ddroutes: Routes = [
  {
    path: '',
    component: DefaultLayoutComponent,
    data: {
      title: 'Home'
    },
    children: [
      {
        path: 'dashboard',
        loadChildren: () => import('./views/dashboard/routes').then((m) => m.routes)
      },
      { path: '**', redirectTo: 'dashboard' }
    ],
  }
];