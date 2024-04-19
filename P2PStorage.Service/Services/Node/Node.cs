using P2PStorage.Common.Enums;
using P2PStorage.Common.Models;
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
        private List<ValueTable> _valueTable;

        public int NodeId { get; set; }
        public string Name { get; set; }
        public List<Node> ConnectedNodes { get; }
        public NodeRoleEnum NodeRole { get; private set; }
        public bool IsLeader { get; private set; }

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


        //public void SetupNodeConnection(Node peer)
        //{
        //    if (!ConnectedNodes.Contains(peer))
        //    {
        //        ConnectedNodes.Add(peer);
        //        peer.ConnectedNodes.Add(this);
        //        Console.WriteLine($"{NodeId} is now connected to {peer.NodeId}");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"{NodeId} is already connected to {peer.NodeId}");
        //    }
        //}

        //public void DisconnectNode(int nodeId)
        //{
        //    Node nodeToRemove = ConnectedNodes.FirstOrDefault(node => node.NodeId == nodeId);
        //    if (nodeToRemove != null)
        //    {
        //         Remove the node from the ConnectedNodes list of other nodes
        //        foreach (var connectedNode in ConnectedNodes)
        //        {
        //            connectedNode.ConnectedNodes.Remove(this);
        //        }
        //         Clear the ConnectedNodes list of the node to remove
        //        nodeToRemove.ConnectedNodes.Clear();
        //         Remove the node from the ConnectedNodes list of this node
        //        ConnectedNodes.Remove(nodeToRemove);
        //    }
        //}

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

        public void DisconnectNode(Node node)
        {
            if (ConnectedNodes.Contains(node))
            {
                ConnectedNodes.Remove(node);
                node.ConnectedNodes.Remove(this);
                Console.WriteLine($"{NodeId} is disconnected from {node.NodeId}");
            }
            else
            {
                Console.WriteLine($"{NodeId} is not connected to {node.NodeId}");
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

        //public void ElectLeader()
        //{
        //    if (ConnectedNodes.All(x => x.NodeId < NodeId))
        //    {
        //        IsLeader = true;
        //        Console.WriteLine($"{NodeId} is elected as the leader.");

        //        // Notify other peers of the leader
        //        foreach (var peer in ConnectedNodes)
        //        {
        //            peer.NotifyNewLeader(this);
        //        }
        //    }
        //}

        public void NotifyNewLeader(Node leader)
        {
            if (!IsLeader && leader.NodeId.CompareTo(NodeId) > 0)
            {
                Console.WriteLine($"{NodeId} received notification of new leader: {leader.NodeId}");
                IsLeader = false;
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
            string firstTenCharacters = sentence.Substring(0, Math.Min(sentence.Length, 10));
            int originalNode = GetOriginalNodeId(firstTenCharacters);
            var storingNodesArray = GetPossibleNodeReceiverNodeList(originalNode);

            foreach (var node in ConnectedNodes)
            {
                if (node.NodeId == storingNodesArray[0] || node.NodeId == storingNodesArray[1])
                {
                    if(!node._valueTable.Any(row => row.Id == firstTenCharacters))
                    {
                        node._valueTable.Add(new ValueTable()
                        {
                            Id = firstTenCharacters,
                            Value = sentence
                        });
                    }
                }

                foreach (var innderNode in node.ConnectedNodes)
                {
                    if (innderNode.NodeId == storingNodesArray[0] || innderNode.NodeId == storingNodesArray[1])
                    {
                        if (!innderNode._valueTable.Any(row => row.Id == firstTenCharacters))
                        {
                            innderNode._valueTable.Add(new ValueTable()
                            {
                                Id = firstTenCharacters,
                                Value = sentence
                            });
                        } 
                    }
                }
            }
        }

        private int GetOriginalNodeId(string firstTenCharacters)
        {
            int originalNode = 0;
            int totalAsciiValue = 0;
            //string firstTenCharacters = sentence.Substring(0, Math.Min(sentence.Length, 10));

            foreach (char c in firstTenCharacters)
            {
                totalAsciiValue += Convert.ToInt32(c);
            }

            int noOfReceiverNodes = this.NodeRole == NodeRoleEnum.Receiver ? 1 : 0;
            noOfReceiverNodes = this.ConnectedNodes.Count(node => node.NodeRole == NodeRoleEnum.Receiver);

            originalNode = totalAsciiValue % noOfReceiverNodes;

            return originalNode;
        }

        private int[] GetPossibleNodeReceiverNodeList(int originalNodeId)
        {
            var receiverNodeList = new int[2];

            var availableReceiverNodeList = ConnectedNodes
                                            .Where(node => node.NodeRole == NodeRoleEnum.Receiver)
                                            .Select(x => x.NodeId).ToList();

            if (this.NodeRole == NodeRoleEnum.Receiver)
                availableReceiverNodeList.Add(this.NodeId);

            availableReceiverNodeList = availableReceiverNodeList.OrderBy(id => id).ToList();

            int firstNode;
            int backupNode;

            if (availableReceiverNodeList.Contains(originalNodeId))
            {
                firstNode = originalNodeId;

                if (availableReceiverNodeList.Any(id => id > firstNode))
                    backupNode = availableReceiverNodeList.Where(id => id > firstNode).OrderBy(id => id).FirstOrDefault();
                else
                    backupNode = availableReceiverNodeList[0];
            }
            else
            {
                if (availableReceiverNodeList.Any(id => id > originalNodeId))
                    firstNode = availableReceiverNodeList.Where(id => id > originalNodeId).OrderBy(id => id).FirstOrDefault();
                else
                    firstNode = availableReceiverNodeList[0];

                if (availableReceiverNodeList.Any(id => id > firstNode))
                    backupNode = availableReceiverNodeList.Where(id => id > firstNode).OrderBy(id => id).FirstOrDefault();
                else
                    backupNode = availableReceiverNodeList[0];
            }

            receiverNodeList[0] = firstNode;
            receiverNodeList[1] = backupNode;

            return receiverNodeList;
        }

        public string GetTextValueRequest(string firstTenCharacters)
        {
            string textValue = "";


            return textValue;
        }

        //// Method to send a message to all connected peers
        //public void SendMessage(string message)
        //{
        //    Console.WriteLine($"{NodeId} sends: {message}");
        //    foreach (var peer in ConnectedNodes)
        //    {
        //        peer.ReceiveMessage(message);
        //    }
        //}

        //// Method to receive a message from another node
        //public void ReceiveMessage(string message)
        //{
        //    Console.WriteLine($"{NodeId} receives: {message}");
        //}
    }
}
