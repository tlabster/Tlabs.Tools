
using System.Linq;
using System.Collections.Generic;

using Tlabs.Data;
using Tlabs.Data.Entity;

namespace ProtoApp.Module.Service {

  ///<summary>A module service</summary>
  ///<param name="userRepo">Data store repo for users as dependency.</param>
  public class ModuleService(IRepo<User> userRepo) {

    ///<summary>ModuleObj(s) from data store</summary>
    public List<Model.ModuleObj> ObjectList()
      => userRepo.AllUntracked.Select(u => new Model.ModuleObj {
        IntProperty= u.Id,
        StrProperty= $"{u.Lastname}, {u.Firstname}"
      }).ToList();
  }
}