namespace Future.Common.csharp.file; 

public class Config : FileManager {
    
    public Config() : base("config", "config.json") {
        this.CreateFile(false);
        
        //this.WriteJson(new Customer() {
        //    Age = 1,
        //    CustomerName = "test",
        //    CustomerEmail = "lolol.com",
        //    TotalSales = 10,
        //    Check = true
        //});

        foreach (var json in this.ReadJson().AsObject()) {
            //if (json.Key == )
            
            Console.WriteLine(json);
        }
    }

    public struct Customer {
        public string CustomerName;
        public string CustomerEmail;
        public int Age;
        public bool Check;
        public decimal TotalSales;
    }
}