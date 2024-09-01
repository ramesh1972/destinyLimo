import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RouterLinkActive } from '@angular/router';

import { HeaderComponent } from './common/header/header.component';
import { FooterComponent } from './common/footer/footer.component';
import { MenuComponent } from './common/menu/menu.component';
import { HomeComponent } from './home/home.component';
import { AboutUsComponent } from './about-us/about-us.component';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink, RouterLinkActive,HeaderComponent, FooterComponent, MenuComponent, HomeComponent, AboutUsComponent,  ],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css'
})
export class LandingComponent {

}
