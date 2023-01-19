namespace Future.Common.csharp.file; 

public class Config : FileManager {
    
    public Config() : base("config", "config.json") {
        this.CreateFile();

        this.WriteJson(new Customer() {
            Age = 1,
            CustomerName = "test",
            CustomerEmail = "lolol.com",
            TotalSales = 10,
            Check = true
        });
        
        Console.WriteLine(this.ReadJson()["CustomerName"]); // Get 1 value

        if (this.ReadJson()["Check"].GetValue<bool>()) { // Convert it to the right Type like a bool
            Console.WriteLine("CHECKED!!!!!");
        }
        
        Console.WriteLine(this.ReadJson()); // Get full list of values
    }
    
    public class Customer {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int Age { get; set; }
        
        public bool Check { get; set; }
        public decimal TotalSales { get; set; }
    }
}