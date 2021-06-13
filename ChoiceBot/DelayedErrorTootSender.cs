using System;
using System.Threading.Tasks;
using Mastonet;

namespace ChoiceBot
{
    public class DelayedErrorTootSender
    {
        private DateTime _errorTootTimeLast = DateTime.MinValue;
        /**
         * Zero means this is first time shooting error after enough delay
         */
        private static   TimeSpan _errorTootIntervalNow = TimeSpan.Zero;
        public TimeSpan ErrorTootIntervalStart { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan ErrorTootIntervalMax { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan ErrorTootIntervalResetSpan { get; set; } = TimeSpan.FromSeconds(30);
        
        private readonly string _exceptionMessageHead = "@sftblw@twingyeo.kr\r\n[!] 예외가 발생하였습니다.";
        private bool _sendErrorMessage = true;

        public DelayedErrorTootSender() { }

        public DelayedErrorTootSender(string exceptionMessageHead, bool sendErrorMessage = true)
        {
            _exceptionMessageHead = exceptionMessageHead;
            _sendErrorMessage = sendErrorMessage;
        }
        
        
        public async Task SendLogTootDelayed(UnhandledExceptionEventArgs e, MastodonClient client)
        {
            if (_errorTootTimeLast + _errorTootIntervalNow >= DateTime.Now) { return; }
            
            _errorTootTimeLast = DateTime.Now;

            UpdateInterval();

            await SendLogToot(e, client);
        }

        private void UpdateInterval()
        {
            if (_errorTootIntervalNow < ErrorTootIntervalStart)
            {
                _errorTootIntervalNow = ErrorTootIntervalStart;
                return;
            }
            
            var newInterval = _errorTootIntervalNow.Multiply(2);
            if (DateTime.Now < _errorTootTimeLast + newInterval)
            {
                _errorTootIntervalNow =
                    (newInterval < ErrorTootIntervalMax)
                        ? newInterval
                        : ErrorTootIntervalMax;
            }
            else
            {
                _errorTootIntervalNow = TimeSpan.Zero;
            }
        }

        private async Task SendLogToot(UnhandledExceptionEventArgs e, MastodonClient client)
        {
            string message = _exceptionMessageHead + "\r\n" + ((e.ExceptionObject as Exception)?.Message ?? e.ToString());
            message = message[..Math.Max((message.Length - 1), 450)];
            
            await client.PostStatus(message, Visibility.Private);
        }
    }
}