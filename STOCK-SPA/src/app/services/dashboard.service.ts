import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(private http: HttpClient) {}

  private baseUrl = environment.apiUrl;

  getDataBarChart(formParams: any): Observable<any> {
    let params = new HttpParams();
    params = params.append('year', formParams.year);
    params = params.append('stock', formParams.stock);
    params = params.append('broker', formParams.broker);
    params = params.append('isTop5', formParams.isTop5);

    return this.http
      .get<any>(this.baseUrl + 'dashboard/barchart/', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }

  getDataStockChart(formParams: any): Observable<any> {
    let params = new HttpParams();
    params = params.append('stock', formParams.stock);
    params = params.append('year', formParams.year);

    return this.http
      .get<any>(this.baseUrl + 'dashboard/stockchart/', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }
}
