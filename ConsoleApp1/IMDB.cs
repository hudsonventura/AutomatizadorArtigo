using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Aumatizador
{
    public class IMDB
    {
        [Key]
        public string nconst { get; set; }
        public string primaryName { get; set; }
        
        public int birthYear { get; set; }
        public int deathYear { get; set; }
        public string primaryProfession { get; set; }
        public string knownForTitles { get; set; }





        public static List<IMDB> importarArquivo(string diretorio, int limite = 999999999)
        {
            List<IMDB> dados = new List<IMDB>();
            var arquivos = ListarArquivos(diretorio);


            //contagem a quantidade de arquivos e linhas totais a importar
            int quantArquivos = arquivos.Count;
            int quantLinhasTotal = 0;
            Console.WriteLine($"Total de {quantArquivos} arquivos a serem importados");
            for (int i = 0; i < arquivos.Count; i++)
            {
                var nomeArquivo = arquivos[i];
                string caminhoArquivo = $"{diretorio}{nomeArquivo}";
                //TextReader arquivo = new StreamReader(caminhoArquivo, true);//Inicializa o Leitor
                Console.Write($"Contando linhas do arquivo {i}");
                int quantLinhas = contarQuantLinhasDeArquivo(caminhoArquivo, i);
                Console.WriteLine($" ====> {quantLinhas} ");
                quantLinhasTotal += quantLinhas;
            }



            int contadorLinhas = 0;

            //varre todas as linhas dos arquivos 
            for (int i = 0; i < arquivos.Count; i++)
            {
                var nomeArquivo = arquivos[i];
                string caminhoArquivo = $"{diretorio}{nomeArquivo}";
                TextReader arquivo = new StreamReader(caminhoArquivo, true);//Inicializa o Leitor
                while (arquivo.Peek() != -1) //Enquanto o arquivo não acabar, o Peek não retorna -1 sendo adequando para o loop while...
                {
                    
                    //limita a quantidade de linhas a serem importadas.
                    if (contadorLinhas >= limite)
                    {
                        return dados;
                    }

                    //linha do arquivo
                    string linha = arquivo.ReadLine();
                    if (linha.Contains("tconst") == true || linha.Contains("nconst") == true) // se for um cabeçalho, ignorar
                    {
                        goto ignorarLinha;
                    }

                    //tenta realizar a conversão para classe
                    try
                    {
                        var itens = linha.Split("	");
                        //quebra a linha em um array de strings

                        string nconst = (itens[0] == @"\N") ? null : itens[0];
                        string primaryName = (itens[1] == @"\N") ? null : itens[1];

                        int birthYear = (itens[2] == @"\N") ? 0 : Int32.Parse(itens[2]);
                        int deathYear = (itens[3] == @"\N") ? 0 : Int32.Parse(itens[3]);
                        
                        string primaryProfession = (itens[4] == @"\N") ? null : itens[4];
                        string knownForTitles = (itens[5] == @"\N") ? null : itens[5];


                        IMDB obj = new IMDB { nconst = nconst, primaryName = primaryName, birthYear = birthYear, deathYear = deathYear, primaryProfession = primaryProfession, knownForTitles = knownForTitles };
                        dados.Add(obj);

                        contadorLinhas++;

                        //Console.WriteLine("");
                    }
                    catch (Exception erro)
                    {
                        goto ignorarLinha;
                    }

                    ignorarLinha:
                    Console.Write("");
                }
                GC.Collect();
            }


            return dados;
        }




        private static List<string> ListarArquivos(string path)
        {
            List<string> arquivos = new List<string>();
            DirectoryInfo Dir = new DirectoryInfo(@path);
            // Busca automaticamente todos os arquivos em todos os subdiretórios
            FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo File in Files)
            {
                arquivos.Add(File.Name);
            }
            return arquivos;
        }




        private static int contarQuantLinhasDeArquivo(string arquivo, int numeroArquivo)
        {
            var size = 256;
            var bytes = new byte[size];
            var count = 0;
            byte query = Convert.ToByte('\n');
            using (var stream = File.OpenRead(arquivo))
            {
                int many;
                do
                {
                    many = stream.Read(bytes, 0, size);
                    count += bytes.Where(a => a == query).Count();
                } while (many == size);
            }
            return count;
        }
    }
}

