import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AlertService } from './services/alert.service';
import { ToastrService } from 'ngx-toastr';
import { MatMenuModule } from '@angular/material/menu';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { ChartsModule } from 'ng2-charts';

import { DashboardComponent } from './displays/dashboard/dashboard.component';
import { BrokerListComponent } from './displays/master/broker/broker-list/broker-list.component';
import { BrokerFormComponent } from './displays/master/broker/broker-form/broker-form.component';
import { StockListComponent } from './displays/master/stock/stock-list/stock-list.component';
import { StockFormComponent } from './displays/master/stock/stock-form/stock-form.component';

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent,
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
  },
  {
    path: 'broker',
    component: BrokerListComponent,
  },
  {
    path: 'broker/form',
    component: BrokerFormComponent,
  },
  {
    path: 'broker/form/:id',
    component: BrokerFormComponent,
  },
  {
    path: 'stock',
    component: StockListComponent,
  },
  {
    path: 'stock/form',
    component: StockFormComponent,
  },
  {
    path: 'stock/form/:id',
    component: StockFormComponent,
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];

@NgModule({
  declarations: [
    DashboardComponent,
    BrokerListComponent,
    BrokerFormComponent,
    StockListComponent,
    StockFormComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forRoot(routes),
    MatCardModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatMenuModule,
    MatDatepickerModule,
    ChartsModule,
  ],
  exports: [RouterModule],
  providers: [AlertService, ToastrService],
})
export class AppRoutingModule {}
