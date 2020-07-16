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
import { StockVolume } from 'src/app/models/stock-volume';
import { PaginatedResult } from 'src/app/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class StockVolumeService {
  constructor(private http: HttpClient) {}

  private baseUrl = environment.apiUrl;
  private itemPerPage = environment.itemPerPage;

  getStockVolume(id: number): Observable<StockVolume> {
    return this.http.get<StockVolume>(this.baseUrl + 'stockvolume/' + id);
  }

  addStockVolume(model: any): Observable<any> {
    return this.http.post(this.baseUrl + 'stockvolume/', model);
  }

  editStockVolume(id: number, model: any): Observable<any> {
    return this.http.put(this.baseUrl + 'stockvolume/' + id, model);
  }

  deleteStockVolume(id: number) {
    return this.http.delete(this.baseUrl + 'stockvolume/' + id);
  }

  getStockVolumes(
    page = 1,
    itemPerPage = this.itemPerPage,
    stockVolumeParams = null
  ): Observable<PaginatedResult<StockVolume[]>> {
    const paginatedResult: PaginatedResult<StockVolume[]> = new PaginatedResult<
      StockVolume[]
    >();

    let params = new HttpParams();
    params = params.append('pageNumber', page.toString());
    params = params.append('pageSize', itemPerPage.toString());

    if (stockVolumeParams != null) {
      Object.keys(stockVolumeParams).forEach((key) => {
        if (stockVolumeParams[key] != null) {
          params = params.append(key, stockVolumeParams[key]);
        }
      });
    }

    return this.http
      .get<StockVolume[]>(this.baseUrl + 'stockvolume/paged/', {
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

  uploadByStockDate(file: Blob): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.request(
      new HttpRequest(
        'POST',
        this.baseUrl + 'stockvolume/bystockdate/',
        formData,
        {
          reportProgress: true,
        }
      )
    );
  }
}
