namespace Api.Dtos.Paycheck;

public class GetPaycheckDto
{
    public decimal GrossPay { get; set; }
    public decimal BaseBenefits { get; set; }
    public decimal DependentBenefits { get; set; }
    public decimal AdditionalBenefits { get; set; }
    public decimal NetPay { get; set; }
}
