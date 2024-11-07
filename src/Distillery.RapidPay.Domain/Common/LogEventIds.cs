namespace Distillery.RapidPay.Domain.Common;

/// <summary>
/// This class has the information used to create EventIds for logging purposes
/// </summary>
public static class LogGroupEventIds
{
    /// <summary>
    /// Define the base level used for common Events
    /// </summary>
    public const int CommonEvent = 100000;

    /// <summary>
    /// Define the base level used for exception Events
    /// </summary>
    public const int ExceptionEvent = 200000;

    /// <summary>
    /// Define the base level used for internal CardManagement Events
    /// </summary>
    public const int CardManagementEvent = 300000;
}
