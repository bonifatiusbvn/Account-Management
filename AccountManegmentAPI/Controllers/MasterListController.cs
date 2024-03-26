using AccountManagement.DBContext.Models.ViewModels;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.MasterList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterListController : ControllerBase
    {
        public MasterListController(IMasterList masterList)
        {
            MasterList = masterList;
        }

        public IMasterList MasterList { get; }

        [HttpGet]
        [Route("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            IEnumerable<CountryView> getCountries = await MasterList.GetCountries();
            return Ok(new { code = 200, data = getCountries.ToList() });
        }

        [HttpGet]
        [Route("GetState")]
        public async Task<IActionResult> GetState(int StateId)
        {
            IEnumerable<StateView> getStates = await MasterList.GetStates(StateId);
            return Ok(new { code = 200, data = getStates.ToList() });
        }

        [HttpGet]
        [Route("GetCities")]
        public async Task<IActionResult> GetCities(int CityId)
        {
            IEnumerable<CityView> getCities = await MasterList.GetCities(CityId);
            return Ok(new { code = 200, data = getCities.ToList() });
        }

        [HttpGet]
        [Route("GetUserRole")]
        public async Task<IActionResult> GetUserRole()
        {
            IEnumerable<UserRoleModel> userRole = await MasterList.GetUserRole();
            return Ok(new { code = 200, data = userRole.ToList() });
        }
    }
}
