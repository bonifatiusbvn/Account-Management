
using AccountManagement.DBContext.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.MasterList
{
    public interface IMasterListServices
    {
        Task<IEnumerable<CountryView>> GetCountries();
        Task<IEnumerable<StateView>> GetStates(int StateId);
        Task<IEnumerable<CityView>> GetCities(int CityId);

    }
}
