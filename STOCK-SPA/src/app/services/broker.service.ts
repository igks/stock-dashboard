import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Broker } from 'src/app/models/broker';
import { PaginatedResult } from 'src/app/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class BrokerService {
  private baseUrl = environment.apiUrl;
  private itemPerPage = environment.itemPerPage;

  constructor(private http: HttpClient) {}

  getBroker(id: number): Observable<Broker> {
    return this.http.get<Broker>(this.baseUrl + 'broker/' + id);
  }

  getAllBrokers(): Observable<Broker[]> {
    return this.http.get<Broker[]>(this.baseUrl + 'broker/');
  }

  addBroker(model: any) {
    return this.http.post(this.baseUrl + 'broker/', model);
  }

  editBroker(id: number, model: any) {
    return this.http.put(this.baseUrl + 'broker/' + id, model);
  }

  deleteBroker(id: number) {
    return this.http.delete(this.baseUrl + 'broker/' + id);
  }

  getBrokers(
    page = 1,
    itemPerPage = this.itemPerPage,
    brokerParams = null
  ): Observable<PaginatedResult<Broker[]>> {
    const paginatedResult: PaginatedResult<Broker[]> = new PaginatedResult<
      Broker[]
    >();

    let params = new HttpParams();
    params = params.append('pageNumber', page.toString());
    params = params.append('pageSize', itemPerPage.toString());

    if (brokerParams != null) {
      Object.keys(brokerParams).forEach((key) => {
        if (brokerParams[key] != null) {
          params = params.append(key, brokerParams[key]);
        }
      });
    }

    return this.http
      .get<Broker[]>(this.baseUrl + 'broker/paged/', {
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
