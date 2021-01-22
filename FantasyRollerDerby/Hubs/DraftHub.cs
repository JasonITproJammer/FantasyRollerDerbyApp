using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using FantasyRollerDerby.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Configuration;

namespace FantasyRollerDerby.Hubs
{
    [HubName("draft")]
    public class DraftHub : Hub
    {
        string groupName = "none";

        public override Task OnConnected()
        {
            string[] vars = Context.QueryString["vars"].ToString().Split('|');
            groupName = vars[0];
            //string draftClock = vars[1];
            Groups.Add(Context.ConnectionId, groupName);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Groups.Remove(Context.ConnectionId, groupName);
            return base.OnDisconnected(stopCalled);
        }

        public void draft(string groupName, string skaterID)
        {
            Clients.Group(groupName).draftSkater(skaterID);
        }

        #region InformationalMethods
        public Task JoinRoom(string roomName)
        {
            //add async before Task if using below
            //https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/working-with-groups
            //await Groups.Add(Context.ConnectionId, roomName);
            //Clients.Group(roomName).addChatMessage(Context.User.Identity.Name + " joined.");
            return Groups.Add(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            //https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/handling-connection-lifetime-events
            return Groups.Remove(Context.ConnectionId, roomName);
        }
        #endregion
    }
}