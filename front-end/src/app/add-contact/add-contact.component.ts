// add-contact.component.ts
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HomeComponent } from "../home/home.component";

@Component({
  selector: 'app-add-contact',
  standalone: true,
  templateUrl: './add-contact.component.html',
  styleUrl: './add-contact.component.css',
  imports: []
})
export class AddContactComponent {
  private router = inject(Router);
  public addingUserUsername: string = '';
  goBack(){
    this.router.navigate(['/']);
  }
}
