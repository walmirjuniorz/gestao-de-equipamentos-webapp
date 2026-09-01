using GestaoDeEquipamentos.WebApp.Modulos.Chamados.Dominio;
using GestaoDeEquipamentos.WebApp.Modulos.Chamados.Infraestrutura;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Apresentacao;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Dominio;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeEquipamentos.WebApp.Modulos.Chamados.Apresentacao;

public class ChamadoController : Controller
{
    private readonly RepositorioChamadoEmArquivo repositorioChamado;
    private readonly RepositorioEquipamentoEmArquivo repositorioEquipamento;

    public ChamadoController(RepositorioChamadoEmArquivo repositorioChamado, RepositorioEquipamentoEmArquivo repositorioEquipamento)
    {
        this.repositorioChamado = repositorioChamado;
        this.repositorioEquipamento = repositorioEquipamento;
    }
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarChamadoViewModel> viewModels = new List<ListarChamadoViewModel>();

        foreach (Chamado c in repositorioChamado.SelecionarTodos())
        {
            ListarChamadoViewModel viewModel = new ListarChamadoViewModel(
                c.Id,
                c.Titulo,
                c.Descricao,
                c.Equipamento.Nome,
                c.DataAbertura
            );

            viewModels.Add(viewModel);
        }

        return View(viewModels);
    }
    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarChamadoViewModel viewModel = new(
            null,
            null,
            0,
            ObterEquipamentosDisponiveis()
        );

        return View(viewModel);
    }
    [HttpPost]
    public ActionResult Cadastrar(CadastrarChamadoViewModel viewModel)
    {
        Equipamento? equipamentoSelecionado = repositorioEquipamento.SelecionarPorId(viewModel.EquipamentoId);

        if (equipamentoSelecionado == null)
            ModelState.AddModelError(nameof(viewModel.EquipamentoId), "Selecione um Equipamento válido!");

        if (!ModelState.IsValid)
        {
            viewModel = viewModel with { EquipamentosDisponiveis = ObterEquipamentosDisponiveis() };

            return View(viewModel);
        }
        Chamado chamado = new(
            viewModel.Titulo ?? string.Empty,
            viewModel.Descricao ?? string.Empty,
            equipamentoSelecionado!,
            DateTime.Now
        );

        repositorioChamado.Cadastrar(chamado);

        return RedirectToAction(nameof(Listar));
    }
    [HttpGet]
    public ActionResult Editar(int id)
    {
        Chamado? chamadoSelecionado = repositorioChamado.SelecionarPorId(id);

        if (chamadoSelecionado == null)
            return NotFound();

        EditarChamadoViewModel viewModel = new(
            chamadoSelecionado.Id,
            chamadoSelecionado.Titulo,
            chamadoSelecionado.Descricao,
            chamadoSelecionado.Equipamento.Id,
            ObterEquipamentosDisponiveis()
        );
        return View(viewModel);
    }
    [HttpPost]
    public ActionResult Editar(int id, EditarChamadoViewModel viewModel)
    {
        Chamado? chamadoSelecionado = repositorioChamado.SelecionarPorId(id);

        Equipamento? equipamentoSelecionado = repositorioEquipamento.SelecionarPorId(viewModel.EquipamentoId);

        if (equipamentoSelecionado == null)
            ModelState.AddModelError(nameof(viewModel.EquipamentoId), "Selecione um Equipamento válido");

        if (!ModelState.IsValid)
        {
            viewModel = viewModel with { EquipamentosDisponiveis = ObterEquipamentosDisponiveis() };
        }

        Chamado chamadoAtualizado = new(
            viewModel.Titulo ?? string.Empty,
            viewModel.Descricao ?? string.Empty,
            equipamentoSelecionado!,
            chamadoSelecionado!.DataAbertura
        );

        bool conseguiuEditar = repositorioChamado.Editar(id, chamadoAtualizado);

        if (!conseguiuEditar)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }
    [HttpGet]
    public ActionResult Excluir(int id)
    {
        Chamado? chamadoSelecionado = repositorioChamado.SelecionarPorId(id);

        if (chamadoSelecionado == null)
            return NotFound();

        ExcluirChamadoViewModel viewModel = new(
            chamadoSelecionado.Id,
            chamadoSelecionado.Titulo
        );
        return View(viewModel);
    }
    [HttpPost]
    public ActionResult Excluir(ExcluirChamadoViewModel viewModel)
    {
        bool conseguiuEditar = repositorioChamado.Excluir(viewModel.Id);

        if (!conseguiuEditar)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }
    private List<SelecionarEquipamentoViewModel> ObterEquipamentosDisponiveis()
    {
        List<SelecionarEquipamentoViewModel> viewModels = new();

        foreach (Equipamento e in repositorioEquipamento.SelecionarTodos())
        {
            SelecionarEquipamentoViewModel viewModel = new(e.Id, e.Nome);

            viewModels.Add(viewModel);
        }
        return viewModels;
    }
}
