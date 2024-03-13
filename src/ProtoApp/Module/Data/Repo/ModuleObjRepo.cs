using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

using Tlabs.Data;
using Tlabs.Data.Model;
using Tlabs.Data.Repo.Intern;

using ProtoApp.Module.Model;

namespace ProtoApp.Module.Data.Repo {
  ///<summary>Repo of <see cref="ModuleObj"/></summary>
  public class ModuleObjRepo : BaseRepo<Tlabs.Data.Entity.User> {

    ///<summary>Ctor from <paramref name="store"/></summary>
    public ModuleObjRepo(IDataStore store) : base(store) { }

    ///<summary>Query with applied <paramref name="filter"/>.</summary>
    public IQueryable<Tlabs.Data.Entity.User> FilteredQuery(QueryFilter filter) => filter.Apply(AllUntracked.OrderBy(u => u.Id), filterMap);


    static Dictionary<string, QueryFilter.FilterExpression<Tlabs.Data.Entity.User>> filterMap= new (StringComparer.OrdinalIgnoreCase) {
      [nameof(User.Username)]= (q, cv) => q.Where(e =>    null != e.UserName
                                                       && !string.IsNullOrEmpty(cv.ToString())
                                                       && e.UserName.Contains(cv.ToString()!)),
    };

  }
}