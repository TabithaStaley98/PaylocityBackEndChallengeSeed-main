using System.Net;
using System.Threading.Tasks;
using Api.Dtos.Paycheck;
using Xunit;

namespace ApiTests.IntegrationTests;

public class PaycheckIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedForAnEmployeeWithHighPay_ShouldReturnCorrectPaycheck()
    {
        var response = await HttpClient.GetAsync("/api/v1/paychecks/3");
        var paycheck = new GetPaycheckDto
        {
            GrossPay = 5508.12m,
            BaseBenefits = 461.54m,
            DependentBenefits = 276.92m,
            AdditionalBenefits = 110.16m,
            NetPay = 4659.50m
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }
    [Fact]
    public async Task WhenAskedForAnEmployeeWithNoDependents_ShouldReturnCorrectPaycheck()
    {
        var response = await HttpClient.GetAsync("/api/v1/paychecks/1");
        var paycheck = new GetPaycheckDto
        {
            GrossPay = 2900.81m,
            BaseBenefits = 461.54m,
            DependentBenefits = 0m,
            AdditionalBenefits = 0m,
            NetPay = 2439.27m
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }
    [Fact]
    public async Task WhenAskedForAnEmployeeWithDependents_ShouldReturnCorrectPaycheck()
    {
        var response = await HttpClient.GetAsync("/api/v1/paychecks/2");
        var paycheck = new GetPaycheckDto
        {
            GrossPay = 3552.51m,
            BaseBenefits = 461.54m,
            DependentBenefits = 830.77m,
            AdditionalBenefits = 71.05m,
            NetPay = 2189.15m
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }
    [Fact]
    public async Task WhenAskedForAnEmployeeWithOlderDependents_ShouldReturnCorrectPaycheck()
    {
        var response = await HttpClient.GetAsync("/api/v1/paychecks/3");
        var paycheck = new GetPaycheckDto
        {
            GrossPay = 5508.12m,
            BaseBenefits = 461.54m,
            DependentBenefits = 276.92m,
            AdditionalBenefits = 110.16m,
            NetPay = 4659.50m
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }

    [Fact]
    public async Task WhenAskedForANonexistentEmployee_ShouldReturn404()
    {
        var response = await HttpClient.GetAsync($"/api/v1/paychecks/{int.MinValue}");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }
}

