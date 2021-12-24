using System.Threading.Tasks;

namespace DecisionTelecom.Examples
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // SMS client examples
            await SmsClientExample.SendMessageAsync();
            await SmsClientExample.GetMessageStatusAsync();
            await SmsClientExample.GetBalanceAsync();
            
            // Viber client examples
            await ViberClientExample.SendTransactionalMessageAsync();
            await ViberClientExample.SendPromotionalMessageAsync();
            await ViberClientExample.GetMessageStatusAsync();
            
            // ViberPlusSms client examples
            await ViberPlusSmsClientExample.SendMessageAsync();
            await ViberPlusSmsClientExample.GetMessageStatusAsync();
        }
    }
}