import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Stock } from 'src/app/models/stock';
import { StockService } from 'src/app/services/stock.service';
import { AlertService } from 'src/app/services/alert.service';
import { Pagination, PaginatedResult } from 'src/app/models/pagination';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-stock-list',
  templateUrl: './stock-list.component.html',
})
export class StockListComponent implements OnInit {
  constructor(
    private stockService: StockService,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  displayedColumns: string[] = ['code', 'name', 'volume', 'option'];
  stocks: Stock[];
  pagination: Pagination;
  stockParams: any = {};
  model: any;
  isLoading: boolean = false;
  isFiltered: boolean = false;
  showFilterForm: boolean = false;
  form: FormGroup;

  ngOnInit() {
    this.form = this.formBuilder.group({
      code: [''],
      name: [''],
      volume: [''],
    });

    this.loadStock();
  }

  loadStock() {
    this.isLoading = true;
    this.stockService.getStocks().subscribe(
      (res: PaginatedResult<Stock[]>) => {
        this.stocks = res.result;
        this.pagination = res.pagination;
        this.isLoading = false;
      },
      (error) => {
        this.alert.Error('', error);
        this.isLoading = false;
      }
    );
  }

  updateStock() {
    this.stockService
      .getStocks(
        this.pagination.currentPage,
        this.pagination.pageSize,
        this.stockParams
      )
      .subscribe(
        (res: PaginatedResult<Stock[]>) => {
          this.stocks = res.result;
          this.pagination = res.pagination;
          this.isLoading = false;
        },
        (error) => {
          this.alert.Error('', error);
        }
      );
  }

  pageEvents(event: any) {
    this.pagination.currentPage = event.pageIndex + 1;
    this.pagination.pageSize = event.pageSize;
    this.updateStock();
  }

  sortChange(event: any) {
    this.pagination.currentPage = 1;
    this.stockParams.OrderBy = event.active;
    this.stockParams.isDescending = event.direction === 'desc' ? true : false;
    this.updateStock();
  }

  setShowFilterForm() {
    this.showFilterForm = true;
  }

  addFilter() {
    this.showFilterForm = false;
    if (
      (this.form.value.code != null && this.form.value.code != '') ||
      (this.form.value.name != null && this.form.value.name != '')
    ) {
      this.isFiltered = true;
      this.pagination.currentPage = 1;
      this.stockParams.code = this.form.value.code;
      this.stockParams.name = this.form.value.name;
      this.updateStock();
    }
  }

  cancelFilter() {
    this.form.reset();
    this.isFiltered = false;
    this.showFilterForm = false;
  }

  clearFilter() {
    this.isFiltered = false;
    this.pagination.currentPage = 1;
    this.stockParams.code = null;
    this.stockParams.name = null;
    this.form.reset();
    this.updateStock();
  }

  deleteStock(id: number) {
    const confirm = this.alert.Confirm();
    confirm.afterClosed().subscribe((result) => {
      if (result === true) {
        this.stockService.deleteStock(id).subscribe(
          () => {
            this.alert.Info('', 'The data has been deleted');
            this.updateStock();
          },
          (error) => {
            this.alert.Error('', error);
          }
        );
      }
    });
  }
}
