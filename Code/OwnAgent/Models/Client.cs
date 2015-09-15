using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OwnAgent.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string Login { get; set; }

        public Client(){}
        public Client(string clientId, string login)
        {
            ClientId = clientId;
            Login = login;
        }
    }
}