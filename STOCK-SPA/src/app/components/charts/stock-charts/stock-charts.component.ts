import { Component, ViewChild, OnInit } from '@angular/core';

import {
  ApexDataLabels,
  ApexStroke,
  ApexMarkers,
  ApexGrid,
  ApexLegend,
  ChartComponent,
  ApexAxisChartSeries,
  ApexChart,
  ApexYAxis,
  ApexXAxis,
  ApexTitleSubtitle,
  ApexTooltip,
  ApexFill,
} from 'ng-apexcharts';

import { DashboardService } from 'src/app/services/dashboard.service';
import { StockService } from 'src/app/services/stock.service';
import { Stock } from 'src/app/models/stock';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DateHelperService } from '../../../services/date-helper.service';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis | ApexYAxis[];
  title: ApexTitleSubtitle;
  tooltip: ApexTooltip;
  stroke: any;
  dataLabels: ApexDataLabels;
  markers: ApexMarkers;
  colors: string[];
  grid: ApexGrid;
  legend: ApexLegend;
  labels: string[];
  fill: ApexFill;
};

@Component({
  selector: 'app-stock-charts',
  templateUrl: './stock-charts.component.html',
  styleUrls: ['./stock-charts.component.css'],
})
export class StockChartsComponent {
  @ViewChild('candleChart') candleChart: ChartComponent;
  @ViewChild('lineChart') lineChart: ChartComponent;
  @ViewChild('comboChart') comboChart: ChartComponent;
  public candleChartOptions: Partial<ChartOptions>;
  public lineChartOptions: Partial<ChartOptions>;
  public comboChartOptions: Partial<ChartOptions>;

  stockList: Stock[];
  formParams: FormGroup;
  net3Data: any = [];
  net5Data: any = [];
  candleData: any = [];
  volumeData: any = [];
  maData: any = [];
  comboLabel: any = [];
  yearList: number[] = [];
  startYear: number = 2020;

  constructor(
    private dashBoardService: DashboardService,
    private stockService: StockService,
    private formBuilder: FormBuilder,
    private dateService: DateHelperService
  ) {}

  ngOnInit(): void {
    for (let i = 0; i <= 10; i++) {
      this.yearList.push(this.startYear);
      this.startYear--;
    }

    this.loadStockList();

    this.formParams = this.formBuilder.group({
      stock: ['', [Validators.required]],
      year: ['', [Validators.required]],
    });

    this.drawCandleChart();
    this.drawLineChart();
    this.drawComboChart();
  }

  refresh() {
    this.dashBoardService
      .getDataStockChart(this.formParams.value)
      .subscribe((data) => {
        this.net3Data.length = 0;
        this.net5Data.length = 0;
        this.candleData.length = 0;
        this.volumeData.length = 0;
        this.maData.length = 0;
        this.comboLabel.length = 0;

        data.net3.map((data) => {
          let dateLabel = this.dateService.dateAndMonth(data.date);
          this.net3Data.push({
            x: dateLabel,
            y: data.net3Value,
          });
        });

        data.net5.map((data) => {
          let dateLabel = this.dateService.dateAndMonth(data.date);
          this.net5Data.push({
            x: dateLabel,
            y: data.net5Value,
          });
        });

        data.candlesList.map((data) => {
          let dateLabel = this.dateService.dateAndMonth(data.date);
          this.candleData.push({
            x: dateLabel,
            y: [data.open, data.high, data.low, data.close],
          });
        });

        data.volumeList.map((data) => {
          this.volumeData.push(data.price);
          let dateLabel = this.dateService.dateAndMonth(data.date);
          this.comboLabel.push(dateLabel);
        });

        data.maList.map((data) => {
          this.maData.push(data.average);
        });

        this.drawCandleChart();
        this.drawLineChart();
        this.drawComboChart();
      });
  }

  loadStockList() {
    this.stockService.getAllStocks().subscribe((data) => {
      this.stockList = data;
    });
  }

  drawCandleChart() {
    this.candleChartOptions = {
      series: [
        {
          name: 'candle',
          data: this.candleData,
        },
      ],
      chart: {
        type: 'candlestick',
        height: 350,
      },
      title: {
        text: 'Price Trend',
        align: 'left',
      },
      xaxis: {
        type: 'category',
      },
      yaxis: {
        tooltip: {
          enabled: true,
        },
      },
    };
  }

  drawLineChart() {
    this.lineChartOptions = {
      series: [
        {
          name: 'Net 3',
          data: this.net3Data,
        },
        {
          name: 'Net 5',
          data: this.net5Data,
        },
      ],
      chart: {
        height: 350,
        type: 'line',
        dropShadow: {
          enabled: true,
          color: '#000',
          top: 18,
          left: 7,
          blur: 10,
          opacity: 0.2,
        },
        zoom: {
          type: 'x',
          enabled: true,
          autoScaleYaxis: true,
        },
        toolbar: {
          autoSelected: 'zoom',
        },
      },
      title: {
        text: 'Net Trend',
        align: 'left',
      },
      colors: ['#77B6EA', '#545454'],
      dataLabels: {
        enabled: true,
      },
      stroke: {
        curve: 'smooth',
      },
      grid: {
        borderColor: '#e7e7e7',
        row: {
          colors: ['#f3f3f3', 'transparent'],
          opacity: 0.5,
        },
      },
      markers: {
        size: 1,
      },
      xaxis: {
        type: 'category',
      },
      yaxis: {},
      legend: {
        position: 'top',
        horizontalAlign: 'right',
        floating: true,
        offsetY: -25,
        offsetX: -5,
      },
    };
  }

  drawComboChart() {
    this.comboChartOptions = {
      series: [
        {
          name: 'Volume',
          type: 'column',
          data: this.volumeData,
        },
        {
          name: 'Moving Average',
          type: 'line',
          data: this.maData,
        },
      ],
      chart: {
        height: 350,
        type: 'line',
      },
      stroke: {
        width: [0, 4],
      },
      title: {
        text: 'Volume and Moving Average',
      },
      dataLabels: {
        enabled: false,
        enabledOnSeries: [1],
      },
      labels: this.comboLabel,
      xaxis: {
        type: 'category',
      },
      yaxis: [],
    };
  }
}
