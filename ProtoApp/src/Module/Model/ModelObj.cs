#pragma warning disable CS1591


using System.Collections.Generic;

namespace ProtoApp.Module.Model {

  public class ModuleObj {
    public string? StrProperty { get; set; }
    public int IntProperty { get; set; }
    public List<string>? ListProperty { get; set; }
  }
}