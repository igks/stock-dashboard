import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Broker } from 'src/app/models/broker';
import { BrokerService } from 'src/app/services/broker.service';
import { AlertService } from 'src/app/services/alert.service';
import { Pagination, PaginatedResult } from 'src/app/models/pagination';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-broker-list',
  templateUrl: './broker-list.component.html',
})
export class BrokerListComponent implements OnInit {
  constructor(
    private brokerService: BrokerService,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  displayedColumns: string[] = ['code', 'name', 'option'];
  brokers: Broker[];
  pagination: Pagination;
  brokerParams: any = {};
  model: any;
  isLoading: boolean = false;
  isFiltered: boolean = false;
  showFilterForm: boolean = false;
  form: FormGroup;

  ngOnInit() {
    this.form = this.formBuilder.group({
      code: [''],
      name: [''],
    });

    this.loadBroker();
  }

  loadBroker() {
    this.isLoading = true;
    this.brokerService.getBrokers().subscribe(
      (res: PaginatedResult<Broker[]>) => {
        this.brokers = res.result;
        this.pagination = res.pagination;
        this.isLoading = false;
      },
      (error) => {
        alert(error);
        this.isLoading = false;
      }
    );
  }

  updateBroker() {
    this.brokerService
      .getBrokers(
        this.pagination.currentPage,
        this.pagination.pageSize,
        this.brokerParams
      )
      .subscribe(
        (res: PaginatedResult<Broker[]>) => {
          this.brokers = res.result;
          this.pagination = res.pagination;
          this.isLoading = false;
        },
        (error) => {
          alert(error);
        }
      );
  }

  pageEvents(event: any) {
    this.pagination.currentPage = event.pageIndex + 1;
    this.pagination.pageSize = event.pageSize;
    this.updateBroker();
  }

  sortChange(event: any) {
    this.pagination.currentPage = 1;
    this.brokerParams.OrderBy = event.active;
    this.brokerParams.isDescending = event.direction === 'desc' ? true : false;
    this.updateBroker();
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
      this.brokerParams.code = this.form.value.code;
      this.brokerParams.name = this.form.value.name;
      this.updateBroker();
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
    this.brokerParams.code = null;
    this.brokerParams.name = null;
    this.form.reset();
    this.updateBroker();
  }

  deleteBroker(id: number) {
    const confirm = this.alert.Confirm();
    confirm.afterClosed().subscribe((result) => {
      if (result === true) {
        this.brokerService.deleteBroker(id).subscribe(
          () => {
            this.alert.Info('', 'The data has been deleted');
            this.updateBroker();
          },
          (error) => {
            this.alert.Error('', error);
          }
        );
      }
    });
  }
}
