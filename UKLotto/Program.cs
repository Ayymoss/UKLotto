namespace UKLotto;

internal static class UkLotto
{
    private static void Main()
    {
        Logging.Enable();

        var lotto = new LottoProgram();

        Console.WriteLine("1. Generate {0:N0} Tickets\n2. Find Games for 1 Ticket\n3. Find Games for 1 Custom Ticket",
            LottoProgram.TicketSales);
        
        var userInput = (InputType)"Value".PromptInt(minValue: 1, maxValue: 3);
        switch (userInput)
        {
            case InputType.ProcessLotto:
                lotto.LottoProcessingInit();
                break;
            case InputType.ProcessTicket:
                lotto.TicketProcessing();
                break;
            case InputType.ProcessCustomTicket:
                lotto.CustomTicket();
                break;
        }
        Console.ReadKey();
        Logging.Disable();
    }


    private enum InputType
    {
        ProcessLotto = 1,
        ProcessTicket = 2,
        ProcessCustomTicket = 3
    }
}
