using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aumatizador
{
    public class Estatisica
    {
        [Key]
        public int id { get; set; }
        public string setup { get; set; } //refere-se ao tipo de estrutura do cluster em teste
        public string tipo { get; set; }  //refere-se ao tipo de operação em teste

        public DateTime horarioInicio { get; set; } //horario do inicio da iteração de teste
        public DateTime horarioFim { get; set; }     //horario do fim da iteração de teste

        public int iteracao { get; set; }           //numero da iteração de teste (para montagem dos graficos)
    }
}
