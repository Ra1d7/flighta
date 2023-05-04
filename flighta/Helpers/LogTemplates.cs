namespace flighta.Helpers
{
    public static partial class LogTemplates
    {
        [LoggerMessage(20,LogLevel.Information,"Conroller {controllerName}/{path} requested at {date}. by {user}}")]
        public static partial void MvcControllerLog(this ILogger logger, string controllerName,string path, DateTime date,string user = "anonymous");
        [LoggerMessage(19,LogLevel.Information,"Feedback Email sent at {date} by {fullName} with email {email}")]
        public static partial void EmailSendLog(this ILogger logger, string fullName, string email, DateTime date);
    }
}
