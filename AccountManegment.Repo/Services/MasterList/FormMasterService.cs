using AccountManagement.DBContext.Models.ViewModels.FormMaster;
using AccountManagement.Repository.Interface.Repository.MasterList;
using AccountManagement.Repository.Interface.Services.MasterList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.MasterList
{
    public class FormMasterService : IFormMasterServices
    {
        public FormMasterService(IFormMaster form)
        {
            Form = form;
        }

        public IFormMaster Form { get; }

        public async Task<IEnumerable<FormMasterModel>> GetFormGroupList()
        {
            return await Form.GetFormGroupList(); 
        }
    }
}
