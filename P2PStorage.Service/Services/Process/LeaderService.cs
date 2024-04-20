using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PStorage.Service.Services.Process
{
    internal class LeaderService
    {
        //public void ElectLeader(List<P2PStorage.Service.Services.Node.Node> ConnectedNodes, int NodeId, bool IsLeader)
        //{
        //    if (ConnectedNodes is null)
        //    {
        //        throw new ArgumentNullException(nameof(ConnectedNodes));
        //    }

        //    int leaderNodeId = ConnectedNodes.Select(node => node.NodeId).Max();

        //    //broadcasting the leader
        //    if (leaderNodeId == this.NodeId)
        //        this.IsLeader = true;

        //    foreach (var node in ConnectedNodes)
        //    {
        //        if (node.NodeId == leaderNodeId)
        //            node.IsLeader = true;

        //        foreach (var innerNode in node.ConnectedNodes)
        //        {
        //            if (innerNode.NodeId == leaderNodeId)
        //                innerNode.IsLeader = true;
        //        }
        //    }
        //}
    }
}
