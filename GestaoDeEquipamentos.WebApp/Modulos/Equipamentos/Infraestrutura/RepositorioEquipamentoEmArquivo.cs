using GestaoDeEquipamentos.WebApp.Compartilhado.Infraestrutura.Arquivos;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Dominio;

namespace GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Infraestrutura;

public sealed class RepositorioEquipamentoEmArquivo : RepositorioBaseEmArquivo<Equipamento>, IRepositorioEquipamento
{
    public RepositorioEquipamentoEmArquivo(ContextoJson contexto) : base(contexto)
    {
    }

    public bool ExisteParaFabricante(int idFabricante)
    {
        return registros.Any(equipamento => equipamento.Fabricante?.Id == idFabricante);
    }

    protected override List<Equipamento> ObterRegistros()
    {
        return contexto.Equipamentos;
    }
}
