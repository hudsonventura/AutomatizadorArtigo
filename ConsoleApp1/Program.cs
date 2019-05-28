
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

            int i = 2;
            int iteracoes = 100;
            string vmPrimaria = "Debian1";
            string vmSecundaria = "Debian2";
            string instalacao = "2-Solucoes 1-On 2-On";
            //string tipo = "2-Solucoes 1-On 2-Off";
            //string tipo = "1-Solucoes 1-On";
            //string tipo = "1-Postgres 1-On";


            Banco cotacao = new Banco(vmPrimaria, iteracoes);

            string importar = "";
            string query = "";

            
                switch (i)
                {
                    case 1:
                        importar = importa1Mil;
                        query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS name";
                        break;

                    case 2:
                        importar = importa10Mil;
                        query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS pessoa JOIN filmes.\"title.principals.tsv\" AS assoc ON assoc.nconst = pessoa.nconst";
                        break;

                    case 3:
                        importar = importa100Mil;
                        query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS pessoa JOIN filmes.\"title.principals.tsv\" AS assoc ON assoc.nconst = pessoa.nconst JOIN filmes.\"title.basics.tsv\" AS titulo ON assoc.tconst = titulo.tconst";
                        break;

                    case 4:
                        importar = importa1Milhao;
                        query = "SELECT count(*) FROM filmes.\"name.basics.tsv2\" AS pessoa JOIN filmes.\"title.principals.tsv\" AS assoc ON assoc.nconst = pessoa.nconst JOIN filmes.\"title.basics.tsv\" AS titulo ON assoc.tconst = titulo.tconst JOIN filmes.\"title.episode.tsv\" AS episodios ON episodios.tconst = assoc.tconst JOIN filmes.\"title.ratings.tsv\" AS estrelas ON estrelas.tconst = assoc.tconst";
                        break;
                }


                //INSERT
                Console.WriteLine();
                Console.WriteLine("Inserts ------------------------");
                cotacao.importarSQL(importar, instalacao);


                //selects
                Console.WriteLine();
                Console.WriteLine("Selects ------------------------");
                cotacao.select(importar, instalacao, query);


                //INSERT + recuperação online
                Console.WriteLine();
                Console.WriteLine("Defeito ------------------------");
                cotacao.importarSQLComRecuperacao(importar, instalacao, vmSecundaria);

                //update + select
                //cotacao.update("SELECT * FROM \"name.basics.tsv\" AS name LIMIT 10", importa1Mil);
        }
    }
}
