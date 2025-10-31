using Microsoft.EntityFrameworkCore;
using SRMDataMigrationIgnite.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace SRMDataMigrationIgnite.Data
{
    public class ApplicationDbContext:DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public static Guid userId = new Guid("A128737D-6AC0-4338-A708-FC9393CA34F8");
        public static Guid projectId = new Guid("EAE364CD-618E-420A-A774-CFAC55F8DB8A");

        public DbSet<DMColumns> dmColumns { get; set; }
        public DbSet<DMExportViewEntities> dmExportViewEntities { get; set; }
        public DbSet<DMExportViewEntityColumns> dmExportViewEntityColumns { get; set; }
    }
}
