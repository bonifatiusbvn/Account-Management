using AccountManagement.DBContext.Models.ViewModels.FormMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.MasterList
{
    public interface IFormMaster
    {
        Task<IEnumerable<FormMasterModel>> GetFormGroupList();
    }
}
