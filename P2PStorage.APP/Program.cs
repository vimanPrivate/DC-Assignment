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

            string text = "There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, " +
                            "or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum, " +
                            "you need to be sure there isn't anything embarrassing hidden in the middle of text.";

            var devidedStringList = SeperateStringInto10Charactors(text);

            var nodeList = new List<NodeService>();

            // assining ids to nodes

            for (int i = 0; i < noOfNodes; i++)
            {
                var node = new NodeService();
                node.nodeId = i;
                nodeList.Add(node);
            }

            // assining string to the nodes 

            foreach (var txt in devidedStringList)
            {
                int nodeId = GettingStringsNodeId(txt, nodeList.Count);
                foreach(var node in nodeList)
                {
                    if(node.nodeId == nodeId)
                    {
                        node.AddValuesToValueTable(txt);
                        break;
                    }
                }
            }

            Console.ReadKey(true);
        }

        private static List<string> SeperateStringInto10Charactors(string text)
        {
            var list = new List<string>();

            int count = 0;
            string tmpString = "";

            foreach (char c in text)
            {
                if (count == 10)
                {
                    count = 0;
                    list.Add(tmpString);
                    tmpString = "";
                }

                tmpString += c;
                count++;
            }

            return list;
        }

        private static int GettingStringsNodeId(string text, int noOfNodes)
        {
            int nodeId = 0;
            int totalAsciiValue = 0;

            foreach (char c in text)
            {
                totalAsciiValue += Convert.ToInt32(c);
            }

            nodeId = totalAsciiValue % noOfNodes;
            return nodeId;
        }
    }
}
