﻿using P2PStorage.Common.Enums;
using P2PStorage.Common.Models;
using P2PStorage.Service.Services.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PStorage.Service.Services.Node
{
    public class Node
    {
        private List<NodeTable> _nodeTable;
        public List<ValueTable> _valueTable;

        public int NodeId { get; set; }
        public string Name { get; set; }
        public List<Node> ConnectedNodes { get; }
        public NodeRoleEnum NodeRole { get; private set; }
        public bool IsLeader { get;  set; }

        public Node()
        {
            ConnectedNodes = new List<Node>();
            _nodeTable = new List<NodeTable>();
            _valueTable = new List<ValueTable>();
        }

        public Node(int nodeId)
        {
            NodeId = nodeId;
            ConnectedNodes = new List<Node>();
            _nodeTable = new List<NodeTable>();
            _valueTable = new List<ValueTable>();
        }

        public void SetupNodeConnection(Node node)
        {
            if (ConnectedNodes.Count == 0)
            {
                ConnectedNodes.Add(node);
                node.ConnectedNodes.Add(this);
            }
            else
            {
                foreach (var nodeFromList in ConnectedNodes)
                {
                    nodeFromList.ConnectedNodes.Add(node);
                    node.ConnectedNodes.Add(nodeFromList);
                }

                ConnectedNodes.Add(node);
                node.ConnectedNodes.Add(this);
            }
        }

        public void DisconnectNode(int nodeId)
        {
            if (nodeId == NodeId)
            {
                int nextNodeId = nodeId;
                Node replacingNode = null;

                bool isNextNodeAvailable = false;

                while (!isNextNodeAvailable)
                {
                    nextNodeId++;

                    if (ConnectedNodes.Any(node => node.NodeId == nextNodeId))
                    {
                        replacingNode = ConnectedNodes.Where(node => node.NodeId == nextNodeId).FirstOrDefault();
                        isNextNodeAvailable = true;
                    }
                }

                foreach (var node in ConnectedNodes)
                {
                    var innerNodeList = node.ConnectedNodes;

                    if (innerNodeList.Any(e => e.NodeId == nodeId))
                    {
                        var innerRemovebleNode = innerNodeList.Where(i => i.NodeId == nodeId).First();
                        innerNodeList.Remove(innerRemovebleNode);
                    }
                }

                ConnectedNodes.Remove(replacingNode);

                if (_nodeTable.Count > 0)
                    _nodeTable.Clear();

                if (_valueTable.Count > 0)
                    _valueTable.Clear();

                this.NodeId = replacingNode.NodeId;
                this.Name = replacingNode.Name;
                this.NodeRole = replacingNode.NodeRole;
                this.IsLeader = replacingNode.IsLeader;
                this._nodeTable = replacingNode._nodeTable;
                this._valueTable = replacingNode._valueTable;
            }

            if (ConnectedNodes.Any(node => node.NodeId == nodeId))
            {
                var removebleNode = ConnectedNodes.Where(node => node.NodeId == nodeId).First();

                ConnectedNodes.Remove(removebleNode);
                removebleNode.ConnectedNodes.Remove(this);

                foreach (var node in ConnectedNodes)
                {
                    var innerNodeList = node.ConnectedNodes;

                    if (innerNodeList.Any(e => e.NodeId == nodeId))
                    {
                        var innerRemovebleNode = innerNodeList.Where(i => i.NodeId == nodeId).First();
                        innerNodeList.Remove(innerRemovebleNode);
                    }
                }
            }
            else
            {
                Console.WriteLine($"{nodeId} is not connected to {nodeId}");
            }
        }

        public void ElectLeader()
        {
            int leaderNodeId = ConnectedNodes.Select(node => node.NodeId).Max();

            //broadcasting the leader
            if (leaderNodeId == this.NodeId)
                this.IsLeader = true;

            foreach (var node in ConnectedNodes)
            {
                if (node.NodeId == leaderNodeId)
                    node.IsLeader = true;

                foreach (var innerNode in node.ConnectedNodes)
                {
                    if (innerNode.NodeId == leaderNodeId)
                        innerNode.IsLeader = true;
                }
            }
        }

        public void AssigningRoles()
        {
            if (IsLeader)
            {
                if (NodeId % 2 == 0)
                    NodeRole = NodeRoleEnum.Receiver;
                else
                    NodeRole = NodeRoleEnum.Hasher;

                foreach (var node in ConnectedNodes)
                {
                    if (node.NodeId % 2 == 0)
                        node.NodeRole = NodeRoleEnum.Receiver;
                    else
                        node.NodeRole = NodeRoleEnum.Hasher;

                    foreach (var innerNode in node.ConnectedNodes)
                    {
                        if (innerNode.NodeId % 2 == 0)
                            innerNode.NodeRole = NodeRoleEnum.Receiver;
                        else
                            innerNode.NodeRole = NodeRoleEnum.Hasher;
                    }
                }
            }
            else
            {
                foreach (var node in ConnectedNodes)
                {
                    if (node.IsLeader)
                    {
                        node.AssigningRoles();
                    }
                }
            }
        }

        public void AddNewNode(Node node)
        {
            node.NodeId = AssigningId();
            node.NodeRole = AssigningRole(node.NodeId);
            SetupNodeConnection(node);
        }

        public NodeRoleEnum AssigningRole(int nodeId)
        {
            var leaderNode = NavigateToLeaderNode();
            return leaderNode.SetNewRole(nodeId);
        }

        private NodeRoleEnum SetNewRole(int nodeId)
        {
            if (nodeId % 2 == 0)
                return NodeRoleEnum.Receiver;
            else
                return NodeRoleEnum.Hasher;
        }

        public int AssigningId()
        {
            var leaderNode = NavigateToLeaderNode();
            return leaderNode.SetNewId();
        }

        private int SetNewId()
        {
            int maxNodeId = ConnectedNodes.Max(x => x.NodeId);
            maxNodeId = this.NodeId > maxNodeId ? this.NodeId : maxNodeId;

            return maxNodeId+1;
        }

        private bool IsLeaderExists()
        {
            if (this.IsLeader)
                return true;
            else if (ConnectedNodes.Any(x => x.IsLeader == true))
                return true;

            return false;
        }

        private Node NavigateToLeaderNode()
        {
            if (IsLeaderExists())
            {
                if (this.IsLeader)
                    return this;
                else 
                    return ConnectedNodes.Where(x => x.IsLeader).FirstOrDefault();
            }
            else
            {
                ElectLeader();
                return NavigateToLeaderNode();
            }
        }

        public void StoreTextValuesRequest(string sentence)
        {
            if (this.NodeRole == NodeRoleEnum.Receiver)
            {
                foreach (var node in ConnectedNodes)
                {
                    if (node.NodeRole == NodeRoleEnum.Hasher)
                    {                      
                        node.StoreTextValuesInReceiver(sentence);
                        break;
                    }
                }
            }
            else
            {
                StoreTextValuesInReceiver(sentence);
            }
        }

        public void StoreTextValuesInReceiver(string sentence)
        {
            var hasherService = new HasingService(NodeId, ConnectedNodes, _valueTable, NodeRole);
            hasherService.StoreTextValuesInReceiver(sentence);
        }

        public string GetStoredTextValueRequest(string firstTenCharacters)
        {
            if (this.NodeRole == NodeRoleEnum.Receiver)
            {
                foreach (var node in ConnectedNodes)
                {
                    if (node.NodeRole == NodeRoleEnum.Hasher)
                    {
                        return node.GetStoredTextValue(firstTenCharacters);
                    }
                }
            }
            else
            {
                return GetStoredTextValue(firstTenCharacters);
            }

            return null;
        }

        public string GetStoredTextValue(string firstTenCharacters)
        {
            var hasherService = new HasingService(NodeId, ConnectedNodes, _valueTable, NodeRole);
            string textValue = hasherService.GetStoredTextValue(firstTenCharacters);
            
            return textValue;
        }
    }
}
