
using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Aumatizador
{
    class Program
    {
        private static int iteracoesMaximo = 100;

        static void Main(string[] args) {
            string diretorio = $@"{Directory.GetCurrentDirectory()}\Importar\";
            List<IMDB> dados = IMDB.importarArquivo(diretorio, 100000); //99000

            List<string> cenariosPossiveis = new List<string>();
            string cenarioUnico = "";

            //seleciona o tipo de instalação
            Console.WriteLine("Informe a instalação a ser operada:");
            Console.WriteLine("1 -> PosgreSQL");
            Console.WriteLine("2 -> Posgresql-DBR - 1 nó");
            Console.WriteLine("3 -> Posgresql-DBR - 2 nó");
            Console.WriteLine("4 -> Posgresql-DBR - 2 nó - sendo 1 com defeito permanente");
            Console.WriteLine("5 -> Posgresql-DBR - 2 nó - sendo 1 com defeito intermitente");
            string opcao1 = Console.ReadLine();
            switch (opcao1)
            {
                case "1": cenarioUnico = "PosgreSQL"; break;
                case "2": cenarioUnico = "Posgresql-DBR - 1 nó"; break;
                case "3": cenarioUnico = "Posgresql-DBR - 2 nó"; break;
                case "4": cenarioUnico = "Posgresql-DBR - 2 nó - sendo 1 com defeito permanente"; break;
                case "5": cenarioUnico = "Posgresql-DBR - 2 nó - sendo 1 com defeito intermitente"; break;
            }
            cenariosPossiveis.Add(cenarioUnico);


            //cenariosPossiveis.Add("PosgreSQL");
            //cenariosPossiveis.Add("Posgresql-DBR - 1 nó");
            //cenariosPossiveis.Add("Posgresql-DBR - 2 nó");
            //cenariosPossiveis.Add("Posgresql-DBR - 2 nó - sendo 1 com defeito permanente");
            //cenariosPossiveis.Add("Posgresql-DBR - 2 nó - sendo 1 com defeito intermitente");


            foreach (var cenario in cenariosPossiveis)
            {
                realizaTestes(dados, cenario);
            }


        }


        private static void realizaTestes(List<IMDB> dados, string cenario) {

            

            for (int i = 0; i < iteracoesMaximo; i++)
            {
                //cada entity framework é iniciado uma vez por iteração para evitar o uso de cache



                // inicia o contexto do entity framework para salvar as estatisicas em banco
                BancoEstatistica estatisticas = new BancoEstatistica();

                //inicia o contexto do entity dos nós
                BancoMaquina1 maquina1 = new BancoMaquina1();
                BancoMaquina2 maquina2 = new BancoMaquina2();

                Console.Write($"{cenario} -> ");
                Console.Write($"Iteração {i}: ");


                desligaMaquina2(cenario);


                //insert
                Console.Write("Inserindo ...");
                DateTime tempoINSERTInicial = DateTime.Now;
                {
                    maquina1.imdb.AddRange(dados);
                    maquina1.SaveChanges();
                }
                DateTime tempoINSERTFinal = DateTime.Now;
                Estatisica insert = new Estatisica { horarioInicio = tempoINSERTInicial, horarioFim = tempoINSERTFinal, iteracao = i, setup = cenario, tipo = "INSERT" };
                estatisticas.Add(insert);
                estatisticas.SaveChanges();
                Console.Write("Inseridos! - ");




                //replicação - aguarda até que no nó 2 tenha a mesma quantidade de tuplas inseridas no nó 1
                if (cenario == "Posgresql-DBR - 2 nó" || cenario == "Posgresql-DBR - 2 nó - sendo 1 com defeito intermitente")
                {
                    Console.Write("Replicando ...");
                    ligaMaquina2(cenario);
                    DateTime tempoREPLICACAOInicial = DateTime.Now;
                    int totalTuplasMaquina1 = dados.Count;
                    int quantTuplasMaquina2 = 0;
                    while (quantTuplasMaquina2 != totalTuplasMaquina1)
                    {
                        quantTuplasMaquina2 = maquina2.imdb.Count();
                    }
                    DateTime tempoREPLICACAOFinal = DateTime.Now;
                    Estatisica replicacao = new Estatisica { horarioInicio = tempoREPLICACAOInicial, horarioFim = tempoREPLICACAOFinal, iteracao = i, setup = cenario, tipo = "REPLICACAO" };
                    estatisticas.Add(replicacao);
                    estatisticas.SaveChanges();
                    Console.Write("Replicado! - ");
                }
                




                //realiza o reset dos entities pois acabou de ser feito um insert, então os dados ainda estão em memoria.
                resetEntity(ref maquina1, ref maquina2);




                //select
                Console.Write("Select ...");
                DateTime tempoSELECTInicial = DateTime.Now;
                
                    var execucaoSelect = maquina1.imdb.ToList();
                
                DateTime tempoSELECTFinal = DateTime.Now;
                Estatisica select = new Estatisica { horarioInicio = tempoSELECTInicial, horarioFim = tempoSELECTFinal, iteracao = i, setup = cenario, tipo = "SELECT" };
                estatisticas.Add(select);
                estatisticas.SaveChanges();
                Console.Write("Select! - ");





                //update
                Console.Write("Atualizando ...");
                DateTime tempoUPDATEInicial = DateTime.Now;
                {
                    maquina1.imdb.UpdateRange(execucaoSelect);
                    maquina1.SaveChanges();
                }
                DateTime tempoUPDATEFinal = DateTime.Now;
                Estatisica update = new Estatisica { horarioInicio = tempoUPDATEInicial, horarioFim = tempoUPDATEFinal, iteracao = i, setup = cenario, tipo = "UPDATE" };
                estatisticas.Add(update);
                estatisticas.SaveChanges();
                Console.Write("Atualizado! - ");





                //deletes
                Console.Write("Removendo ...");
                DateTime tempoDELETEInicial = DateTime.Now;
                {
                    maquina1.imdb.RemoveRange(execucaoSelect);
                    maquina1.SaveChanges();
                }
                DateTime tempoDELETEFinal = DateTime.Now;
                Estatisica delete = new Estatisica { horarioInicio = tempoDELETEInicial, horarioFim = tempoDELETEFinal, iteracao = i, setup = cenario, tipo = "DELETE" };
                estatisticas.Add(delete);
                estatisticas.SaveChanges();
                Console.WriteLine("Removidos! - ");




                //força a limpesa de possiveis caches do entity framework entre as iterações
                resetEntity(ref maquina1, ref maquina2);
            }
            GC.Collect();
        }

        






        private static void resetEntity(ref BancoMaquina1 maquina1, ref BancoMaquina2 maquina2) {
            maquina1.Dispose();
            maquina2.Dispose();

            maquina1 = null;
            maquina2 = null;

            GC.Collect();

            maquina1 = new BancoMaquina1();
            maquina2 = new BancoMaquina2();
        }






        private static void ligaMaquina2(string cenario)
        {
            if (cenario == "Posgresql-DBR - 2 nó - sendo 1 com defeito intermitente")
            {
                ExecutarCMD("powershell Connect-VMNetworkAdapter -VMName Maquina2 -SwitchName Internet");
                Console.Write(" Cabo conectado ");
            }
        }

        private static void desligaMaquina2(string cenario)
        {
            if (cenario == "Posgresql-DBR - 2 nó - sendo 1 com defeito intermitente")
            {
                ExecutarCMD("powershell Disconnect-VMNetworkAdapter -VMName Maquina2");
                Console.Write(" Cabo desconectado ");
            }
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
