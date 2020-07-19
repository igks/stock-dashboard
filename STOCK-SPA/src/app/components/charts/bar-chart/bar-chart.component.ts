import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
declare var google: any;
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DashboardService } from 'src/app/services/dashboard.service';
import { Stock } from 'src/app/models/stock';
import { Broker } from 'src/app/models/broker';
import { StockService } from 'src/app/services/stock.service';
import { BrokerService } from 'src/app/services/broker.service';
import { DateHelperService } from 'src/app/services/date-helper.service';

@Component({
  selector: 'app-bar-chart',
  templateUrl: './bar-chart.component.html',
  styleUrls: ['./bar-chart.component.css'],
})
export class BarChartComponent implements OnInit {
  constructor(
    private dashboardService: DashboardService,
    private stockService: StockService,
    private dateService: DateHelperService,
    private brokerService: BrokerService,
    private formBuilder: FormBuilder
  ) {}

  @ViewChild('Chart') Chart: ElementRef;

  stockList: Stock[];
  brokerList: Broker[];
  isTop5: boolean = false;
  toggleText: string = 'Select top 5 broker';
  yearList: number[] = [];
  startYear: number = 2020;
  broker: string[] = ['Label 1', 'Label 2', 'Label 3', 'Label 4', 'Label 5'];
  total: number[] = [0, 0, 0, 0, 0];
  axisMax: number = 100;
  sliderList: any = [];
  maxSlider: number = 0;
  sliderLabel: string = 'Refresh...';
  initialValue: number = 1;
  // sliderValue: string = 'Refresh...';

  formParams: FormGroup;

  ngOnInit(): void {
    // create year list
    for (let i = 0; i <= 10; i++) {
      this.yearList.push(this.startYear);
      this.startYear--;
    }
    // create stock list
    this.loadStockList();
    // create broker list
    this.loadBrokerList();

    this.formParams = this.formBuilder.group({
      isTop5: ['false', [Validators.required]],
      stock: ['', [Validators.required]],
      broker: [[], Validators.maxLength(5)],
      year: ['', [Validators.required]],
    });
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

  toggler(event) {
    if (event.checked) {
      this.formParams.patchValue({
        broker: [],
      });
      this.isTop5 = event.checked;
      this.toggleText = 'Select multiple broker';
    } else {
      this.isTop5 = event.checked;
      this.toggleText = 'Select top 5 broker';
    }
  }

  refresh() {
    this.dashboardService
      .getDataBarChart(this.formParams.value)
      .subscribe((data) => {
        // Graph data processing
        this.broker.length = 0;
        this.total.length = 0;
        this.axisMax = data.maxStock;

        data.summary.map((el, ind) => {
          this.broker[ind] = el.broker;
          this.total[ind] = el.total;
        });
        this.drawChart();

        // Slider data processing
        this.sliderList.length = 0;
        data.slider.map((el) => {
          let stockDate = this.dateService.dateToTextView(el.stockDate);
          this.sliderList.push(`${stockDate}, ${el.price}`);
        });
        this.maxSlider = data.slider.length;
        this.sliderLabel = this.sliderList[0];
        this.formatLabel(1);
      });
  }

  changeLabel(event) {
    this.sliderLabel = this.sliderList[event.value - 1];
  }

  formatLabel = (lable) => {
    return (value) => {
      return lable;
    };
  };

  drawChart = () => {
    const data = new google.visualization.DataTable();
    const color = ['#FF7433', '#6FCCEC', '#6F91EC', '#C66FEC', '#234CCC'];

    data.addColumn('string', 'Broker');
    data.addColumn('number', 'Volume');
    data.addColumn({ role: 'style' });

    const preData = [];
    this.broker.map((item, index) => {
      preData.push([item, this.total[index], color[index]]);
    });

    data.addRows(preData);

    const view = new google.visualization.DataView(data);
    view.setColumns([
      0,
      1,
      {
        calc: 'stringify',
        sourceColumn: 1,
        type: 'string',
        role: 'annotation',
      },
      2,
    ]);

    const options = {
      bars: 'horizontal',
      title: 'Broker volume distribution',
      legend: { position: 'none' },
      width: '100%',
      height: 450,
      hAxis: {
        title: 'Volume',
        minValue: 0,
        maxValue: this.axisMax,
      },
      vAxis: {
        title: 'Broker',
      },
      axes: {
        y: {
          0: { side: 'left' },
        },
      },
    };

    const chart = new google.visualization.BarChart(this.Chart.nativeElement);
    chart.draw(view, options);
  };

  ngAfterViewInit() {
    google.charts.load('current', { packages: ['corechart', 'bar'] });
    google.charts.setOnLoadCallback(this.drawChart);
  }
}
