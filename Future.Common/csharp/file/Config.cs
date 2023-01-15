namespace Future.Common.csharp.file; 

public class Config : FileManager {
    
    public Config(string directory, string fileName) : base(directory, fileName) {
        if (!File.Exists(this.GetPath())) {
            this.CreateFile();
        }
    }

    public void WriteConfig<T>(T configType) {
        this.WriteJson(configType);
    }

    public T ReadConfig<T>(T configType) {
        return this.ReadJson(configType);
    }
}