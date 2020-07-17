import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { StockService } from 'src/app/services/stock.service';
import { DateHelperService } from 'src/app/services/date-helper.service';
import { AlertService } from 'src/app/services/alert.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StockPrice } from 'src/app/models/stock-price';
import { StockPriceService } from 'src/app/services/stock-price.service';
import { Pagination, PaginatedResult } from 'src/app/models/pagination';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-stock-form',
  templateUrl: './stock-form.component.html',
})
export class StockFormComponent implements OnInit {
  constructor(
    private stockService: StockService,
    private stockPriceService: StockPriceService,
    private dateService: DateHelperService,
    private route: ActivatedRoute,
    private router: Router,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  displayedColumns: string[] = [
    'date',
    'price',
    'change',
    'change-ratio',
    'open',
    'high',
    'low',
  ];

  id: number = +this.route.snapshot.params.id;
  isUpdate: boolean = false;
  form: FormGroup;
  formFilter: FormGroup;
  stockPrices: StockPrice[];
  pagination: Pagination;
  isLoading: boolean = false;
  isFiltered: boolean = false;
  showFilterForm: boolean = false;
  stockPriceParams: any = {};

  ngOnInit() {
    this.form = this.formBuilder.group({
      code: ['', [Validators.required]],
      name: ['', [Validators.required]],
      maxVolume: ['', [Validators.required]],
      firstUpdateVolume: [''],
    });

    this.formFilter = this.formBuilder.group({
      date: [''],
      price: [''],
    });

    this.loadStockPrice();
    this.checkUpdate();
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
    this.stockPriceParams.price = '';

    if (
      this.formFilter.value.date != null &&
      this.formFilter.value.date != ''
    ) {
      this.stockPriceParams.date = this.dateService.dateToSave(
        this.formFilter.value.date
      );
      this.isFiltered = true;
    }
    if (
      this.formFilter.value.price != null &&
      this.formFilter.value.price != ''
    ) {
      this.stockPriceParams.price = this.formFilter.value.price;
      this.isFiltered = true;
    }

    this.showFilterForm = false;
    this.updateStockPrice();
  }

  cancelFilter() {
    this.formFilter.reset();
    this.isFiltered = false;
    this.showFilterForm = false;
  }

  clearFilter() {
    this.isFiltered = false;
    this.pagination.currentPage = 1;
    this.stockPriceParams.date = null;
    this.stockPriceParams.price = null;
    this.formFilter.reset();
    this.updateStockPrice();
  }

  checkUpdate() {
    if (this.id) {
      this.isUpdate = true;

      this.stockService.getStock(this.id).subscribe((data) => {
        this.form.setValue({
          code: data.code,
          name: data.name,
          maxVolume: data.maxVolume,
          firstUpdateVolume: data.firstUpdateVolume,
        });
      });
    }
  }

  addNewStock() {
    this.form.value.isUpdate = false;
    this.stockService.addStock(this.form.value).subscribe(
      () => {
        this.alert.Success(
          'Add Successfully',
          'Data has been added to the record'
        );
        this.router.navigate(['/stock']);
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  updateStock() {
    this.form.value.isUpdate = true;
    this.stockService.editStock(this.id, this.form.value).subscribe(
      () => {
        this.alert.Success('Edit Successfully', 'Data has been edited');
        this.router.navigate(['/stock']);
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  submit() {
    const validDate = this.dateService.dateToSave(
      this.form.get('firstUpdateVolume').value
    );
    this.form.patchValue({
      firstUpdateVolume: validDate.toString(),
    });
    if (!this.isUpdate) {
      this.addNewStock();
    } else {
      this.updateStock();
    }
  }
}
