﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBetaApi.Business.Models;

namespace TestBetaApi.Business.Intefaces
{
    public interface IFornecedorService : IDisposable
    {
        Task Adicionar(Fornecedor fornecedor);
        Task Atualizar(Fornecedor fornecedor);
        Task Remover(Guid id);
        Task AtualizarEndereco(Endereco endereco);
    }
}
