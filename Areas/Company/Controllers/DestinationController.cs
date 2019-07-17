using Microsoft.AspNetCore.Mvc;
using ZTourist.Models;

namespace ZTourist.Areas.Company.Controllers
{
    public class DestinationController : Controller
    {
        private readonly TouristDAL touristDAL;
        private readonly BlobService blobService;

        public DestinationController(TouristDAL touristDAL, BlobService blobService)
        {
            this.touristDAL = touristDAL;
            this.blobService = blobService;
        }

        public IActionResult Index()
        {

            return View();
        }
    }
}
