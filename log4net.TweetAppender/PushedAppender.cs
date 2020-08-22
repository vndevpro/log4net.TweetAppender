using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace log4net.TweetAppender
{
    public class PushedAppender : AppenderSkeleton
    {
        private string _appKey = string.Empty;
        private string _appSecret = string.Empty;
        private string _appUrl = string.Empty;
        private string _targetType = string.Empty;

        protected override void Append(LoggingEvent loggingEvent)
        {
            PostEvent(loggingEvent).ConfigureAwait(false);
        }

        public void AddAppKey(string appKey)
        {
            _appKey = appKey;
        }

        public void AddAppSecret(string appSecret)
        {
            _appSecret = appSecret;
        }

        public void AddAppUrl(string appUrl)
        {
            _appUrl = appUrl;
        }

        public void AddTargetType(string targetType)
        {
            _targetType = targetType;
        }

        private async Task PostEvent(LoggingEvent loggingEvent)
        {
            using (var client = new HttpClient())
            {
                {
                    var json = BuildJson(loggingEvent);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(_appUrl, content);
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private string BuildJson(LoggingEvent loggingEvent)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("\"app_key\": \"{0}\",", _appKey);
            sb.AppendFormat("\"app_secret\": \"{0}\",", _appSecret);
            sb.AppendFormat("\"target_type\": \"{0}\",", _targetType);

            if (loggingEvent.ExceptionObject != null)
            {
                sb.AppendFormat("\"content\": \"{0}\"", loggingEvent.ExceptionObject.Message);
            }

            if (loggingEvent.MessageObject != null)
            {
                sb.AppendFormat("\"content\": \"{0}\"", loggingEvent.MessageObject);
            }

            return "{" + sb + "}";
        }
    }
}