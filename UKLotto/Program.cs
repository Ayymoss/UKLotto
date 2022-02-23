namespace UKLotto;

internal static class UkLotto
{
    public const int TicketSales = 1_000_000;
    
    private static void Main()
    {
        Logging.Enable();
        
        Console.WriteLine("1. Generate {0:N0} Tickets\n2. Find Games for 1 Ticket\n3. Find Games for 1 Custom Ticket",
            TicketSales);
        var userInput = (UserInput) "Value".PromptInt(minValue: 1, maxValue: 3);
        var lotto = new Lotto();
        switch (userInput)
        {
            case UserInput.ProcessLotto:
                lotto.LottoProcessingInit();
                break;
            case UserInput.ProcessTicket:
                lotto.TicketProcessing(new Generation().GenerateNumbers());
                break;
            case UserInput.ProcessCustomTicket:
                lotto.CustomTicket();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Console.ReadKey();
        Logging.Disable();
    }

    private enum UserInput
    {
        ProcessLotto = 1,
        ProcessTicket = 2,
        ProcessCustomTicket = 3
    }
}