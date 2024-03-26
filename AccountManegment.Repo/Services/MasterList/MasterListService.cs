
using AccountManagement.API;
using AccountManagement.DBContext.Models.ViewModels;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
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

        public async Task<IEnumerable<CountryView>> GetCountries()
        {
            try
            {
                return await MasterList.GetCountries();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public async Task<IEnumerable<StateView>> GetStates(int stateId)
        {
            try
            {
                return await MasterList.GetStates(stateId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<UserRoleModel>> GetUserRole()
        {
            try
            {
                return await MasterList.GetUserRole();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
