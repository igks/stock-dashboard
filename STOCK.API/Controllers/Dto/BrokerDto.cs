namespace STOCK.API.Controllers.Dto
{
    public class SaveBrokerDto
    {
        public string Code { get; set; }
        public string Name { get; set; }

    }

    public class ViewBrokerDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}