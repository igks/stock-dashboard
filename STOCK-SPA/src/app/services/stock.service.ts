import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, ObservableLike } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Stock } from 'src/app/models/stock';
import { PaginatedResult } from 'src/app/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class StockService {
  constructor(private http: HttpClient) {}

  private baseUrl = environment.apiUrl;
  private itemPerPage = environment.itemPerPage;

  getStock(id: number): Observable<Stock> {
    return this.http.get<Stock>(this.baseUrl + 'stock/' + id);
  }

  getAllStocks(): Observable<Stock[]> {
    return this.http.get<Stock[]>(this.baseUrl + 'stock/');
  }

  addStock(model: any) {
    return this.http.post(this.baseUrl + 'stock/', model);
  }

  editStock(id: number, model: any) {
    return this.http.put(this.baseUrl + 'stock/' + id, model);
  }

  deleteStock(id: number) {
    return this.http.delete(this.baseUrl + 'stock/' + id);
  }

  getStocks(
    page = 1,
    itemPerPage = this.itemPerPage,
    stockParams = null
  ): Observable<PaginatedResult<Stock[]>> {
    const paginatedResult: PaginatedResult<Stock[]> = new PaginatedResult<
      Stock[]
    >();

    let params = new HttpParams();
    params = params.append('pageNumber', page.toString());
    params = params.append('pageSize', itemPerPage.toString());

    if (stockParams != null) {
      Object.keys(stockParams).forEach((key) => {
        if (stockParams[key] != null) {
          params = params.append(key, stockParams[key]);
        }
      });
    }

    return this.http
      .get<Stock[]>(this.baseUrl + 'stock/paged/', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResult;
        })
      );
  }
}
