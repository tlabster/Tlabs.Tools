using Microsoft.EntityFrameworkCore;
using Tlabs.Config;

namespace Tlabs.Data.Store {  //required namespace to supporte generated migrations

  /// <summary>Proto app data EF model.</summary>
  public class ProtoDataEntityModel : SelfEnumConfigurator<IDStoreConfigModel>, IDStoreConfigModel {

    /// <inheritdoc/>
    public void ConfigureDb(DbContextOptionsBuilder optBuilder) {
      return; // do nothing
    }

    /// <inheritdoc/>
    public void ConfigureModel(ModelBuilder modBuilder) {
      //empty
    }
  }
}