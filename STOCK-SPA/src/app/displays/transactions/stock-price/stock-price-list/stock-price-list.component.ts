import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { StockPrice } from 'src/app/models/stock-price';
import { StockPriceService } from 'src/app/services/stock-price.service';
import { AlertService } from 'src/app/services/alert.service';
import { DateHelperService } from 'src/app/services/date-helper.service';
import { Pagination, PaginatedResult } from 'src/app/models/pagination';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-stock-price-list',
  templateUrl: './stock-price-list.component.html',
})
export class StockPriceListComponent implements OnInit {
  constructor(
    private stockPriceService: StockPriceService,
    private dateHelperService: DateHelperService,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('stockPriceHistory', { static: false })
  stockPriceHistory: ElementRef;
  @ViewChild('dailyPrice', { static: false }) dailyPrice: ElementRef;

  displayedColumns: string[] = [
    'date',
    'stock',
    'price',
    'change',
    'change-ratio',
    'open',
    'high',
    'low',
    'volume',
    'option',
  ];
  stockPrices: StockPrice[];
  pagination: Pagination;
  stockPriceParams: any = {};
  model: any;
  isLoading: boolean = false;
  isFiltered: boolean = false;
  showFilterForm: boolean = false;
  form: FormGroup;

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      date: [''],
      stock: [''],
    });

    this.loadStockPrice();
    this.stockPriceService.getStockPrices().subscribe((data) => {});
  }

  loadStockPrice() {
    this.isLoading = true;
    this.stockPriceService.getStockPrices().subscribe(
      (res: PaginatedResult<StockPrice[]>) => {
        this.stockPrices = res.result;
        this.pagination = res.pagination;
        this.isLoading = false;
      },
      (error) => {
        this.alert.Error('', error);
        this.isLoading = false;
      }
    );
  }

  updateStockPrice() {
    this.stockPriceService
      .getStockPrices(
        this.pagination.currentPage,
        this.pagination.pageSize,
        this.stockPriceParams
      )
      .subscribe(
        (res: PaginatedResult<StockPrice[]>) => {
          this.stockPrices = res.result;
          this.pagination = res.pagination;
        },
        (error) => {
          this.alert.Error('', error);
        }
      );
  }

  pageEvents(event: any) {
    this.pagination.currentPage = event.pageIndex + 1;
    this.pagination.pageSize = event.pageSize;
    this.updateStockPrice();
  }

  sortChange(event: any) {
    this.pagination.currentPage = 1;
    this.stockPriceParams.OrderBy = event.active;
    this.stockPriceParams.isDescending =
      event.direction === 'desc' ? true : false;
    this.updateStockPrice();
  }

  setShowFilterForm() {
    this.showFilterForm = true;
  }

  addFilter() {
    this.stockPriceParams.date = '';
    this.stockPriceParams.stock = '';

    if (this.form.value.date != null && this.form.value.date != '') {
      this.stockPriceParams.date = this.dateHelperService.dateToSave(
        this.form.value.date
      );
      this.isFiltered = true;
    }
    if (this.form.value.stock != null && this.form.value.stock != '') {
      this.stockPriceParams.stock = this.form.value.stock;
      this.isFiltered = true;
    }

    this.showFilterForm = false;
    this.updateStockPrice();
  }

  cancelFilter() {
    this.form.reset();
    this.isFiltered = false;
    this.showFilterForm = false;
  }

  clearFilter() {
    this.isFiltered = false;
    this.pagination.currentPage = 1;
    this.stockPriceParams.date = null;
    this.stockPriceParams.stock = null;
    this.form.reset();
    this.updateStockPrice();
  }

  deleteStockPrice(id: number) {
    const confirm = this.alert.Confirm();
    confirm.afterClosed().subscribe((result) => {
      if (result === true) {
        this.stockPriceService.deleteStockPrice(id).subscribe(
          () => {
            this.alert.Info('', 'The data has been deleted');
            this.updateStockPrice();
          },
          (error) => {
            this.alert.Error('', error);
          }
        );
      }
    });
  }

  uploadFile(type, event) {
    if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];
      const ext = file.name.split('.').pop().toLowerCase();

      if (!this.isFileValid(ext)) {
        this.alert.Error('File not allowed', 'Please upload only csv file!');
        this.stockPriceHistory.nativeElement.value = '';
        this.dailyPrice.nativeElement.value = '';
        return;
      }

      switch (type) {
        case 'daily':
          this.stockPriceService.uploadDailyPrice(file).subscribe(
            (data) => {
              if (data) {
                if (data.type == HttpEventType.Response) {
                  this.alert.Success('', 'File successfully uploaded.');
                  this.dailyPrice.nativeElement.value = '';
                  this.updateStockPrice();
                }
              }
            },
            (error) => {
              this.alert.Error('', error);
              this.dailyPrice.nativeElement.value = '';
            }
          );
          break;

        case 'history':
          this.stockPriceService.uploadPriceHistory(file).subscribe(
            (data) => {
              if (data) {
                if (data.type == HttpEventType.Response) {
                  this.alert.Success('', 'File successfully uploaded.');
                  this.stockPriceHistory.nativeElement.value = '';
                  this.updateStockPrice();
                }
              }
            },
            (error) => {
              this.alert.Error('', error);
              this.stockPriceHistory.nativeElement.value = '';
            }
          );
          break;
      }
    }
  }

  isFileValid(ext) {
    if (ext !== 'csv') {
      return false;
    }
    return true;
  }
}
