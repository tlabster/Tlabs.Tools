#pragma warning disable CS1591


using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ProtoApp.Module.Model {

  public class ApiDesc {
    public string? GroupName { get; set; }
    public string? RelPath { get; set; }
    public string? Method { get; set; }
    public string? DisplayName { get; set; }
    public IEnumerable<ApiParam> Parameter { get; set; }= Enumerable.Empty<ApiParam>();
  }

  public class ApiParam {
    public ApiParam() { }
    public ApiParam(ParameterDescriptor pd) {
      this.Name= pd.Name;
      this.Type= pd.ParameterType.Name;
      this.Source= pd.BindingInfo?.BindingSource?.DisplayName;
    }
    public ApiParam(ApiParameterDescription pd) {
      this.Name= pd.Name;
      this.Type= pd.Type.Name;
      this.Source= pd.Source?.DisplayName;
    }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Source { get; set; }
  }
}