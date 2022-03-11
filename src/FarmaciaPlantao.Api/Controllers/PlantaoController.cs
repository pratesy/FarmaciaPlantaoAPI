﻿using FarmaciaPlantao.Api.DTOs;
using FarmaciaPlantao.Api.Repository;
using FarmaciaPlantao.Core.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmaciaPlantao.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlantaoController : Controller
    {
        private readonly AgendasRepository _agendasRepository;

        public PlantaoController(AgendasRepository agendasRepository)
        {
            _agendasRepository = agendasRepository;
        }

        [HttpGet]
        public ObjectResult Inicio()
        {
            if (!FarmaciaAberta())
                return NotFound("Farmácias fechadas!");
            
            var agora = DateTime.Now;
            var tem = _agendasRepository.Find(x => x.Inicio <= agora && x.Fim >= agora);

            if (tem == null)
                return NotFound("Sem farmácias de plantão no momento");

            return Ok(ToAgendaDTO(tem));
        }

        [HttpGet("por-cidadeId")]
        public ObjectResult PorCidadeId(string cidadeId)
        {           
            if (!FarmaciaAberta())
                return NotFound("Farmácias fechadas!");

            var agora = DateTime.Now;
            var tem = _agendasRepository.Find(x => x.Farmacia.Cidade.Id == new ObjectId(cidadeId) && x.Inicio <= agora && x.Fim >= agora);

            if (tem == null)
                return NotFound("Sem farmácias de plantão no momento");

            return Ok(ToAgendaDTO(tem));
        }

        public bool FarmaciaAberta()
        {
            var agora = DateTime.Now;
            if (agora < new DateTime(agora.Year, agora.Month, agora.Day, 8, 0, 0) || agora > new DateTime(agora.Year, agora.Month, agora.Day, 22, 0, 0))
                return false;

            return true;
        }


        private AgendaDTO ToAgendaDTO(Agenda agenda)
        {
            return new AgendaDTO()
            {
                Inicio = agenda.Inicio,
                Fim = agenda.Fim,
                Farmacia = ToFarmaciaDTO(agenda.Farmacia)
            };
        }
        private FarmaciaDTO ToFarmaciaDTO(Farmacia farmacia)
        {
            return new FarmaciaDTO()
            {
                Id = farmacia.Id.ToString(),
                Nome = farmacia.Nome,
                Endereco = farmacia.Endereco,
                UrlImagem = farmacia.UrlImagem,
                Telefone = farmacia.Telefone,
                Whatsapp = farmacia.Whatsapp,
                Cidade = farmacia.Cidade.Nome,
                CidadeId = farmacia.Cidade.Id.ToString(),
                Estado = farmacia.Cidade.Estado.Nome,
                EstadoId = farmacia.Cidade.Estado.Id.ToString(),
                UF = farmacia.Cidade.Estado.UF
            };
        }
    }
}
