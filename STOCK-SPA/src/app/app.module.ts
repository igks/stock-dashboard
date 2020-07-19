import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConfirmDialogModule } from 'src/app/components/confirm-dialog/confirm-dialog.module';
import { NavbarComponent } from './displays/navbar/navbar.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';

import { BrokerService } from './services/broker.service';
import { StockService } from './services/stock.service';
import { HttpClientModule } from '@angular/common/http';
import { AlertService } from './services/alert.service';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { MatNativeDateModule } from '@angular/material/core';
import { StockPriceService } from './services/stock-price.service';
import { ErrorInterceptorProvider } from './services/error.interceptor';
import { StockVolumeService } from './services/stock-volume.service';
import { DashboardService } from './services/dashboard.service';

@NgModule({
  declarations: [AppComponent, NavbarComponent],
  imports: [
    ToastrModule.forRoot(),
    AppRoutingModule,
    HttpClientModule,
    ConfirmDialogModule,

    // Angular material module
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatMenuModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  providers: [
    AlertService,
    ToastrService,
    BrokerService,
    StockService,
    StockPriceService,
    StockVolumeService,
    ErrorInterceptorProvider,
    DashboardService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
