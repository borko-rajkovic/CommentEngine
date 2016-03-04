using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CommentEngine.Hubs
{
    public class NewCommentsHub: Hub
    {
        public void Send(int id_vijesti, int brojKomentara)
        {
            Clients.All.broadcastMessage(id_vijesti, brojKomentara);
        }
    }
}