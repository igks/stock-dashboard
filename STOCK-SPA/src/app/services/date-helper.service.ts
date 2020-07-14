import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DateHelperService {
  constructor() {}

  dateToSave(unformat) {
    const dgt = (d) => {
      return d < 10 ? '0' + d : d;
    };
    const d = new Date(unformat);
    return [d.getFullYear(), dgt(d.getMonth() + 1), dgt(d.getDate())].join('-');
  }

  dateToView(unformat) {
    const dgt = (d) => {
      return d < 10 ? '0' + d : d;
    };
    const d = new Date(unformat);
    return [dgt(d.getMonth() + 1), dgt(d.getDate()), d.getFullYear()].join('/');
  }
}
