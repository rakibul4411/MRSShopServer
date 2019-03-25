using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MRSShop.AppIdentity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRSShop.AppIdentity.Data
{
    public class IdentityDataContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public string CurrentUserId { get; set; }
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options)
            : base(options) { }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<IdentityUserRole<Guid>>().HasKey(p => new { p.UserId, p.RoleId });
        //}
    }
}
