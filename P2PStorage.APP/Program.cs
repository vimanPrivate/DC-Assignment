using P2PStorage.Common.Enums;
using P2PStorage.Common.Models;
using P2PStorage.Service.Services.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace P2PStorage.APP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter number of nodes : ");
            int noOfNodes = int.Parse(Console.ReadLine());

            string text = "The Mapogo Lions, also known as the Mapogo Coalition or the Mapogo pride, were a legendary coalition of" +
                            "male lions in the Sabi Sand Game Reserve in South Africa. The coalition gained fame for their dominance and" +
                            "ruthless behavior, particularly in their efforts to assert control over territory and prides." +
                            "The coalition consisted of six to eight male lions at its peak, and they were known for their exceptional " +
                            "hunting skills and aggressive tactics. They would often target rival male lions, as well as lionesses and" +
                            "cubs from other prides, in order to expand their territory and ensure their dominance." +
                            "The Mapogo Lions became the subject of numerous documentaries and wildlife films due to" +
                            "their remarkable story and behavior. However, their reign eventually came to an end as they " +
                            "grew older and weaker, and new coalitions of younger lions challenged their dominance. Today, they are " +
                            "remembered as one of the most formidable lion coalitions in African wildlife history";

            var textList = text.Split('.').ToList();
            var nodeList = new List<NodeService>();

            // assining ids to nodes

            int nID = 0;
            for (int i = 0; i < (noOfNodes*2); i++)
            {
                var node = new NodeService();

                if (i % 2 == 1)
                {
                    node.nodeId = nID;
                    node.Role = NodeRoleEnum.Receiver;
                    nID++;
                }
                else
                {
                    node.Role = NodeRoleEnum.Hasher;
                    node.nodeId = -i;
                }
                
                nodeList.Add(node);
            }

            // assining string to the nodes 

            foreach (var sentence in textList)
            {
                int nodeId = GettingStringsNodeId(sentence, nodeList.Where(x => x.Role == NodeRoleEnum.Receiver).ToList().Count);
                int backupNodeId = GetBackupNodeId(nodeId);

                foreach (var node in nodeList)
                {
                    if (node.nodeId == nodeId || node.nodeId == backupNodeId)
                    {
                        node.AddValuesToValueTable(sentence);
                    }
                }
            }


            Node nodeOne = new Node(0);
            Node nodeTwo = new Node(1);
            Node nodeThree = new Node(2);

            // Connect peers
            nodeOne.SetupNodeConnection(nodeTwo);
            nodeOne.SetupNodeConnection(nodeThree);
            nodeTwo.SetupNodeConnection(nodeThree);

            //nodeOne.DisconnectNode(nodeTwo);

            nodeOne.ElectLeader();
            nodeTwo.ElectLeader();
            nodeThree.ElectLeader();

            // Send messages between peers
            nodeOne.SendMessage("Hello, Bob and Charlie!");
            nodeTwo.SendMessage("Hi, Alice!");
            nodeThree.SendMessage("Hey, everyone!");

            Console.ReadKey(true);
        }



        private static int GettingStringsNodeId(string sentence, int noOfReceiverNodes)
        {
            int nodeId = 0;
            int totalAsciiValue = 0;
            string firstTenCharacters = sentence.Substring(0, Math.Min(sentence.Length, 10));

            foreach (char c in firstTenCharacters)
            {
                totalAsciiValue += Convert.ToInt32(c);
            }

            nodeId = totalAsciiValue % noOfReceiverNodes;
            return nodeId;
        }

        private static int GetBackupNodeId(int originalNodeId)
        {
            if (originalNodeId - 1 < 0)
                return originalNodeId+1;
            else
                return originalNodeId - 1;
        }
    }

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
                peer.ConnectedNodes.Add(this); // Bidirectional connection for simplicity
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
                node.ConnectedNodes.Remove(this); // Remove bidirectional connection
                Console.WriteLine($"{NodeId} is disconnected from {node.NodeId}");
            }
            else
            {
                Console.WriteLine($"{NodeId} is not connected to {node.NodeId}");
            }
        }

        public void ElectLeader()
        {
            // Check if this peer is the highest-ranked
            if (ConnectedNodes.All(x => x.NodeId < NodeId))
            {
                IsLeader = true;
                Console.WriteLine($"{NodeId} is elected as the leader.");
                // Assign roles based on leadership
                if (NodeId % 2 == 0)
                {
                    NodeRole = NodeRoleEnum.Hasher;
                }
                else
                {
                    NodeRole = NodeRoleEnum.Receiver;
                }

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
