using System;

public interface ISellElementSubmitable
{
    /// <summary>
    /// int is SellElement ID.
    /// </summary>
    public Action<int> Buy { get; set; }
    /// <summary>
    /// int is SellElement ID.
    /// </summary>
    public Action<int> OpenDescription { get; set; }
    /// <summary>
    /// int is SellElement ID.
    /// </summary>
    public Action<int> CloseDescription { get; set; }
}
