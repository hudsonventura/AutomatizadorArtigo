using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Aumatizador
{
    class Banco
    {
        //string dbStringConnection = "Server=3.19.113.193; Port=5432; User Id=postgres; Password=052300; Database=test;";
        string bancoImportacao;
        string bancoAlternativo;
        string bancoEstatistica = "Server=hudsonventura.no-ip.org; Port=54320; User Id=postgres; Password=#Timetecnica001; Database=artigo;";
        string vm;
        string ipPrincipal = "192.168.18.87";
        string ipAlternativo = "192.168.18.88";
        int iteracoes = 10;

        public Banco(string vmAtual, int itera) {
            vm = vmAtual;
            iteracoes = itera;
            Console.WriteLine("Conectando na VM "+vm);

            switch (vm)
            {       
                case "Debian1": ipPrincipal = "192.168.18.87";
                    ipAlternativo = "192.168.18.88";
                    break;

                case "Debian2": ipPrincipal = "192.168.18.88";
                    ipAlternativo = "192.168.18.87";
                    break;

               case "Debian3":
                    ipPrincipal = "192.168.18.89";
                    ipAlternativo = "192.168.18.88";
                    break;

            }

            bancoImportacao = "Server="+ ipPrincipal + "; Port=5432; User Id=postgres; Password=052300; Database=artigo;Pooling=false;Timeout=1024;CommandTimeout=1024;";
            bancoAlternativo = "Server="+ ipAlternativo + "; Port=5432; User Id=postgres; Password=052300; Database=artigo;Pooling=false;Timeout=1024;CommandTimeout=1024;";

        }
        private void registraMomentoEstatistica(int iteracao, string momento, string arquivo, string tipo, string instalacao)
        {
            NpgsqlConnection conexaoEstatistica = new NpgsqlConnection(bancoEstatistica);
            conexaoEstatistica.Open();

            string sqlEstatistica;

            //caso de fim
            sqlEstatistica = "UPDATE amostras SET fim = now() WHERE tipo = '" + tipo + "' and arquivo = '" + arquivo + "' AND iteracao = " + iteracao.ToString();

            if (momento == "inicio")
            {
                //caso inicio
                sqlEstatistica = "INSERT INTO amostras (arquivo, iteracao, tipo, instalacao) VALUES ('" + arquivo + "', " + iteracao.ToString() + ", '" + tipo + "', '" + instalacao + "')";
            }


            using (NpgsqlCommand pgsqlcommand = new NpgsqlCommand(sqlEstatistica, conexaoEstatistica))
            {
                pgsqlcommand.ExecuteNonQuery();
            }
            conexaoEstatistica.Close();
        }


        public void importarSQL(string arquivo, string instalacao) {
            string localArquivo = @"C:\log\" + arquivo;
            
            

            NpgsqlConnection conexaoBanco = new NpgsqlConnection(bancoImportacao);
            conexaoBanco.Open();

            

            Console.WriteLine("Arquivo: " + arquivo);

            
            for (int i = 1; i <= iteracoes; i++)
            {
                var timeStampInicio = DateTime.Now;
                registraMomentoEstatistica(i, "inicio", arquivo, "INSERT", instalacao);
                Console.Write("Iteração " + i);
                insert(localArquivo, arquivo, i, conexaoBanco, instalacao);
                registraMomentoEstatistica(i, "fim", arquivo, "INSERT", instalacao);
                var timeStampFim = DateTime.Now;

                Console.WriteLine(" -> " + timeStampFim.Subtract(timeStampInicio));

                /*
                using (FileStream stream = File.Open(localArquivo, FileMode.Open))
                {
                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    string linha = "";
                    string LinhaResto = "";

                    
                    registraMomentoEstatistica(i, "inicio", arquivo, "INSERT", instalacao);
                    Console.Write("Iteração " + i);



                    /*
                     * contadorLinha = 0;
                    while (stream.Read(b, 0, b.Length) > 0)
                    {
                        string linhaTemp = temp.GetString(b);

                        if (linhaTemp.Contains(linhaDelimitador))
                        {
                            linha = LinhaResto + linhaTemp.Substring(0, linhaTemp.IndexOf(linhaDelimitador));
                            if (linhaTemp.Length - linhaTemp.IndexOf(linhaDelimitador) > 0)
                                LinhaResto = linhaTemp.Substring(linhaTemp.IndexOf(linhaDelimitador), linhaTemp.Length - linhaTemp.IndexOf(linhaDelimitador));
                            else
                                LinhaResto = string.Empty;
                        }
                        else
                            LinhaResto += linhaTemp;


                        
                        if (linha != "")
                        {
                            contadorLinha++;
                            //Console.Write(contadorLinha.ToString());

                            using (NpgsqlCommand pgsqlcommand = new NpgsqlCommand(linha, conexaoBanco))
                            {
                                try
                                {
                                    pgsqlcommand.ExecuteNonQuery();
                                }
                                catch (Exception error) {
                                    Console.WriteLine(error.Message);
                                }
                                
                            }


                        }
                    }
                    

                }

                
                registraMomentoEstatistica(i, "fim", arquivo, "INSERT", instalacao);
                var timeStampFim = DateTime.Now;

                Console.WriteLine(" -> " + timeStampFim.Subtract(timeStampInicio));
                */
            }
            

            conexaoBanco.Close();
            System.Threading.Thread.Sleep(1000);
        }




        public void select(string arquivo, string instalacao, string query = null)
        {

            NpgsqlConnection conexaoBanco = new NpgsqlConnection(bancoImportacao);
            


            for (int i = 1; i <= iteracoes; i++)
            {
                Console.WriteLine("Arquivo " + arquivo + " - Iteração " + i);

                conexaoBanco.Open();
                var timeStampInicio = DateTime.Now;
                registraMomentoEstatistica(i, "inicio", arquivo, "SELECT", instalacao);
                selectUnico(bancoImportacao, query);
                registraMomentoEstatistica(i, "fim", arquivo, "SELECT", instalacao);
                var timeStampFim = DateTime.Now;

                Console.WriteLine("Duração desta iteração: ------------> "+ timeStampFim.Subtract(timeStampInicio));
                Console.WriteLine();
                conexaoBanco.Close();
            }
            
            System.Threading.Thread.Sleep(1000);
        }


        

        public int selectUnico(string conexaoBancoString, string query)
        {
            NpgsqlConnection conexaoBanco = new NpgsqlConnection(conexaoBancoString);
            
            
            using (NpgsqlCommand pgsqlcommand = new NpgsqlCommand(query, conexaoBanco))
            {
                try
                {
                    try
                    {
                        conexaoBanco.Open();
                    }
                    catch (Exception error1)
                    {
                        Console.WriteLine(error1.Message);
                    }
                    NpgsqlDataReader rd = pgsqlcommand.ExecuteReader();
                    while (rd.Read())
                    {
                        int j;
                        //Console.WriteLine(rd.FieldCount.ToString());
                        for (j = 0; j < rd.FieldCount; j++)
                        {
                            //Console.Write("Count: {0} \t", rd[j]);
                            int retorno = int.Parse(rd[j].ToString());
                            conexaoBanco.Close();
                            return retorno;
                        }
                            
                    }
                }
                catch (Exception error2)
                {
                    Console.WriteLine(error2.Message);
                }
                    
            }
            conexaoBanco.Close();
            return 0;
        }




        public void update(string query, string arquivo, string instalacao)
        {

            NpgsqlConnection conexaoBanco = new NpgsqlConnection(bancoImportacao);
            conexaoBanco.Open();


            for (int i = 1; i <= iteracoes; i++)
            {
                Console.WriteLine("Arquivo " + arquivo + " - Iteração " + i);

                registraMomentoEstatistica(i, "inicio", arquivo, "UPDATE", instalacao);
                using (NpgsqlCommand pgsqlcommand = new NpgsqlCommand(query, conexaoBanco))
                {
                    try
                    {
                        pgsqlcommand.ExecuteNonQuery();
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.Message);
                    }
                }
                registraMomentoEstatistica(i, "fim", arquivo, "UPDATE", instalacao);
            }
            conexaoBanco.Close();
            System.Threading.Thread.Sleep(1000);
        }



        private void insert(string localArquivo, string arquivo, int i, NpgsqlConnection conexaoBanco, string instalacao) {

            int contadorLinha = 0;
            string linhaDelimitador = System.Environment.NewLine;


            using (FileStream stream = File.Open(localArquivo, FileMode.Open))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                string linha = "";
                string LinhaResto = "";

                try
                {
                    conexaoBanco.Open();
                }
                catch (Exception)
                {
                }


                contadorLinha = 0;

                while (stream.Read(b, 0, b.Length) > 0)
                {
                    string linhaTemp = temp.GetString(b);

                    if (linhaTemp.Contains(linhaDelimitador))
                    {
                        linha = LinhaResto + linhaTemp.Substring(0, linhaTemp.IndexOf(linhaDelimitador));
                        if (linhaTemp.Length - linhaTemp.IndexOf(linhaDelimitador) > 0)
                            LinhaResto = linhaTemp.Substring(linhaTemp.IndexOf(linhaDelimitador), linhaTemp.Length - linhaTemp.IndexOf(linhaDelimitador));
                        else
                            LinhaResto = string.Empty;
                    }
                    else
                        LinhaResto += linhaTemp;



                    if (linha != "")
                    {
                        contadorLinha++;
                        //Console.Write(contadorLinha.ToString());

                        using (NpgsqlCommand pgsqlcommand = new NpgsqlCommand(linha, conexaoBanco))
                        {
                            try
                            {
                                pgsqlcommand.ExecuteNonQuery();
                            }
                            catch (Exception error) {
                                Console.WriteLine(error.Message);
                            }

                        }


                    }
                }


            }

        }





        public void importarSQLComRecuperacao(string arquivo, string instalacao, string vmSecundaria)
        {
            string localArquivo = @"C:\log\" + arquivo;

            string retornoConexao;
            string retornoDesconexao;


            NpgsqlConnection conexaoBanco = new NpgsqlConnection(bancoImportacao);
            NpgsqlConnection conexaoAlternativa = new NpgsqlConnection(bancoAlternativo);

            conexaoBanco.Open();
            conexaoAlternativa.Open();

            for (int i = 1; i <= iteracoes; i++)
            {
                

                var timeStampInicio = DateTime.Now;

                Console.WriteLine("--> Iniciando o processo. ITERAÇÃO "+i.ToString()+"...");
                System.Threading.Thread.Sleep(500);

                //limpa as tabelas
                Console.Write("Limpando os registros...");
                
                registraMomentoEstatistica(i, "inicio", arquivo, "DELETE", instalacao);
                limparTabela(conexaoBanco, "TRUNCATE TABLE \"filmes\".\"name.basics.tsv\";");
                registraMomentoEstatistica(i, "fim", arquivo, "DELETE", instalacao);
                Console.WriteLine("OK!");




                //desconecta o cabo da maquina 2
                Console.Write("Desconectando cabo de rede ...");
                retornoDesconexao = desconectarMaquina(vmSecundaria);
                Console.WriteLine("OK!" + retornoDesconexao);




                //começa a inserção de dados
                Console.Write("Iniciando inserts...");
                registraMomentoEstatistica(i, "inicio", arquivo, "DEFEITO-INSERT", instalacao);
                insert(localArquivo, arquivo, i, conexaoBanco, instalacao);
                registraMomentoEstatistica(i, "fim", arquivo, "DEFEITO-INSERT", instalacao);
                Console.WriteLine("OK!");



                //aguarda
                Console.Write("Dando um tempo...");
                System.Threading.Thread.Sleep(10000);
                Console.WriteLine("OK!");



                //reconecta o cabo de rede
                Console.Write("Reconectando cabo de rede...");
                retornoConexao = conectarMaquina(vmSecundaria);
                Console.WriteLine("OK! " + retornoConexao);

                

                //aguarda a sincronização
                int resultadoSelectAtual = 0;
                int resultadoSelectUltimo = -1;
                //inicia a contagem de tempo da recuperação online
                registraMomentoEstatistica(i, "inicio", arquivo, "DEFEITO-RECUPERACAO", instalacao);
                Console.Write("Aguardando sincronia ");
                    while (resultadoSelectAtual != resultadoSelectUltimo || resultadoSelectAtual == 0)
                    {
                        resultadoSelectUltimo = resultadoSelectAtual;
                        resultadoSelectAtual = selectUnico(bancoAlternativo, "SELECT count(*) FROM filmes.\"name.basics.tsv\" AS name");
                        Console.Write(".");
                        System.Threading.Thread.Sleep(500);
                    }
                    Console.WriteLine(resultadoSelectUltimo.ToString() + " registros retornados!");
                //registra o fim da sincronização da recuperação online
                registraMomentoEstatistica(i, "fim", arquivo, "DEFEITO-RECUPERACAO", instalacao);

                Console.Write("--> FIM. ");
                var timeStampFim = DateTime.Now;
                Console.WriteLine("Duração: ------------> " + timeStampFim.Subtract(timeStampInicio));
                Console.WriteLine();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
            }
            conexaoBanco.Close();
            conexaoAlternativa.Close();
        }





        private void limparTabela(NpgsqlConnection conexaoBanco, string query)
        {
            try
            {
                conexaoBanco.Open();
            }
            catch (Exception)
            {
            }
            

            using (NpgsqlCommand pgsqlcommand = new NpgsqlCommand(query, conexaoBanco))
            {
                pgsqlcommand.ExecuteNonQuery();
            }

            conexaoBanco.Close();
        }






        private string conectarMaquina(string maquina)
        {
            //enviar comando para conectar a maquina
           return ExecutarCMD("powershell Connect-VMNetworkAdapter -VMName " + maquina + " -SwitchName RedeLocal");
        }

        private string desconectarMaquina(string maquina)
        {
            //enviar comando para desconectar a maquina
            return ExecutarCMD("powershell Disconnect-VMNetworkAdapter -VMName " + maquina);
        }

        public static string ExecutarCMD(string comando)
        {
            using (Process processo = new Process())
            {
                processo.StartInfo.FileName = Environment.GetEnvironmentVariable("comspec");

                // Formata a string para passar como argumento para o cmd.exe
                processo.StartInfo.Arguments = string.Format("/c {0}", comando);

                processo.StartInfo.RedirectStandardOutput = true;
                processo.StartInfo.UseShellExecute = false;
                processo.StartInfo.CreateNoWindow = true;

                processo.Start();
                processo.WaitForExit();

                string saida = processo.StandardOutput.ReadToEnd();
                return saida;
            }
        }
    }
}
