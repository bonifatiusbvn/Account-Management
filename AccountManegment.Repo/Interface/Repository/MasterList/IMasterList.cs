using AccountManagement.DBContext.Models.ViewModels;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.MasterList
{
    public interface IMasterList
    {
        Task<IEnumerable<CountryView>> GetCountries();
        Task<IEnumerable<UserRoleModel>> GetUserRole();
        Task<IEnumerable<StateView>> GetStates(int StateId);
        Task<IEnumerable<CityView>> GetCities(int CityId);

    }
}
