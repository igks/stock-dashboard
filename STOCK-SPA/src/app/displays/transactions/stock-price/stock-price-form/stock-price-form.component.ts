import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Stock } from 'src/app/models/stock';
import { StockPriceService } from 'src/app/services/stock-price.service';
import { StockService } from 'src/app/services/stock.service';
import { DateHelperService } from 'src/app/services/date-helper.service';
import { AlertService } from 'src/app/services/alert.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-stock-price-form',
  templateUrl: './stock-price-form.component.html',
})
export class StockPriceFormComponent implements OnInit {
  constructor(
    private stockService: StockService,
    private stockPriceService: StockPriceService,
    private dateService: DateHelperService,
    private route: ActivatedRoute,
    private router: Router,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  id: number = +this.route.snapshot.params.id;
  isUpdate: boolean = false;
  stockList: Stock[];
  form: FormGroup;

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      date: ['', [Validators.required]],
      stockId: ['', [Validators.required]],
      price: ['', [Validators.required]],
      open: ['', [Validators.required]],
      high: ['', [Validators.required]],
      low: ['', [Validators.required]],
      volume: [''],
      change: [''],
      changeRatio: [''],
    });

    this.loadStockList();
    this.checkUpdate();
  }

  loadStockList() {
    this.stockService.getAllStocks().subscribe((data) => {
      this.stockList = data;
    });
  }

  checkUpdate() {
    if (this.id) {
      this.isUpdate = true;

      this.stockPriceService.getStockPrice(this.id).subscribe((data) => {
        let stockId;

        this.stockList.map((el) => {
          if (el.name === data.stock) {
            stockId = el.id;
          }
        });

        this.form.setValue({
          date: data.date,
          stockId: stockId,
          price: data.price,
          open: data.open,
          high: data.high,
          low: data.low,
          volume: data.volume,
          change: data.change,
          changeRatio: data.changeRatio,
        });
      });
    }
  }

  addStockPrice() {
    this.form.value.isUpdate = false;
    this.stockPriceService.addStockPrice(this.form.value).subscribe(
      (data) => {
        if (data.str == 'duplicate') {
          this.id = data.recordId;
          const confirm = this.alert.Confirm(
            'Overwrite',
            'The data you want to input already exist, do you want to overwrite it?'
          );
          confirm.afterClosed().subscribe((result) => {
            if (result === true) {
              this.updateStockPrice();
            }
          });
        }
        if (data.str == null || data.str == '') {
          this.alert.Success(
            'Add Successfully',
            'Data has been added to the record'
          );
          this.router.navigate(['/stockprice']);
        }
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  saveAndAddStockPrice() {
    this.form.value.isUpdate = false;
    this.stockPriceService.addStockPrice(this.form.value).subscribe(
      (data) => {
        if (data.str == 'duplicate') {
          this.id = data.recordId;
          const confirm = this.alert.Confirm(
            'Overwrite',
            'The data you want to input already exist, do you want to overwrite it?'
          );
          confirm.afterClosed().subscribe((result) => {
            if (result === true) {
              this.stockPriceService
                .editStockPrice(this.id, this.form.value)
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

  updateStockPrice() {
    this.form.value.isUpdate = true;
    this.stockPriceService.editStockPrice(this.id, this.form.value).subscribe(
      () => {
        this.alert.Success('Edit Successfully', 'Data has been edited');
        this.router.navigate(['/stockprice']);
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
      this.updateStockPrice();
    }
  }
}
