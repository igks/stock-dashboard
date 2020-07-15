export interface StockPrice {
  date: Date;
  stock: string;
  price: number;
  change: number;
  changeRatio: number;
  open: number;
  high: number;
  low: number;
  volume: number;
}
