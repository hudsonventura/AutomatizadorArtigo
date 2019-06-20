
using System;
using System.IO;

namespace Aumatizador
{
    class Program
    {



        static void Main(string[] args)
        {

            string importa1Mil =        "insert-1000.sql";
            string importa10Mil =       "insert-10000.sql";
            string importa100Mil =      "insert-100000.sql";
            string importa1Milhao =     "insert-1000000.sql";
            string importa10Milhao =    "insert-10000000.sql";

            int iteracoes = 100;
            string vmPrimaria = "Debian1";
            string vmSecundaria = "Debian2";
            string instalacao = "";
            //string instalacao = "1-Solucoes 1-On";
            //string instalacao = "1-Postgres 1-On";

            
            //seleciona o tipo de instalação
            Console.WriteLine("Informe a instalação a ser operada:");
            Console.WriteLine("1 -> 2-Solucoes 1-On 2-On");
            Console.WriteLine("2 -> 2-Solucoes 1-On 2-Off (ainda nao preparado)");
            Console.WriteLine("3 -> 1-Solucoes 1-On");
            Console.WriteLine("4 -> 1-Postgres 1-On");
            string opcao1 = Console.ReadLine();
            switch (opcao1)
            {
                case "1": instalacao = "2-Solucoes 1-On 2-On"; break;
                case "2": instalacao = "2-Solucoes 1-On 2-Off"; break;
                case "3": instalacao = "1-Solucoes 1-On"; break;
                case "4": instalacao = "1-Postgres 1-On"; vmPrimaria = "Debian3"; break;
            }


            //informa a quantidade de tuplas
            Console.WriteLine("Informe a quantida de tuplas dos testes:");
            Console.WriteLine("1 -> ....1.000");
            Console.WriteLine("2 -> ...10.000");
            Console.WriteLine("3 -> ..100.000");
            Console.WriteLine("4 -> 1.000.000");
            string opcao2 = Console.ReadLine();


            //seleciona as operações a serem feitas
            //INSERT
            Console.WriteLine("Realizar INSERT?");
            Console.WriteLine("0 -> Nao");
            Console.WriteLine("1 -> Sim");
            string insert = Console.ReadLine();
            
            //SELECT
            Console.WriteLine("Realizar SELECT?");
            Console.WriteLine("0 -> Nao");
            Console.WriteLine("1 -> Sim");
            string select = Console.ReadLine();

            //UPDATE
            Console.WriteLine("Realizar UPDATE?");
            Console.WriteLine("0 -> Nao");
            Console.WriteLine("1 -> Sim");
            string update = Console.ReadLine();
            



            string importar = "";
            string query = "";

            
            switch (opcao2)
            {
                case "1":
                    importar = importa1Mil;
                    query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS name";
                    break;

                case "2":
                    importar = importa10Mil;
                    query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS pessoa JOIN filmes.\"title.principals.tsv\" AS assoc ON assoc.nconst = pessoa.nconst";
                    break;

                case "3":
                    importar = importa100Mil;
                    query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS pessoa JOIN filmes.\"title.principals.tsv\" AS assoc ON assoc.nconst = pessoa.nconst JOIN filmes.\"title.basics.tsv\" AS titulo ON assoc.tconst = titulo.tconst";
                    break;

                case "4":
                    importar = importa1Milhao;
                    query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS pessoa JOIN filmes.\"title.principals.tsv\" AS assoc ON assoc.nconst = pessoa.nconst JOIN filmes.\"title.basics.tsv\" AS titulo ON assoc.tconst = titulo.tconst JOIN filmes.\"title.episode.tsv\" AS episodios ON episodios.tconst = assoc.tconst JOIN filmes.\"title.ratings.tsv\" AS estrelas ON estrelas.tconst = assoc.tconst";
                    break;
            }

            string operacoes = "";
            if (insert == "1")
            {
                operacoes += " INSERT ";
            }
            if (select == "1")
            {
                operacoes += " SELECT ";
            }
            if (update == "1")
            {
                operacoes += " UPDATE+DELETE ";
            }

            Console.WriteLine("Opção escolhida: ----> '" + instalacao + "' e o arquivo '" + importar + "' tuplas" + ", nas operações: "+operacoes);
            Console.WriteLine("Precione qualquer tecla para iniciar o processo ou Ctrl+C para parar agora...");
            Console.ReadLine();


            
            Banco cotacao = new Banco(vmPrimaria, iteracoes);

            //INSERT
            if (insert == "1")
            {
                Console.WriteLine();
                Console.WriteLine("Inserts ------------------------");
                cotacao.importarSQL(importar, instalacao);
            }



            //selects
            if (select == "1")
            {
                Console.WriteLine();
                Console.WriteLine("Selects ------------------------");
                cotacao.select(importar, instalacao, query);
            }


            //UPDATE + DELETE
            if (update == "1")
            {
                Console.WriteLine();
                Console.WriteLine("Delete (com inserts)---------------");
                cotacao.updateAndDelete(importar, instalacao);
            }
            


            //FUNCIONA APENAS PARA 2 MAQUINAS PERMANENTEMENTE ONLINE, SIMULANDO A RECUPERAÇÃO DE UMA FALHA
            if (instalacao == "2-Solucoes 1-On 2-On")
            {
                //INSERT + recuperação online
                Console.WriteLine();
                Console.WriteLine("Defeito ------------------------");
                cotacao.importarSQLComRecuperacao(importar, instalacao, vmSecundaria);
            }
            
        }
    }
}
