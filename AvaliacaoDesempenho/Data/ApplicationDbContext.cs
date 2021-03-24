using System;
using System.Collections.Generic;
using System.Text;
using AvaliacaoDesempenho.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AvaliacaoDesempenho.Data
{
    public class ApplicationDbContext
     : IdentityDbContext<
         Usuario, AppRole, string,
         IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>,
         IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<AppRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
        }

        public DbSet<Curso> Curso { get; set; }
        public DbSet<Questionario> Questionario { get; set; }
        public DbSet<Turma> Turma { get; set; }
        public DbSet<Professor> Professor { get; set; }
        public DbSet<Disci_Turma> Disci_Tuma { get; set; }
        public DbSet<Disciplina> Disciplina { get; set; }
        public DbSet<Matricula> Matricula { get; set; }
        public DbSet<Avaliacao> Avaliacao { get; set; }
        public DbSet<Questao_Avaliacao> Questao_Avaliacao { get; set; }        

    }
}
