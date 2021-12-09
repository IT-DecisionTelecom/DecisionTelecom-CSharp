using System.Threading.Tasks;

namespace DecisionTelecom.Examples
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // SMS client examples
            await SmsClientExample.SendMessageAsync();
            await SmsClientExample.GetMessageStatusAsync(31885463);
            await SmsClientExample.GetBalanceAsync();
            
            // Viber client examples
            await ViberClientExample.SendMessageAsync();
            await ViberClientExample.GetMessageStatusAsync(429);
            
            // ViberPlusSms client examples
            await ViberPlusSmsClientExample.SendMessageAsync();
            await ViberPlusSmsClientExample.GetMessageStatusAsync(429);
        }
    }
}