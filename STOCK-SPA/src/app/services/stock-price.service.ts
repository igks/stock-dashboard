import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams,
  HttpEvent,
  HttpRequest,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { StockPrice } from 'src/app/models/stock-price';
import { PaginatedResult } from 'src/app/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class StockPriceService {
  constructor(private http: HttpClient) {}

  private baseUrl = environment.apiUrl;
  private itemPerPage = environment.itemPerPage;

  getStockPrice(id: number): Observable<StockPrice> {
    return this.http.get<StockPrice>(this.baseUrl + 'stockprice/' + id);
  }

  addStockPrice(model: any): Observable<any> {
    return this.http.post(this.baseUrl + 'stockprice/', model);
  }

  editStockPrice(id: number, model: any): Observable<any> {
    return this.http.put(this.baseUrl + 'stockprice/' + id, model);
  }

  deleteStockPrice(id: number) {
    return this.http.delete(this.baseUrl + 'stockprice/' + id);
  }

  getStockPrices(
    page = 1,
    itemPerPage = this.itemPerPage,
    stockPriceParams = null
  ): Observable<PaginatedResult<StockPrice[]>> {
    const paginatedResult: PaginatedResult<StockPrice[]> = new PaginatedResult<
      StockPrice[]
    >();

    let params = new HttpParams();
    params = params.append('pageNumber', page.toString());
    params = params.append('pageSize', itemPerPage.toString());

    if (stockPriceParams != null) {
      Object.keys(stockPriceParams).forEach((key) => {
        if (stockPriceParams[key] != null) {
          params = params.append(key, stockPriceParams[key]);
        }
      });
    }

    return this.http
      .get<StockPrice[]>(this.baseUrl + 'stockprice/paged/', {
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

  uploadDailyPrice(file: Blob): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.request(
      new HttpRequest(
        'POST',
        this.baseUrl + 'stockprice/dailyprice/',
        formData,
        {
          reportProgress: true,
        }
      )
    );
  }

  uploadPriceHistory(file: Blob): Observable<HttpEvent<void>> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.request(
      new HttpRequest(
        'POST',
        this.baseUrl + 'stockprice/historyprice/',
        formData,
        {
          reportProgress: true,
        }
      )
    );
  }
}
