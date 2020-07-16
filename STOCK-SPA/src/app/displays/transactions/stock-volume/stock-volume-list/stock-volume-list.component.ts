import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { StockVolume } from 'src/app/models/stock-volume';
import { StockVolumeService } from 'src/app/services/stock-volume.service';
import { AlertService } from 'src/app/services/alert.service';
import { DateHelperService } from 'src/app/services/date-helper.service';
import { Pagination, PaginatedResult } from 'src/app/models/pagination';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-stock-volume-list',
  templateUrl: './stock-volume-list.component.html',
})
export class StockVolumeListComponent implements OnInit {
  constructor(
    private stockVolumeService: StockVolumeService,
    private dateHelperService: DateHelperService,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('stockPerDate', { static: false })
  stockPerDate: ElementRef;

  displayedColumns: string[] = [
    'date',
    'broker',
    'stock',
    'buyVolume',
    'buyAverage',
    'sellVolume',
    'sellAverage',
    'volume',
    'option',
  ];
  stockVolumes: StockVolume[];
  pagination: Pagination;
  stockVolumeParams: any = {};
  model: any;
  isLoading: boolean = false;
  isUploading: boolean = false;
  isFiltered: boolean = false;
  showFilterForm: boolean = false;
  form: FormGroup;

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      date: [''],
      broker: [''],
      stock: [''],
    });

    this.loadStockVolume();
    console.log(this.stockVolumes);
  }

  loadStockVolume() {
    this.isLoading = true;
    this.stockVolumeService.getStockVolumes().subscribe(
      (res: PaginatedResult<StockVolume[]>) => {
        this.stockVolumes = res.result;
        this.pagination = res.pagination;
        this.isLoading = false;
      },
      (error) => {
        this.alert.Error('', error);
        this.isLoading = false;
      }
    );
  }

  updateStockVolume() {
    this.stockVolumeService
      .getStockVolumes(
        this.pagination.currentPage,
        this.pagination.pageSize,
        this.stockVolumeParams
      )
      .subscribe(
        (res: PaginatedResult<StockVolume[]>) => {
          this.stockVolumes = res.result;
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
    this.updateStockVolume();
  }

  sortChange(event: any) {
    this.pagination.currentPage = 1;
    this.stockVolumeParams.OrderBy = event.active;
    this.stockVolumeParams.isDescending =
      event.direction === 'desc' ? true : false;
    this.updateStockVolume();
  }

  setShowFilterForm() {
    this.showFilterForm = true;
  }

  addFilter() {
    this.stockVolumeParams.date = '';
    this.stockVolumeParams.broker = '';
    this.stockVolumeParams.stock = '';

    if (this.form.value.date != null && this.form.value.date != '') {
      this.stockVolumeParams.date = this.dateHelperService.dateToSave(
        this.form.value.date
      );
      this.isFiltered = true;
    }
    if (this.form.value.stock != null && this.form.value.stock != '') {
      this.stockVolumeParams.stock = this.form.value.stock;
      this.isFiltered = true;
    }
    if (this.form.value.broker != null && this.form.value.broker != '') {
      this.stockVolumeParams.broker = this.form.value.broker;
      this.isFiltered = true;
    }

    this.showFilterForm = false;
    this.updateStockVolume();
  }

  cancelFilter() {
    this.form.reset();
    this.isFiltered = false;
    this.showFilterForm = false;
  }

  clearFilter() {
    this.isFiltered = false;
    this.pagination.currentPage = 1;
    this.stockVolumeParams.date = null;
    this.stockVolumeParams.broker = null;
    this.stockVolumeParams.stock = null;
    this.form.reset();
    this.updateStockVolume();
  }

  deleteStockVolume(id: number) {
    const confirm = this.alert.Confirm();
    confirm.afterClosed().subscribe((result) => {
      if (result === true) {
        this.stockVolumeService.deleteStockVolume(id).subscribe(
          () => {
            this.alert.Info('', 'The data has been deleted');
            this.updateStockVolume();
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
        this.stockPerDate.nativeElement.value = '';
        return;
      }

      switch (type) {
        case 'stockperdate':
          this.isUploading = true;
          this.stockVolumeService.uploadByStockDate(file).subscribe(
            (data) => {
              if (data) {
                if (data.type == HttpEventType.Response) {
                  this.alert.Success('', 'File successfully uploaded.');
                  this.stockPerDate.nativeElement.value = '';
                  this.updateStockVolume();
                  this.isUploading = false;
                }
              }
            },
            (error) => {
              this.alert.Error('', error);
              this.stockPerDate.nativeElement.value = '';
              this.isUploading = false;
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
