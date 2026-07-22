using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Helpers
{
    internal class AccountSession
    {
        public static Account? Current { get; set; }
    }
}
