using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestBetaApi.API.Controllers;
using TestBetaApi.Business.Intefaces;

namespace TestBetaApi.API.V2.Controller
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {

        public TesteController(INotificador notificador, IUser appUser) : base(notificador, appUser)
        {
        }

        [HttpGet]
        public string Valor()
        {
            return "Sou a V2";
        }
    }
}