namespace intelliPWR.MasterScanner
{
    /// <summary>
    /// Delegate that defines event handler for connected.
    /// </summary>
    /// <param name="array">An array of connected device list.</param>
    /// <param name="count">Size of connected device.</param>
    public delegate void ConnectedEventHandler(string[] array, byte count);

    /// <summary>
    /// Delegate that defines event handler for disconnected.
    /// </summary>
    /// <param name="array">An array of disconnected device list.</param>
    /// <param name="count">Size of disconnected device.</param>
    public delegate void DisconnectedEventHandler(string[] array, byte count);
}
