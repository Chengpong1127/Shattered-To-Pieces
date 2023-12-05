using UnityEngine;

public interface IDeviceRoot{
    public Device Device { get; set; }
    public PlayerNameDisplay PlayerDisplayer { get; }
}