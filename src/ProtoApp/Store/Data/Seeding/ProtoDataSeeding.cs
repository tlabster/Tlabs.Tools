using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Tlabs;
using Tlabs.Misc;
using Tlabs.Data;
using Tlabs.Data.Entity;

namespace ProtoApp.Data.Store.Seeding {
  /// <summary>ProtoApp data seeding (a.k.a. one time database initalization)</summary>
  public class ProtoDataSeeding : AbstractDataSeed {
    static readonly ILogger<ProtoDataSeeding> log= App.Logger<ProtoDataSeeding>();

    UserManager<User> userManager;

    RoleManager<Role> rolesManager;

    ///<summary>Ctor from <paramref name="store"/> and <paramref name="userManager"/>.</summary>
    public ProtoDataSeeding(IDataStore store, UserManager<User> userManager, RoleManager<Role> rolesManager) : base(store) {
      this.userManager= userManager;
      this.rolesManager= rolesManager;
    }

    /// <inheritoc/>
    public override string Campaign => "ProtoApp data";

    /// <inheritoc/>
    public override void Perform() {
      defaultUsers();
      store.CommitChanges();
    }

    void defaultUsers() {
      log.LogWarning("Seeding default {type}(s).", nameof(Role));
      SeedRoles();

      if (!store.Query<User>().Any()) {
        log.LogWarning("Seeding default {type}(s).", nameof(User));

        createUser("j.riedel",    "001", "", "", "j.riedel@mailinator.com", "PROTO_USER");
        createUser("p.lindlau",   "002", "", "", "p.lindlau@mailinator.com", "PROTO_USER");
        createUser("d.shepilov",  "003", "", "", "d.shepilov@mailinator.com", "PROTO_USER", "DEVELOPER");
        createUser("l.deutsch",   "004", "", "", "l.deutsch@mailinator.com", "PROTO_USER", "DEVELOPER");
        createUser("c.jaeckel",   "005", "", "", "c.jaeckel@mailinator.com", "PROTO_USER", "DEVELOPER", "ADMINISTRATOR");
        createUser("m.valero",    "006", "", "", "m.valero@mailinator.com", "PROTO_USER", "DEVELOPER");
        createUser("m.wiegand",   "007", "", "", "m.wiegand@mailinator.com", "PROTO_USER", "DEVELOPER");
        createUser("l.lengemann", "008", "", "", "l.lengemann@mailinator.com", "PROTO_USER", "DEVELOPER");
        createUser("p.oltmanns",  "009", "", "", "p.oltmanns@mailinator.com", "PROTO_USER");

      }

      if(store.Query<User>().Any(u => u.UserName == "operator")){
        store.Delete<User>(store.Query<User>().Single(u => u.UserName == "operator"));
        store.Delete<Role>(store.Query<Role>().Single(r => r.Name == "OPERATORS"));
        store.CommitChanges();
      }

    }

    private void createUser(string usr, string pwd, string firstName, string lastName, string email, params string[] roles) {
      var user= new User { UserName= usr, Firstname= firstName, Lastname= lastName, Email= email };
      var res= userManager.CreateAsync(user, pwd).GetAwaiter().GetResult();
      if (!res.Succeeded) {
        log.LogWarning("Faild to seed user: {user} - ({err})", user, string.Join("; ", res.Errors.Select(err => $"[{err.Code}] - {err.Description}")));
        return;
      }
      foreach (var roleName in roles) {
        log.LogInformation("Adding {roleName} to {user}", roleName, user.UserName);
        res= userManager.AddToRoleAsync(user, roleName).GetAwaiter().GetResult();
        if (!res.Succeeded) {
          log.LogWarning("Faild to add role {roleName} to user: {user} - ({err})", roleName, user, string.Join("; ", res.Errors.Select(err => $"[{err.Code}] - {err.Description}")));
          return;
        }
      }
    }

    private void SeedRoles() {
      insertOrIgnore(new Role {
        Name= "PROTO_USER",
        Description= "Default user role",
        AllowAccessPattern= @"[A-Z]+:.*"
      });

      insertOrIgnore(new Role {
        Name= "DEVELOPER",
        Description= "Developer user role",
        AllowAccessPattern= @"[A-Z]+:.*"
      });

      insertOrIgnore(new Role {
        Name= "ADMINISTRATOR",
        Description= "Admin role",
        AllowAccessPattern= @"[A-Z]+:.*"
      });

    }

    private void insertOrIgnore(Role role) {
      if (null == rolesManager.FindByNameAsync(role.Name!).AwaitWithTimeout(500))
        rolesManager.CreateAsync(role).AwaitWithTimeout(500);
    }
  }

}