using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Mail;

namespace Primavera.Data
{
    /// <summary>
    /// Not Implemeted
    /// </summary>
   public class Aluno
    {


        public string nrEstudante;
        public string nomeCompleto;
        public string empresa;
        public string morada;
        public string bairro;
        public string nrTelefone;
        public string nrTelefone2;
        public string curso;
        public string emailIsutc;
        public string emailAlternativo;
        public string turma;
        public string tipoIngresso;
        public string vendedor="ISU";
        

        public string observacao;

        public long? nrTalao;

        public System.DateTime dataRegistro;

        public Boolean bolsa;

        public Boolean geraMulta;

        public Aluno()
        {
        }

       /// <summary>
       /// Inicializa Atributos, usado para validar os campos na inscrição dos alunos
       /// </summary>
       /// <param name="nrEstudante"></param>
       /// <param name="nomeCompleto"></param>
       /// <param name="morada"></param>
       /// <param name="bairro"></param>
       /// <param name="nrTelefone"></param>
       /// <param name="nrTelefone2"></param>
       /// <param name="nuit"></param>
       /// <param name="bolsa"></param>
       /// <param name="emailIsutc"></param>
       /// <param name="emailAlternativo"></param>
       /// <param name="tipoIngresso"></param>
       /// <param name="status"></param>
       /// <param name="observacao"></param>
       /// <returns></returns>

        public PrimaveraResultStructure InicializaAtributos(string nrEstudante, string nomeCompleto, string morada, string bairro,
             string nrTelefone, string nrTelefone2, string nuit, bool bolsa, string emailIsutc, string emailAlternativo,
            string tipoIngresso, string observacao, string vendedor)
        {

            string mensaguem = "";
            dynamic result = new PrimaveraResultStructure();

            result.codigo = 0;


            if ((!string.IsNullOrEmpty(nrEstudante) & nrEstudante.Length < 13))
            {
                this.nrEstudante = nrEstudante;
            }
            else
            {
                mensaguem = mensaguem + "O Nr. de Estudante não pode ter mais de 12 caractes e não pode estar vazia";
            }

            if ((!string.IsNullOrEmpty(nomeCompleto) & nomeCompleto.Length < 51))
            {
                this.nomeCompleto = nomeCompleto;
            }
            else
            {
                mensaguem = mensaguem + "O Nome Completo não pode ter mais de 50 caractes e não pode estar vazia";
            }


            if ((!string.IsNullOrEmpty(morada) & morada.Length < 51))
            {
                this.morada = morada;
            }
            else
            {
                mensaguem = mensaguem + "A Morada não pode ter mais de 50 caractes e não pode estar vazia";
            }

            if ((!string.IsNullOrEmpty(bairro) | bairro.Length < 51))
            {
                this.bairro = bairro;
            }
            else
            {
                mensaguem = mensaguem + "O Bairro não pode ter mais de 12 caractes e nem pode estar vazia";
            }

            if ((!string.IsNullOrEmpty(nrTelefone) | nrTelefone.Length < 21))
            {
                this.nrTelefone = nrTelefone;
            }
            else
            {
                mensaguem = mensaguem + "O Nr. de Telefone não pode ter mais de 20 caractes e nem pode estar vazia";
            }

            if ((nrTelefone2.Length < 21))
            {
                this.nrTelefone2 = nrTelefone2;
            }
            else
            {
                mensaguem = mensaguem + "O Nr. de Telefone 2 não pode ter mais de 20 caractes";
            }
            
            if ((emailIsutc.Length < 101))
            {
                this.emailIsutc = emailIsutc;
            }
            else
            {
                mensaguem = mensaguem + "O emailIsutc não pode ter mais de 100 caractes e nem pode estar vazia";
            }

            if ((emailAlternativo.Length < 101))
            {
                this.emailAlternativo = emailAlternativo;
            }
            else
            {
                mensaguem = mensaguem + "O email Alternativo não pode ter mais de 100 caractes e nem pode estar vazia";
            }
                        

            if ((!string.IsNullOrEmpty(tipoIngresso) | tipoIngresso.Length < 101))
            {
                this.tipoIngresso = tipoIngresso;
            }
            else
            {
                mensaguem = mensaguem + "O Tipo de Ingresso não pode ter mais de 100 caractes e não pode estar vazia";
            }
        
            if ((observacao.Length < 251))
            {
                this.observacao = observacao;
            }
            else
            {
                mensaguem = mensaguem + "A observação não pode ter mais de 250 caractes";
            }

            if ((!string.IsNullOrEmpty(vendedor)) |(vendedor.Length < 4))
            {
                this.vendedor = vendedor;
            }
            else
            {
                mensaguem = mensaguem + "O Instituto não pode ter mais de 3 caractes e não pode estar vazia";
            }
            
            if ((string.IsNullOrEmpty(mensaguem)))
            {
                result.codigo = 0;
            }
            else
            {
                result.codigo = 1;
                result.tipoProblema = "Erro de Marshalling de dados";
                result.codeLevel = "10 - Validação de Campos";
                result.descricao = mensaguem;
            }

            return result;
        }


        public PrimaveraResultStructure InicializaAtributos(string nrEstudante, string nomeCompleto, string morada, string bairro,
            string nrTelefone, string nrTelefone2, string nuit, bool bolsa, string curso, string emailIsutc, string emailAlternativo,
           string tipoIngresso,  string turma, string observacao, string dataRegisto)
        {

            string mensaguem = "";
            dynamic result = new PrimaveraResultStructure();

            result.codigo = 0;


            if ((!string.IsNullOrEmpty(nrEstudante) & nrEstudante.Length < 13))
            {
                this.nrEstudante = nrEstudante;
            }
            else
            {
                mensaguem = mensaguem + "O Nr. de Estudante não pode ter mais de 12 caractes e não pode estar vazia";
            }

            if ((!string.IsNullOrEmpty(nomeCompleto) & nomeCompleto.Length < 51))
            {
                this.nomeCompleto = nomeCompleto;
            }
            else
            {
                mensaguem = mensaguem + "O Nome Completo não pode ter mais de 50 caractes e não pode estar vazia";
            }
                        

            if ((!string.IsNullOrEmpty(morada) & morada.Length < 51))
            {
                this.morada = morada;
            }
            else
            {
                mensaguem = mensaguem + "A Morada não pode ter mais de 50 caractes e não pode estar vazia";
            }

            if ((!string.IsNullOrEmpty(bairro) | bairro.Length < 51))
            {
                this.bairro = bairro;
            }
            else
            {
                mensaguem = mensaguem + "O Bairro não pode ter mais de 12 caractes e nem pode estar vazia";
            }

            if ((!string.IsNullOrEmpty(nrTelefone) | nrTelefone.Length < 21))
            {
                this.nrTelefone = nrTelefone;
            }
            else
            {
                mensaguem = mensaguem + "O Nr. de Telefone não pode ter mais de 20 caractes e nem pode estar vazia";
            }

            if ((nrTelefone2.Length < 21))
            {
                this.nrTelefone2 = nrTelefone2;
            }
            else
            {
                mensaguem = mensaguem + "O Nr. de Telefone 2 não pode ter mais de 20 caractes";
            }

            try
            {
                this.bolsa = Convert.ToBoolean(bolsa);
            }
            catch
            {
                this.bolsa = false;
            }


            if ((!string.IsNullOrEmpty(curso) | curso.Length < 21))
            {
                this.curso = curso;
            }
            else
            {
                mensaguem = mensaguem + "O Curso não pode ter mais de 21 caractes e não pode estar vazia";
            }

            if ((emailIsutc.Length < 101))
            {
                this.emailIsutc = emailIsutc;
            }
            else
            {
                mensaguem = mensaguem + "O emailIsutc não pode ter mais de 100 caractes e nem pode estar vazia";
            }

            if ((emailAlternativo.Length < 101))
            {
                this.emailAlternativo = emailAlternativo;
            }
            else
            {
                mensaguem = mensaguem + "O email Alternativo não pode ter mais de 100 caractes e nem pode estar vazia";
            }

            if ((turma.Length < 21))
            {
                this.turma = turma;
            }
            else
            {
                mensaguem = mensaguem + "A turma não pode ter mais de 21 caractes";
            }

            if ((!string.IsNullOrEmpty(tipoIngresso) | tipoIngresso.Length < 101))
            {
                this.tipoIngresso = tipoIngresso;
            }
            else
            {
                mensaguem = mensaguem + "O Tipo de Ingresso não pode ter mais de 100 caractes e não pode estar vazia";
            }
            
            if ((observacao.Length < 251))
            {
                this.observacao = observacao;
            }
            else
            {
                mensaguem = mensaguem + "A observação não pode ter mais de 250 caractes";
            }

            try
            {
                dynamic provider = new CultureInfo("pt-PT");
                this.dataRegistro = System.DateTime.ParseExact(dataRegisto, "dd/MM/yyyy", CultureInfo.InstalledUICulture);
            }
            catch
            {
                mensaguem = mensaguem + "'A Data de Registro ( " + dataRegisto + ") é invalida, deve obedecer o formato dd/MM/yyyy' ";
            }

            if ((string.IsNullOrEmpty(mensaguem)))
            {
                result.codigo = 0;
            }
            else
            {
                result.codigo = 1;
                result.tipoProblema = "Erro de Marshalling de dados";
                result.codeLevel = "10 - Validação de Campos";
                result.descricao = mensaguem;
            }

            return result;
        }


       /// <summary>
       /// Valida se email é valido
       /// </summary>
       /// <param name="emailaddress"></param>
       /// <returns></returns>
        public string IsValidEmail(string emailaddress, bool isNull)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public string isValidString(string word, bool maxLength, int length, bool isRequired, bool isNull)
        {
            return null;
        }

    }
}
