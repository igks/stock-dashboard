import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Stock } from 'src/app/models/stock';
import { StockService } from 'src/app/services/stock.service';
import { DateHelperService } from 'src/app/services/date-helper.service';
import { AlertService } from 'src/app/services/alert.service';
import { FormBuilder, FormGroup, Validators, Form } from '@angular/forms';

@Component({
  selector: 'app-stock-form',
  templateUrl: './stock-form.component.html',
})
export class StockFormComponent implements OnInit {
  constructor(
    private stockService: StockService,
    private dateService: DateHelperService,
    private route: ActivatedRoute,
    private router: Router,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  id: number = +this.route.snapshot.params.id;
  isUpdate: boolean = false;
  stock: Stock;
  form: FormGroup;

  ngOnInit() {
    this.form = this.formBuilder.group({
      code: ['', [Validators.required]],
      name: ['', [Validators.required]],
      maxVolume: ['', [Validators.required]],
      firstUpdateVolume: [''],
    });

    this.checkUpdate();
  }

  checkUpdate() {
    if (this.id) {
      this.isUpdate = true;

      this.stockService.getStock(this.id).subscribe((data) => {
        this.stock = data;
        this.form.setValue({
          code: this.stock.code,
          name: this.stock.name,
          maxVolume: this.stock.maxVolume,
          firstUpdateVolume: this.stock.firstUpdateVolume,
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

    console.log(this.form.value);

    if (!this.isUpdate) {
      this.addNewStock();
    } else {
      this.updateStock();
    }
  }
}
