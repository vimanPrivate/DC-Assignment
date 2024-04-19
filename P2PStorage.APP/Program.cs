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
            /*
             * Initial Process goes here
             * Letting user select how many nodes does user wants ( for Demo only )
             * Seperating entire paragraph by '.'
             * Generating and linking nodes each other according to the user entered node count
             * Selecting leader
             * Assigning roles
             * storing values to the nodes
             */


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

            /*
             * Initiating Nodes 
            */

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
            initNode.AssigningRoles();
            initNode.DisconnectNode(1);

            foreach(string sentence in textList) 
            {
                initNode.StoreTextValuesRequest(sentence);
            }
            





            var val1 = initNode.GetStoredTextValueRequest("The Mapogo");
            var val2 = initNode.GetStoredTextValueRequest(" The coali");

            Console.ReadKey(true);
        }
    }
}
