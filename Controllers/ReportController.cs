using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace ReportApi8.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class RelatorioController : ControllerBase
  {

    private string dataJson = """
    Json='{
        "autor":"João Junior",
        "alunos":[
            {
              "nome":"Jefferson",
              "email":"jefferson@ifro.edu.br"
            },
            {
              "nome":"Elias",
              "email":"elias@ifro.edu.br"
            },
            {
              "nome":"Reinaldo",
              "email":"reinaldo@ifro.edu.br"
            },
            {
              "nome":"João",
              "email":"joao@ifro.edu.br"
            }
        ]
      }';JsonSchema='{"type":"object","properties":{"autor":{"type":"string"},"alunos":{"type":"array","items":{"type":"object","properties":{"nome":{"type":"string"},"email":{"type":"string"}}}}}}';Encoding=utf-8;SimpleStructure=false
    """;

    [HttpGet]
    public IActionResult Get()
    {
      return Ok("ReportAPI: ON");
    }

    [HttpGet("pdf")]
    public IActionResult GerarRelatorioPdf()
    {
      var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "report01.frx");

      // Verifique se o arquivo de template existe
      if (!System.IO.File.Exists(reportPath))
      {
        return NotFound("Template de relatório não encontrado.");
      }

      // Carregar o template do relatório
      using (Report report = new Report())
      {
        try
        {
          // Carrega o arquivo .frx (template do relatório)
          report.Load(reportPath);

          // var jsonData = new JsonDataConnection();
          // jsonData.Json = dataJson;

          // jsonData.CreateAllTables();

          // // Registra o DataSource no relatório
          // report.RegisterData(jsonData.Tables, "DataConnection");

          JsonDataConnection connection = new JsonDataConnection();
          connection.ConnectionString = dataJson;
          connection.CreateAllTables();
          report.Dictionary.Connections.Add(connection);
          report.RegisterData(connection.DataSet);


          // Prepara o relatório para ser gerado
          report.Prepare();

          // Exporta para PDF em memória
          using (MemoryStream ms = new MemoryStream())
          {
            PDFSimpleExport pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, ms);
            ms.Position = 0;
            // ms.Flush();

            Response.Headers.Add("Content-Disposition", "inline; filename=Relatorio.pdf");

            // Retorna o arquivo PDF no response
            return File(ms.ToArray(), "application/pdf");
          }
        }
        catch (Exception ex)
        {
          // Retorna um erro caso algo dê errado
          return BadRequest(ex.ToString());
        }
      }
    }
  }
}
