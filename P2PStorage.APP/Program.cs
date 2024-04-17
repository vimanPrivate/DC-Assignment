using P2PStorage.Common.Enums;
using P2PStorage.Service.Services.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
