using P2PStorage.Common.Enums;
using P2PStorage.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PStorage.Service.Services.Node
{
    public class NodeService
    {
        private List<NodeTable> _nodeTable;
        private List<ValueTable> _valueTable;
        public bool isLeader;
        public int nodeId;
        public NodeRoleEnum Role;

        public NodeService() 
        { 
            _nodeTable = new List<NodeTable>();
            _valueTable = new List<ValueTable>();
            nodeId = -999;
        }

        public void AddValuesToNodeTable(NodeTable nodeTable)
        {
            _nodeTable.Add(nodeTable);
        }

        public void AddValuesToValueTable(string sentence) 
        {
            var tmpValueTbl = new ValueTable()
            {
                Id = sentence.Substring(0,Math.Min(sentence.Length,10)),
                Value = sentence
            };

            _valueTable.Add(tmpValueTbl);
        }

        public List<NodeTable> GetNodeValues()
        { 
            return _nodeTable;
        }

        public NodeTable GetNodeValuesById(int Id)
        {
            var nodeTable = _nodeTable.Where(x => x.Id == Id).First();
            return nodeTable;
        }

        public NodeTable GetNodeValuesByName(string name)
        {
            var nodeTable = _nodeTable.Where(x => x.NodeName == name).First();
            return nodeTable;
        }

        public NodeTable GetNodeValuesByRole(string role)
        {
            var nodeTable = _nodeTable.Where(x => x.Role == role).First();
            return nodeTable;
        }

        public List<ValueTable> GetValueTable() 
        {
            return _valueTable;
        }

        //public ValueTable GetValueTableById(int Id)
        //{
        //    var valueTable = _valueTable.Where(x =>x.Id == Id).First();
        //    return valueTable;
        //}

        public ValueTable GetValueTableByValue(string value)
        {
            var valueTable = _valueTable.Where(x => x.Value == value).First();
            return valueTable;
        }
    }
}
