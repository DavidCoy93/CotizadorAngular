import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { AdmonRatesComponent } from './pages/admon-rates/admon-rates.component';
import { ProgramPageComponent } from './pages/program-page/program-page.component';
import { StartPageComponent } from './pages/start-page/start-page.component';

const routes: Routes = [
  { 
    path: '',
    component: HomeComponent,
    children: [
      { path: 'admon-rates', component: AdmonRatesComponent },
      { path: 'program', component: ProgramPageComponent },
      { path: 'inicio', component: StartPageComponent  },
      { path: '', redirectTo: 'inicio', pathMatch: 'full' }
    ] 
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
