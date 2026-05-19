namespace Fina.Core.Requests.Transactions;

public class GetTransactionsByPeriodRequest : PagedResquest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
