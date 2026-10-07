using GestaoDeEquipamentos.WebApp.Compartilhado.Infraestrutura.Arquivos;
using GestaoDeEquipamentos.WebApp.Modulos.Chamados.Infraestrutura;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Dominio;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Infraestrutura;
using GestaoDeEquipamentos.WebApp.Modulos.Fabricantes.Dominio;
using GestaoDeEquipamentos.WebApp.Modulos.Fabricantes.Infraestrutura;

namespace GestaoDeEquipamentos.WebApp.Compartilhado.Infraestrutura;

public static class InjecaoDeDependencia
{
    public static void AdicionarCamadaDeInfraestrutura(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(services =>
        {
            ContextoJson contexto = new ContextoJson();
            contexto.Carregar();
            return contexto;
        });

        services.AddScoped<IRepositorioFabricante, RepositorioFabricanteEmArquivo>();
        services.AddScoped<IRepositorioEquipamento, RepositorioEquipamentoEmArquivo>();
        services.AddScoped<RepositorioChamadoEmArquivo>();
    }
}
