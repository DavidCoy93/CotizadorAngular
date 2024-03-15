import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home.component';
import { MenuComponent } from './components/menu/menu.component';
import { MatIconModule } from '@angular/material/icon';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AdmonRatesComponent } from './pages/admon-rates/admon-rates.component';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ModalCrudComponent } from './components/modal-crud/modal-crud.component';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { ProgramPageComponent } from './pages/program-page/program-page.component';
import { ModalCrudRateComponent } from './components/modal-crud-rate/modal-crud-rate.component';
import { StartPageComponent } from './pages/start-page/start-page.component';



@NgModule({
  declarations: [
    HomeComponent,
    MenuComponent,
    AdmonRatesComponent,
    ModalCrudComponent,
    ProgramPageComponent,
    ModalCrudRateComponent,
    StartPageComponent
  ],
  imports: [
    CommonModule,
    HomeRoutingModule,
    MatIconModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatDialogModule
  ],
  
})
export class HomeModule { }
