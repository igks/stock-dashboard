import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
// declare var DojiChart;

import { DashboardService } from 'src/app/services/dashboard.service';
import { StockService } from 'src/app/services/stock.service';
import { Stock } from 'src/app/models/stock';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-stock-charts',
  templateUrl: './stock-charts.component.html',
  styleUrls: ['./stock-charts.component.css'],
})
export class StockChartsComponent implements OnInit {
  constructor(
    private dashBoardService: DashboardService,
    private stockService: StockService,
    private formBuilder: FormBuilder
  ) {}

  @ViewChild('stockChart') stockChart: ElementRef;

  stockList: Stock[];
  formParams: FormGroup;

  refresh() {
    this.dashBoardService
      .getDataStockChart(this.formParams.value)
      .subscribe((data) => {
        console.log(data);
      });
  }

  loadStockList() {
    this.stockService.getAllStocks().subscribe((data) => {
      this.stockList = data;
    });
  }

  ngOnInit(): void {
    this.loadStockList();

    this.formParams = this.formBuilder.group({
      stock: ['', [Validators.required]],
    });
    // const dojichart = new DojiChart.core.Chart(this.stockChart, {
    //   fieldMap: {
    //     time: 'time',
    //     open: 'openBid',
    //     high: 'highBid',
    //     low: 'lowBid',
    //     close: 'closeBid',
    //     volume: 'volume',
    //   },
    //   crosshair: true,
    // });
    // // Candlestick layer
    // var candle_layer = new DojiChart.layer.CandleLayer({});
    // // Price chart panel
    // var price_chart_panel = new DojiChart.panel.TimeValuePanel({
    //   primaryLayer: candle_layer,
    //   height: 250,
    //   grid: true,
    // });
    // dojichart.addComponent('price', price_chart_panel);
    // // Moving average
    // var sma_layer = new DojiChart.layer.indicator.SimpleMovingAverageLayer({
    //   period: 50,
    // });
    // price_chart_panel.addLayer(sma_layer);
    // // Time labels (at top of chart)
    // var time_labels_panel = new DojiChart.panel.TimeLabelsPanel();
    // dojichart.addComponent('timelabels', time_labels_panel);
    // // Volume
    // var volume_layer = new DojiChart.layer.indicator.VolumeLayer({
    //   barColor: '#3377FF',
    //   barWidth: 5,
    // });
    // var volume_chart_panel = new DojiChart.panel.TimeValuePanel({
    //   height: 100,
    //   primaryLayer: volume_layer,
    // });
    // dojichart.addComponent('volume', volume_chart_panel);
    // var priceData = [
    //   {
    //     t: '2015-11-11T17:25:00.000000Z',
    //     o: 4672.3,
    //     h: 4675.3,
    //     l: 4671.0,
    //     c: 4671.4,
    //   },
    //   {
    //     t: '2015-11-11T17:30:00.000000Z',
    //     o: 4671.5,
    //     h: 4675.1,
    //     l: 4671.3,
    //     c: 4674.5,
    //   },
    //   {
    //     t: '2015-11-11T17:35:00.000000Z',
    //     o: 4674.5,
    //     h: 4678.6,
    //     l: 4674.5,
    //     c: 4676.2,
    //   },
    //   {
    //     t: '2015-11-11T17:40:00.000000Z',
    //     o: 4676.0,
    //     h: 4677.3,
    //     l: 4674.5,
    //     c: 4674.9,
    //   },
    //   {
    //     t: '2015-11-11T17:45:00.000000Z',
    //     o: 4674.7,
    //     h: 4676.2,
    //     l: 4673.2,
    //     c: 4673.3,
    //   },
    // ];
    // dojichart.loadData(priceData, 'NASDAQ', 'M5');
  }
}
