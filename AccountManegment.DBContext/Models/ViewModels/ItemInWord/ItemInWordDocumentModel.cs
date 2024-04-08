using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.ItemInWord
{
    public class ItemInWordDocumentModel
    {
        public int Id { get; set; }

        public Guid RefInWordId { get; set; }

        public string? DocumentName { get; set; }
    }
}
