import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Broker } from 'src/app/models/broker';
import { BrokerService } from 'src/app/services/broker.service';
import { AlertService } from 'src/app/services/alert.service';
import { FormBuilder, FormGroup, Validators, Form } from '@angular/forms';

@Component({
  selector: 'app-broker-form',
  templateUrl: './broker-form.component.html',
})
export class BrokerFormComponent implements OnInit {
  constructor(
    private brokerService: BrokerService,
    private route: ActivatedRoute,
    private router: Router,
    private alert: AlertService,
    private formBuilder: FormBuilder
  ) {}

  id: number = +this.route.snapshot.params.id;
  isUpdate: boolean = false;
  broker: Broker;
  form: FormGroup;

  ngOnInit() {
    this.form = this.formBuilder.group({
      code: ['', [Validators.required]],
      name: ['', [Validators.required]],
    });

    this.checkUpdate();
  }

  checkUpdate() {
    if (this.id) {
      this.isUpdate = true;

      this.brokerService.getBroker(this.id).subscribe((data) => {
        this.broker = data;
        this.form.setValue({
          code: this.broker.code,
          name: this.broker.name,
        });
      });
    }
  }

  addNewDepartment() {
    this.form.value.isUpdate = false;
    this.brokerService.addBroker(this.form.value).subscribe(
      () => {
        this.alert.Success(
          'Add Successfully',
          'Data has been added to the record'
        );
        this.router.navigate(['/broker']);
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  updateDepartment() {
    this.form.value.isUpdate = true;
    this.brokerService.editBroker(this.id, this.form.value).subscribe(
      () => {
        this.alert.Success('Edit Successfully', 'Data has been edited');
        this.router.navigate(['/broker']);
      },
      (error) => {
        this.alert.Error('', error);
      }
    );
  }

  submit() {
    if (!this.isUpdate) {
      this.addNewDepartment();
    } else {
      this.updateDepartment();
    }
  }
}
