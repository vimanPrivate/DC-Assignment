using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace P2PStorage.Common.Models
{
    public class ValueTable
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}
