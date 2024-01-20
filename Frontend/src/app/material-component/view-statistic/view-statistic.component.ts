import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Chart } from 'chart.js';
@Component({
  selector: 'app-view-statistic',
  templateUrl: './view-statistic.component.html',
  styleUrls: ['./view-statistic.component.scss']
})
export class ViewStatisticComponent implements OnInit {

  // @ViewChild('piechart') pieChartRef: ElementRef;

  constructor() { 
    // this.pieChartRef = new ElementRef(document.createElement('canvas'));
  }

  ngOnInit(): void {
    
    this.RenderChart();
  }

  RenderChart(){
    // const ctx = this.pieChartRef.nativeElement.getContext('2d');
    // const pieChart = new Chart(ctx, {
    //   type: 'pie',
    //   data: {
    //     labels: ['Label 1', 'Label 2', 'Label 3'],
    //     datasets: [{
    //       data: [30, 40, 30],
    //       backgroundColor: ['red', 'green', 'blue']
    //     }]
    //   }
    // });
  }
}
