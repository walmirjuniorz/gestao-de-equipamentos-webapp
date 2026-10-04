using GestaoDeEquipamentos.WebApp.Modulos.Fabricantes.Dominio;
using GestaoDeEquipamentos.WebApp.Modulos.Fabricantes.Infraestrutura;
using GestaoDeEquipamentos.WebApp.Modulos.Equipamentos.Dominio;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeEquipamentos.WebApp.Modulos.Fabricantes.Apresentacao;

public sealed class FabricanteController : Controller
{
    private readonly IRepositorioFabricante repositorio;
    private readonly IRepositorioEquipamento repositorioEquipamento;

    public FabricanteController(IRepositorioFabricante repositorio, IRepositorioEquipamento repositorioEquipamento)
    {
        this.repositorio = repositorio;
        this.repositorioEquipamento = repositorioEquipamento;
    }
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarFabricanteViewModel> viewModels = new List<ListarFabricanteViewModel>();

        foreach (Fabricante fabricante in repositorio.SelecionarTodos())
        {
            viewModels.Add(new ListarFabricanteViewModel(
                fabricante.Id,
                fabricante.Nome,
                fabricante.Email,
                fabricante.Telefone
            ));
        }
        return View(viewModels);
    }
    [HttpGet]
    public ActionResult Cadastrar()
    {
        return View();
    }
    [HttpPost]
    public ActionResult Cadastrar(CadastrarFabricanteViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        Fabricante fabricante = new Fabricante(
            cadastrarVm.Nome ?? string.Empty,
            cadastrarVm.Email ?? string.Empty,
            cadastrarVm.Telefone ?? string.Empty
        );

        repositorio.Cadastrar(fabricante);

        return RedirectToAction(nameof(Listar));
    }
    [HttpGet]
    public ActionResult Editar(int id)
    {
        Fabricante? fabricanteSelecionado = repositorio.SelecionarPorId(id);

        if (fabricanteSelecionado == null)
            return NotFound();

        EditarFabricanteViewModel viewModel = new EditarFabricanteViewModel(
            fabricanteSelecionado.Id,
            fabricanteSelecionado.Nome,
            fabricanteSelecionado.Email,
            fabricanteSelecionado.Telefone
        );

        return View(viewModel);
    }
    [HttpPost]
    public ActionResult Editar(EditarFabricanteViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        Fabricante fabricanteAtualizado = new Fabricante(
            editarVm.Nome ?? string.Empty,
            editarVm.Email ?? string.Empty,
            editarVm.Telefone ?? string.Empty
        );

        bool conseguiuEditar = repositorio.Editar(editarVm.Id, fabricanteAtualizado);

        if (!conseguiuEditar)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(int id)
    {
        Fabricante? fabricanteSelecionado = repositorio.SelecionarPorId(id);

        if (fabricanteSelecionado == null)
            return NotFound();

        return View(new ExcluirFabricanteViewModel(
            fabricanteSelecionado.Id,
            fabricanteSelecionado.Nome
        ));
    }
    [HttpPost]
    public ActionResult Excluir(ExcluirFabricanteViewModel excluirVm)
    {
        Fabricante? fabricante = repositorio.SelecionarPorId(excluirVm.Id);

        if (fabricante == null)
            return NotFound();

        if (repositorioEquipamento.ExisteParaFabricante(excluirVm.Id))
        {
            ModelState.AddModelError(string.Empty,
                "Não é possível excluir este fabricante enquanto houver equipamentos vinculados a ele.");

            return View(new ExcluirFabricanteViewModel(fabricante.Id, fabricante.Nome));
        }

        bool conseguiuExcluir = repositorio.Excluir(excluirVm.Id);

        if (!conseguiuExcluir)
            return NotFound();

        return RedirectToAction(nameof(Listar));
    }
}
