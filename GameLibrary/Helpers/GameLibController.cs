using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Helpers
{
    public abstract class GameLibController : Controller
    {
        [TempData]
        public string Message { get; set; }

        [TempData]
        public string MessageType { get; set; }

        public void SetMessage(string messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }
    }
}
