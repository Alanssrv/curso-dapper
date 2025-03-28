﻿using Microsoft.Data.SqlClient;
using eCommerce.API.Models;
using eCommerce.API.Repository.Interfaces;
using System.Data;
using Dapper;

namespace eCommerce.API.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;

        public UsuarioRepository()
        {
            string? connectionString = Environment.GetEnvironmentVariable("eCommerceConnectionString");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Environment connection string not provide");

            _connection = new SqlConnection(connectionString);
        }

        public List<Usuario> Get()
        {
            string sqlCommand = "SELECT * FROM Usuarios as US LEFT JOIN Contatos CT ON US.id = CT.UsuarioId LEFT JOIN EnderecosEntrega EE ON US.id = EE.UsuarioId";

            List<Usuario> usuarios = [];

            _ = _connection.Query<Usuario, Contato, EnderecoEntrega, Usuario>(sqlCommand,
                (usuario, contato, enderecoEntrega) =>
                {
                    if (usuarios.SingleOrDefault(usr => usr.Id == usuario.Id) is null)
                    {
                        usuario.EnderecosEntrega = [];
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                    {
                        usuario = usuarios.SingleOrDefault(usr => usr.Id == usuario.Id);
                    }

                    usuario!.EnderecosEntrega!.Add(enderecoEntrega);
                    return usuario;
                }
            ).ToList();

            return usuarios;
        }

        public Usuario? GetById(int id)
        {
            string sqlCommand = "SELECT * FROM Usuarios as US LEFT JOIN Contatos CT ON US.id = CT.UsuarioId LEFT JOIN EnderecosEntrega EE ON US.id = EE.UsuarioId WHERE US.id = @Id";

            Usuario? refUsuario = null;

            _ = _connection.Query<Usuario, Contato, EnderecoEntrega , Usuario>(
                sqlCommand,
                (usuario, contato, enderecoEntrega) =>
                {
                    refUsuario ??= usuario;
                    refUsuario.Contato ??= contato;
                    refUsuario.EnderecosEntrega ??= [];
                    refUsuario.EnderecosEntrega.Add(enderecoEntrega);
                    return usuario;
                },
                new { Id = id }
            );

            return refUsuario;
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();

            var transaction = _connection.BeginTransaction();
            try
            {
                string sqlCommand = "INSERT INTO Usuarios(Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) VALUES (@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                usuario.Id = _connection.Query<int>(sqlCommand, usuario, transaction).Single();

                if (usuario.Contato != null)
                {
                    usuario.Contato.UsuarioId = usuario.Id;
                    sqlCommand = "INSERT INTO Contatos (UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                    usuario.Contato.Id = _connection.Query<int>(sqlCommand, usuario.Contato, transaction).Single();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Update(Usuario usuario)
        {
            _connection.Open();

            var transaction = _connection.BeginTransaction();
            try
            {
                string sqlCommand = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
                _connection.Execute(sqlCommand, usuario, transaction);

                if (usuario.Contato != null)
                {
                    sqlCommand = "UPDATE Contatos SET Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";
                    _connection.Execute(sqlCommand, usuario.Contato, transaction);
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            _connection.Execute("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
        }
    }
}
