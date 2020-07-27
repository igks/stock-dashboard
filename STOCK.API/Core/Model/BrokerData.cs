using System;
using System.Collections.Generic;

namespace STOCK.API.Core.Model
{
    public class BrokerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Int32 AccVolume { get; set; }
        public ICollection<Data> Data { get; set; }
    }
}