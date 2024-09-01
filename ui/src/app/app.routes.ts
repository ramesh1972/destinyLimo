import { Routes } from '@angular/router';

import { HomeComponent } from '../components/landing/home/home.component';

import { landingRoutes } from '../components/landing/routes';
import { adminConsoleRoutes } from '../components/console/admin-console-app/views/routes';
import { learnerConsoleRoutes } from '../components/console/learner-console-app/views/routes';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'home',
        children: landingRoutes
    },
    {
        path: 'admin/console',
        data: {
            title: 'Admin Home'
        },
        children: adminConsoleRoutes
    },
    {
        path: 'learner/console',
        data: {
            title: "Learner's Home"
        },
        children: learnerConsoleRoutes
    }
];
