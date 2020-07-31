import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
declare var google: any;

@Component({
  selector: 'app-line-chart',
  templateUrl: './line-chart.component.html',
  styleUrls: ['./line-chart.component.css'],
})
export class LineChartComponent implements OnInit {
  constructor() {}

  @ViewChild('lineChart') lineChart: ElementRef;
  @ViewChild('volumeChart') volumeChart: ElementRef;

  ngOnInit() {}

  drawVisualization = () => {
    // Some raw data (not necessarily accurate)
    const data = new google.visualization.DataTable();
    data.addColumn('string', 'Month');
    // data.addColumn('number', 'Low');
    // data.addColumn('number', 'Open');
    data.addColumn('number', 'Close');
    // data.addColumn('number', 'Height');
    data.addColumn('number', 'Net5');
    data.addColumn('number', 'Net3');

    const preData = [];

    for (let index = 0; index < 135; index++) {
      let low = Math.floor(Math.random() * 1000 + 1);
      let open = Math.floor(Math.random() * 1000 + 1);
      let close = Math.floor(Math.random() * 1000 + 1);
      let height = Math.floor(Math.random() * 1000 + 1);
      let net5 = Math.floor(Math.random() * 1000 + 1);
      let net3 = Math.floor(Math.random() * 1000 + 1);
      preData.push([index.toString(), close, net5, net3]);
    }

    data.addRows(preData);

    // var data = google.visualization.arrayToDataTable([
    //   ['Month', 'low', 'open', 'close', 'height', 'net5', 'net 3'],
    //   ['2004/05', 165, 938, 522, 998, 450, 500],
    //   ['2005/06', 135, 1120, 599, 1268, 288, 550],
    //   ['2006/07', 157, 1167, 587, 807, 397, 450],
    //   ['2007/08', 139, 1110, 615, 968, 215, 330],
    //   ['2008/09', 136, 691, 629, 1026, 366, 400],
    // ]);

    var options = {
      width: 900,
      legend: 'none',
      title: '',
      vAxis: {},
      seriesType: 'line',
      series: { 1: { type: 'line' }, 2: { type: 'line' } },
      explorer: { axis: 'horizontal' },
    };

    var chart = new google.visualization.ComboChart(
      this.lineChart.nativeElement
    );
    chart.draw(data, options);
  };

  // drawVolumeVisualization() {
  //   // Some raw data (not necessarily accurate)
  //   var data = google.visualization.arrayToDataTable([
  //     ['Month', 'Bolivia', 'Average'],
  //     ['2004/05', 165, 614.6],
  //     ['2005/06', 135, 682],
  //     ['2006/07', 157, 623],
  //     ['2007/08', 139, 609.4],
  //     ['2008/09', 136, 569.6],
  //   ]);

  //   var options = {
  //     height: 200,
  //     legend: 'none',
  //     seriesType: 'bars',
  //     series: { 1: { type: 'line' } },
  //   };

  //   var volumeChart = new google.visualization.ComboChart(
  //     this.volumeChart.nativeElement
  //   );
  //   volumeChart.draw(data, options);
  // }

  ngAfterViewInit() {
    google.charts.load('current', { packages: ['corechart'] });
    setTimeout(() => {
      google.charts.setOnLoadCallback(this.drawVisualization());
      // google.charts.setOnLoadCallback(this.drawVolumeVisualization());
    }, 2000);
  }
}
