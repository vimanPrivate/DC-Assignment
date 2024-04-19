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

        public int NodeId { get; }
        public List<Node> ConnectedNodes { get; }
        public NodeRoleEnum NodeRole { get; private set; }
        public bool IsLeader { get; private set; }

        public Node(int nodeId)
        {
            NodeId = nodeId;
            ConnectedNodes = new List<Node>();
        }

        // Method to connect to another node
        public void SetupNodeConnection(Node peer)
        {
            if (!ConnectedNodes.Contains(peer))
            {
                ConnectedNodes.Add(peer);
                peer.ConnectedNodes.Add(this);
                Console.WriteLine($"{NodeId} is now connected to {peer.NodeId}");
            }
            else
            {
                Console.WriteLine($"{NodeId} is already connected to {peer.NodeId}");
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
            if (ConnectedNodes.All(x => x.NodeId < NodeId))
            {
                IsLeader = true;
                Console.WriteLine($"{NodeId} is elected as the leader.");

                if (NodeId % 2 == 0)
                    NodeRole = NodeRoleEnum.Hasher;
                else
                    NodeRole = NodeRoleEnum.Receiver;

                // Notify other peers of the leader
                foreach (var peer in ConnectedNodes)
                {
                    peer.NotifyNewLeader(this);
                }
            }
        }

        // Method to notify peers of the new leader
        public void NotifyNewLeader(Node leader)
        {
            if (!IsLeader && leader.NodeId.CompareTo(NodeId) > 0)
            {
                Console.WriteLine($"{NodeId} received notification of new leader: {leader.NodeId}");
                IsLeader = false;
                NodeRole = NodeRoleEnum.Receiver;
            }
        }

        public void AssigningRoles()
        {
            if (IsLeader)
            {
                int totalNodesInTheSystem = ConnectedNodes.Count+1;
                for(int x = 0; x < ConnectedNodes.Count;x++)
                {
                    if(x < totalNodesInTheSystem/2)
                        ConnectedNodes[x].NodeRole = NodeRoleEnum.Receiver;
                    else
                        ConnectedNodes[x].NodeRole = NodeRoleEnum.Hasher;
                }
            }
        }

        // Method to send a message to all connected peers
        public void SendMessage(string message)
        {
            Console.WriteLine($"{NodeId} sends: {message}");
            foreach (var peer in ConnectedNodes)
            {
                peer.ReceiveMessage(message);
            }
        }

        // Method to receive a message from another node
        public void ReceiveMessage(string message)
        {
            Console.WriteLine($"{NodeId} receives: {message}");
        }
    }
}
