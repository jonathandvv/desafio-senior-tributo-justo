﻿namespace TributoJustoBackend.Models.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; }
        public string SenhaHash { get; set; }
    }

}
