using Microsoft.Data.SqlClient;
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
            string sqlCommand = "SELECT US.*, CT.*, EE.*, DP.* FROM Usuarios as US LEFT JOIN Contatos CT ON US.id = CT.UsuarioId LEFT JOIN EnderecosEntrega EE ON US.id = EE.UsuarioId LEFT JOIN UsuariosDepartamentos UD ON UD.UsuarioId = US.Id LEFT JOIN Departamentos DP on UD.DepartamentoId = DP.Id";

            List<Usuario> usuarios = [];

            _ = _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(sqlCommand,
                (usuario, contato, enderecoEntrega, departamento) =>
                {
                    if (usuarios.SingleOrDefault(usr => usr.Id == usuario.Id) is null)
                    {
                        usuario.Departamentos = [];
                        usuario.EnderecosEntrega = [];
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                    {
                        usuario = usuarios.SingleOrDefault(usr => usr.Id == usuario.Id);
                    }

                    if (usuario!.EnderecosEntrega!.SingleOrDefault(endEntrega => endEntrega.Id == enderecoEntrega.Id) is null && enderecoEntrega is not null)
                        usuario!.EnderecosEntrega!.Add(enderecoEntrega);

                    if (usuario!.Departamentos!.SingleOrDefault(depart => depart.Id == departamento.Id) is null && departamento is not null)
                        usuario!.Departamentos!.Add(departamento);

                    return usuario;
                }
            ).ToList();

            return usuarios;
        }

        public Usuario? GetById(int id)
        {
            string sqlCommand = "SELECT US.*, CT.*, EE.*, DP.* FROM Usuarios as US LEFT JOIN Contatos CT ON US.id = CT.UsuarioId LEFT JOIN EnderecosEntrega EE ON US.id = EE.UsuarioId LEFT JOIN UsuariosDepartamentos UD ON UD.UsuarioId = US.Id LEFT JOIN Departamentos DP on UD.DepartamentoId = DP.Id WHERE US.Id = @Id";

            Usuario? refUsuario = null;

            _ = _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(
                sqlCommand,
                (usuario, contato, enderecoEntrega, departamento) =>
                {
                    refUsuario ??= usuario;
                    refUsuario.Contato ??= contato;
                    refUsuario.EnderecosEntrega ??= [];
                    if (refUsuario.EnderecosEntrega.SingleOrDefault(end => end.Id == enderecoEntrega.Id) is null && enderecoEntrega is not null)
                        refUsuario.EnderecosEntrega.Add(enderecoEntrega);

                    refUsuario.Departamentos ??= [];
                    if (refUsuario.Departamentos.SingleOrDefault(dep => dep.Id == departamento.Id) is null && departamento is not null)
                        refUsuario.Departamentos.Add(departamento);

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

                if (usuario.EnderecosEntrega != null)
                {
                    foreach (var enderecoEntrega in usuario.EnderecosEntrega)
                    {
                        enderecoEntrega.UsuarioId = usuario.Id;
                        sqlCommand = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                        enderecoEntrega.Id = _connection.Query<int>(sqlCommand, enderecoEntrega, transaction).Single();
                    }
                }

                if (usuario.Departamentos != null)
                {
                    foreach (var departamento in usuario.Departamentos)
                    {
                        sqlCommand = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId)";
                        _connection.Execute(sqlCommand, new { UsuarioId = usuario.Id, DepartamentoId = departamento.Id }, transaction);
                    }
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

                sqlCommand = "DELETE FROM EnderecosEntrega WHERE UsuarioId = @Id";
                _connection.Execute(sqlCommand, usuario, transaction);

                if (usuario.EnderecosEntrega != null)
                {
                    foreach (var enderecoEntrega in usuario.EnderecosEntrega)
                    {
                        enderecoEntrega.UsuarioId = usuario.Id;
                        sqlCommand = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                        enderecoEntrega.Id = _connection.Query<int>(sqlCommand, enderecoEntrega, transaction).Single();
                    }
                }

                sqlCommand = "DELETE FROM UsuariosDepartamentos WHERE UsuarioId = @Id";
                _connection.Execute(sqlCommand, usuario, transaction);
                if (usuario.Departamentos != null)
                {
                    foreach (var departamento in usuario.Departamentos)
                    {
                        sqlCommand = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId)";
                        _connection.Execute(sqlCommand, new { UsuarioId = usuario.Id, DepartamentoId = departamento.Id }, transaction);
                    }
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
