// app.routes.ts
import { Routes } from '@angular/router';
import { AddContactComponent } from './add-contact/add-contact.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { authGuard } from '../guard/auth.guard';
import { UserConfigComponent } from './user-config/user-config.component';
import { SigninComponent } from './signin/signin.component';

export const routes: Routes = [
  { 
    path: 'login', 
    component: LoginComponent
  },
  { 
    path: 'signin', 
    component: SigninComponent
  },
  { 
    path: '', 
    component: HomeComponent,
    canActivate: [authGuard]  // Proteger la ruta principal
  },
  {
    path: 'add-contact',
    component: AddContactComponent,
    canActivate: [authGuard]  // Proteger esta ruta también
  },
  {
    path: 'user-config',
    component: UserConfigComponent,
    canActivate: [authGuard]  // Proteger esta ruta también
  },
  { 
    path: '**', 
    redirectTo: '' 
  }
];