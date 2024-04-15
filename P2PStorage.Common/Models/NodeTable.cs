using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace P2PStorage.Common.Models
{
    public class NodeTable
    {
        public int Id { get; set; }
        public string NodeName { get; set; }
        public string Role { get; set; }
    }
}
