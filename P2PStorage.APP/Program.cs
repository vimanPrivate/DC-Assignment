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
            var nodeList = new List<Node>();

            int i = 0;
            Node initNode = null;
            do
            {
                if (i == 0)
                    initNode = new Node(i);
                else
                    initNode.SetupNodeConnection(new Node(i));

                i++;
            }
            while (i < (noOfNodes * 2));

            initNode.ElectLeader();
            //initNode.DisconnectNode(4);
            initNode.AssigningRoles();

            initNode.StoreTextValuesRequest(textList[0]);
            initNode.StoreTextValuesRequest(textList[1]);

            var val1 = initNode.GetStoredTextValueRequest("The Mapogo");
            var val2 = initNode.GetStoredTextValueRequest(" The coali");

            int xxxxxx = 00;

            //for (int i = 0; i < (noOfNodes * 2); i++)
            //{
            //    nodeList.Add(new Node(i));
            //}

            //CreateNodeConnection(nodeList);
            //ElectingALeader(nodeList);
            //AssignRoles(nodeList);


            // **************************************************************************************



            //Node nodeOne = new Node(0);
            //Node nodeTwo = new Node(1);
            //Node nodeThree = new Node(2);

            //// Connecting nodes
            //nodeOne.SetupNodeConnection(nodeTwo);
            //nodeOne.SetupNodeConnection(nodeThree);
            //nodeTwo.SetupNodeConnection(nodeThree);

            //// Disconnecting Nodes
            ////nodeOne.DisconnectNode(nodeTwo);

            //// Selecting Leader
            //nodeOne.ElectLeader();
            //nodeTwo.ElectLeader();
            //nodeThree.ElectLeader();

            //// Send messages between peers
            //nodeOne.SendMessage("Hello, Bob and Charlie!");
            //nodeTwo.SendMessage("Hi, Alice!");
            //nodeThree.SendMessage("Hey, everyone!");

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

        public static void CreateNodeConnection(List<Node> nodeList)
        {
            for(int x = 0; x < nodeList.Count; x++)
            {
                var node = nodeList[x];
                for(int y = x+1; y < nodeList.Count; y++)
                {
                    //node.SetupNodeConnection(nodeList[y]);
                }
            }
        }

        public static void ElectingALeader(List<Node> nodeList)
        {
            foreach(var node in nodeList)
            {
                node.ElectLeader();
            }
        }

        public static void AssignRoles(List<Node> nodeList)
        {
            foreach(var node in nodeList)
            {
                node.AssigningRoles();
            }
        }
    }
}
