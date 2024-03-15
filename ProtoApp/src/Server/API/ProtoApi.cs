

namespace ProtoApp.Server.API {

  /// <summary>Proto API</summary>
  public static class ProtoApi {

    /// <summary>End-point action method</summary>
    public static string ActionMethod(int id) {
      return id.ToString();
    }
  }
}