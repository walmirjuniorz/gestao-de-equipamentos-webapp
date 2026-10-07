using System.Text.Json;
using System.Text.Json.Serialization;
using GestaoDeEquipamentos.WebApp.Modulos.Chamados.Dominio;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Dominio;
using GestaoDeEquipamentos.WebApp.Modulos.Fabricantes.Dominio;

namespace GestaoDeEquipamentos.WebApp.Compartilhado.Infraestrutura.Arquivos;

public sealed class ContextoJson
{
    private readonly string caminhoArquivoDados;

    public List<Fabricante> Fabricantes { get; set; } = new List<Fabricante>();
    public List<Equipamento> Equipamentos { get; set; } = new List<Equipamento>();
    public List<Chamado> Chamados { get; set; } = new List<Chamado>();

    public ContextoJson()
    {
        string caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string caminhoDiretorioAplicativo = Path.Join(caminhoAppData, "GestaoDeEquipamentos-Backend");

        Directory.CreateDirectory(caminhoDiretorioAplicativo);

        caminhoArquivoDados = Path.Join(caminhoDiretorioAplicativo, "dados.json");
    }

    public void Salvar()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.ReferenceHandler = ReferenceHandler.Preserve;

        string jsonString = JsonSerializer.Serialize(this, options);

        File.WriteAllText(caminhoArquivoDados, jsonString);
    }

    public void Carregar()
    {
        if (!File.Exists(caminhoArquivoDados))
        {
            Carregar(CarregarDadosPredefinidos());
            Salvar();
            return;
        }

        string jsonString = File.ReadAllText(caminhoArquivoDados);

        if (string.IsNullOrWhiteSpace(jsonString))
        {
            Carregar(CarregarDadosPredefinidos());
            Salvar();
            return;
        }

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.ReferenceHandler = ReferenceHandler.Preserve;

        ContextoJson? contextoSalvo =
            JsonSerializer.Deserialize<ContextoJson>(jsonString, options);

        if (contextoSalvo == null || !contextoSalvo.PossuiDados())
            contextoSalvo = CarregarDadosPredefinidos();


        bool alterado = contextoSalvo.IncluirDadosPredefinidosAusentes();
        Carregar(contextoSalvo);
        if (alterado)
            Salvar();
    }

    private void Carregar(ContextoJson contexto)
    {
        Fabricantes = contexto.Fabricantes;
        Equipamentos = contexto.Equipamentos;
        Chamados = contexto.Chamados;
    }

    public ContextoJson CarregarDadosPredefinidos()
    {
        ContextoJson contextoPredefinido = new ContextoJson();

        contextoPredefinido.Fabricantes.AddRange(new List<Fabricante>
        {
            new Fabricante("Samsung Electronics Co., Ltd.", "samsungbrasil@gmail.com.br", "(11) 3456-7801") { Id = 1 },
            new Fabricante("ASUSTeK Computer Inc.", "asusrogtek@gmail.com.br", "(21) 2345-6702") { Id = 2 },
            new Fabricante("LG Electronics Inc.", "lgbrasil@gmail.com.br", "(31) 3234-5603") { Id = 3 },
            new Fabricante("NVIDIA Corporation", "nvidiacorporation@gmail.com.br", "(41) 3345-6704") { Id = 4 },
            new Fabricante("Apple Inc.", "apple@gmail.com.br", "(51) 3123-4505") { Id = 5 }
        });

        contextoPredefinido.Equipamentos.AddRange(new List<Equipamento>
        {
            new("Monitor Gamer ASUS ROG Strix OLED 27\" QHD 280Hz 0.03ms", 4599m, new DateTime(2025, 2, 10), contextoPredefinido.Fabricantes[1]) { Id = 1 },
            new("Monitor Gamer Samsung Odyssey G5, 27\", QHD, 165Hz, 1ms", 1140m, new DateTime(2023, 1, 7), contextoPredefinido.Fabricantes[0]) { Id = 2 },
            new("Placa de Vídeo ASUS RTX 5090 ROG Astral Gaming 32GB, GDDR7", 42999m, new DateTime(2026, 5, 9), contextoPredefinido.Fabricantes[3]) { Id = 3 },
            new("Smart TV 4K LG OLED 55\" Evo AI, Dolby Vision e Dolby Atmos", 6599m, new DateTime(2025, 10, 6), contextoPredefinido.Fabricantes[2]) { Id = 4 },
            new("iPhone 18 Pro Max 2TB Bordô", 6599m, new DateTime(2026, 9, 1), contextoPredefinido.Fabricantes[4]) { Id = 5 }
        });
        contextoPredefinido.Chamados.AddRange(new List<Chamado>
        {
            new("Monitor com retenção de imagem", "Deu burn-in e a tela está com defeitos de RGB.", contextoPredefinido.Equipamentos[0], DateTime.Now) { Id = 1 },
            new("Monitor piscando", "O monitor apresenta oscilações na imagem.", contextoPredefinido.Equipamentos[1], DateTime.Now) { Id = 2 },
            new("queimou por voltagem errada", "a placa foi colocada numa fonte 220v e colocaram a fonte na tomada 110v", contextoPredefinido.Equipamentos[2],  DateTime.Now) { Id = 3 },
            new("tela preta", "tv liga normal mas fica com a tela preta", contextoPredefinido.Equipamentos[3],  DateTime.Now) { Id = 4 },


        });

        return contextoPredefinido;
    }

    private bool PossuiDados()
    {
        return Fabricantes.Count > 0 && Equipamentos.Count > 0 && Chamados.Count > 0;
    }

    private bool IncluirDadosPredefinidosAusentes()
    {
        bool alterado = false;
        ContextoJson predefinidos = CarregarDadosPredefinidos();

        foreach (Fabricante fabricantePadrao in predefinidos.Fabricantes)
        {
            if (Fabricantes.Any(f => Normalizar(f.Nome) == Normalizar(fabricantePadrao.Nome)))
                continue;
            fabricantePadrao.Id = ProximoId(Fabricantes.Select(f => f.Id));
            Fabricantes.Add(fabricantePadrao);
            alterado = true;
        }

        foreach (Equipamento equipamentoPadrao in predefinidos.Equipamentos)
        {
            if (Equipamentos.Any(e => Normalizar(e.Nome) == Normalizar(equipamentoPadrao.Nome) ||
                (e.Id == equipamentoPadrao.Id && Normalizar(e.Fabricante.Nome) == Normalizar(equipamentoPadrao.Fabricante.Nome))))
                continue;
            equipamentoPadrao.Id = ProximoId(Equipamentos.Select(e => e.Id));
            equipamentoPadrao.Fabricante = Fabricantes.First(f =>
                Normalizar(f.Nome) == Normalizar(equipamentoPadrao.Fabricante.Nome));
            Equipamentos.Add(equipamentoPadrao);
            alterado = true;
        }

        foreach (Chamado chamadoPadrao in predefinidos.Chamados)
        {
            if (Chamados.Any(c => Normalizar(c.Titulo) == Normalizar(chamadoPadrao.Titulo)))
                continue;
            chamadoPadrao.Id = ProximoId(Chamados.Select(c => c.Id));
            chamadoPadrao.Equipamento = Equipamentos.First(e =>
                Normalizar(e.Nome) == Normalizar(chamadoPadrao.Equipamento.Nome));
            Chamados.Add(chamadoPadrao);
            alterado = true;
        }

        return alterado;
    }

    private static int ProximoId(IEnumerable<int> ids) => ids.DefaultIfEmpty(0).Max() + 1;

    private static string Normalizar(string texto) => texto
        .Replace("Ã­", "i", StringComparison.OrdinalIgnoreCase)
        .Replace("Ã´", "o", StringComparison.OrdinalIgnoreCase)
        .Replace("Ã£", "a", StringComparison.OrdinalIgnoreCase)
        .Replace("Ã§", "c", StringComparison.OrdinalIgnoreCase)
        .Replace("Ã¡", "a", StringComparison.OrdinalIgnoreCase)
        .Replace("Ã©", "e", StringComparison.OrdinalIgnoreCase)
        .Replace("Ã³", "o", StringComparison.OrdinalIgnoreCase)
        .Replace("Ãº", "u", StringComparison.OrdinalIgnoreCase)
        .Where(char.IsLetterOrDigit)
        .Aggregate(new System.Text.StringBuilder(), (builder, caractere) => builder.Append(char.ToUpperInvariant(caractere)))
        .ToString();
}
