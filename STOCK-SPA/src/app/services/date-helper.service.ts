import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DateHelperService {
  constructor() {}
  private monthList = [
    'Jan',
    'Feb',
    'Mar',
    'Apr',
    'May',
    'Jun',
    'Jul',
    'Aug',
    'Sep',
    'Oct',
    'Nov',
    'Dec',
  ];

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

  dateToTextView(unformat) {
    const dgt = (d) => {
      return d < 10 ? '0' + d : d;
    };

    const d = new Date(unformat);
    return [
      dgt(d.getDate()),
      this.monthList[d.getMonth()],
      d.getFullYear(),
    ].join(' ');
  }

  dateAndMonth(unformat) {
    const dgt = (d) => {
      return d < 10 ? '0' + d : d;
    };

    const d = new Date(unformat);
    return [dgt(d.getDate()), this.monthList[d.getMonth()]].join(' ');
  }
}
