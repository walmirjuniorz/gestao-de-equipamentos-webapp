using GestaoDeEquipamentos.WebApp.Compartilhado.Infraestrutura.Arquivos;
using GestaoDeEquipamentos.WebApp.Modulos.Chamados.Dominio;

namespace GestaoDeEquipamentos.WebApp.Modulos.Chamados.Infraestrutura;

public sealed class RepositorioChamadoEmArquivo : RepositorioBaseEmArquivo<Chamado>, IRepositorioChamado
{
    public RepositorioChamadoEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    protected override List<Chamado> ObterRegistros()
    {
        return contexto.Chamados;
    }

    public bool ExisteParaEquipamento(int equipamentoId)
    {
        return registros.Any(chamado => chamado.Equipamento?.Id == equipamentoId);
    }
}
