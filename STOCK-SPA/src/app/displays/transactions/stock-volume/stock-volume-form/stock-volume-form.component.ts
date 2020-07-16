import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Stock } from 'src/app/models/stock';
import { Broker } from 'src/app/models/broker';
import { StockVolumeService } from 'src/app/services/stock-volume.service';
import { BrokerService } from 'src/app/services/broker.service';
import { StockService } from 'src/app/services/stock.service';
import { DateHelperService } from 'src/app/services/date-helper.service';
import { AlertService } from 'src/app/services/alert.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-stock-volume-form',
  templateUrl: './stock-volume-form.component.html',
})
export class StockVolumeFormComponent implements OnInit {
  constructor(
    private stockService: StockService,
    private brokerService: BrokerService,
    private stockVolumeService: StockVolumeService,
    private dateService: DateHelperService,
    private route: ActivatedRoute,
    private router: Router,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  id: number = +this.route.snapshot.params.id;
  isUpdate: boolean = false;
  stockList: Stock[];
  brokerList: Broker[];
  form: FormGroup;

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      date: ['', [Validators.required]],
      brokerId: ['', [Validators.required]],
      stockId: ['', [Validators.required]],
      buyVolume: ['', [Validators.required]],
      buyAverage: ['', [Validators.required]],
      sellVolume: ['', [Validators.required]],
      sellAverage: ['', [Validators.required]],
      netVolume: ['', [Validators.required]],
    });

    this.loadStockList();
    this.loadBrokerList();
    this.checkUpdate();
  }

  loadStockList() {
    this.stockService.getAllStocks().subscribe((data) => {
      this.stockList = data;
    });
  }

  loadBrokerList() {
    this.brokerService.getAllBrokers().subscribe((data) => {
      this.brokerList = data;
    });
  }

  checkUpdate() {
    if (this.id) {
      this.isUpdate = true;

      this.stockVolumeService.getStockVolume(this.id).subscribe((data) => {
        let stockId;
        let brokerId;

        this.stockList.map((el) => {
          if (el.name === data.stock) {
            stockId = el.id;
          }
        });

        this.brokerList.map((el) => {
          if (el.name === data.broker) {
            brokerId = el.id;
          }
        });

        this.form.setValue({
          date: data.date,
          brokerId: brokerId,
          stockId: stockId,
          buyVolume: data.buyVolume,
          buyAverage: data.buyAverage,
          sellVolume: data.sellVolume,
          sellAverage: data.sellAverage,
          netVolume: data.netVolume,
        });
      });
    }
  }

  addStockPrice() {
    this.form.value.isUpdate = false;
    this.stockVolumeService.addStockVolume(this.form.value).subscribe(
      (data) => {
        if (data.str == 'duplicate') {
          this.id = data.recordId;
          const confirm = this.alert.Confirm(
            'Overwrite',
            'The data you want to input already exist, do you want to overwrite it?'
          );
          confirm.afterClosed().subscribe((result) => {
            if (result === true) {
              this.updateStockVolume();
            }
          });
        }
        if (data.str == null || data.str == '') {
          this.alert.Success(
            'Add Successfully',
            'Data has been added to the record'
          );
          this.router.navigate(['/stockvolume']);
        }
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  saveAndAddStockPrice() {
    this.form.value.isUpdate = false;
    this.stockVolumeService.addStockVolume(this.form.value).subscribe(
      (data) => {
        if (data.str == 'duplicate') {
          this.id = data.recordId;
          const confirm = this.alert.Confirm(
            'Overwrite',
            'The data you want to input already exist, do you want to overwrite it?'
          );
          confirm.afterClosed().subscribe((result) => {
            if (result === true) {
              this.stockVolumeService
                .editStockVolume(this.id, this.form.value)
                .subscribe(
                  () => {
                    this.alert.Success(
                      'Edit Successfully',
                      'Data has been edited'
                    );
                    this.form.reset();
                  },
                  (error) => {
                    this.alert.Error('', error);
                  }
                );
            }
          });
        }
        if (data.str == null || data.str == '') {
          this.alert.Success(
            'Add Successfully',
            'Data has been added to the record'
          );
          this.form.reset();
        }
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  updateStockVolume() {
    this.form.value.isUpdate = true;
    this.stockVolumeService.editStockVolume(this.id, this.form.value).subscribe(
      () => {
        this.alert.Success('Edit Successfully', 'Data has been edited');
        this.router.navigate(['/stockvolume']);
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  submit(isBackToList: boolean = false) {
    const validDate = this.dateService.dateToSave(this.form.get('date').value);
    this.form.patchValue({
      date: validDate.toString(),
    });
    if (!this.isUpdate) {
      if (isBackToList) {
        this.addStockPrice();
      } else {
        this.saveAndAddStockPrice();
      }
    } else {
      this.updateStockVolume();
    }
  }
}
