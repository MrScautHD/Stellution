namespace Future.Common.csharp.network; 

public class DistHelper {

    protected bool ClientSide;
    protected bool ServerSide;

    /**
     * Set Client and Server to true
     */
    protected void SetHost() {
        this.ClientSide = true;
        this.ServerSide = true;
    }

    /**
     * This checks if the code run on the CLIENT.
     */
    public bool IsClient() {
        return this.ClientSide;
    }
    
    /**
     * This checks if the code run on the SERVER.
     */
    public bool IsServer() {
        return this.ServerSide;
    }
    
    /**
     * This checks if the code run on the SERVER.
     */
    public bool IsHost() {
        return this.IsClient() && this.IsServer();
    }
}