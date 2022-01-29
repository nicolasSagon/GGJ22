using UnityEngine.InputSystem;

public struct PlayerData
{
    private string playerName;

    public string PlayerName => playerName;

    public Key PlayerKey => playerKey;

    private Key playerKey;

    public PlayerData(string playerName, Key playerKey)
    {
        this.playerName = playerName;
        this.playerKey = playerKey;
    }
}