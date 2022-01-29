public struct PlayerData
{
    private string playerName;

    public string PlayerName => playerName;

    public CustomInputDevice InputDevice => _customInputDevice;

    private CustomInputDevice _customInputDevice;

    public PlayerData(string playerName, CustomInputDevice playerInputDevice)
    {
        this.playerName = playerName;
        _customInputDevice = playerInputDevice;
    }
}