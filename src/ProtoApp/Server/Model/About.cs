#pragma warning disable 1591

namespace ProtoApp.Server.Model {

  public record class About(
    string Version= "0",
    string BuildDate= "0",
    double DbPing= 0
  );

}