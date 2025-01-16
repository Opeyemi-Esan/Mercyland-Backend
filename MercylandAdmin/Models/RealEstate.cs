namespace MercylandAdmin.Models
{
    public class RealEstate
    {
        public int Id { get; set; }
        public decimal PricePerAcre { get; set; }
        public decimal PricePerPlot { get; set; }
        public decimal OutrightPaymentPerAcre { get; set; }
        public decimal OutrightPaymentPerPlot { get; set; }
        public decimal MonthlyPaymentFor12Months { get; set; }
        public decimal MonthlyPaymentFor24Months { get; set; }
        public decimal MonthlyPaymentFor54Months { get; set; }
        public decimal MonthlyPaymentFor8Months { get; set; }
        public decimal MonthlyPaymentFor8MonthsDeposit { get; set; }
        public decimal MonthlyPaymentFor45Months { get; set; }
        public decimal DeedOfAssignment { get; set; }
        public decimal SurveyFeesPerAcre { get; set; }
        public decimal SurveyFeesPerPlot { get; set; }
        public decimal SurveyInBusinessname { get; set; }
        public decimal DevelopmentFeesPerPlot { get; set; }
        public decimal DevelopmentFeesPerAcre { get; set; }
        public string BankDetails { get; set; } 
        public string PhoneNumbers { get; set; }
        public string SizePerPlot { get; set; }
        public string Title { get; set; }
        public decimal OriginalPrice { get; set; } 
        public decimal CurrentPaymentPlan { get; set; }
        public List<DelightBankDetail> DelightBankDetails { get; set; }
        public string RealEstateFlyer { get; set; }
    }
}
