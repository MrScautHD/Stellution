using System.Collections;

public class PrefabRegistry {

    private static ArrayList prefabs = new ArrayList();

    public static string Player = Register("entity/player");
    public static string FlyingCar = Register("entity/vehicle/FlyingCar");

    private static string Register(string name) {
        string path = "prefabs/" + name;
        prefabs.Add(path);
        return path;
    }
}