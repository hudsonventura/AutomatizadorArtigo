//using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;
using Npgsql;

public class Singleton {
    private static Singleton instance;
    private static NpgsqlConnection conexao;
    private Singleton() { }

    public static Singleton Instance {
        get {
            if (instance == null)
                lock (typeof(Singleton))
                    if (instance == null) instance = new Singleton();

            return instance;
        }
    }

    public static DataTable query(string query, string stringConexao) {
        
        if (conexao == null || conexao.State == ConnectionState.Closed)
        {
            conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
        }

        //ArrayList retorno = new ArrayList();

        if (conexao == null) {
            lock (typeof(NpgsqlFactory))
            {
                ;
            }
        }


        NpgsqlCommand comando = new NpgsqlCommand();
        comando.Connection = conexao;
        comando.CommandText = query;
        
        comando.Prepare();

        NpgsqlDataReader read = comando.ExecuteReader();
        DataTable schemaTable = new DataTable();

        try
        {
            
            schemaTable.Load(read);

            
        }
        catch (Exception error) {
            Console.WriteLine(error);
        }
        read.Close();
        read.Dispose();
        //conexao.Close();
        //conexao.Dispose();
        return schemaTable;

    }

    public static int execute(string query, string stringConexao) {
        ArrayList retorno = new ArrayList();
        NpgsqlConnection conexao = new NpgsqlConnection(stringConexao);

        if (conexao == null) {
            lock (typeof(Singleton)) ;
        }


        NpgsqlCommand comando = new NpgsqlCommand();
        comando.Connection = conexao;
        comando.CommandText = query;
        if (conexao.State == ConnectionState.Closed) {
            try {
                conexao.Open();
            } catch (Exception) {

            }
        }
        try {
            comando.Prepare();
        } catch (Exception) {
            
        }

        int rows = 0;
        try {
            rows = comando.ExecuteNonQuery();
        } catch (Exception erro) {
            Console.WriteLine("------------------Erro ao executar query---------------------------");
			Console.WriteLine(query);
			Console.WriteLine(erro);
			Console.WriteLine("------------------Erro ao executar query---------------------------");
        }

        //read.Dispose();
        try {
            conexao.Close();
            conexao.Dispose();
        } catch (Exception) {

        }

        return rows;
    }



}
