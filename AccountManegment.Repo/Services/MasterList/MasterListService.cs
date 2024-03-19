
using AccountManagement.DBContext.Models.ViewModels;
using AccountManagement.Repository.Interface.Repository.MasterList;
using AccountManagement.Repository.Interface.Services.MasterList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.MasterList
{
    public class MasterListService : IMasterListServices
    {
        public IMasterList MasterList { get; }

        public MasterListService(IMasterList masterList)
        {
            MasterList = masterList;
        }



        public async Task<IEnumerable<CityView>> GetCities(int cityId)
        {
            try
            {
                return await MasterList.GetCities(cityId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task<IEnumerable<CountryView>> GetCountries()
        {
            try
            {
                return MasterList.GetCountries();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public Task<IEnumerable<StateView>> GetStates(int stateId)
        {
            try
            {
                return MasterList.GetStates(stateId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
