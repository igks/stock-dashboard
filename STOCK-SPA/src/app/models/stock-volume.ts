export interface StockVolume {
  id: number;
  date: Date;
  broker: string;
  stock: string;
  buyVolume: number;
  buyAverage: number;
  sellVolume: number;
  sellAverage: number;
  netVolume: number;
}
