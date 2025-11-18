using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back_end.DTOs;
using back_end.Interfaces;

namespace back_end.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageReposiroty _messageReposiroty;
        public MessageService(IMessageReposiroty messageReposiroty)
        {
            _messageReposiroty = messageReposiroty;
        }
    }
}