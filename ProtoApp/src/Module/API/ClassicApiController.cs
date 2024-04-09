
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;


using Tlabs.Server.Model;
using ProtoApp.Module.Service;

namespace ProtoApp.Module.API {

  /// <summary>Classic REST API controller of the Module</summary>
  [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ClassicApiController))]
  [Route("api/v1/module/classic")]
  public class ClassicApiController(ModuleService moduleSvc) : Tlabs.Server.Controller.ApiCtrl {
    static ILogger log= Tlabs.App.Logger<ClassicApiController>();

    /// <summary>Get object(s) from (scoped) ModuleService</summary>
    [HttpGet("objects")]
    public QueryCover<Model.ModuleObj> GetServiceObjects([FromQuery] FilterParam<Model.ModuleObj>? param) => new QueryCover<Model.ModuleObj>(
      cover => moduleSvc.ObjectList(param?.AsQueryFilter()),
      e => e.Message
    );

  }
}