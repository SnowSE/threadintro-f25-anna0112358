namespace inclassLottery;

class Ticket
{
    // 2) Modify Ticket class to be able to judge a winner level
    public int[] RegTickets { get; set; }
    public int PowerBall { get; set; }
    public Ticket(int[] numbers, int powerBall)
    {
        RegTickets = new int[5];
        RegTickets[0] = numbers[0];
        RegTickets[1] = numbers[1];
        RegTickets[2] = numbers[2];
        RegTickets[3] = numbers[3];
        RegTickets[4] = numbers[4];
        PowerBall = powerBall;
    }
    public Ticket()
    {
        RegTickets = new int[5];
        for (int i = 0; i < 5; i++)
        {
            RegTickets[i] = Random.Shared.Next(1, 70);
        }
        PowerBall = Random.Shared.Next(1, 27);
    }

    public WinnerLevel JudgeWinnerLevel(Ticket winningTicket)
    {
        bool powerBallRight = false;
        int numOfRegTicketsRight = 0;
        WinnerLevel winnerLevel;
        if (winningTicket.PowerBall == PowerBall) powerBallRight = true;
        for (int index = 0; index < 5; index++)
        {
            if (winningTicket.RegTickets.Contains(RegTickets[index])) numOfRegTicketsRight++;
        }
        // Match 5 white balls: $1,000,000 
        if (numOfRegTicketsRight == 5)
        {
            winnerLevel = WinnerLevel.OneMillion;

            // Match 5 white balls plus the Powerball: Jackpot 
            if (powerBallRight)
            {
                winnerLevel = WinnerLevel.Jackpot;
            }
        }
        // Match 4 white balls: $100 
        else if (numOfRegTicketsRight == 4)
        {
            winnerLevel = WinnerLevel.OneHundred;

            // Match 4 white balls plus the Powerball: $50,000 
            if (powerBallRight)
            {
                winnerLevel = WinnerLevel.FiftyThousand;
            }
        }
        // Match 3 white balls: $7 
        else if (numOfRegTicketsRight == 3)
        {
            winnerLevel = WinnerLevel.Seven;

            // Match 3 white balls plus the Powerball: $100 
            if (powerBallRight)
            {
                winnerLevel = WinnerLevel.OneHundred;
            }
        }
        // Match 2 white balls plus the Powerball: $7
        else if (numOfRegTicketsRight == 2 && powerBallRight)
        {
            winnerLevel = WinnerLevel.Seven;
        }
        // Match 1 white ball plus the Powerball: $4 
        else if (numOfRegTicketsRight == 2 && powerBallRight)
        {
            winnerLevel = WinnerLevel.Four;
        }
        // Match only the Powerball: $4 
        else if (powerBallRight)
        {
            winnerLevel = WinnerLevel.Four;
        }
        else
        {
            winnerLevel = WinnerLevel.NoWinnings;
        }
        return winnerLevel;
    }
}
class LotteryPeriod
{
    public Ticket WinningTicket { get; set; }
    public List<Ticket> SoldTickets { get; set; } = new List<Ticket>();
    public LotteryPeriod()
    {
        int[] numbers = new int[5] { 1, 2, 3, 4, 5 };
        SetWinningTicket(numbers, 6);

    }
    public void SetWinningTicket(int[] numbers, int powerBall)
    {
        WinningTicket = new Ticket(numbers, powerBall);
    }
}
class LotteryVendor
{
    public LotteryVendor()
    {
    }
    public void SellTickets(LotteryPeriod period, int numberOfTickets)
    {
        for (int i = 0; i < numberOfTickets; i++)
        {
            Ticket ticket = new Ticket();
            period.SoldTickets.Add(ticket);
        }
    }
}
class Program
{
    static object x = new object();
    static void Main(string[] args)
    {
        int numOfVendors = 3;
        int ticketsSoldPerVendor = 10_000_000;
        Console.WriteLine($"Hello, Lets sell {numOfVendors * ticketsSoldPerVendor} Tickets!");
        LotteryPeriod period = new LotteryPeriod();
        List<LotteryVendor> lotteryVendors = [];
        for (int index = 0; index < numOfVendors; index++)
        {
            LotteryVendor vendor = new LotteryVendor();
            lotteryVendors.Add(vendor);
        }
        // 1a) make 3 vendors sell tickets each
        // 1b) 3 vendors sell tickets in parallel
        Parallel.ForEach(lotteryVendors, vendor =>
        {
            lock (x)
            {
                vendor.SellTickets(period, ticketsSoldPerVendor);
            }
        });
        Console.WriteLine($"SOLD {period.SoldTickets.Count} Tickets!");

        // 3) Gather statistics on how many winners of each level there are
        Dictionary<WinnerLevel, int> winningStats = new Dictionary<WinnerLevel, int>
        {
            { WinnerLevel.Jackpot, 0 },
            { WinnerLevel.OneMillion, 0 },
            { WinnerLevel.FiftyThousand, 0 },
            { WinnerLevel.OneHundred, 0 },
            { WinnerLevel.Seven, 0 },
            { WinnerLevel.Four, 0 },
            { WinnerLevel.NoWinnings, 0 }
        };
        Parallel.ForEach(period.SoldTickets, ticket =>
        {
            lock(x)
            {
                WinnerLevel ticketWinnings = ticket.JudgeWinnerLevel(period.WinningTicket);
                winningStats[ticketWinnings]++;
            }
        });
        // 4) Print out the statistics
        foreach (KeyValuePair<WinnerLevel, int> level in winningStats)
        {
            Console.WriteLine($"{level.Key}: {level.Value}");
        }
        // AFTER 1-4 is working, try to do (GatherStatistics) with Parallel Programming
    }
}

enum WinnerLevel { Jackpot, OneMillion, FiftyThousand, OneHundred, Seven, Four, NoWinnings }
