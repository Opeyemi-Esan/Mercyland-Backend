using MercylandAdmin.Interface;
using MercylandAdmin.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MercylandAdmin.Inplementation
{
    public class RealEstateService : IRealEstateService
    {
        private readonly AppDbContext _appDbContext;
        private readonly string _uploadFolder;

        public RealEstateService(AppDbContext context)
        {
            _appDbContext = context;
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }
        public async Task<string> AddData(RealEstateDTO realEstateDTO)
        {
            if (realEstateDTO == null)
            {
                throw new ArgumentNullException(nameof(realEstateDTO), "Property data transfer object cannot be null.");
            }

            string filePath = null;

            // Handle file upload  
            if (realEstateDTO.RealEstateFlyer != null && realEstateDTO.RealEstateFlyer.Length > 0)
            {

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(realEstateDTO.RealEstateFlyer.FileName).ToLower();

                // Check if the file extension is valid  
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, and PNG files are allowed.");
                }

                try
                {
                    var RealEstateImage = Path.GetFileName(realEstateDTO.RealEstateFlyer.FileName);
                    filePath = Path.Combine(_uploadFolder, RealEstateImage);

                    // Check for file existence and handle conflicts  
                    if (System.IO.File.Exists(filePath))
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(RealEstateImage);
                        var uniqueFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";
                        filePath = Path.Combine(_uploadFolder, uniqueFileName);
                    }

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await realEstateDTO.RealEstateFlyer.CopyToAsync(stream);
                }
                catch (IOException ex)
                {
                    // Handle the exception (e.g., log it, rethrow it, etc.)  
                    // For demonstration, we'll just throw it higher up  
                    throw new Exception("An error occurred during the file upload.", ex);
                }
            }

            var realEstate = new RealEstate()
            {
                PricePerAcre = realEstateDTO.PricePerAcre,
                PricePerPlot = realEstateDTO.PricePerPlot,
                OutrightPaymentPerAcre = realEstateDTO.OutrightPaymentPerAcre,
                OutrightPaymentPerPlot = realEstateDTO.OutrightPaymentPerPlot,
                MonthlyPaymentFor12Months = realEstateDTO.MonthlyPaymentFor12Months,
                MonthlyPaymentFor24Months = realEstateDTO.MonthlyPaymentFor24Months,
                MonthlyPaymentFor54Months = realEstateDTO.MonthlyPaymentFor54Months,
                MonthlyPaymentFor8Months = realEstateDTO.MonthlyPaymentFor8Months,
                MonthlyPaymentFor8MonthsDeposit = realEstateDTO.MonthlyPaymentFor8MonthsDeposit,
                MonthlyPaymentFor45Months = realEstateDTO.MonthlyPaymentFor45Months,
                DeedOfAssignment = realEstateDTO.DeedOfAssignment,
                SurveyFeesPerAcre = realEstateDTO.SurveyFeesPerAcre,
                SurveyFeesPerPlot = realEstateDTO.SurveyFeesPerPlot,
                SurveyInBusinessname = realEstateDTO.SurveyInBusinessname,
                DevelopmentFeesPerAcre = realEstateDTO.DevelopmentFeesPerAcre,
                DevelopmentFeesPerPlot = realEstateDTO.DevelopmentFeesPerPlot,
                BankDetails = realEstateDTO.BankDetails,
                PhoneNumbers = realEstateDTO.PhoneNumbers,
                SizePerPlot = realEstateDTO.SizePerPlot,
                Title = realEstateDTO.Title,
                OriginalPrice = realEstateDTO.OriginalPrice,
                CurrentPaymentPlan = realEstateDTO.CurrentPaymentPlan,
                DelightBankDetails = realEstateDTO.DelightBankDetails,
                RealEstateFlyer = filePath
            };
            _appDbContext.RealEstates.Add(realEstate);
            await _appDbContext.SaveChangesAsync();
            return "Datas Added Successully";

        }

        public Task<RealEstate> EditDatas(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RealEstate>> GetAllData()
        {
           var datas = await _appDbContext.RealEstates.Include(b => b.DelightBankDetails).ToListAsync();
            return datas;
        }
    }
}
